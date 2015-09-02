/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package com.fruitfactory.pstriver.river.parsers.settings;

import com.google.gson.annotations.SerializedName;

/**
 *
 * @author Yariki
 */
public class PstEveryHourPeriodSettings {

    @SerializedName("hour_period")
    public int hourPeriod;

    public PstEveryHourPeriodSettings() {
    }

    public int getHourPeriod() {
        return hourPeriod;
    }

    public void setHourPeriod(int hourPeriod) {
        this.hourPeriod = hourPeriod;
    }

}
