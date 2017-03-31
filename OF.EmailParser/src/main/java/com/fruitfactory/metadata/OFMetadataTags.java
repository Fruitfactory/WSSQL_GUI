package com.fruitfactory.metadata;

/**
 * Created by Yariki on 2/5/2017.
 */
public class OFMetadataTags {

    private static final String PROPERTIES = "properties";

    public static final String INDEX_NAME = "outlookfinder";
    public static final String INDEX_TYPE_EMAIL_MESSAGE = "email";
    public static final String INDEX_TYPE_FOLDER = "folder";
    public static final String INDEX_TYPE_CONTACT = "contact";
    public static final String INDEX_TYPE_CALENDAR = "calendar";
    public static final String INDEX_TYPE_ATTACHMENT = "attachment";
    public static final String INDEX_TYPE_STORE = "store";

    public static final class Email {

        public static final String ITEM_NAME = "itemname";
        public static final String ITEM_URL = "itemurl";
        public static final String ITEM_NAME_DISPLAY = "itemnamedisplay";
        public static final String FOLDER = "folder";
        public static final String FOLDER_MESSAGE_STORE_ID_PART = "foldermessagestoreidpart";
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
        public static final String FROM_NAME_SUGGEST = "fromnamesuggest";
        public static final String FROM_ADDRESS_SUGGEST = "fromaddresssuggest";
        public static final String SUBJECT_SUGGEST = "subjectsuggest";
        public static final String STOREID = "storeid";

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
        public static final String OUTLOOK_EMAIL_ID = "outlookemailid";
        public static final String CREATED_DATE = "datecreated";
        public static final String FILENAME_SUGGEST = "filenamesuggest";
        public static final String STORE_ID = "storeid";
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

        public static final String ITEM_FIRST_NAME_SUGGEST = "firstnamesuggest";
        public static final String ITEM_LAST_NAME_SUGGEST = "lastnamesuggest";
        public static final String ITEM_NAME_SUGGEST = "namesuggest";
        public static final String ITEM_EMAILADDRESS1_SUGGEST = "emailaddress1suggest";
        public static final String ITEM_EMAILADDRESS2_SUGGEST = "emailaddress2suggest";
        public static final String ITEM_EMAILADDRESS3_SUGGEST = "emailaddress3suggest";
        public static final String ITEM_STORE_ID = "storeid";

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

    public static final class Store{
        public static final String ITEM_NAME = "name";
        public static final String ITEM_STORE_ID = "storeid";
    }

}
