package com.fruitfactory.pstriver.rest.data;

import com.carrotsearch.ant.tasks.junit4.dependencies.com.google.gson.annotations.SerializedName;

/**
 * Created by Yariki on 8/26/2015.
 */
public class PstAttachmentContent {

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

    public PstAttachmentContent() {
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
}
