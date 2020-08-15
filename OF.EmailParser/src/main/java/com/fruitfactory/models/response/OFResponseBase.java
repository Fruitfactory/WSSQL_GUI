package com.fruitfactory.models.response;

import com.google.gson.annotations.SerializedName;


/**
 * Created by Yariki on 1/29/2017.
 */
public class OFResponseBase {
    @SerializedName("status")
    public boolean status;

    @SerializedName("message")
    public String message;

    public OFResponseBase(boolean status, String message) {
        this.status = status;
        this.message = message;
    }

    public boolean isStatus() {
        return status;
    }

    public void setStatus(boolean status) {
        this.status = status;
    }

    public String getMessage() {
        return message;
    }

    public void setMessage(String message) {
        this.message = message;
    }
}
