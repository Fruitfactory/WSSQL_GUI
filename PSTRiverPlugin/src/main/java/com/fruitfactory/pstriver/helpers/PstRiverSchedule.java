/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package com.fruitfactory.pstriver.helpers;

/**
 *
 * @author Yariki
 */
public enum PstRiverSchedule {
        
    EveryNightOrIdle(0), OnlyAt(1), EveryHours(2), Never(3);

    private int value;

    private PstRiverSchedule(int value) {
        this.value = value;
    }

    public int getValue() {
        return (value);
    }

    public static PstRiverSchedule getValue(int key) {
        for (PstRiverSchedule type : PstRiverSchedule.values()) {
            if (type.getValue() == key) {
                return type;
            }
        }
        return null;
    }
}
