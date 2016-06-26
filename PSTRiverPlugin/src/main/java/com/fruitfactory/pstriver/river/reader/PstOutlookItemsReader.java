package com.fruitfactory.pstriver.river.reader;

import com.fruitfactory.pstriver.helpers.AttachmentHelper;
import com.fruitfactory.pstriver.helpers.PstReaderStatus;
import com.fruitfactory.pstriver.helpers.PstReaderStatusInfo;
import com.fruitfactory.pstriver.helpers.Triple;
import com.fruitfactory.pstriver.interfaces.IPstOutlookItemsProcessor;
import com.fruitfactory.pstriver.rest.PstRESTRepository;
import com.fruitfactory.pstriver.rest.data.*;
import com.fruitfactory.pstriver.utils.PstMetadataTags;
import com.fruitfactory.pstriver.utils.PstSignTool;
import com.pff.PSTConversationIndexData;
import com.sun.org.apache.xerces.internal.impl.dv.util.Base64;
import org.apache.tika.Tika;
import org.apache.tika.exception.TikaException;
import org.apache.tika.metadata.Metadata;
import org.elasticsearch.action.bulk.BulkProcessor;
import org.elasticsearch.common.io.stream.BytesStreamInput;
import org.elasticsearch.common.logging.ESLogger;
import org.elasticsearch.common.xcontent.XContentBuilder;

import java.io.IOException;
import java.util.Date;
import java.util.List;
import java.util.UUID;

import static org.elasticsearch.common.xcontent.XContentFactory.jsonBuilder;

/**
 * Created by Yariki on 8/26/2015.
 */
public class PstOutlookItemsReader extends PstBaseOutlookIndexer implements IPstOutlookItemsProcessor {



    private Date _lastUpdateDate;
    private String _indexName;
    private String _name;

    private final Object _lock = new Object();

    private final Tika _tika = new Tika();

    public PstOutlookItemsReader(String indexName, Date lastUpdated, BulkProcessor bulkProcessor, String name, ESLogger _logger) {
        super(name, _logger,bulkProcessor);
        this._bulkProcessor = bulkProcessor;
        this._lastUpdateDate = lastUpdated;
        this._indexName = indexName;
        setDaemon(true);
        setPriority(MIN_PRIORITY);
        _name = "outlookitems";
        _status = PstReaderStatus.None;
        PstRESTRepository.setStatusInfo(new PstReaderStatusInfo(_name,0));
    }

    @Override
    protected String getReaderName() {
        return _name;
    }

    // one by one

    @Override
    public void run() {
        try {

            PstRESTRepository.setAttachmentProcessor(this);
            _status = PstReaderStatus.Busy;
            PstRESTRepository.setStatus(_name, PstReaderStatus.Busy);
            while(!_closed){
                tryToWait();
                synchronized (_lock){
                    if(_status == PstReaderStatus.Finished){
                        break;
                    }
                }
                Thread.sleep(100);
            }
        }catch (Exception e){
            _logger.error(e.getMessage());
        }finally {
            _logger.info("!!!!!!!! Exit From Attachment Reader");
            PstRESTRepository.clearAttachmentProcess();
            PstRESTRepository.setStatus(_name, PstReaderStatus.Finished);
        }

    }

    @Override
    public void processOutlookItems(PstOutlookItemsContainer pstOutlookItemsContainer) {

        if(PstAttachmentIndexProcess.getValue(pstOutlookItemsContainer.getProcess()) == PstAttachmentIndexProcess.End){
            synchronized (_lock){
                _status = PstReaderStatus.Finished;
            }
            return;
        }
        try {
            saveEmail(pstOutlookItemsContainer);
            saveAttachments(pstOutlookItemsContainer);
            saveContact(pstOutlookItemsContainer);
        }catch(Exception ex){

        }
    }

    private void saveContact(PstOutlookItemsContainer pstOutlookItemsContainer) throws IOException {
        if(pstOutlookItemsContainer == null || pstOutlookItemsContainer.getContact() == null){
            return;
        }

        try {
            PstContactContent contact = pstOutlookItemsContainer.getContact();

            XContentBuilder source = jsonBuilder().startObject();

            _logger.info(String.format("---- Contact => %s %s",contact.getFirstname(),contact.getLastname()));

            source
                    .field(PstMetadataTags.Contact.ITEM_FIRST_NAME, contact.getFirstname())
                    .field(PstMetadataTags.Contact.ITEM_LAST_NAME, contact.getLastname())
                    .field(PstMetadataTags.Contact.ITEM_EMAILADDRESS1, contact.getEmailaddress1())
                    .field(PstMetadataTags.Contact.ITEM_EMAILADDRESS2, contact.getEmailaddress2())
                    .field(PstMetadataTags.Contact.ITEM_EMAILADDRESS3, contact.getEmailaddress3())
                    .field(PstMetadataTags.Contact.ITEM_BUSINESSTELEPHONE, contact.getBusinesstelephone())
                    .field(PstMetadataTags.Contact.ITEM_HOMETELEPHONE, contact.getHometelephone())
                    .field(PstMetadataTags.Contact.ITEM_MOBILETELEPHONE, contact.getMobiletelephone())
                    .field(PstMetadataTags.Contact.ITEM_HOMEADDRESSCITY, contact.getHomeaddresscity())
                    .field(PstMetadataTags.Contact.ITEM_HOMEADDRESSCOUNTRY, contact.getHomeaddresscountry())
                    .field(PstMetadataTags.Contact.ITEM_HOMEADDRESSPOSTALCODE, contact.getHomeaddresspostalcode())
                    .field(PstMetadataTags.Contact.ITEM_HOMEADDRESSSTATE, contact.getHomeaddressstate())
                    .field(PstMetadataTags.Contact.ITEM_HOMEADRESSSTREET, contact.getHomeaddressstreet())
                    .field(PstMetadataTags.Contact.ITEM_HOMEADDRESSPOSTOFFICEBOX, contact.getHomeaddresspostofficebox())
                    .field(PstMetadataTags.Contact.ITEM_BUSINESSADDRESSCITY, contact.getBusinessaddresscity())
                    .field(PstMetadataTags.Contact.ITEM_BUSINESSADDRESSCOUTRY, contact.getBusinessaddresscountry())
                    .field(PstMetadataTags.Contact.ITEM_BUSINESSADDRESSSTATE, contact.getBusinessaddressstate())
                    .field(PstMetadataTags.Contact.ITEM_BUSINESSADDRESSSTREET, contact.getBusinessaddressstreet())
                    .field(PstMetadataTags.Contact.ITEM_BUSINESSADDRESSPOSTOFFICEBOX, contact.getHomeaddresspostofficebox())
                    .field(PstMetadataTags.Contact.ITEM_KEYWORD, contact.getKeyword())
                    .field(PstMetadataTags.Contact.ITEM_LOCATION, contact.getLocation())
                    .field(PstMetadataTags.Contact.ITEM_COMPANY_NAME, contact.getCompanyname())
                    .field(PstMetadataTags.Contact.ITEM_TITLE, contact.getTitle())
                    .field(PstMetadataTags.Contact.ITEM_DEPARTMENT_NAME, contact.getDepartmentname())
                    .field(PstMetadataTags.Contact.ITEM_MIDDLE_NAME, contact.getMiddlename())
                    .field(PstMetadataTags.Contact.ITEM_DISPLAY_NAME_PREFIX, contact.getDisplynameprefix())
                    .field(PstMetadataTags.Contact.ITEM_PROFESSION, contact.getProfession())
                    .field(PstMetadataTags.Contact.ITEM_NOTE, contact.getNote())
                    .field(PstMetadataTags.Contact.ITEM_HOME_ADDRESS, contact.getHomeaddress())
                    .field(PstMetadataTags.Contact.ITEM_WORK_ADDRESS, contact.getWorkaddress())
                    .field(PstMetadataTags.Contact.ITEM_OTHER_ADDRESS, contact.getOtheraddress())
                    .field(PstMetadataTags.Contact.ITEM_BIRTHDAY, contact.getBirthday())
                    .field(PstMetadataTags.Contact.ENTRY_ID, contact.getEntryid())
                    .field(PstMetadataTags.Contact.ADDRESS_TYPE,contact.getAddresstype())
                    .field(PstMetadataTags.Contact.ITEM_FIRST_NAME_SUGGEST, contact.getFirstname())
                    .field(PstMetadataTags.Contact.ITEM_LAST_NAME_SUGGEST, contact.getLastname())
                    .field(PstMetadataTags.Contact.ITEM_NAME_SUGGEST,String.format("%s %s",contact.getFirstname(),contact.getLastname()).trim())
                    .field(PstMetadataTags.Contact.ITEM_EMAILADDRESS1_SUGGEST, contact.getEmailaddress1())
                    .field(PstMetadataTags.Contact.ITEM_EMAILADDRESS2_SUGGEST, contact.getEmailaddress2())
                    .field(PstMetadataTags.Contact.ITEM_EMAILADDRESS3_SUGGEST, contact.getEmailaddress3());
            source.endObject();

            esIndex(_indexName, PstMetadataTags.INDEX_TYPE_CONTACT, PstSignTool.sign(UUID.randomUUID().toString()).toString(), source);

        }catch(Exception ex){
            System.out.println(ex.toString());
            _logger.error(ex.toString());
        }
    }

    private void saveAttachments(PstOutlookItemsContainer pstOutlookItemsContainer) {
        if(pstOutlookItemsContainer.getAttachments() == null || pstOutlookItemsContainer.getAttachments().size() == 0){
            return;
        }
        try {
            for(PstAttachmentContent content : pstOutlookItemsContainer.getAttachments()){
                saveAttachment(content);
            }
        }catch(Exception ex){
            _logger.error(ex.getMessage());
        }
    }

    private void saveEmail(PstOutlookItemsContainer pstOutlookItemsContainer) throws IOException, TikaException {
        if(pstOutlookItemsContainer.getEmail() == null){
            return;
        }
        PstEmailContent email = pstOutlookItemsContainer.getEmail();

        try {
            String subject = email.getSubject();
            String folderName = email.getFolder();

            PstRESTRepository.setProcessFolder(_name,folderName);

            String storageName = email.getStoragename();
            String storePartId = email.getFoldermessagestoreidpart();
            Date dateCreated = email.getDatecreated();
            Date dateReceived = email.getDatereceived();
            long size = email.getSize();
            String conversationID = email.getConversationid();

            PSTConversationIndexData indexData = new PSTConversationIndexData(email.getConversationindex().getBytes());
            String conversationIndex = Integer.toString(indexData.getConversationIndex());
            String outlookConversationId = indexData.getOutlookConversationId();
            String body = new String(Base64.decode(email.getContent()));
            String htmlBody = new String(Base64.decode(email.getHtmlcontent()));
            String analyzedContent = _tika.parseToString(new BytesStreamInput(htmlBody.getBytes()),new Metadata());
            String hasAttachments = email.getHasattachments();
            String fromName = email.getFromname();
            String fromAddress = email.getFromaddress();
            String entryId = email.getEntryid();

            _logger.info(String.format("---- Email => %s",subject));

            XContentBuilder source = jsonBuilder().startObject();
            source
                    .field(PstMetadataTags.Email.ITEM_NAME, subject)
                    .field(PstMetadataTags.Email.ITEM_URL, subject)
                    .field(PstMetadataTags.Email.ITEM_NAME_DISPLAY, subject)
                    .field(PstMetadataTags.Email.FOLDER, folderName)
                    .field(PstMetadataTags.Email.STORAGE_NAME, storageName)
                    .field(PstMetadataTags.Email.FOLDER_MESSAGE_STORE_ID_PART, storePartId)
                    .field(PstMetadataTags.Email.DATE_CREATED, dateCreated)
                    .field(PstMetadataTags.Email.DATE_RECEIVED, dateReceived)
                    .field(PstMetadataTags.Email.SIZE, size)
                    .field(PstMetadataTags.Email.CONVERSATION_ID, conversationID)
                    .field(PstMetadataTags.Email.CONVERSATION_INDEX, conversationIndex)
                    .field(PstMetadataTags.Email.OUTLOOK_CONVERSATION_ID, outlookConversationId)
                    .field(PstMetadataTags.Email.SUBJECT, subject)
                    .field(PstMetadataTags.Email.CONTENT, body)
                    .field(PstMetadataTags.Email.HTML_CONTENT, htmlBody)
                    .field(PstMetadataTags.Email.ANALYZED_CONTENT,analyzedContent)
                    .field(PstMetadataTags.Email.HAS_ATTACHMENTS, hasAttachments)
                    .field(PstMetadataTags.Email.FROM_NAME, fromName)
                    .field(PstMetadataTags.Email.FROM_ADDRESS, fromAddress)
                    .field(PstMetadataTags.Email.ENTRY_ID, entryId)
                    .field(PstMetadataTags.Email.FROM_NAME_SUGGEST,fromName)
                    .field(PstMetadataTags.Email.FROM_ADDRESS_SUGGEST,fromAddress)
                    .field(PstMetadataTags.Email.SUBJECT_SUGGEST,subject);

            addArrayOfEmails(source, email.getTo(),
                    PstMetadataTags.Email.TO,
                    PstMetadataTags.Email.To.NAME,
                    PstMetadataTags.Email.To.ADDRESS,
                    PstMetadataTags.Email.To.EMAIL_ADDRESS_TYPE);
            addArrayOfEmails(source, email.getCc(),
                    PstMetadataTags.Email.CC,
                    PstMetadataTags.Email.Cc.NAME,
                    PstMetadataTags.Email.Cc.ADDRESS,
                    PstMetadataTags.Email.Cc.EMAIL_ADDRESS_TYPE);
            addArrayOfEmails(source, email.getBcc(),
                    PstMetadataTags.Email.BCC,
                    PstMetadataTags.Email.Bcc.NAME,
                    PstMetadataTags.Email.Bcc.ADDRESS,
                    PstMetadataTags.Email.Bcc.EMAIL_ADDRESS_TYPE);

            if (email.getAttachments() != null &&  !email.getAttachments().isEmpty()) {
                source.field(PstMetadataTags.Email.ATTACHMENTS).startArray();
                for (PstAttachmentSimpleContent listAttachment : email.getAttachments()) {
                    source.startObject();
                    source.field(PstMetadataTags.Email.Attachments.FILENAME,
                            listAttachment.getFileName());
                    source.field(PstMetadataTags.Email.Attachments.PATH,
                            listAttachment.getPath());
                    source.field(PstMetadataTags.Email.Attachments.SIZE,
                            listAttachment.getSize());
                    source.field(PstMetadataTags.Email.Attachments.MIME_TAG,
                            listAttachment.getMimeTag());
                    source.endObject();
                }
                source.endArray();
            }
            source.endObject();

            esIndex(_indexName, PstMetadataTags.INDEX_TYPE_EMAIL_MESSAGE, PstSignTool.sign(UUID.randomUUID().toString()).toString(), source);

        }catch(Exception ex){
            System.out.println(ex.toString());
            _logger.error(ex.toString());
        }
    }

    private void addArrayOfEmails(XContentBuilder builder, List<PstRecipientContent> list, String objectName, String nameTag, String addressTag, String addressTypeTag) throws IOException {
        if (list == null || list.isEmpty()) {
            return;
        }
        builder.field(objectName).startArray();
        for (PstRecipientContent item : list) {
            builder.startObject();
            builder.field(nameTag, item.getName());
            builder.field(addressTag, item.getAddress());
            builder.field(addressTypeTag, item.getEmailAddressType());
            builder.endObject();
        }
        builder.endArray();
    }


    private void saveAttachment(PstAttachmentContent attachment) throws IOException, Exception {
        if (attachment == null) {
            return;
        }

        try{
            String filename = attachment.getFilename();
            String pathname = attachment.getPath();
            long size = attachment.getSize();
            String mime = attachment.getMimetag();
            String entryid = attachment.getEntryid();
            StringBuilder strBuilder = new StringBuilder();

            String parsedContent = "";

            if(attachment.getContent() != null){
                byte[] byteBuffer = Base64.decode(attachment.getContent());
                parsedContent = _tika.parseToString(new BytesStreamInput(byteBuffer), new Metadata());
            }

            XContentBuilder source = jsonBuilder().startObject();

            if (_logger.isTraceEnabled()) {
                source.prettyPrint();
            }
            _logger.info(String.format("---- AttachmentFile => %s; Size => %d",filename, size / 1048576));
            source
                    .field(PstMetadataTags.Attachment.FILENAME, filename)
                    .field(PstMetadataTags.Attachment.PATH, pathname)
                    .field(PstMetadataTags.Attachment.SIZE, size)
                    .field(PstMetadataTags.Attachment.MIME_TAG, mime)
                    .field(PstMetadataTags.Attachment.ANALYZED_CONTENT,parsedContent)
                    .field(PstMetadataTags.Attachment.CONTENT, attachment.getContent())
                    .field(PstMetadataTags.Attachment.EMAIL_ID, attachment.getEmailid())
                    .field(PstMetadataTags.Attachment.ENTRYID, entryid)
                    .field(PstMetadataTags.Attachment.CREATED_DATE,attachment.getDatecreated())
                    .field(PstMetadataTags.Attachment.OUTLOOK_EMAIL_ID, attachment.getOutlookemailid())
                    .field(PstMetadataTags.Attachment.FILENAME_SUGGEST,filename);

            source.endObject();
            esIndex(_indexName, PstMetadataTags.INDEX_TYPE_ATTACHMENT, PstSignTool.sign(UUID.randomUUID().toString()).toString(), source);
        }catch(Exception ex){
            _logger.error(ex.getMessage());
        }

    }

}
