package com.fruitfactory.pstriver.rest.data;

import com.fruitfactory.pstriver.helpers.PstRiverSchedule;
import com.google.gson.JsonDeserializationContext;
import com.google.gson.JsonDeserializer;
import com.google.gson.JsonElement;
import com.google.gson.JsonParseException;

import java.lang.reflect.Type;

/**
 * Created by Yariki on 8/27/2015.
 */
public class PstAttachmentIndexProcessDeserializer implements  JsonDeserializer<PstAttachmentIndexProcess> {
    @Override
    public PstAttachmentIndexProcess deserialize(JsonElement json, Type typeOfT, JsonDeserializationContext context) throws JsonParseException {
        int key = json.getAsInt();
        return PstAttachmentIndexProcess.getValue(key);
    }
}
