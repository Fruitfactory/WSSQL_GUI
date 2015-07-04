/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package com.fruitfactory.pstriver.river.parsers.core;

import com.fruitfactory.pstriver.helpers.PstRiverSchedule;
import com.google.gson.JsonDeserializationContext;
import com.google.gson.JsonDeserializer;
import com.google.gson.JsonElement;
import java.lang.reflect.Type;
import org.elasticsearch.common.jackson.core.JsonParseException;

/**
 *
 * @author Yariki
 */
public class PstRiverScheduleDeserializer implements JsonDeserializer<PstRiverSchedule> {

    @Override
    public PstRiverSchedule deserialize(JsonElement je, Type type, JsonDeserializationContext jdc) throws com.google.gson.JsonParseException {
        int key = je.getAsInt();
        return PstRiverSchedule.getValue(key);
    }
    
    
}
