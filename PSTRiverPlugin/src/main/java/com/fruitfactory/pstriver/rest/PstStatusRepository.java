/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package com.fruitfactory.pstriver.rest;

import com.fruitfactory.pstriver.helpers.PstReaderStatus;
import com.fruitfactory.pstriver.helpers.PstReaderStatusInfo;
import java.io.IOException;
import java.util.HashMap;
import java.util.Map;
import org.elasticsearch.common.xcontent.XContentBuilder;
import org.elasticsearch.common.xcontent.XContentFactory;

/**
 *
 * @author Yariki
 */
public class PstStatusRepository {

    private static HashMap<String, PstReaderStatusInfo> _repository = new HashMap<String, PstReaderStatusInfo>();

    public static void setStatusInfo(PstReaderStatusInfo statusInfo) {
        synchronized (_repository) {
            _repository.put(statusInfo.getName(), statusInfo);
        }
    }

    public static void setProcessCount(String name, int processCount) {
        synchronized (_repository) {
            if (_repository.containsKey(name)) {
                PstReaderStatusInfo statusInfo = _repository.get(name);
                statusInfo.setProccedCount(statusInfo.getProccedCount() + processCount);
            }
        }
    }

    public static void setProcessFolder(String name, String folder){
        synchronized(_repository){
            if(_repository.containsKey(name)){
                _repository.get(name).setFolderName(folder);
                
            }
        }
    }
    
    public static void setStatus(String name, PstReaderStatus readerStatus) {
        synchronized (_repository) {
            if (_repository.containsKey(name)) {
                _repository.get(name).setStatus(readerStatus);
            }
        }
    }

    public static XContentBuilder getStatusInfo() throws IOException {
        synchronized (_repository) {
            XContentBuilder status = XContentFactory.jsonBuilder().prettyPrint();
            status.startObject();
            status.field("items").startArray();

            for (Map.Entry<String, PstReaderStatusInfo> entrySet : _repository.entrySet()) {
                String key = entrySet.getKey();
                PstReaderStatusInfo value = entrySet.getValue();
                status.startObject();
                status.field("name", key);
                status.field("count", value.getCount());
                status.field("processing", value.getProccedCount());
                status.field("status", value.getStatus());
                status.field("folder",value.getFolderName());
                status.endObject();
            }
            status.endArray();
            status.endObject();
            return status;
        }
    }

}
