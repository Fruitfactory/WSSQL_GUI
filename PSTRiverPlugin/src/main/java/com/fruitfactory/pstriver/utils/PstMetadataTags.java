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
    
    public static final class Email{
        public static final String ITEM_NAME = "itemname";
        public static final String ITEM_URL = "itemurl";
        public static final String FOLDER = "folder";
        public static final String DATE_CREATED = "datecreated";
        public static final String DATE_RECEIVED = "datereceived";
        public static final String SIZE = "size";
        public static final String CONVERSATION_ID = "conversationid";
        public static final String CONVERSATION_INDEX = "conversationindex";
        public static final String SUBJECT = "subject";
        public static final String CONTENT = "content";
        public static final String HAS_ATTACHMENTS = "hasattachments";
        public static final String FROM_NAME = "fromname";
        public static final String FROM_ADDRESS = "fromaddress";
        
        public static final String TO = "to";
        public static final class To{
            public static final String NAME = "name";
            public static final String ADDRESS = "address";
        }
        
        public static final String CC = "cc";
        public static final class Cc{
            public static final String NAME = "name";
            public static final String ADDRESS = "address";
        }
        public static final String BCC = "bcc";
        public static final class Bcc{
            public static final String NAME = "name";
            public static final String ADDRESS = "address";
        }
        
        public static final String ATTACHMENTS = "attachments";
        public static final class Attachments{
            public static final String FILENAME = "filename";
            public static final String PATH = "path";
            public static final String SIZE = "size";
            public static final String MIME_TAG = "mimetag";
        }
    }
    
    public static XContentBuilder buildPstEmailMapping() throws IOException{
        XContentBuilder mapping = jsonBuilder().prettyPrint().startObject();
        mapping.startObject(INDEX_TYPE_EMAIL_MESSAGE);
        mapping.startObject(PROPERTIES);
            addAnalyzedString(mapping, Email.ITEM_NAME);
            addAnalyzedString(mapping, Email.ITEM_URL);
            addAnalyzedString(mapping, Email.FOLDER);
            addDate(mapping, Email.DATE_CREATED);
            addDate(mapping, Email.DATE_RECEIVED);
            addLong(mapping, Email.SIZE);
            addNotAnalyzedString(mapping, Email.CONVERSATION_ID);
            addNotAnalyzedString(mapping, Email.CONVERSATION_INDEX);
            addAnalyzedString(mapping, Email.SUBJECT);
            addAnalyzedString(mapping, Email.CONTENT);
            addNotAnalyzedString(mapping, Email.HAS_ATTACHMENTS);
            addAnalyzedString(mapping, Email.FROM_NAME);
            addAnalyzedString(mapping, Email.FROM_ADDRESS);
            
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
                .endObject();
    }
    
    
}
