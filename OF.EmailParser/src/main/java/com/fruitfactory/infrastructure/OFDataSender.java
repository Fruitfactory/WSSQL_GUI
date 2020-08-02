package com.fruitfactory.infrastructure;

import com.fruitfactory.infrastructure.core.OFDataProcess;
import com.fruitfactory.interfaces.IOFDataRepositoryPipe;
import com.fruitfactory.metadata.OFMetadataTags;
import com.fruitfactory.models.*;
import com.google.gson.JsonObject;
import org.apache.http.HttpHost;
import org.elasticsearch.action.*;
import org.elasticsearch.action.bulk.BulkProcessor;
import org.elasticsearch.action.bulk.BulkRequest;
import org.elasticsearch.action.bulk.BulkResponse;
import org.elasticsearch.action.index.IndexRequest;
import org.elasticsearch.client.*;
import org.elasticsearch.common.unit.ByteSizeUnit;
import org.elasticsearch.common.unit.ByteSizeValue;
import org.elasticsearch.common.unit.TimeValue;
import org.elasticsearch.common.xcontent.XContentType;
import org.elasticsearch.threadpool.ThreadPool;
import org.jglue.fluentjson.JsonArrayBuilder;
import org.jglue.fluentjson.JsonBuilderFactory;
import org.jglue.fluentjson.JsonObjectBuilder;

import java.io.IOException;

import java.text.SimpleDateFormat;
import java.util.Collections;
import java.util.Date;
import java.util.List;
import java.util.Set;

import org.apache.log4j.Logger;
import org.springframework.cglib.beans.BulkBean;

/**
 * Created by Yariki on 2/5/2017.
 */
public class OFDataSender extends OFDataProcess {

    private final int COUNT_DOCUMENTS = 100;

    private Logger logger;

    RestHighLevelClient client;
    BulkRequest bulkRequest;


    private int countDocs = 0;
    

    public OFDataSender(IOFDataRepositoryPipe dataSource, String name, Logger logger) {
        super(dataSource,name);
        this.logger = logger;
        client = new RestHighLevelClient(
                RestClient.builder(
                        new HttpHost("localhost", 9200, "http")));
//        listener = getListener();
//        BulkProcessor.Builder builder = BulkProcessor.builder((request,bulkListener) -> client.bulkAsync(request,RequestOptions.DEFAULT, bulkListener),
//                listener);
//        builder.setBulkActions(100);
//        builder.setBulkSize(new ByteSizeValue(10L, ByteSizeUnit.MB));
//        builder.setConcurrentRequests(0);
//        builder.setFlushInterval(TimeValue.timeValueSeconds(10L));
//        bulkProcessor = builder.build();


    }

    @Override
    protected void processData(OFItemsContainer container) {
        indexEmail(container);
        indexAttachments(container);
        indexContact(container);
        try{

            if(container != null && container.getEmail() != null){
                logger.info(String.format("Processing: %s",container.getEmail().getSubject()));
            }else if(container != null && container.getAttachments() != null && container.getAttachments().size() > 0){
                logger.info(String.format("Processing: %s",container.getAttachments().get(0).getFilename()));
            }else if(container != null && container.getContact() != null){
                logger.info(String.format("Processing: %s",container.getContact().getEmailaddress1()));
            }    
        }catch(Exception e){
            logger.error(e.toString());
        }

    }

    private void indexContact(OFItemsContainer container) {
        if(container == null || container.getContact() == null){
            return;
        }
        try {
            OFContact contact = container.getContact();
            JsonObject json = JsonBuilderFactory.buildObject()
                    .add(OFMetadataTags.Contact.ITEM_FIRST_NAME, contact.getFirstname())
                    .add(OFMetadataTags.Contact.ITEM_LAST_NAME, contact.getLastname())
                    .add(OFMetadataTags.Contact.ITEM_EMAILADDRESS1, contact.getEmailaddress1())
                    .add(OFMetadataTags.Contact.ITEM_EMAILADDRESS2, contact.getEmailaddress2())
                    .add(OFMetadataTags.Contact.ITEM_EMAILADDRESS3, contact.getEmailaddress3())
                    .add(OFMetadataTags.Contact.ITEM_BUSINESSTELEPHONE, contact.getBusinesstelephone())
                    .add(OFMetadataTags.Contact.ITEM_HOMETELEPHONE, contact.getHometelephone())
                    .add(OFMetadataTags.Contact.ITEM_MOBILETELEPHONE, contact.getMobiletelephone())
                    .add(OFMetadataTags.Contact.ITEM_HOMEADDRESSCITY, contact.getHomeaddresscity())
                    .add(OFMetadataTags.Contact.ITEM_HOMEADDRESSCOUNTRY, contact.getHomeaddresscountry())
                    .add(OFMetadataTags.Contact.ITEM_HOMEADDRESSPOSTALCODE, contact.getHomeaddresspostalcode())
                    .add(OFMetadataTags.Contact.ITEM_HOMEADDRESSSTATE, contact.getHomeaddressstate())
                    .add(OFMetadataTags.Contact.ITEM_HOMEADRESSSTREET, contact.getHomeaddressstreet())
                    .add(OFMetadataTags.Contact.ITEM_HOMEADDRESSPOSTOFFICEBOX, contact.getHomeaddresspostofficebox())
                    .add(OFMetadataTags.Contact.ITEM_BUSINESSADDRESSCITY, contact.getBusinessaddresscity())
                    .add(OFMetadataTags.Contact.ITEM_BUSINESSADDRESSCOUTRY, contact.getBusinessaddresscountry())
                    .add(OFMetadataTags.Contact.ITEM_BUSINESSADDRESSSTATE, contact.getBusinessaddressstate())
                    .add(OFMetadataTags.Contact.ITEM_BUSINESSADDRESSSTREET, contact.getBusinessaddressstreet())
                    .add(OFMetadataTags.Contact.ITEM_BUSINESSADDRESSPOSTOFFICEBOX, contact.getHomeaddresspostofficebox())
                    .add(OFMetadataTags.Contact.ITEM_KEYWORD, contact.getKeyword())
                    .add(OFMetadataTags.Contact.ITEM_LOCATION, contact.getLocation())
                    .add(OFMetadataTags.Contact.ITEM_COMPANY_NAME, contact.getCompanyname())
                    .add(OFMetadataTags.Contact.ITEM_TITLE, contact.getTitle())
                    .add(OFMetadataTags.Contact.ITEM_DEPARTMENT_NAME, contact.getDepartmentname())
                    .add(OFMetadataTags.Contact.ITEM_MIDDLE_NAME, contact.getMiddlename())
                    .add(OFMetadataTags.Contact.ITEM_DISPLAY_NAME_PREFIX, contact.getDisplynameprefix())
                    .add(OFMetadataTags.Contact.ITEM_PROFESSION, contact.getProfession())
                    .add(OFMetadataTags.Contact.ITEM_NOTE, contact.getNote())
                    .add(OFMetadataTags.Contact.ITEM_HOME_ADDRESS, contact.getHomeaddress())
                    .add(OFMetadataTags.Contact.ITEM_WORK_ADDRESS, contact.getWorkaddress())
                    .add(OFMetadataTags.Contact.ITEM_OTHER_ADDRESS, contact.getOtheraddress())
                    .add(OFMetadataTags.Contact.ITEM_BIRTHDAY,  formatDate(contact.getBirthday()))
                    .add(OFMetadataTags.Contact.ENTRY_ID, contact.getEntryid())
                    .add(OFMetadataTags.Contact.ADDRESS_TYPE,contact.getAddresstype())
                    .add(OFMetadataTags.Contact.ITEM_FIRST_NAME_SUGGEST, contact.getFirstname())
                    .add(OFMetadataTags.Contact.ITEM_LAST_NAME_SUGGEST, contact.getLastname())
                    .add(OFMetadataTags.Contact.ITEM_NAME_SUGGEST,String.format("%s %s",contact.getFirstname(),contact.getLastname()).trim())
                    .add(OFMetadataTags.Contact.ITEM_EMAILADDRESS1_SUGGEST, contact.getEmailaddress1())
                    .add(OFMetadataTags.Contact.ITEM_EMAILADDRESS2_SUGGEST, contact.getEmailaddress2())
                    .add(OFMetadataTags.Contact.ITEM_EMAILADDRESS3_SUGGEST, contact.getEmailaddress3())
                    .add(OFMetadataTags.Contact.ITEM_STORE_ID,contact.getStoreid())
                    .getJson();

            index(OFMetadataTags.CONTACT_INDEX_NAME,json.toString());

        }catch (Exception ex){
            logger.error(ex.toString());
        }
    }

    private void indexAttachments(OFItemsContainer container) {
        if(container == null || container.getAttachments() == null || container.getAttachments().isEmpty()){
            return;
        }
        try {
            for (OFAttachment attachment :
                    container.getAttachments()) {
                saveAttachment(attachment);
            }
        }catch (Exception ex){
            logger.error(ex.toString());
        }
    }

    private void saveAttachment(OFAttachment attachment) {
        if((attachment == null)){
            return;
        }
        try {
            JsonObject json = JsonBuilderFactory.buildObject()
                    .add(OFMetadataTags.Attachment.FILENAME, attachment.getFilename())
                    .add(OFMetadataTags.Attachment.PATH, attachment.getPath())
                    .add(OFMetadataTags.Attachment.SIZE, attachment.getSize())
                    .add(OFMetadataTags.Attachment.MIME_TAG, attachment.getMimetag())
                    .add(OFMetadataTags.Attachment.ANALYZED_CONTENT,attachment.getAnalyzedcontent())
                    .add(OFMetadataTags.Attachment.CONTENT, attachment.getContent())
                    .add(OFMetadataTags.Attachment.EMAIL_ID, attachment.getEmailid())
                    .add(OFMetadataTags.Attachment.ENTRYID, attachment.getEntryid())
                    .add(OFMetadataTags.Attachment.CREATED_DATE,formatDate(attachment.getDatecreated()))
                    .add(OFMetadataTags.Attachment.OUTLOOK_EMAIL_ID, attachment.getOutlookemailid())
                    .add(OFMetadataTags.Attachment.FILENAME_SUGGEST,attachment.getFilename())
                    .add(OFMetadataTags.Attachment.STORE_ID,attachment.getStoreid())
                    .getJson();

            index(OFMetadataTags.ATTACHMENT_INDEX_NAME,json.toString());

        }catch(Exception e){
            logger.error(e.toString());
        }
    }

    private void indexEmail(OFItemsContainer container) {
        if(container.getEmail() == null){
            return;
        }
        OFEmail email = container.getEmail();

        try {
            JsonObjectBuilder json =  JsonBuilderFactory.buildObject()
                    .add(OFMetadataTags.Email.ITEM_NAME, email.getSubject())
                    .add(OFMetadataTags.Email.ITEM_URL, email.getSubject())
                    .add(OFMetadataTags.Email.ITEM_NAME_DISPLAY, email.getSubject())
                    .add(OFMetadataTags.Email.FOLDER, email.getFolder())
                    .add(OFMetadataTags.Email.STORAGE_NAME, email.getStoragename())
                    .add(OFMetadataTags.Email.FOLDER_MESSAGE_STORE_ID_PART, email.getFoldermessagestoreidpart())
                    .add(OFMetadataTags.Email.DATE_CREATED, formatDate(email.getDatecreated()))
                    .add(OFMetadataTags.Email.DATE_RECEIVED, formatDate(email.getDatereceived()))
                    .add(OFMetadataTags.Email.SIZE, email.getSize())
                    .add(OFMetadataTags.Email.CONVERSATION_ID, email.getConversationid())
                    .add(OFMetadataTags.Email.CONVERSATION_INDEX, email.getConversationindex())
                    .add(OFMetadataTags.Email.OUTLOOK_CONVERSATION_ID, email.getOutlookconversationid())
                    .add(OFMetadataTags.Email.SUBJECT, email.getSubject())
                    .add(OFMetadataTags.Email.CONTENT, email.getContent())
                    .add(OFMetadataTags.Email.HTML_CONTENT, email.getHtmlcontent())
                    .add(OFMetadataTags.Email.ANALYZED_CONTENT,email.getAnalyzedcontent())
                    .add(OFMetadataTags.Email.HAS_ATTACHMENTS, email.getHasattachments())
                    .add(OFMetadataTags.Email.FROM_NAME, email.getFromname())
                    .add(OFMetadataTags.Email.FROM_ADDRESS, email.getFromaddress())
                    .add(OFMetadataTags.Email.ENTRY_ID, email.getEntryid())
                    .add(OFMetadataTags.Email.FROM_NAME_SUGGEST,email.getFromname())
                    .add(OFMetadataTags.Email.FROM_ADDRESS_SUGGEST,email.getFromaddress())
                    .add(OFMetadataTags.Email.SUBJECT_SUGGEST,email.getSubject())
                    .add(OFMetadataTags.Email.STOREID,email.getStoreid());
            AddArrayOfEmail(json, email.getTo(),OFMetadataTags.Email.TO,OFMetadataTags.Email.To.NAME,OFMetadataTags.Email.To.ADDRESS,OFMetadataTags.Email.To.EMAIL_ADDRESS_TYPE);
            AddArrayOfEmail(json, email.getCc(),OFMetadataTags.Email.CC,OFMetadataTags.Email.Cc.NAME,OFMetadataTags.Email.Cc.ADDRESS,OFMetadataTags.Email.Cc.EMAIL_ADDRESS_TYPE);
            AddArrayOfEmail(json, email.getBcc(),OFMetadataTags.Email.BCC,OFMetadataTags.Email.Bcc.NAME,OFMetadataTags.Email.Bcc.ADDRESS,OFMetadataTags.Email.Bcc.EMAIL_ADDRESS_TYPE);

            if(email.getAttachments()  != null && !email.getAttachments().isEmpty()){
                JsonArrayBuilder array = json.addArray(OFMetadataTags.Email.ATTACHMENTS);
                for (OFAttachmentSimple attachment :
                        email.getAttachments()) {
                    JsonObjectBuilder object = array.addObject();
                    object
                            .add(OFMetadataTags.Email.Attachments.FILENAME,attachment.getFilename())
                            .add(OFMetadataTags.Email.Attachments.PATH,attachment.getPath())
                            .add(OFMetadataTags.Email.Attachments.SIZE,attachment.getSize())
                            .add(OFMetadataTags.Email.Attachments.MIME_TAG,attachment.getMimetag())
                            .end();
                }
                array.end();
            }

            JsonObject objectJson = (JsonObject) json.getJson();

            index(OFMetadataTags.INDEX_TYPE_EMAIL_MESSAGE,objectJson.toString());

        }catch (Exception ex){
            logger.error(ex.toString());
        }
    }

    private String formatDate(Date date){
        return date == null ? null : new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss.SSS").format(date);
    }

    private void AddArrayOfEmail(JsonObjectBuilder builder, List<OFRecipient> list, String objectName,String nameTag, String addresssTag,String addressTypeTag){
        if(list == null || list.isEmpty()){
            return;
        }
        JsonArrayBuilder array =  builder.addArray(objectName);
        for (OFRecipient recipient: list) {
            JsonObjectBuilder object = array.addObject();
            object
                    .add(nameTag,recipient.getName())
                    .add(addresssTag,recipient.getAddress())
                    .add(addressTypeTag,recipient.getEmailAddressType())
                    .end();

        }
        array.end();
    }

    private void index(String indexName, String json){
        try {
            bulkRequest.add(new IndexRequest(indexName).source(XContentType.JSON,json));
            processBulkRequest();
        }catch (Exception ex){
            logger.error(ex.toString());
        }
    }

    private void processBulkRequest() throws IOException {
        if(bulkRequest.numberOfActions() < COUNT_DOCUMENTS) return;
        try {
            BulkResponse response = client.bulk(bulkRequest,RequestOptions.DEFAULT);
            if(response.hasFailures()){
                logger.warn("Bulk request has finished with some failures..." + response.buildFailureMessage());
            }else{
                logger.debug("Bulk request completed in " + response.getTookInMillis()+ " ms");
            }
            bulkRequest = new BulkRequest();
        }catch (Exception ex){
            logger.error(ex.getMessage());
        }
    }


}
