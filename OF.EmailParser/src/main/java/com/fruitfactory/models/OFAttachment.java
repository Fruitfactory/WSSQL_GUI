package com.fruitfactory.models;

import com.carrotsearch.ant.tasks.junit4.dependencies.com.google.gson.annotations.SerializedName;

import java.util.Date;

/**
 * Created by Yariki on 1/29/2017.
 */
public class OFAttachment {
    @SerializedName("entryid")
    public String entryid;

    @SerializedName("filename")
    public String filename;

    @SerializedName("path")
    public String path;

    @SerializedName("size")
    public long size;

    @SerializedName("mimetag")
    public String mimetag;

    @SerializedName("content")
    public String content;

    @SerializedName("analyzedcontent")
    public String analyzedcontent;

    @SerializedName("emailid")
    public String emailid;

    @SerializedName("outlookemailid")
    public String outlookemailid;

    @SerializedName("datecreated")
    public Date datecreated;

    @SerializedName("storeid")
    public String storeid;

    public OFAttachment() {
    }

    public String getEntryid() {
        return entryid;
    }

    public void setEntryid(String entryid) {
        this.entryid = entryid;
    }

    public String getFilename() {
        return filename;
    }

    public void setFilename(String filename) {
        this.filename = filename;
    }

    public String getPath() {
        return path;
    }

    public void setPath(String path) {
        this.path = path;
    }

    public long getSize() {
        return size;
    }

    public void setSize(long size) {
        this.size = size;
    }

    public String getMimetag() {
        return mimetag;
    }

    public void setMimetag(String mimetag) {
        this.mimetag = mimetag;
    }

    public String getContent() {
        return content;
    }

    public void setContent(String content) {
        this.content = content;
    }

    public String getAnalyzedcontent() {
        return analyzedcontent;
    }

    public void setAnalyzedcontent(String analyzedcontent) {
        this.analyzedcontent = analyzedcontent;
    }

    public String getEmailid() {
        return emailid;
    }

    public void setEmailid(String emailid) {
        this.emailid = emailid;
    }

    public String getOutlookemailid() {
        return outlookemailid;
    }

    public void setOutlookemailid(String outlookemailid) {
        this.outlookemailid = outlookemailid;
    }

    public Date getDatecreated() {
        return datecreated;
    }

    public void setDatecreated(Date datecreated) {
        this.datecreated = datecreated;
    }

    public String getStoreid() {
        return storeid;
    }

    public void setStoreid(String storeid) {
        this.storeid = storeid;
    }
}
