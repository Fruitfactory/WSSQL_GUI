/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package com.fruitfactory.pstriver.river;

import com.fruitfactory.pstriver.helpers.AttachmentHelper;
import com.fruitfactory.pstriver.helpers.PstReaderStatus;
import com.fruitfactory.pstriver.helpers.PstReaderStatusInfo;
import com.fruitfactory.pstriver.helpers.Triple;
import com.fruitfactory.pstriver.rest.PstStatusRepository;
import static com.fruitfactory.pstriver.river.PstRiver.LOG_TAG;
import com.fruitfactory.pstriver.utils.PstMetadataTags;
import com.fruitfactory.pstriver.utils.PstSignTool;
import com.pff.PSTAppointment;
import com.pff.PSTAttachment;
import com.pff.PSTContact;
import com.pff.PSTConversationIndexData;
import com.pff.PSTException;
import com.pff.PSTFile;
import com.pff.PSTFolder;
import com.pff.PSTMessage;
import com.pff.PSTNodeInputStream;
import com.pff.PSTObject;
import com.pff.PSTRecipient;
import com.pff.PSTTimeZone;
import com.sun.org.apache.xerces.internal.impl.dv.util.Base64;
import example.TestGui;
import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.nio.file.Path;
import java.nio.file.Paths;
import java.security.NoSuchAlgorithmException;
import java.util.ArrayList;
import java.util.Date;
import java.util.List;
import java.util.UUID;
import java.util.Vector;
import java.util.logging.Level;
import java.util.logging.Logger;
import org.apache.tika.Tika;
import org.apache.tika.metadata.Metadata;
import org.elasticsearch.action.bulk.BulkProcessor;
import org.elasticsearch.action.index.IndexRequest;
import org.elasticsearch.common.io.stream.BytesStreamInput;
import org.elasticsearch.common.logging.ESLogger;
import org.elasticsearch.common.xcontent.XContentBuilder;
import static org.elasticsearch.common.xcontent.XContentFactory.jsonBuilder;

/**
 *
 * @author Yariki
 */
public class PstOutlookFileReader implements Runnable {

    protected ESLogger _logger;
    protected BulkProcessor _bulkProcessor;
    private boolean _closed = false;
    private String _filename;
    private Date _lastUpdateDate;
    private String _indexName;
    private String _name;
    private int _emailCount = 0;
    
    private final Tika _tika = new Tika();

    public PstOutlookFileReader(String indexName, String filename, Date lastUpdatedDate, ESLogger _logger, BulkProcessor _bulkProcessor) {
        this._filename = filename;
        this._lastUpdateDate = lastUpdatedDate;
        this._logger = _logger;
        this._bulkProcessor = _bulkProcessor;
        this._indexName = indexName;
    }

    private void esIndex(String index, String type, String id,
            XContentBuilder xb) throws Exception {
        if (_logger.isDebugEnabled()) {
            _logger.debug("Indexing in ES " + index + ", " + type + ", " + id);
        }
        if (_logger.isTraceEnabled()) {
            _logger.trace("JSon indexed : {}", xb.string());
        }

        if (!_closed) {
            _bulkProcessor.add(new IndexRequest(index, type, id).source(xb));
        } else {
            _logger.warn("trying to add new file while closing river. Document [{}]/[{}]/[{}] has been ignored", index, type, id);
        }
    }

    public void close() {
        _closed = true;
    }

    @Override
    public void run() {
        try {
            PSTFile file = new PSTFile(_filename);
            prepareStatusInfo(file.getRootFolder());
            PstStatusRepository.setStatus(_name, PstReaderStatus.Busy);
            processFolder(file.getRootFolder());
            PstStatusRepository.setStatus(_name, PstReaderStatus.Finished);
            file.close();

        } catch (Exception e) {
            _logger.error(LOG_TAG + e.getMessage());
            PstStatusRepository.setStatus(_name, PstReaderStatus.Finished);
        }
    }

    private void prepareStatusInfo(PSTFolder rootFolder) {
        int count = countItems(rootFolder);
        Path pathFileName = Paths.get(_filename).getFileName();
        _name = pathFileName.toString();
        PstReaderStatusInfo statusInfo = new PstReaderStatusInfo(_name, count);
        statusInfo.setStatus(PstReaderStatus.NonStarted);
        PstStatusRepository.setStatusInfo(statusInfo);
    }

    private int countItems(PSTFolder rootFolder) {
        try {
            int count = 0;

            if (rootFolder.hasSubfolders()) {
                for (PSTFolder subFolder : rootFolder.getSubFolders()) {
                    count += countItems(subFolder);
                }
            }
            if (rootFolder.getContentCount() > 0) {
                count += rootFolder.getContentCount();
            }
            return count;

        } catch (Exception e) {
            _logger.error(LOG_TAG, e);
        }
        return 0;
    }

    private void processFolder(PSTFolder pstFolder) {

        try {
            String folderName = pstFolder.getDisplayName();

            _logger.warn(LOG_TAG + " Folder: " + folderName);

            if (pstFolder.hasSubfolders()) {
                Vector<PSTFolder> folders = pstFolder.getSubFolders();
                for (PSTFolder folder : folders) {
                    processFolder(folder);
                }
            }

            if (pstFolder.getContentCount() > 0) {
                PSTMessage message = (PSTMessage) pstFolder.getNextChild();
                while (message != null) {
                    if (_lastUpdateDate == null) {
                        processObject(message, folderName);
                        message = (PSTMessage) pstFolder.getNextChild();

                        continue;
                    } else {
                        long creationTime = message.getCreationTime().getTime();
                        long lastUpdated = _lastUpdateDate.getTime();

                        if (lastUpdated > creationTime) {
                            message = (PSTMessage) pstFolder.getNextChild();
                            continue;
                        }
                        processObject(message, folderName);
                        message = (PSTMessage) pstFolder.getNextChild();
                    }

                }
                PstStatusRepository.setProcessCount(_name, pstFolder.getContentCount());
            }
        } catch (Exception e) {
            _logger.error(LOG_TAG + e.getMessage());
        }
    }

    private void processObject(PSTObject object, String folderName) throws Exception {
        if (object instanceof PSTContact) {
            indexContact((PSTContact) object);
        } else if (object instanceof PSTAppointment) {
            indexAppointment((PSTAppointment) object);
        } else if (object != null && object instanceof PSTMessage) {
            indexEmail((PSTMessage)object, folderName);
        }
    }

    private void indexEmail(PSTMessage message, String folderName) throws IOException, Exception {
        String subject = message.getSubject();
        String sender = message.getSentRepresentingName();
        String senderEmail = message.getSentRepresentingEmailAddress();
        String body = message.getBody();
        String htmlbody = message.getBodyHTML();
        String analyzedContent = _tika.parseToString(new BytesStreamInput(htmlbody.getBytes(),false),new Metadata());
        String entryID = message.getEntryID();
        boolean hasAttachment = message.hasAttachments();
        Date dateCreated = message.getCreationTime();
        Date dateReceived = message.getMessageDeliveryTime();
        long size = message.getMessageSize();
        PSTConversationIndexData indexData = message.getConversationIndexData();
        UUID id = null;
        String conversationIndex = "";
        String outlookConversationId = "";
        try {
            id = indexData.getConversationUUID(message.getConversationTopic(), message.getConversationIndexTracking());
            conversationIndex = Integer.toString(indexData.getConversationIndex());
            outlookConversationId = indexData.getOutlookConversationId();
        } catch (NoSuchAlgorithmException ex) {
            Logger.getLogger(TestGui.class.getName()).log(Level.SEVERE, null, ex);
        }
        String conversationId = id != null ? id.toString() : "";

        List<Triple<String, String,String>> listTo = new ArrayList<Triple<String, String,String>>();
        List<Triple<String, String,String>> listCc = new ArrayList<Triple<String, String,String>>();
        List<Triple<String, String,String>> listBcc = new ArrayList<Triple<String, String,String>>();
        List<AttachmentHelper> listAttachments = new ArrayList<AttachmentHelper>();
        int count = 0;
        try {
            count = message.getNumberOfRecipients();

            for (int i = 0; i < count; i++) {
                PSTRecipient recipient = message.getRecipient(i);
                Triple<String, String,String> pairRecipient = new Triple<String, String,String>(recipient.getDisplayName(), recipient.getEmailAddress(),recipient.getEmailAddressType());
                int flag = recipient.getRecipientType();
                switch (flag) {
                    case PSTRecipient.MAPI_TO:
                        listTo.add(pairRecipient);
                        break;
                    case PSTRecipient.MAPI_CC:
                        listCc.add(pairRecipient);
                        break;
                    case PSTRecipient.MAPI_BCC:
                        listBcc.add(pairRecipient);
                        break;
                }
            }
        } catch (PSTException ex) {
            Logger.getLogger(TestGui.class.getName()).log(Level.SEVERE, null, ex);
        } catch (IOException ex) {
            Logger.getLogger(TestGui.class.getName()).log(Level.SEVERE, null, ex);
        }
        try {
            int countAttachments = message.getNumberOfAttachments();
            for (int i = 0; i < countAttachments; i++) {
                PSTAttachment attachment = message.getAttachment(i);
                if (attachment == null) {
                    continue;
                }
                AttachmentHelper helper = new AttachmentHelper(attachment.getLongFilename(), attachment.getLongPathname(), attachment.getFilesize(), attachment.getMimeTag());
                listAttachments.add(helper);

                saveAttachment(attachment, entryID);
            }
        } catch (Exception ex) {
            System.out.println(ex.getMessage());
        }

        XContentBuilder source = jsonBuilder().startObject();

        if (_logger.isTraceEnabled()) {
            source.prettyPrint();
        }

        source
                .field(PstMetadataTags.Email.ITEM_NAME, subject)
                .field(PstMetadataTags.Email.ITEM_URL, subject)
                .field(PstMetadataTags.Email.ITEM_NAME_DISPLAY, subject)
                .field(PstMetadataTags.Email.FOLDER, folderName)
                .field(PstMetadataTags.Email.DATE_CREATED, dateCreated)
                .field(PstMetadataTags.Email.DATE_RECEIVED, dateReceived)
                .field(PstMetadataTags.Email.SIZE, size)
                .field(PstMetadataTags.Email.CONVERSATION_ID, conversationId)
                .field(PstMetadataTags.Email.CONVERSATION_INDEX, conversationIndex)
                .field(PstMetadataTags.Email.OUTLOOK_CONVERSATION_ID, outlookConversationId)
                .field(PstMetadataTags.Email.SUBJECT, subject)
                .field(PstMetadataTags.Email.CONTENT, body)
                .field(PstMetadataTags.Email.HTML_CONTENT, htmlbody)
                .field(PstMetadataTags.Email.ANALYZED_CONTENT,analyzedContent)
                .field(PstMetadataTags.Email.HAS_ATTACHMENTS, Boolean.toString(hasAttachment))
                .field(PstMetadataTags.Email.FROM_NAME, sender)
                .field(PstMetadataTags.Email.FROM_ADDRESS, senderEmail)
                .field(PstMetadataTags.Email.ENTRY_ID, entryID);

        AddArrayOfEmails(source, listTo,
                PstMetadataTags.Email.TO,
                PstMetadataTags.Email.To.NAME,
                PstMetadataTags.Email.To.ADDRESS,
                PstMetadataTags.Email.To.EMAIL_ADDRESS_TYPE);
        AddArrayOfEmails(source, listCc,
                PstMetadataTags.Email.CC,
                PstMetadataTags.Email.Cc.NAME,
                PstMetadataTags.Email.Cc.ADDRESS,
                PstMetadataTags.Email.Cc.EMAIL_ADDRESS_TYPE);
        AddArrayOfEmails(source, listBcc,
                PstMetadataTags.Email.BCC,
                PstMetadataTags.Email.Bcc.NAME,
                PstMetadataTags.Email.Bcc.ADDRESS,
                PstMetadataTags.Email.Bcc.EMAIL_ADDRESS_TYPE);

        if (!listAttachments.isEmpty()) {
            source.field(PstMetadataTags.Email.ATTACHMENTS).startArray();
            for (AttachmentHelper listAttachment : listAttachments) {
                source.startObject();
                source.field(PstMetadataTags.Email.Attachments.FILENAME,
                        listAttachment.getFilename());
                source.field(PstMetadataTags.Email.Attachments.PATH,
                        listAttachment.getPath());
                source.field(PstMetadataTags.Email.Attachments.SIZE,
                        listAttachment.getSize());
                source.field(PstMetadataTags.Email.Attachments.MIME_TAG,
                        listAttachment.getMimetype());
                source.endObject();
            }
            source.endArray();
        }

        source.endObject();

        esIndex(_indexName, PstMetadataTags.INDEX_TYPE_EMAIL_MESSAGE, PstSignTool.sign(UUID.randomUUID().toString()).toString(), source);
    }

    private void saveAttachment(PSTAttachment attachment, String emailID) throws IOException, Exception {
        if (attachment == null) {
            return;
        }
        String filename = attachment.getLongFilename();
        String pathname = attachment.getLongPathname();
        int size = attachment.getSize();
        String mime = attachment.getMimeTag();
        String entryid = attachment.getEntryID();
        StringBuilder strBuilder = new StringBuilder();
        String parsedContent = "";
        try (InputStream reader = attachment.getFileInputStream()) {
            ByteArrayOutputStream bufferStream = new ByteArrayOutputStream();
            final int lenght = 8176;
            byte[] output = new byte[lenght];
            int nRead;
            while ( (nRead = reader.read(output)) != -1) {
                bufferStream.write(output,0,nRead);
            }
            bufferStream.flush();
            byte[] byteBuffer = bufferStream.toByteArray();
            strBuilder.append(Base64.encode(byteBuffer));
            parsedContent = _tika.parseToString(new BytesStreamInput(byteBuffer,false), new Metadata());
        } catch (Exception ex) {
            _logger.error(LOG_TAG, ex.getMessage());
        }

        XContentBuilder source = jsonBuilder().startObject();

        if (_logger.isTraceEnabled()) {
            source.prettyPrint();
        }

        source
                .field(PstMetadataTags.Attachment.FILENAME, filename)
                .field(PstMetadataTags.Attachment.PATH, pathname)
                .field(PstMetadataTags.Attachment.SIZE, size)
                .field(PstMetadataTags.Attachment.MIME_TAG, mime)
                .field(PstMetadataTags.Attachment.ANALYZED_CONTENT,parsedContent)
                .field(PstMetadataTags.Attachment.CONTENT, strBuilder.toString())
                .field(PstMetadataTags.Attachment.EMAIL_ID, emailID)
                .field(PstMetadataTags.Attachment.ENTRYID, entryid);
        source.endObject();
        esIndex(_indexName, PstMetadataTags.INDEX_TYPE_ATTACHMENT, PstSignTool.sign(UUID.randomUUID().toString()).toString(), source);
    }

    private void indexContact(PSTContact contact) throws IOException, Exception {
        String firstName = contact.getGivenName();
        String lastName = contact.getSurname();
        String middleName = contact.getMiddleName();

        String emailAddress1 = contact.getEmail1EmailAddress();
        String emailAddress2 = contact.getEmail2EmailAddress();
        String emailAddress3 = contact.getEmail3EmailAddress();
        String businessPhone = contact.getBusinessTelephoneNumber();
        String homePhone = contact.getHomeTelephoneNumber();
        String mobilePhone = contact.getMobileTelephoneNumber();
        String addressType = contact.getAddrType();

        String homeCity = contact.getHomeAddressCity();
        String homeCountry = contact.getHomeAddressCountry();
        String homePostalCode = contact.getHomeAddressPostalCode();
        String homeState = contact.getHomeAddressStateOrProvince();
        String homeStreet = contact.getHomeAddressStreet();
        String homePostofficeBox = contact.getHomeAddressPostOfficeBox();

        String bCity = contact.getBusinessAddressCity();
        String bCountry = contact.getBusinessAddressCountry();
        String bState = contact.getBusinessAddressStateOrProvince();
        String bStreet = contact.getBusinessAddressStreet();
        String bPostalBox = contact.getBusinessPostalCode();

        String keyword = contact.getKeyword();
        String location = contact.getLocation();
        String companyName = contact.getCompanyName();
        String title = contact.getTitle();
        String departName = contact.getDepartmentName();
        String displyNamePrefix = contact.getDisplayNamePrefix();
        String profession = contact.getProfession();
        String note = contact.getNote();

        String homeAddress = contact.getHomeAddress();
        String workAddress = contact.getWorkAddress();
        String otherAddress = contact.getOtherAddress();

        Date birthday = contact.getBirthday();
        String entryID = contact.getEntryID();

        XContentBuilder source = jsonBuilder().startObject();

        if (_logger.isTraceEnabled()) {
            source.prettyPrint();
        }

        source
                .field(PstMetadataTags.Contact.ITEM_FIRST_NAME, firstName)
                .field(PstMetadataTags.Contact.ITEM_LAST_NAME, lastName)
                .field(PstMetadataTags.Contact.ITEM_EMAILADDRESS1, emailAddress1)
                .field(PstMetadataTags.Contact.ITEM_EMAILADDRESS2, emailAddress2)
                .field(PstMetadataTags.Contact.ITEM_EMAILADDRESS3, emailAddress3)
                .field(PstMetadataTags.Contact.ITEM_BUSINESSTELEPHONE, businessPhone)
                .field(PstMetadataTags.Contact.ITEM_HOMETELEPHONE, homePhone)
                .field(PstMetadataTags.Contact.ITEM_MOBILETELEPHONE, mobilePhone)
                .field(PstMetadataTags.Contact.ITEM_HOMEADDRESSCITY, homeCity)
                .field(PstMetadataTags.Contact.ITEM_HOMEADDRESSCOUNTRY, homeCountry)
                .field(PstMetadataTags.Contact.ITEM_HOMEADDRESSPOSTALCODE, homePostalCode)
                .field(PstMetadataTags.Contact.ITEM_HOMEADDRESSSTATE, homeState)
                .field(PstMetadataTags.Contact.ITEM_HOMEADRESSSTREET, homeStreet)
                .field(PstMetadataTags.Contact.ITEM_HOMEADDRESSPOSTOFFICEBOX, homePostofficeBox)
                .field(PstMetadataTags.Contact.ITEM_BUSINESSADDRESSCITY, bCity)
                .field(PstMetadataTags.Contact.ITEM_BUSINESSADDRESSCOUTRY, bCountry)
                .field(PstMetadataTags.Contact.ITEM_BUSINESSADDRESSSTATE, bState)
                .field(PstMetadataTags.Contact.ITEM_BUSINESSADDRESSSTREET, bStreet)
                .field(PstMetadataTags.Contact.ITEM_BUSINESSADDRESSPOSTOFFICEBOX, bPostalBox)
                .field(PstMetadataTags.Contact.ITEM_KEYWORD, keyword)
                .field(PstMetadataTags.Contact.ITEM_LOCATION, location)
                .field(PstMetadataTags.Contact.ITEM_COMPANY_NAME, companyName)
                .field(PstMetadataTags.Contact.ITEM_TITLE, title)
                .field(PstMetadataTags.Contact.ITEM_DEPARTMENT_NAME, departName)
                .field(PstMetadataTags.Contact.ITEM_MIDDLE_NAME, middleName)
                .field(PstMetadataTags.Contact.ITEM_DISPLAY_NAME_PREFIX, displyNamePrefix)
                .field(PstMetadataTags.Contact.ITEM_PROFESSION, profession)
                .field(PstMetadataTags.Contact.ITEM_NOTE, note)
                .field(PstMetadataTags.Contact.ITEM_HOME_ADDRESS, homeAddress)
                .field(PstMetadataTags.Contact.ITEM_WORK_ADDRESS, workAddress)
                .field(PstMetadataTags.Contact.ITEM_OTHER_ADDRESS, otherAddress)
                .field(PstMetadataTags.Contact.ITEM_BIRTHDAY, birthday)
                .field(PstMetadataTags.Contact.ENTRY_ID, entryID)
                .field(PstMetadataTags.Contact.ADDRESS_TYPE,addressType);
        source.endObject();

        esIndex(_indexName, PstMetadataTags.INDEX_TYPE_CONTACT, PstSignTool.sign(contact.toString()).toString(), source);
    }

    private void indexAppointment(PSTAppointment appointment) throws IOException, Exception {
        String location = appointment.getLocation();
        Date startTime = appointment.getStartTime();
        Date endTime = appointment.getEndTime();
        PSTTimeZone zone = appointment.getStartTimeZone();
        String timezone = zone.getName();
        int duration = appointment.getDuration();
        int meetingStatus = appointment.getMeetingStatus();
        String allAttendees = appointment.getAllAttendees();
        String toAttendees = appointment.getToAttendees();
        String ccAttendees = appointment.getCCAttendees();
        boolean isOnliheMeeting = appointment.isOnlineMeeting();
        String meetingServer = appointment.getNetMeetingServer();
        String docPath = appointment.getNetMeetingDocumentPathName();
        String url = appointment.getNetShowURL();
        String requiredAttendees = appointment.getRequiredAttendees();
        String entryID = appointment.getEntryID();

        XContentBuilder source = jsonBuilder().startObject();

        if (_logger.isTraceEnabled()) {
            source.prettyPrint();
        }

        source
                .field(PstMetadataTags.Appointment.ITEM_LOCATION, location)
                .field(PstMetadataTags.Appointment.ITEM_STARTTIME, startTime)
                .field(PstMetadataTags.Appointment.ITEM_ENDTIME, endTime)
                .field(PstMetadataTags.Appointment.ITEM_TIMEZONE, timezone)
                .field(PstMetadataTags.Appointment.ITEM_DURATION, duration)
                .field(PstMetadataTags.Appointment.ITEM_MEETING_STATUS, meetingStatus)
                .field(PstMetadataTags.Appointment.ITEM_ALL_ATTENDEES, allAttendees)
                .field(PstMetadataTags.Appointment.ITEM_TO_ATTENDEES, toAttendees)
                .field(PstMetadataTags.Appointment.ITEM_CC_ATTENDEES, ccAttendees)
                .field(PstMetadataTags.Appointment.ITEM_ISONLINE_MEETING, isOnliheMeeting)
                .field(PstMetadataTags.Appointment.ITEM_NETMEETING_DOCUMENT_PATH, docPath)
                .field(PstMetadataTags.Appointment.ITEM_NETMEETING_SERVER, meetingServer)
                .field(PstMetadataTags.Appointment.ITEM_NETSHOW_URL, url)
                .field(PstMetadataTags.Appointment.ITEM_REQUIRED_ATTENDEES, requiredAttendees)
                .field(PstMetadataTags.Appointment.ENTRY_ID, entryID);
        source.endObject();

        esIndex(_indexName, PstMetadataTags.INDEX_TYPE_CALENDAR, PstSignTool.sign(UUID.randomUUID().toString()).toString(), source);
    }

    private void AddArrayOfEmails(XContentBuilder builder, List<Triple<String, String,String>> list, String objectName, String nameTag, String addressTag, String addressTypeTag) throws IOException {
        if (list.isEmpty()) {
            return;
        }
        builder.field(objectName).startArray();
        for (Triple<String, String,String> item : list) {
            builder.startObject();
            builder.field(nameTag, item.getItem1());
            builder.field(addressTag, item.getItem2());
            builder.field(addressTypeTag, item.getItem3());
            builder.endObject();
        }
        builder.endArray();
    }

}
