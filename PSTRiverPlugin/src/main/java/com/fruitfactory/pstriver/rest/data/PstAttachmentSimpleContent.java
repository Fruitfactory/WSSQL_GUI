package com.fruitfactory.pstriver.rest.data;

import com.carrotsearch.ant.tasks.junit4.dependencies.com.google.gson.annotations.SerializedName;

/**
 * Created by Yariki on 1/1/2016.
 */
public class PstAttachmentSimpleContent {

    @SerializedName("entryid")
    public String entryId;

    @SerializedName("filename")
    public String fileName;

    @SerializedName("path")
    public String path;

    @SerializedName("size")
    public long size;

    @SerializedName("mimetag")
    public String mimeTag;

    public PstAttachmentSimpleContent() {
    }

    public String getEntryId() {
        return entryId;
    }

    public void setEntryId(String entryId) {
        this.entryId = entryId;
    }

    public String getFileName() {
        return fileName;
    }

    public void setFileName(String fileName) {
        this.fileName = fileName;
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

    public String getMimeTag() {
        return mimeTag;
    }

    public void setMimeTag(String mimeTag) {
        this.mimeTag = mimeTag;
    }
}
