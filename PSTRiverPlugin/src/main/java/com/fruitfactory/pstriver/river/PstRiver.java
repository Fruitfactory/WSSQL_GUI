/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

package com.fruitfactory.pstriver.river;

import com.fruitfactory.pstriver.utils.PstMetadataTags;
import com.fruitfactory.pstriver.utils.PstSignTool;
import com.pff.PSTFile;
import com.pff.PSTFolder;
import com.pff.PSTMessage;
import java.util.Vector;
import org.elasticsearch.ExceptionsHelper;
import org.elasticsearch.action.admin.indices.mapping.put.PutMappingResponse;
import org.elasticsearch.action.bulk.BulkItemResponse;
import org.elasticsearch.action.bulk.BulkProcessor;
import org.elasticsearch.action.bulk.BulkRequest;
import org.elasticsearch.action.bulk.BulkResponse;
import org.elasticsearch.action.index.IndexRequest;
import org.elasticsearch.client.Client;
import org.elasticsearch.cluster.ClusterState;
import org.elasticsearch.cluster.block.ClusterBlockException;
import org.elasticsearch.cluster.metadata.IndexMetaData;
import org.elasticsearch.cluster.metadata.MappingMetaData;
import org.elasticsearch.common.inject.Inject;
import org.elasticsearch.common.unit.TimeValue;
import org.elasticsearch.common.util.concurrent.EsExecutors;
import org.elasticsearch.common.xcontent.XContentBuilder;
import static org.elasticsearch.common.xcontent.XContentFactory.jsonBuilder;
import org.elasticsearch.indices.IndexAlreadyExistsException;
import org.elasticsearch.river.AbstractRiverComponent;
import org.elasticsearch.river.River;
import org.elasticsearch.river.RiverName;
import org.elasticsearch.river.RiverSettings;

/**
 *
 * @author Yariki
 */
public class PstRiver extends AbstractRiverComponent implements River{
    
    public static final String LOG_TAG = "PST-RIVER: ";
    
    
    private final Client _client;
    
    private final String _indexName;

    private final String _typeName;

    
    
    private volatile BulkProcessor _bulkProcessor;
    
    private volatile Thread _pstParseThread;
    private volatile boolean _closed = false;
    
    @Inject
    public PstRiver(RiverName riverName, RiverSettings settings, Client client) {
        super(riverName, settings);
        this._client = client;
        this._indexName = riverName.name();
        this._typeName = PstMetadataTags.INDEX_TYPE_EMAIL_MESSAGE;
        logger.warn(LOG_TAG + "River was created...");
    }
    
    @Override
    public void start() {
        
         try {
            _client.admin().indices().prepareCreate(_indexName).execute()
                    .actionGet();
        } catch (Exception e) {
            if (ExceptionsHelper.unwrapCause(e) instanceof IndexAlreadyExistsException) {
                // that's fine
            } else if (ExceptionsHelper.unwrapCause(e) instanceof ClusterBlockException) {
                // ok, not recovered yet..., lets start indexing and hope we
                // recover by the first bulk
                // TODO: a smarter logic can be to register for cluster event
                // listener here, and only start sampling when the block is
                // removed...
            } else {
                logger.warn("failed to create index [{}], disabling river...",
                        e, _indexName);
                return;
            }
        }
        
         try {
            pushMapping(_indexName,_typeName);
        } catch (Exception e) {
            logger.warn("failed to create mapping for [{}/{}], disabling river...",
                    e, _indexName, _typeName);
            return;
        }
        
        
        // Creating bulk processor
        this._bulkProcessor = BulkProcessor.builder(this._client, new BulkProcessor.Listener() {
            @Override
            public void beforeBulk(long executionId, BulkRequest request) {
                logger.debug("Going to execute new bulk composed of {} actions", request.numberOfActions());
            }

            @Override
            public void afterBulk(long executionId, BulkRequest request, BulkResponse response) {
                logger.debug("Executed bulk composed of {} actions", request.numberOfActions());
                if (response.hasFailures()) {
                    logger.warn("There was failures while executing bulk", response.buildFailureMessage());
                    if (logger.isDebugEnabled()) {
                        for (BulkItemResponse item : response.getItems()) {
                            if (item.isFailed()) {
                                logger.debug("Error for {}/{}/{} for {} operation: {}", item.getIndex(),
                                        item.getType(), item.getId(), item.getOpType(), item.getFailureMessage());
                            }
                        }
                    }
                }
            }

            @Override
            public void afterBulk(long executionId, BulkRequest request, Throwable failure) {
                logger.warn("Error executing bulk", failure);
            }
        })
                .setBulkActions(100)
                .setConcurrentRequests(1)
                .setFlushInterval(TimeValue.timeValueSeconds(5))
                .build();
        
        _pstParseThread = EsExecutors.daemonThreadFactory(settings.globalSettings(), "pst_slurper")
                .newThread(new PstParser());
        _pstParseThread.start();
        logger.warn(LOG_TAG + "PstRiver was started...");
    }

    @Override
    public void close() {
        logger.warn(LOG_TAG + "Closing pst river");
        _closed = true;
        if(_pstParseThread  != null){
            _pstParseThread.interrupt();
        }
        if(_bulkProcessor != null){
            _bulkProcessor.close();
        }
    }
    
    
     private boolean isMappingExist(String index, String type) {
        ClusterState cs = _client.admin().cluster().prepareState().setIndices(index).execute().actionGet().getState();
        IndexMetaData imd = cs.getMetaData().index(index);

        if (imd == null) return false;

        MappingMetaData mdd = imd.mapping(type);

        if (mdd != null) return true;
        return false;
    }

    private void pushMapping(String index, String type) throws Exception {
        if (logger.isTraceEnabled()) logger.trace("pushMapping(" + index + "," + type + ")");

        // If type does not exist, we create it
        boolean mappingExist = isMappingExist(index, type);
        if (!mappingExist) {
            logger.debug("Mapping [" + index + "]/[" + type + "] doesn't exist. Creating it.");
            XContentBuilder xcontent = PstMetadataTags.buildPstEmailMapping();
            // Read the mapping json file if exists and use it
            if (xcontent != null) {
                if (logger.isTraceEnabled())
                    logger.trace("Mapping for [" + index + "]/[" + type + "]=" + xcontent.string());
                // Create type and mapping
                PutMappingResponse response = _client.admin().indices()
                        .preparePutMapping(index)
                        .setType(type)
                        .setSource(xcontent)
                        .execute().actionGet();
                if (!response.isAcknowledged()) {
                    throw new Exception("Could not define mapping for type [" + index + "]/[" + type + "].");
                } else {
                    if (logger.isDebugEnabled()) {
                        logger.debug("Mapping definition for [" + index + "]/[" + type + "] succesfully created.");
                    }
                }
            } else {
                if (logger.isDebugEnabled())
                    logger.debug("No mapping definition for [" + index + "]/[" + type + "]. Ignoring.");
            }
        } else {
            if (logger.isDebugEnabled()) logger.debug("Mapping [" + index + "]/[" + type + "] already exists.");
        }
        if (logger.isTraceEnabled()) logger.trace("/pushMapping(" + index + "," + type + ")");
    }
    
    class PstParser implements Runnable {
        private static final String FILE_NAME = "c:\\Users\\Yariki\\AppData\\Local\\Microsoft\\Outlook\\iyariki.ya@gmail.com.ost";

        public PstParser() {
            
        }
        
        @Override
        public void run() {
            if(_closed){
                logger.warn(LOG_TAG + "River pst was closed...");
                return;
            }
            try {
                PSTFile pstFile = new PSTFile(FILE_NAME);
                processFolder(pstFile.getRootFolder());
            } catch (Exception e) {
                logger.error(LOG_TAG, e.getMessage());
            }
        }
        
        public void processFolder(PSTFolder pstFolder){
            
            try {
                
                logger.warn(LOG_TAG + " Folder: " + pstFolder.getDisplayName());
                
                
                if(pstFolder.hasSubfolders() ){
                    Vector<PSTFolder> folders = pstFolder.getSubFolders();
                    for(PSTFolder folder : folders){
                        processFolder(folder);
                    }
                }
                
                if(pstFolder.getContentCount() > 0 ){
                    PSTMessage message = (PSTMessage)pstFolder.getNextChild();
                    while(message  != null){
                        String subject = message.getSubject();
                        String sender = message.getSenderName();
                        String senderEmail = message.getSenderEmailAddress();
                        String body = message.getBody();
                        if(body.isEmpty()){
                            body = message.getBodyHTML();
                        }
                        boolean hasAttachment = message.hasAttachments();
                        
                        XContentBuilder source = jsonBuilder().startObject();
                        
                        source.startObject(PstMetadataTags.INDEX_TYPE_EMAIL_MESSAGE)
                                .field(PstMetadataTags.Email.SUBJECT, subject)
                                .field(PstMetadataTags.Email.SENDER,sender)
                                .field(PstMetadataTags.Email.SENDER_EMAIL_ADDRESS, senderEmail)
                                .field(PstMetadataTags.Email.BODY,body)
                                .field(PstMetadataTags.Email.HAS_ATTACHMENTS, Boolean.toString(hasAttachment));
                        source.endObject().endObject();
                        
                        esIndex(_indexName, _typeName, PstSignTool.sign(body).toString(), source);
                        //logger.info(LOG_TAG + message.getSubject());
                        message = (PSTMessage)pstFolder.getNextChild();
                    }
                }
            } catch (Exception e) {
                logger.error(LOG_TAG + e.getMessage());
                logger.error(LOG_TAG + e.getStackTrace().toString());
            }
        }
        
        private void esIndex(String index, String type, String id,
                             XContentBuilder xb) throws Exception {
            //if (logger.isDebugEnabled()) logger.debug("Indexing in ES " + index + ", " + type + ", " + id);
            logger.warn("Indexing in ES " + index + ", " + type + ", " + id);
            if (logger.isTraceEnabled()) logger.trace("JSon indexed : {}", xb.string());
            //logger.warn("JSon indexed : {}", xb.string());

            if (!_closed) {
                _bulkProcessor.add(new IndexRequest(index, type, id).source(xb));
            } else {
                logger.warn("trying to add new file while closing river. Document [{}]/[{}]/[{}] has been ignored", index, type, id);
            }
        }
        
    }
    
}
