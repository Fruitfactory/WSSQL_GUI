package com.fruitfactory.pstriver.rest.data;

import com.carrotsearch.ant.tasks.junit4.dependencies.com.google.gson.annotations.SerializedName;

import java.util.List;

/**
 * Created by Yariki on 8/26/2015.
 */
public class PstOutlookItemsContainer {

    @SerializedName("email")
    public PstEmailContent email;

    @SerializedName("attachments")
    public List<PstAttachmentContent> attachments;

    @SerializedName("process")
    public int process;

    public PstOutlookItemsContainer() {
    }

    public PstEmailContent getEmail() {
        return email;
    }

    public void setEmail(PstEmailContent email) {
        this.email = email;
    }

    public List<PstAttachmentContent> getAttachments() {
        return attachments;
    }

    public void setAttachments(List<PstAttachmentContent> attachments) {
        this.attachments = attachments;
    }

    public int getProcess() {
        return process;
    }

    public void setProcess(int process) {
        this.process = process;
    }
}
