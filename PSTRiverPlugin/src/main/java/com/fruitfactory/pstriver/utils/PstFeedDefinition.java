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
    public static final String ONLINE_TIME = "online_time";
    public static final String IDLE_TIME = "idle_time";
    public static final String PST_LIST_PATH = "pst.pst_list";
    public static final String INDEX_NAME = "pst.index_name";
    private static final String SEPARATOR = ",";

    private String _riverName;
    private String[] _dataArray;
    private TimeValue _updateRate;
    private TimeValue _onlineTime;
    private TimeValue _idleTime;

    public PstFeedDefinition(String _riverName, String[] _dataArray, TimeValue _updateRate, TimeValue onlineTime, TimeValue idleTime) {
        this._riverName = _riverName;
        this._dataArray = _dataArray;
        this._updateRate = _updateRate;
        this._onlineTime = onlineTime;
        this._idleTime = idleTime;
    }

    public TimeValue getOnlineTime() {
        return _onlineTime;
    }

    public TimeValue getIdleTime() {
        return _idleTime;
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

    public static String[] getListOfPst(Map<String, Object> settings, String path) {
        String temp[];

        if (XContentMapValues.isArray(XContentMapValues.extractValue(path, settings))) {
            List<String> listData = (List<String>) XContentMapValues.extractValue(path, settings);
            int i = 0;
            temp = new String[listData.size()];
            for (String value : listData) {
                temp[i++] = value.trim();
            }
        } else {
            String tempValues = (String) XContentMapValues.extractValue(path, settings);
            temp = Strings.commaDelimitedListToStringArray(tempValues);
        }
        return temp;
    }

}
