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
    
    public static final String INDEX_TYPE_EMAIL_MESSAGE = "email";
    public static final String INDEX_TYPE_FOLDER = "folder";
    public static final String INDEX_TYPE_CONTACT = "contact";
    public static final String INDEX_TYPE_CALENDAR = "calendar";
    
    public static final class Email{
        public static final String SUBJECT = "subject";
        public static final String SENDER = "sender";
        public static final String SENDER_EMAIL_ADDRESS = "senderemailaddress";
        public static final String BODY = "body";
        public static final String HAS_ATTACHMENTS = "hasattachments";
    }
    
    public static XContentBuilder buildPstEmailMapping() throws IOException{
        XContentBuilder mapping = jsonBuilder().prettyPrint().startObject();
        mapping.startObject(INDEX_TYPE_EMAIL_MESSAGE);
        mapping.startObject("properties");
        addAnalyzedString(mapping, Email.SUBJECT);
        addAnalyzedString(mapping, Email.SENDER);
        addAnalyzedString(mapping, Email.SENDER_EMAIL_ADDRESS);
        addAnalyzedString(mapping, Email.BODY);
        addNotAnalyzedString(mapping, Email.HAS_ATTACHMENTS);
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
