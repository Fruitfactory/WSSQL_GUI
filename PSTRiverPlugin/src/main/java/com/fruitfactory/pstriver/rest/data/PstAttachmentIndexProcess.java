package com.fruitfactory.pstriver.rest.data;

/**
 * Created by Yariki on 8/26/2015.
 */
public enum PstAttachmentIndexProcess {
    None(0),
    Chunk(1),
    End(2);

    private int value;

    private PstAttachmentIndexProcess(int value) {
        this.value = value;
    }

    public int getValue() {
        return (value);
    }

    public static PstAttachmentIndexProcess getValue(int key) {
        for (PstAttachmentIndexProcess type : PstAttachmentIndexProcess.values()) {
            if (type.getValue() == key) {
                return type;
            }
        }
        return null;
    }
}
