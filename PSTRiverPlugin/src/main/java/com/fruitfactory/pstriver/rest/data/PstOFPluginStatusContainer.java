package com.fruitfactory.pstriver.rest.data;

import com.carrotsearch.ant.tasks.junit4.dependencies.com.google.gson.annotations.SerializedName;

/**
 * Created by Yariki on 9/2/2015.
 */
public class PstOFPluginStatusContainer {
    public PstOFPluginStatusContainer() {
    }

    @SerializedName("status")
    public int status;

    public int getStatus() {
        return this.status;
    }

    public void setStatus(int status) {
        this.status = status;
    }
}
