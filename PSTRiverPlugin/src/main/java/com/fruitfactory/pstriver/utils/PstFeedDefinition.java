/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package com.fruitfactory.pstriver.utils;

import java.util.*;
import org.elasticsearch.common.Strings;
import org.elasticsearch.common.unit.TimeValue;
import org.elasticsearch.common.xcontent.support.XContentMapValues;

/**
 *
 * @author Yariki
 */
public class PstFeedDefinition {
    
    public static final String UPDATE_RATE = "update_rate";
    public static final String PST_LIST_PATH = "pst.pst_list";
    private static final String SEPARATOR = ",";
    
    private String _riverName;
    private String[] _dataArray;
    private TimeValue _updateRate;

    public PstFeedDefinition(String _riverName, String[] _dataArray, TimeValue _updateRate) {
        this._riverName = _riverName;
        this._dataArray = _dataArray;
        this._updateRate = _updateRate;
    }

    public String getRiverName() {
        return _riverName;
    }

    public String[] getDataArray() {
        return _dataArray;
    }

    public TimeValue getUpdateRate() {
        return _updateRate;
    }
    
    public static String[] getListOfPst(Map<String,Object> settings, String path){
        String temp[];
        
        if(XContentMapValues.isArray(XContentMapValues.extractValue(path, settings))){
            List<String> listData =  (List<String>)XContentMapValues.extractValue(path, settings);
            int i = 0;
            temp = new String[listData.size()];
            for(String value : listData){
                temp[i++] = Strings.trimAllWhitespace(value);
            }
        }else{
            String tempValues = (String)XContentMapValues.extractValue(path, settings);
            temp = Strings.commaDelimitedListToStringArray(tempValues);
        }
        return temp;
    }
    
}
