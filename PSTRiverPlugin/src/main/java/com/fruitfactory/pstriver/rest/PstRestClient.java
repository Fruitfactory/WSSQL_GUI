package com.fruitfactory.pstriver.rest;

import com.fruitfactory.pstriver.interfaces.IPstRestAttachmentClient;
import com.fruitfactory.pstriver.interfaces.IPstRestClient;
import org.elasticsearch.common.logging.ESLogger;
import us.monoid.json.JSONObject;
import us.monoid.web.JSONResource;
import us.monoid.web.Resty;

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


    private Resty  restyClient;
    private ESLogger logger;

    public PstRestClient(ESLogger logger){
        this.logger = logger;
        restyClient = new Resty();
    }

    @Override
    public void stop() {
        SendSimpleCommand(STOP_CMD);
    }

    @Override
    public void status() {
        SendSimpleCommand(STATUS_CMD);
    }

    @Override
    public void startRead(Date date) {
        try{
            if(date !=null){
                DateFormat df = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss");
                String frm = df.format(date);
                String url = java.lang.String.format("%s/%s", getUrl(START_READ_CMD),frm);
                JSONResource response = restyClient.json(new URI(url));
                JSONObject obj = response.object();
                System.out.println(obj);
            }else{
                JSONResource response = restyClient.json(new URI(getUrl(START_READ_CMD)));
                JSONObject obj = response.object();
                System.out.println(obj);
            }
        }catch(Exception ex){
            System.out.println(ex.getMessage());
        }
    }

    @Override
    public void stopRead() {
        SendSimpleCommand(STOP_READ_CMD);
    }

    @Override
    public void suspentRead() {
        SendSimpleCommand(SUSPEND_READ_CMD);
    }

    @Override
    public void resumeRead() {
        SendSimpleCommand(RESUME_READ_CMD);
    }

    private String getUrl(String cmd){
        return ROOT_URL + cmd;
    }

    private JSONResource SendSimpleCommand(String cmd){
        JSONResource response = null;
        try{
            response = restyClient.json(new URI(getUrl(cmd)));
        }catch(Exception ex){
            logger.error("REST CLIENT: " + ex.toString());
        }
        return response;
    }
}
