/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package com.fruitfactory.pstriver.rest;

import java.io.IOException;
import org.elasticsearch.client.Client;
import org.elasticsearch.common.inject.Inject;
import org.elasticsearch.common.jackson.core.JsonParser;
import org.elasticsearch.common.settings.Settings;
import org.elasticsearch.common.xcontent.XContentBuilder;
import org.elasticsearch.rest.BaseRestHandler;
import org.elasticsearch.rest.BytesRestResponse;
import org.elasticsearch.rest.RestChannel;
import org.elasticsearch.rest.RestController;
import org.elasticsearch.rest.RestRequest;
import org.elasticsearch.rest.RestStatus;
import org.json.simple.JSONObject;
import org.json.simple.parser.JSONParser;

/**
 *
 * @author Yariki
 */
public class PstRestModule extends BaseRestHandler {

    @Inject
    public PstRestModule(Settings stngs, Client client, RestController controlller) {
        super(stngs, client);
        controlller.registerHandler(RestRequest.Method.GET, "_river/{rivername}/{command}", this);
        controlller.registerHandler(RestRequest.Method.PUT, "_river/{rivername}/useractivity", this);
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
    }

    private void processStatusRequest(RestChannel rc) {
        try {

            XContentBuilder status = PstStatusRepository.getPstReaderStatusInfo();
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
            XContentBuilder status = PstStatusRepository.getPstRiverStatusInfo();
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
                PstStatusRepository.setLastUserActivity(value);    
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
    
}
