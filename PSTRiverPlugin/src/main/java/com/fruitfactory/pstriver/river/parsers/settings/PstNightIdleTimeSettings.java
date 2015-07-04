package com.fruitfactory.pstriver.river.parsers.settings;

import com.google.gson.annotations.*;

/**
 * Created by Yariki on 7/3/2015.
 */
public class PstNightIdleTimeSettings {

    @SerializedName("idle_time")
    public int idleTime;

    public PstNightIdleTimeSettings() {
    }

    public int getIdleTime() {
        return idleTime;
    }

    public void setIdleTime(int idleTime) {
        this.idleTime = idleTime;
    }
}
