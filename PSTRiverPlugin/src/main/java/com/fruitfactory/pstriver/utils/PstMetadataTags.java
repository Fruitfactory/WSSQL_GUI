/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package com.fruitfactory.pstriver.utils;

import java.io.IOException;
import org.elasticsearch.common.xcontent.XContentBuilder;
import org.elasticsearch.common.xcontent.XContentFactory;
import static org.elasticsearch.common.xcontent.XContentFactory.jsonBuilder;

/**
 *
 * @author Yariki
 */
public class PstMetadataTags {

    private static final String PROPERTIES = "properties";

    public static final String INDEX_TYPE_EMAIL_MESSAGE = "email";
    public static final String INDEX_TYPE_FOLDER = "folder";
    public static final String INDEX_TYPE_CONTACT = "contact";
    public static final String INDEX_TYPE_CALENDAR = "calendar";
    public static final String INDEX_TYPE_ATTACHMENT = "attachment";

    public static final class Email {

        public static final String ITEM_NAME = "itemname";
        public static final String ITEM_URL = "itemurl";
        public static final String ITEM_NAME_DISPLAY = "itemnamedisplay";
        public static final String FOLDER = "folder";
        public static final String STORE_DISPLAY_NAME = "storedisplayname";
        public static final String FOLDER_MESSAGE_STORE = "foldermessagestore";
        public static final String STORAGE_NAME = "storagename";
        public static final String DATE_CREATED = "datecreated";
        public static final String DATE_RECEIVED = "datereceived";
        public static final String SIZE = "size";
        public static final String CONVERSATION_ID = "conversationid";
        public static final String OUTLOOK_CONVERSATION_ID = "outlookconversationid";
        public static final String CONVERSATION_INDEX = "conversationindex";
        public static final String SUBJECT = "subject";
        public static final String CONTENT = "content";
        public static final String HTML_CONTENT = "htmlcontent";
        public static final String ANALYZED_CONTENT = "analyzedcontent";
        public static final String HAS_ATTACHMENTS = "hasattachments";
        public static final String FROM_NAME = "fromname";
        public static final String FROM_ADDRESS = "fromaddress";
        public static final String ENTRY_ID = "entryid";

        public static final String TO = "to";

        public static final class To {

            public static final String NAME = "name";
            public static final String ADDRESS = "address";
            public static final String EMAIL_ADDRESS_TYPE = "emailaddresstype";
        }

        public static final String CC = "cc";

        public static final class Cc {

            public static final String NAME = "name";
            public static final String ADDRESS = "address";
            public static final String EMAIL_ADDRESS_TYPE = "emailaddresstype";
        }
        public static final String BCC = "bcc";

        public static final class Bcc {

            public static final String NAME = "name";
            public static final String ADDRESS = "address";
            public static final String EMAIL_ADDRESS_TYPE = "emailaddresstype";
        }

        public static final String ATTACHMENTS = "attachments";

        public static final class Attachments {

            public static final String FILENAME = "filename";
            public static final String PATH = "path";
            public static final String SIZE = "size";
            public static final String MIME_TAG = "mimetag";
        }
    }

    public static class Attachment {

        public static final String FILENAME = "filename";
        public static final String PATH = "path";
        public static final String SIZE = "size";
        public static final String MIME_TAG = "mimetag";
        public static final String ANALYZED_CONTENT = "analyzedcontent";
        public static final String CONTENT = "content";
        public static final String EMAIL_ID = "emailid";
        public static final String ENTRYID = "entryid";
    }

    public static XContentBuilder buildPstEmailMapping() throws IOException {
        XContentBuilder mapping = jsonBuilder().prettyPrint().startObject();
        mapping.startObject(INDEX_TYPE_EMAIL_MESSAGE);
        mapping.startObject(PROPERTIES);
        addAnalyzedString(mapping, Email.ITEM_NAME);
        addAnalyzedString(mapping, Email.ITEM_URL);
        addAnalyzedString(mapping, Email.ITEM_NAME_DISPLAY);
        addAnalyzedString(mapping, Email.FOLDER);
        addAnalyzedString(mapping, Email.STORE_DISPLAY_NAME);
        addDate(mapping, Email.DATE_CREATED);
        addDate(mapping, Email.DATE_RECEIVED);
        addLong(mapping, Email.SIZE);
        addNotAnalyzedString(mapping, Email.CONVERSATION_ID);
        addNotAnalyzedString(mapping, Email.CONVERSATION_INDEX);
        addNotAnalyzedString(mapping, Email.OUTLOOK_CONVERSATION_ID);
        addAnalyzedString(mapping, Email.SUBJECT);
        addNotIndexedString(mapping, Email.CONTENT);
        addNotIndexedString(mapping, Email.HTML_CONTENT);
        addAnalyzedString(mapping, Email.ANALYZED_CONTENT);
        addNotAnalyzedString(mapping, Email.HAS_ATTACHMENTS);
        addAnalyzedString(mapping, Email.FROM_NAME);
        addAnalyzedString(mapping, Email.FROM_ADDRESS);
        addNotAnalyzedString(mapping, Email.ENTRY_ID);

        mapping.startObject(Email.TO);
        mapping.startObject(PROPERTIES);
        addAnalyzedString(mapping, Email.To.NAME);
        addAnalyzedString(mapping, Email.To.ADDRESS);
        mapping.endObject().endObject();

        mapping.startObject(Email.CC);
        mapping.startObject(PROPERTIES);
        addAnalyzedString(mapping, Email.Cc.NAME);
        addAnalyzedString(mapping, Email.Cc.ADDRESS);
        mapping.endObject().endObject();

        mapping.startObject(Email.BCC);
        mapping.startObject(PROPERTIES);
        addAnalyzedString(mapping, Email.Bcc.NAME);
        addAnalyzedString(mapping, Email.Bcc.ADDRESS);
        mapping.endObject().endObject();

        mapping.startObject(Email.ATTACHMENTS);
        mapping.startObject(PROPERTIES);
        addAnalyzedString(mapping, Email.Attachments.FILENAME);
        addAnalyzedString(mapping, Email.Attachments.PATH);
        addLong(mapping, Email.Attachments.SIZE);
        addAnalyzedString(mapping, Email.Attachments.MIME_TAG);
        mapping.endObject().endObject();

        mapping.endObject();
        mapping.endObject();
        mapping.endObject();
        return mapping;
    }

    public static XContentBuilder buildPstAttachmentMapping() throws IOException {
        XContentBuilder mapping = jsonBuilder().prettyPrint().startObject();
        mapping.startObject(INDEX_TYPE_ATTACHMENT);
        mapping.startObject(PROPERTIES);
        addAnalyzedString(mapping, Attachment.FILENAME);
        addNotAnalyzedString(mapping, Attachment.PATH);
        addLong(mapping, Attachment.SIZE);
        addNotAnalyzedString(mapping, Attachment.MIME_TAG);
        addAnalyzedString(mapping, Attachment.ANALYZED_CONTENT);
        addBinary(mapping, Attachment.CONTENT);
        addNotAnalyzedString(mapping, Attachment.EMAIL_ID);
        addNotAnalyzedString(mapping, Attachment.ENTRYID);
        mapping.endObject();
        mapping.endObject();
        mapping.endObject();
        return mapping;
    }

    public static final class Contact {

        public static final String ITEM_FIRST_NAME = "firstname";
        public static final String ITEM_LAST_NAME = "lastname";
        public static final String ITEM_EMAILADDRESS1 = "emailaddress1";
        public static final String ITEM_EMAILADDRESS2 = "emailaddress2";
        public static final String ITEM_EMAILADDRESS3 = "emailaddress3";
        public static final String ITEM_BUSINESSTELEPHONE = "businesstelephone";
        public static final String ITEM_HOMETELEPHONE = "hometelephone";
        public static final String ITEM_MOBILETELEPHONE = "mobiletelephone";

        public static final String ITEM_HOMEADDRESSCITY = "homeaddresscity";
        public static final String ITEM_HOMEADDRESSCOUNTRY = "homeaddresscountry";
        public static final String ITEM_HOMEADDRESSPOSTALCODE = "homeaddresspostalcode";
        public static final String ITEM_HOMEADDRESSSTATE = "homeaddressstate";
        public static final String ITEM_HOMEADRESSSTREET = "homeaddressstreet";
        public static final String ITEM_HOMEADDRESSPOSTOFFICEBOX = "homeaddresspostofficebox";

        public static final String ITEM_BUSINESSADDRESSCITY = "businessaddresscity";
        public static final String ITEM_BUSINESSADDRESSCOUTRY = "businessaddresscountry";
        public static final String ITEM_BUSINESSADDRESSSTATE = "businessaddressstate";
        public static final String ITEM_BUSINESSADDRESSSTREET = "businessaddressstreet";
        public static final String ITEM_BUSINESSADDRESSPOSTOFFICEBOX = "businessaddresspostofficebox";

        public static final String ITEM_KEYWORD = "keyword";
        public static final String ITEM_LOCATION = "location";
        public static final String ITEM_COMPANY_NAME = "companyname";
        public static final String ITEM_TITLE = "title";
        public static final String ITEM_DEPARTMENT_NAME = "departmentname";
        public static final String ITEM_MIDDLE_NAME = "middlename";
        public static final String ITEM_DISPLAY_NAME_PREFIX = "displaynameprefix";
        public static final String ITEM_PROFESSION = "profession";
        public static final String ITEM_NOTE = "note";

        public static final String ITEM_HOME_ADDRESS = "homeaddress";
        public static final String ITEM_WORK_ADDRESS = "workaddress";
        public static final String ITEM_OTHER_ADDRESS = "otheraddress";

        public static final String ITEM_BIRTHDAY = "birthday";
        public static final String ENTRY_ID = "entryid";
        public static final String ADDRESS_TYPE = "addresstype";

    }

    public static final class Appointment {

        public static final String ITEM_LOCATION = "location";
        public static final String ITEM_STARTTIME = "starttime";
        public static final String ITEM_ENDTIME = "endtime";
        public static final String ITEM_TIMEZONE = "timezone";
        public static final String ITEM_DURATION = "duration";
        public static final String ITEM_MEETING_STATUS = "meetingstatus";
        public static final String ITEM_ALL_ATTENDEES = "allattendees";
        public static final String ITEM_TO_ATTENDEES = "toattendees";
        public static final String ITEM_CC_ATTENDEES = "ccattendees";
        public static final String ITEM_ISONLINE_MEETING = "isonlinemeeting";
        public static final String ITEM_NETMEETING_SERVER = "netmeetingserver";
        public static final String ITEM_NETMEETING_DOCUMENT_PATH = "meetingdocumentpath";
        public static final String ITEM_NETSHOW_URL = "netshowurl";
        public static final String ITEM_REQUIRED_ATTENDEES = "requiredattendees";
        public static final String ENTRY_ID = "entryid";

    }

    public static XContentBuilder buildPstContactMapping() throws IOException {
        XContentBuilder mapping = jsonBuilder().prettyPrint().startObject();
        mapping.startObject(INDEX_TYPE_CONTACT);
        mapping.startObject(PROPERTIES);
        addAnalyzedString(mapping, Contact.ITEM_FIRST_NAME);
        addAnalyzedString(mapping, Contact.ITEM_LAST_NAME);
        addAnalyzedString(mapping, Contact.ITEM_EMAILADDRESS1);
        addAnalyzedString(mapping, Contact.ITEM_EMAILADDRESS2);
        addAnalyzedString(mapping, Contact.ITEM_EMAILADDRESS3);

        addNotAnalyzedString(mapping, Contact.ITEM_BUSINESSTELEPHONE);
        addNotAnalyzedString(mapping, Contact.ITEM_HOMETELEPHONE);
        addNotAnalyzedString(mapping, Contact.ITEM_MOBILETELEPHONE);

        addAnalyzedString(mapping, Contact.ITEM_HOMEADDRESSCITY);
        addAnalyzedString(mapping, Contact.ITEM_HOMEADDRESSCOUNTRY);
        addAnalyzedString(mapping, Contact.ITEM_HOMEADDRESSPOSTALCODE);
        addAnalyzedString(mapping, Contact.ITEM_HOMEADDRESSPOSTOFFICEBOX);
        addAnalyzedString(mapping, Contact.ITEM_HOMEADDRESSSTATE);
        addAnalyzedString(mapping, Contact.ITEM_HOMEADRESSSTREET);

        addAnalyzedString(mapping, Contact.ITEM_BUSINESSADDRESSCITY);
        addAnalyzedString(mapping, Contact.ITEM_BUSINESSADDRESSCOUTRY);
        addAnalyzedString(mapping, Contact.ITEM_BUSINESSADDRESSPOSTOFFICEBOX);
        addAnalyzedString(mapping, Contact.ITEM_BUSINESSADDRESSSTATE);
        addAnalyzedString(mapping, Contact.ITEM_BUSINESSADDRESSSTREET);

        addAnalyzedString(mapping, Contact.ITEM_KEYWORD);
        addAnalyzedString(mapping, Contact.ITEM_LOCATION);
        addAnalyzedString(mapping, Contact.ITEM_COMPANY_NAME);
        addNotAnalyzedString(mapping, Contact.ITEM_TITLE);
        addAnalyzedString(mapping, Contact.ITEM_DEPARTMENT_NAME);
        addAnalyzedString(mapping, Contact.ITEM_MIDDLE_NAME);
        addNotAnalyzedString(mapping, Contact.ITEM_DISPLAY_NAME_PREFIX);
        addAnalyzedString(mapping, Contact.ITEM_PROFESSION);
        addAnalyzedString(mapping, Contact.ITEM_NOTE);

        addAnalyzedString(mapping, Contact.ITEM_HOME_ADDRESS);
        addAnalyzedString(mapping, Contact.ITEM_WORK_ADDRESS);
        addAnalyzedString(mapping, Contact.ITEM_OTHER_ADDRESS);

        addDate(mapping, Contact.ITEM_BIRTHDAY);
        addNotAnalyzedString(mapping, Contact.ENTRY_ID);

        mapping.endObject();
        mapping.endObject();
        mapping.endObject();

        return mapping;
    }

    public static XContentBuilder buildPstAppointmentMapping() throws IOException {
        XContentBuilder mapping = jsonBuilder().prettyPrint().startObject();
        mapping.startObject(INDEX_TYPE_CALENDAR);
        mapping.startObject(PROPERTIES);
        addAnalyzedString(mapping, Appointment.ITEM_LOCATION);
        addDate(mapping, Appointment.ITEM_STARTTIME);
        addDate(mapping, Appointment.ITEM_ENDTIME);
        addNotAnalyzedString(mapping, Appointment.ITEM_TIMEZONE);
        addLong(mapping, Appointment.ITEM_DURATION);
        addLong(mapping, Appointment.ITEM_MEETING_STATUS);
        addAnalyzedString(mapping, Appointment.ITEM_ALL_ATTENDEES);
        addAnalyzedString(mapping, Appointment.ITEM_TO_ATTENDEES);
        addAnalyzedString(mapping, Appointment.ITEM_CC_ATTENDEES);
        addNotAnalyzedString(mapping, Appointment.ITEM_ISONLINE_MEETING);
        addNotIndexedString(mapping, Appointment.ITEM_NETMEETING_SERVER);
        addNotIndexedString(mapping, Appointment.ITEM_NETMEETING_DOCUMENT_PATH);
        addNotAnalyzedString(mapping, Appointment.ITEM_NETSHOW_URL);
        addNotAnalyzedString(mapping, Appointment.ITEM_REQUIRED_ATTENDEES);
        addNotAnalyzedString(mapping, Appointment.ENTRY_ID);

        mapping.endObject();
        mapping.endObject();
        mapping.endObject();

        return mapping;
    }

    private static void addAnalyzedString(XContentBuilder xcb, String fieldName) throws IOException {
        xcb.startObject(fieldName)
                .field("type", "string")
                .field("store", "yes")
                .endObject();
    }

    private static void addNotAnalyzedString(XContentBuilder xcb, String fieldName) throws IOException {
        xcb.startObject(fieldName)
                .field("type", "string")
                .field("store", "yes")
                .field("index", "not_analyzed")
                .endObject();
    }

    private static void addNotIndexedString(XContentBuilder xcb, String fieldName) throws IOException {
        xcb.startObject(fieldName)
                .field("type", "string")
                .field("store", "yes")
                .field("index", "no")
                .endObject();
    }

    private static void addDate(XContentBuilder xcb, String fieldName) throws IOException {
        xcb.startObject(fieldName)
                .field("type", "date")
                .field("format", "dateOptionalTime")
                .field("store", "yes")
                .endObject();
    }

    private static void addLong(XContentBuilder xcb, String fieldName) throws IOException {
        xcb.startObject(fieldName)
                .field("type", "long")
                .field("store", "yes")
                .endObject();
    }

    private static void addBinary(XContentBuilder xcb, String fieldName) throws IOException {
        xcb.startObject(fieldName)
                .field("type", "binary")
                .field("store", "yes")
                .endObject();
    }

}
