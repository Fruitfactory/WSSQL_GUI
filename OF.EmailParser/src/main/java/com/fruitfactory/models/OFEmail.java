package com.fruitfactory.models;

import com.carrotsearch.ant.tasks.junit4.dependencies.com.google.gson.annotations.SerializedName;

import java.util.Date;
import java.util.List;

/**
 * Created by Yariki on 1/29/2017.
 */
public class OFEmail {
    @SerializedName("entryid")
    public String entryid;

    @SerializedName("itemName")
    public String itemName;

    @SerializedName("itemUrl")
    public String itemUrl;

    @SerializedName("folder")
    public String folder;

    @SerializedName("foldermessagestoreidpart")
    public String foldermessagestoreidpart;

    @SerializedName("storagename")
    public String storagename;

    @SerializedName("datecreated")
    public Date datecreated;

    @SerializedName("datereceived")
    public Date datereceived;

    @SerializedName("size")
    public long size;

    @SerializedName("conversationid")
    public String conversationid;

    @SerializedName("conversationindex")
    public String conversationindex;

    @SerializedName("outlookconversationid")
    public String outlookconversationid;

    @SerializedName("subject")
    public String subject;

    @SerializedName("content")
    public String content;

    @SerializedName("htmlcontent")
    public String htmlcontent;

    @SerializedName("analyzedcontent")
    public String analyzedcontent;

    @SerializedName("hasattachments")
    public String hasattachments;

    @SerializedName("fromname")
    public String fromname;

    @SerializedName("fromaddress")
    public String fromaddress;

    @SerializedName("storeid")
    public String storeid;

    @SerializedName("to")
    public List<OFRecipient> to;

    @SerializedName("cc")
    public List<OFRecipient> cc;

    @SerializedName("bcc")
    public List<OFRecipient> bcc;

    @SerializedName("attachments")
    public List<OFAttachmentSimple> attachments;

    public OFEmail() {
    }

    public String getEntryid() {
        return entryid;
    }

    public void setEntryid(String entryid) {
        this.entryid = entryid;
    }

    public String getItemName() {
        return itemName;
    }

    public void setItemName(String itemName) {
        this.itemName = itemName;
    }

    public String getItemUrl() {
        return itemUrl;
    }

    public void setItemUrl(String itemUrl) {
        this.itemUrl = itemUrl;
    }

    public String getFolder() {
        return folder;
    }

    public void setFolder(String folder) {
        this.folder = folder;
    }

    public String getFoldermessagestoreidpart() {
        return foldermessagestoreidpart;
    }

    public void setFoldermessagestoreidpart(String foldermessagestoreidpart) {
        this.foldermessagestoreidpart = foldermessagestoreidpart;
    }

    public String getStoragename() {
        return storagename;
    }

    public void setStoragename(String storagename) {
        this.storagename = storagename;
    }

    public Date getDatecreated() {
        return datecreated;
    }

    public void setDatecreated(Date datecreated) {
        this.datecreated = datecreated;
    }

    public Date getDatereceived() {
        return datereceived;
    }

    public void setDatereceived(Date datereceived) {
        this.datereceived = datereceived;
    }

    public long getSize() {
        return size;
    }

    public void setSize(long size) {
        this.size = size;
    }

    public String getConversationid() {
        return conversationid;
    }

    public void setConversationid(String conversationid) {
        this.conversationid = conversationid;
    }

    public String getConversationindex() {
        return conversationindex;
    }

    public void setConversationindex(String conversationindex) {
        this.conversationindex = conversationindex;
    }

    public String getOutlookconversationid() {
        return outlookconversationid;
    }

    public void setOutlookconversationid(String outlookconversationid) {
        this.outlookconversationid = outlookconversationid;
    }

    public String getSubject() {
        return subject;
    }

    public void setSubject(String subject) {
        this.subject = subject;
    }

    public String getContent() {
        return content;
    }

    public void setContent(String content) {
        this.content = content;
    }

    public String getHtmlcontent() {
        return htmlcontent;
    }

    public void setHtmlcontent(String htmlcontent) {
        this.htmlcontent = htmlcontent;
    }

    public String getAnalyzedcontent() {
        return analyzedcontent;
    }

    public void setAnalyzedcontent(String analyzedcontent) {
        this.analyzedcontent = analyzedcontent;
    }

    public String getHasattachments() {
        return hasattachments;
    }

    public void setHasattachments(String hasattachments) {
        this.hasattachments = hasattachments;
    }

    public String getFromname() {
        return fromname;
    }

    public void setFromname(String fromname) {
        this.fromname = fromname;
    }

    public String getFromaddress() {
        return fromaddress;
    }

    public void setFromaddress(String fromaddress) {
        this.fromaddress = fromaddress;
    }

    public List<OFRecipient> getTo() {
        return to;
    }

    public void setTo(List<OFRecipient> to) {
        this.to = to;
    }

    public List<OFRecipient> getCc() {
        return cc;
    }

    public void setCc(List<OFRecipient> cc) {
        this.cc = cc;
    }

    public List<OFRecipient> getBcc() {
        return bcc;
    }

    public void setBcc(List<OFRecipient> bcc) {
        this.bcc = bcc;
    }

    public List<OFAttachmentSimple> getAttachments() {
        return attachments;
    }

    public void setAttachments(List<OFAttachmentSimple> attachments) {
        this.attachments = attachments;
    }

    public String getStoreid() {
        return storeid;
    }

    public void setStoreid(String storeid) {
        this.storeid = storeid;
    }


}
