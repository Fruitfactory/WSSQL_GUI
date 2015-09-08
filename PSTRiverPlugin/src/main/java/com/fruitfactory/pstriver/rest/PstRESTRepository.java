/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package com.fruitfactory.pstriver.rest;

import com.fruitfactory.pstriver.helpers.PstReaderStatus;
import com.fruitfactory.pstriver.helpers.PstReaderStatusInfo;
import com.fruitfactory.pstriver.helpers.PstRiverStatusInfo;
import java.io.IOException;
import java.util.HashMap;
import java.util.Map;

import com.fruitfactory.pstriver.interfaces.IPstAttachmentProcessor;
import com.fruitfactory.pstriver.rest.data.PstAttachmentContainer;
import org.elasticsearch.common.logging.ESLogger;
import org.elasticsearch.common.logging.Loggers;
import org.elasticsearch.common.xcontent.XContentBuilder;
import org.elasticsearch.common.xcontent.XContentFactory;

/**
 *
 * @author Yariki
 */
public class PstRESTRepository {

    private final static String PST_RIVER_STATUS = "PST_RIVER_STATUS";
    
    private static HashMap<String, PstReaderStatusInfo> _repository = new HashMap<String, PstReaderStatusInfo>();
    private static HashMap<String,PstRiverStatusInfo> _reposirotyRiverStatus = new HashMap<String, PstRiverStatusInfo>();
    private static int lastUserActivity = 0;
    private static boolean isOFPluginRunning = false;

    private static Object lockUserActivity = new Object();
    private static Object lockOFPluginRunning = new Object();

    private static final ESLogger logger = Loggers.getLogger(PstRESTRepository.class);

    private static IPstAttachmentProcessor _pstAttachmentProcessor;
    private static final Object _attachmentLock = new Object();


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
    
    public static void setRiverStatus(PstRiverStatusInfo riverStatus){
        synchronized(_reposirotyRiverStatus){
            _reposirotyRiverStatus.put(PST_RIVER_STATUS, riverStatus);
        }
    }
    
    public static XContentBuilder getPstRiverStatusInfo() throws IOException{
        synchronized(_reposirotyRiverStatus){
            XContentBuilder status = XContentFactory.jsonBuilder().prettyPrint();
            status.startObject();
            if(_reposirotyRiverStatus.containsKey(PST_RIVER_STATUS) && _reposirotyRiverStatus.get(PST_RIVER_STATUS) !=  null){
                PstRiverStatusInfo riverStatus = _reposirotyRiverStatus.get(PST_RIVER_STATUS);
                status.field("success",true);
                status.field("status",riverStatus.getStatus());
                status.field("lastupdated",riverStatus.getLastDateUpdated());
                status.field("emailcount",riverStatus.getCountEmails());
                status.field("attachmentcount",riverStatus.getCountAttachments());
            }else{
                status.field("success",false);
            }
            status.endObject();
            return status;
        }
    }
    
    public static XContentBuilder getPstReaderStatusInfo() throws IOException {
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
    
    public static void setLastUserActivity(String value){
        synchronized(lockUserActivity){
            lastUserActivity = Integer.parseInt(value);
        }
    }
    public static int getLastUserActivity(){
        synchronized(lockUserActivity){
            return lastUserActivity;
        }
    }

    public static void putAttachmentContainer(PstAttachmentContainer container){
        if(_pstAttachmentProcessor == null){
            return;
        }
        synchronized (_attachmentLock){
            log(String.format("!!! Process Attachment: %d",container.attachments != null ? container.attachments.size() : 0));
            _pstAttachmentProcessor.processAttachment(container);
        }
    }

    public static void setAttachmentProcessor(IPstAttachmentProcessor attachmentProcessor){

        synchronized (_attachmentLock){
            _pstAttachmentProcessor = attachmentProcessor;
        }
    }

    public static void clearAttachmentProcess(){
        synchronized (_attachmentLock){
            _pstAttachmentProcessor = null;
        }
    }

    public static void setIsOFPluginRunning(boolean status){
        synchronized (lockOFPluginRunning){
            isOFPluginRunning = status;
        }
    }

    public static boolean gettIsOFPluginRunning(){
        boolean result =  false;
        synchronized (lockOFPluginRunning){
            result = isOFPluginRunning;
        }
        return result;
    }

    private static void log(String message){
        if(logger != null){
            logger.info(message);
        }
    }


}
