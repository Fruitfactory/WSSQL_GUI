package com.fruitfactory.pstriver.rest;

import com.fruitfactory.pstriver.interfaces.IPstRestAttachmentClient;
import com.fruitfactory.pstriver.interfaces.IPstRestClient;
import com.sun.jersey.api.client.Client;
import com.sun.jersey.api.client.ClientResponse;
import com.sun.jersey.api.client.WebResource;
import org.elasticsearch.common.logging.ESLogger;


import java.io.IOException;
import java.net.URI;
import java.text.DateFormat;
import java.text.SimpleDateFormat;
import java.util.Date;

/**
 * Created by Yariki on 12/4/2015.
 */
public class PstRestClient implements IPstRestClient, IPstRestAttachmentClient{

    private final String ROOT_URL = "http://localhost:11223/serviceapp/";
    private final String STOP_CMD = "stop";
    private final String STATUS_CMD = "status";

    private final String START_READ_CMD = "startread";
    private final String STOP_READ_CMD = "stopread";
    private final String RESUME_READ_CMD = "resumeread";
    private final String SUSPEND_READ_CMD = "suspendread";

    private ESLogger logger;
    private final Object lock = new Object();
    private boolean isStarted = false;

    public PstRestClient(ESLogger logger){
        this.logger = logger;
    }

    @Override
    public void stop() {
        synchronized (lock){
            sendSimpleCommand(STOP_CMD);
        }
    }

    @Override
    public void status() {
        sendSimpleCommand(STATUS_CMD);
    }

    @Override
    public void startRead(Date date) {
        synchronized (lock){
            isStarted = true;
            sendWithDate(START_READ_CMD,date);
        }
    }

    @Override
    public void stopRead() {
        synchronized (lock){
            isStarted = false;
            sendSimpleCommand(STOP_READ_CMD);
        }
    }

    @Override
    public void suspentRead() {
        synchronized (lock){
            if(isStarted){
                sendSimpleCommand(SUSPEND_READ_CMD);
            }
        }
    }

    @Override
    public void resumeRead(Date date ) {
        synchronized (lock){
            if(isStarted){
                sendWithDate(RESUME_READ_CMD,date);
            }
        }
    }

    private String getUrl(String cmd){
        return ROOT_URL +  cmd;
    }

    private void sendSimpleCommand(String cmd){
        try {
            Client client = Client.create();
            WebResource webResource = client.resource(getUrl(cmd));
            ClientResponse clientResponse = webResource.accept("application/json")
                    .get(ClientResponse.class);
            logger.info(String.format("Response for %s: %s",cmd,clientResponse.getStatus()));
            logger.info(String.format("Entity: %s",clientResponse.getEntity(String.class)));
        }catch(Exception ex){
            logger.error("REST CLIENT: " + ex.toString());
        }
    }

    private void sendWithDate(String cmd, Date date){
        try{
            Client client = Client.create();
            ClientResponse clientResponse = null;
            if(date != null){
                DateFormat df = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss");
                String frm = df.format(date);
                String url = java.lang.String.format("%s/%s", getUrl(cmd),frm);
                WebResource webResource = client.resource(url);
                clientResponse = webResource.accept("application/json").get(ClientResponse.class);
            }else{
                WebResource webResource = client.resource(getUrl(cmd));
                clientResponse = webResource.accept("application/json").get(ClientResponse.class);
            }
            if(clientResponse != null){
                logger.info(String.format("Response for %s: %s",cmd,clientResponse.getStatus()));
                logger.info(String.format("Entity: %s",clientResponse.getEntity(String.class)));
            }
        }catch(Exception ex){
            System.out.println(ex.getMessage());
        }
    }
}
