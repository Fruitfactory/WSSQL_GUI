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
public class PstOnlyAtSettings {
    
    @SerializedName("hour_only_at")
    public int hourOnlyAt;
    
    @SerializedName("hour_type")
    public String hourType;

    public PstOnlyAtSettings() {
    }

    public int getHourOnlyAt() {
        return hourOnlyAt;
    }

    public void setHourOnlyAt(int hourOnlyAt) {
        this.hourOnlyAt = hourOnlyAt;
    }

    public String getHourType() {
        return hourType;
    }

    public void setHourType(String hourType) {
        this.hourType = hourType;
    }
    
}
