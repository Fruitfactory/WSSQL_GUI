package com.fruitfactory.models;

import com.carrotsearch.ant.tasks.junit4.dependencies.com.google.gson.annotations.SerializedName;

/**
 * Created by Yariki on 1/29/2017.
 */
public class OFRecipient {
    @SerializedName("entryID")
    public String entryId;

    @SerializedName("name")
    public String name;

    @SerializedName("address")
    public String address;

    @SerializedName("emailaddresstype")
    public String emailAddressType;

    public OFRecipient() {
    }

    public String getName() {
        return name;
    }

    public void setName(String name) {
        this.name = name;
    }

    public String getAddress() {
        return address;
    }

    public void setAddress(String address) {
        this.address = address;
    }

    public String getEmailAddressType() {
        return emailAddressType;
    }

    public void setEmailAddressType(String emailAddressType) {
        this.emailAddressType = emailAddressType;
    }

    public String getEntryId() {
        return entryId;
    }

    public void setEntryId(String entryId) {
        this.entryId = entryId;
    }

}
