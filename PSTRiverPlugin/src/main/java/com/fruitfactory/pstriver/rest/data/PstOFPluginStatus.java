package com.fruitfactory.pstriver.rest.data;

/**
 * Created by Yariki on 9/2/2015.
 */
public enum PstOFPluginStatus {
    None(0),
    Running(1),
    Shutdown(2);

    private int value;

    private PstOFPluginStatus(int value) {
        this.value = value;
    }

    public int getValue() {
        return (value);
    }

    public static PstOFPluginStatus getValue(int key) {
        for (PstOFPluginStatus type : PstOFPluginStatus.values()) {
            if (type.getValue() == key) {
                return type;
            }
        }
        return null;
    }
}
