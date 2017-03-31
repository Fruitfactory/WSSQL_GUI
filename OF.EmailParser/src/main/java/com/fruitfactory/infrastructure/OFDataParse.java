package com.fruitfactory.infrastructure;

import com.fasterxml.jackson.databind.deser.Deserializers;
import com.fruitfactory.infrastructure.core.OFDataProcess;
import com.fruitfactory.interfaces.IOFDataRepositoryPipe;
import com.fruitfactory.models.OFAttachment;
import com.fruitfactory.models.OFEmail;
import com.fruitfactory.models.OFItemsContainer;
import com.sun.xml.internal.messaging.saaj.util.ByteInputStream;
import org.apache.tika.Tika;
import com.sun.org.apache.xerces.internal.impl.dv.util.Base64;
import org.apache.tika.metadata.Metadata;

/**
 * Created by Yariki on 2/1/2017.
 */
public class OFDataParse extends OFDataProcess {

    private final IOFDataRepositoryPipe dataTarget;

    private final Tika tika = new Tika();

    public OFDataParse(IOFDataRepositoryPipe dataSource, IOFDataRepositoryPipe dataTarget, String name) {
        super(dataSource, name);
        this.dataTarget = dataTarget;
    }

    @Override
    protected void processData(OFItemsContainer container) {
        try {
            processEmail(container);
            processAttachments(container);
            dataTarget.pushData(container);
        }catch (Exception ex){
            getLogger().error(ex.toString());
        }
    }

    private void processAttachments(OFItemsContainer container) {
        if(container.getAttachments() == null || container.getAttachments().isEmpty()){
            return;
        }
        try {
            for(OFAttachment attachment : container.getAttachments()){
                processAttachment(attachment);
            }
        }catch (Exception ex){
            getLogger().error(ex.toString());
        }
    }

    private void processAttachment(OFAttachment attachment) {
        if(attachment.getContent() == null){
            return;
        }
        try{
            String parsedContent = "";
            byte[] byteBuffer = Base64.decode(attachment.getContent());
            parsedContent = tika.parseToString(new ByteInputStream(byteBuffer,byteBuffer.length), new Metadata());
            attachment.setAnalyzedcontent(parsedContent);
        }catch (Exception ex){
            getLogger().error(ex.toString());
        }
    }

    private void processEmail(OFItemsContainer container){
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
                body = new String(Base64.decode(email.getContent()));
            }catch (Exception ex){
                getLogger().error(ex.toString());
            }
            try {
                htmlBody = new String(Base64.decode(email.getHtmlcontent()));
            }catch(Exception ex){
                getLogger().error(ex.toString());
            }
            String analyzedContent = null;

            if(body != null && !body.isEmpty()){
                byte[] bytes = body.getBytes();
                analyzedContent = tika.parseToString(new ByteInputStream(bytes,bytes.length),new Metadata());
            } else if(htmlBody != null && !htmlBody.isEmpty()){
                byte[] bytes = htmlBody.getBytes();
                analyzedContent = tika.parseToString(new ByteInputStream(bytes,bytes.length),new Metadata());
            }
            email.setAnalyzedcontent(analyzedContent);

        }catch (Exception ex){
            getLogger().error(ex.toString());
        }
    }

}
