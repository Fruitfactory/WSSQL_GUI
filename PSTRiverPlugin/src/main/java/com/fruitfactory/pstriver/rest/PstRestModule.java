/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package com.fruitfactory.pstriver.rest;

import java.io.IOException;

import com.fruitfactory.pstriver.rest.data.PstAttachmentContainer;
import org.elasticsearch.client.Client;
import org.elasticsearch.common.inject.Inject;
import org.elasticsearch.common.settings.Settings;
import org.elasticsearch.common.xcontent.XContentBuilder;
import org.elasticsearch.rest.BaseRestHandler;
import org.elasticsearch.rest.BytesRestResponse;
import org.elasticsearch.rest.RestChannel;
import org.elasticsearch.rest.RestController;
import org.elasticsearch.rest.RestRequest;
import org.elasticsearch.rest.RestStatus;
import com.google.gson.Gson;
import com.google.gson.GsonBuilder;
import org.json.simple.JSONObject;
import org.json.simple.parser.JSONParser;

/**
 *
 * @author Yariki
 */
public class PstRestModule extends BaseRestHandler {

    @Inject
    public PstRestModule(Settings stngs, Client client, RestController controller) {
        super(stngs, controller, client);
        controller.registerHandler(RestRequest.Method.GET, "_river/{rivername}/{command}", this);
        controller.registerHandler(RestRequest.Method.PUT, "_river/{rivername}/useractivity", this);
        controller.registerHandler(RestRequest.Method.PUT, "_river/{rivername}/indexattachment", this);
    }

    @Override
    protected void handleRequest(RestRequest rr, RestChannel rc, Client client) throws Exception {

        String command = rr.param("command");

        if ("status".equals(command)) {
            processStatusRequest(rc);
            return;
        }
        if("pstriverstatus".equals(command)){
            processRiverStatus(rc);
            return;
        }
        if(rr.rawPath().contains("useractivity")){
            processRiverUserActivity(rr, rc);
            return;
        }
        if(rr.rawPath().contains("indexattachment")){
            processIndexingAttachment(rr,rc);
            return;
        }
    }

    private void processStatusRequest(RestChannel rc) {
        try {

            XContentBuilder status = PstRESTRepository.getPstReaderStatusInfo();
            rc.sendResponse(new BytesRestResponse(RestStatus.OK, status));

        } catch (IOException e) {
            try {
                rc.sendResponse(new BytesRestResponse(rc, e));
            } catch (IOException e1) {
                rc.sendResponse(new BytesRestResponse(RestStatus.INTERNAL_SERVER_ERROR));
            }
        }
    }
    
    private void processRiverStatus(RestChannel rc){
        try{
            XContentBuilder status = PstRESTRepository.getPstRiverStatusInfo();
            rc.sendResponse(new BytesRestResponse(RestStatus.OK,status));
        }catch(IOException ex){
            try {
                rc.sendResponse(new BytesRestResponse(rc, ex));
            } catch (IOException e1) {
                rc.sendResponse(new BytesRestResponse(RestStatus.INTERNAL_SERVER_ERROR));
            }
        }
    }
    
    private void processRiverUserActivity(RestRequest rr, RestChannel rc){
        try {
            String content = rr.content().toUtf8();
            JSONParser parser = new JSONParser();
            Object putContent = parser.parse(content);
            JSONObject json = (JSONObject)putContent;
            if(json != null){
                String value = json.get("idle_time").toString();
                PstRESTRepository.setLastUserActivity(value);
            }
            rc.sendResponse(new BytesRestResponse(RestStatus.OK));    
        } catch (Exception e) {
            try {
                rc.sendResponse(new BytesRestResponse(rc, e));
            } catch (IOException e1) {
                rc.sendResponse(new BytesRestResponse(RestStatus.INTERNAL_SERVER_ERROR));
            }
        }
    }

    private void processIndexingAttachment(RestRequest rr, RestChannel rc){
        try {
            String content = rr.content().toUtf8();
            Gson gson = new GsonBuilder().create();
            PstAttachmentContainer container =  gson.fromJson(content,PstAttachmentContainer.class);
            PstRESTRepository.putAttachmentContainer(container);
            rc.sendResponse(new BytesRestResponse(RestStatus.OK));
        } catch (Exception e) {
            try {
                rc.sendResponse(new BytesRestResponse(rc, e));
            } catch (IOException e1) {
                rc.sendResponse(new BytesRestResponse(RestStatus.INTERNAL_SERVER_ERROR));
            }
        }
    }
    
}
