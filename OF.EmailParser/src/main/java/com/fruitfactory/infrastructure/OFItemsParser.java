package com.fruitfactory.infrastructure;

import java.io.*;
import com.fruitfactory.models.OFAttachment;
import com.fruitfactory.models.OFEmail;
import com.fruitfactory.models.OFItemsContainer;
import java.util.concurrent.TimeUnit;
import org.apache.tika.Tika;
import org.apache.tika.metadata.Metadata;
import org.apache.log4j.Logger;
import  java.util.Base64;

/**
 * Created by Yariki on 6/2/2017.
 */
public class OFItemsParser {

    private final Tika tika = new Tika();
    private Logger logger = null;

    public OFItemsParser(Logger logger){
        this.logger = logger;
    }


    public void processAttachments(OFItemsContainer container) {
        if(container.getAttachments() == null || container.getAttachments().isEmpty()){
            return;
        }
        try {
            for(OFAttachment attachment : container.getAttachments()){
                processAttachment(attachment);
            }
        }catch (Exception ex){
            logger.error(ex.toString());
        }
    }

    private void processAttachment(OFAttachment attachment) {
        if(attachment.getContent() == null){
            return;
        }
        try{
            String parsedContent = "";

            byte[] byteBuffer = Base64.getDecoder().decode(attachment.getContent());
            OFTimeWatch watch = new OFTimeWatch();
            logger.info("Start parsing...");
            parsedContent = tika.parseToString(new ByteArrayInputStream(byteBuffer), new Metadata());
            logger.info(String.format("Parsed time: %s ms", watch.timeInSeconds()));
            attachment.setAnalyzedcontent(parsedContent);
        }catch (Exception ex){
            logger.error(ex.toString());
        }
    }

    public void processEmail(OFItemsContainer container){
        if(container.getEmail() == null){
            return;
        }
        OFEmail email = container.getEmail();
        try {
            OFConversationIndexData indexData = new OFConversationIndexData(email.getConversationindex().getBytes()) ;
            email.setConversationindex(Integer.toString(indexData.getConversationIndex()));
            email.setOutlookconversationid(indexData.getOutlookConversationId());
            String body = null;
            String htmlBody = null;
            try {
                body = new String(Base64.getDecoder().decode(email.getContent()));
            }catch (Exception ex){
                logger.error(ex.toString());
            }
            try {
                htmlBody = new String(Base64.getDecoder().decode(email.getHtmlcontent()));
            }catch(Exception ex){
                logger.error(ex.toString());
            }
            String analyzedContent = null;

            if(body != null && !body.isEmpty()){
                byte[] bytes = body.getBytes();
                analyzedContent = tika.parseToString(new ByteArrayInputStream(bytes),new Metadata());
            } else if(htmlBody != null && !htmlBody.isEmpty()){
                byte[] bytes = htmlBody.getBytes();
                analyzedContent = tika.parseToString(new ByteArrayInputStream(bytes),new Metadata());
            }
            email.setAnalyzedcontent(analyzedContent);

        }catch (Exception ex){
            logger.error(ex.toString());
        }
    }




}
