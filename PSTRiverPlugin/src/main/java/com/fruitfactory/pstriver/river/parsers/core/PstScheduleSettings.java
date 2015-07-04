/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package com.fruitfactory.pstriver.river.parsers.core;

import com.fruitfactory.pstriver.helpers.PstRiverSchedule;
import com.google.gson.annotations.SerializedName;

/**
 *
 * @author Yariki
 */
public class PstScheduleSettings {

    @SerializedName("schedule_type")
    public PstRiverSchedule type;

    @SerializedName("settings")
    public String settings;

    public PstRiverSchedule getType() {
        return type;
    }

    public void setType(PstRiverSchedule schedule_type) {
        this.type = schedule_type;
    }

    public String getSettings() {
        return settings;
    }

    public void setSettings(String settings) {
        this.settings = settings;
    }
}
