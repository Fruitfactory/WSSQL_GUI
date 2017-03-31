package com.fruitfactory.models;

import com.carrotsearch.ant.tasks.junit4.dependencies.com.google.gson.annotations.SerializedName;

import java.util.List;

/**
 * Created by Yariki on 1/29/2017.
 */
public class OFItemsContainer {
    @SerializedName("email")
    public OFEmail email;

    @SerializedName("attachments")
    public List<OFAttachment> attachments;

    @SerializedName("contact")
    public OFContact contact;

    @SerializedName("process")
    public int process;

    public OFItemsContainer() {
    }

    public OFContact getContact() {
        return contact;
    }

    public void setContact(OFContact contact) {
        this.contact = contact;
    }

    public OFEmail getEmail() {
        return email;
    }

    public void setEmail(OFEmail email) {
        this.email = email;
    }

    public List<OFAttachment> getAttachments() {
        return attachments;
    }

    public void setAttachments(List<OFAttachment> attachments) {
        this.attachments = attachments;
    }

    public int getProcess() {
        return process;
    }

    public void setProcess(int process) {
        this.process = process;
    }
}
