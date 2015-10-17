/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package com.fruitfactory.pstriver.river;

import com.fruitfactory.pstriver.interfaces.IPstRiverInitializer;
import com.fruitfactory.pstriver.river.parsers.core.IPstParser;
import com.fruitfactory.pstriver.river.parsers.*;

import com.fruitfactory.pstriver.utils.PstFeedDefinition;
import com.fruitfactory.pstriver.utils.PstGlobalConst;
import com.fruitfactory.pstriver.utils.PstMetadataTags;

import java.util.Map;

import org.elasticsearch.ExceptionsHelper;
import org.elasticsearch.action.admin.indices.mapping.put.PutMappingResponse;
import org.elasticsearch.action.bulk.BulkItemResponse;
import org.elasticsearch.action.bulk.BulkProcessor;
import org.elasticsearch.action.bulk.BulkRequest;
import org.elasticsearch.action.bulk.BulkResponse;
import org.elasticsearch.client.Client;
import org.elasticsearch.cluster.block.ClusterBlockException;
import org.elasticsearch.common.inject.Inject;
import org.elasticsearch.common.unit.ByteSizeValue;
import org.elasticsearch.common.unit.TimeValue;
import org.elasticsearch.common.util.concurrent.EsExecutors;
import org.elasticsearch.common.xcontent.XContentBuilder;
import static org.elasticsearch.common.xcontent.XContentFactory.jsonBuilder;
import org.elasticsearch.common.xcontent.support.XContentMapValues;
import org.elasticsearch.indices.IndexAlreadyExistsException;
import org.elasticsearch.river.AbstractRiverComponent;
import org.elasticsearch.river.River;
import org.elasticsearch.river.RiverName;
import org.elasticsearch.river.RiverSettings;

/**
 *
 * @author Yariki
 */
public class PstRiver extends AbstractRiverComponent implements River, IPstRiverInitializer {

    public static final String LOG_TAG = "PST-RIVER: ";

    private final Client _client;

    private final String _indexName;

    private final String _typeName;

    private final String DEFAULT_SCHEDULE_SETTINGS = "{\"schedule_type\":0,\"settings\":\"\"}";
    
    private volatile BulkProcessor _bulkProcessor;

    private volatile Thread _pstParseThread;
    private volatile boolean _closed = false;
    private PstFeedDefinition _definition;
    private IPstParser _parser;
    
    @Inject
    public PstRiver(RiverName riverName, RiverSettings settings, Client client) {
        super(riverName, settings);
        this._client = client;
        this._typeName = PstMetadataTags.INDEX_TYPE_EMAIL_MESSAGE;

        if (settings.settings().containsKey(PstGlobalConst.PST_PREFIX)) {
            Map<String, Object> feed = (Map<String, Object>) settings.settings().get(PstGlobalConst.PST_PREFIX);
            TimeValue updateRate = XContentMapValues.nodeTimeValue(feed.get(PstFeedDefinition.UPDATE_RATE), TimeValue.timeValueMinutes(60));
            TimeValue onlineTime = XContentMapValues.nodeTimeValue(feed.get(PstFeedDefinition.ONLINE_TIME), TimeValue.timeValueMinutes(2));
            TimeValue idleTime = XContentMapValues.nodeTimeValue(feed.get(PstFeedDefinition.IDLE_TIME), TimeValue.timeValueMinutes(2));
            this._indexName = XContentMapValues.nodeStringValue(feed.get(PstFeedDefinition.INDEX_NAME), riverName.name());
            Map<String,Object> schedule = (Map<String,Object>)feed.get(PstFeedDefinition.SCHEDULE_TYPE);
            
            logger.info(LOG_TAG + " _indexName = " + this._indexName );

            String[] pstList = PstFeedDefinition.getListOfPst(settings.settings(), PstFeedDefinition.PST_LIST_PATH);
            _definition = new PstFeedDefinition(riverName.getName(), pstList, updateRate,onlineTime,idleTime,schedule);
        } else {
            _definition = new PstFeedDefinition(riverName.getName(), null, TimeValue.timeValueMinutes(60),TimeValue.timeValueMinutes(2),TimeValue.timeValueMinutes(2));
            this._indexName = riverName.getName();
        }
        logger.warn(LOG_TAG + "River was created...");
        String es_home_path = System.getProperty("es.path.home");
        if(es_home_path != null && !es_home_path.isEmpty()){
            System.setProperty("jna.library.path", es_home_path + "\\lib\\" );
        }else{
            System.setProperty("jna.library.path", "f:\\Visual\\WORK\\elasticsearch\\elasticsearch\\lib\\" );
        }
        logger.warn(LOG_TAG + es_home_path);
        
    }

    @Override
    public void start() {

        logger.warn(LOG_TAG + "River is starting...");
        
        logger.warn(LOG_TAG + "River is creating BulkProcessor...");
        try{
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
                    .setBulkActions(-1)
                    .setConcurrentRequests(1)
                    .setBulkSize(ByteSizeValue.parseBytesSizeValue("10m"))
                    .build();
        
        
        }catch(Exception ex){
            logger.error(LOG_TAG, ex.getMessage());
        }
        logger.warn(LOG_TAG + "River has created BulkProcessor...");        
        logger.warn(LOG_TAG + "River is creating parse thread...");

        try{
            _parser = PstParsersFactory.getInstance().getParser(_definition.getScheduleSettings().getType(), _definition, _client, _bulkProcessor, riverName, _indexName, logger,this);
            if(_parser == null){
                logger.warn(PstGlobalConst.LOG_TAG + "Schedule type is '" + _definition.getScheduleSettings().getType().toString() + "'");
                return;
            }
            _pstParseThread = EsExecutors.daemonThreadFactory(settings.globalSettings(), "pst_slurper")
                    .newThread(_parser);
            _pstParseThread.start();
            
        }catch(Exception ex){
            logger.error(LOG_TAG, ex.getMessage());
        }
        logger.warn(LOG_TAG + "PstRiver has started...");    
    }

    @Override
    public void close() {
        logger.warn(LOG_TAG + "Closing pst river");
            _closed = true;

            if(_parser != null){
                _parser.close();
                _parser = null;
            }
            if (_pstParseThread != null) {
                _pstParseThread.interrupt();
            }
            if (_bulkProcessor != null) {
            _bulkProcessor.close();
        }
    }

    private boolean isMappingExist(String index, String type) {
        return _client.admin().indices().prepareGetMappings(index).setTypes(type).get().getMappings().isEmpty();
    }

    private void pushMapping(String index, String type, XContentBuilder mapping) throws Exception {
        //if (logger.isTraceEnabled()) {
            logger.trace("pushMapping(" + index + "," + type + ")");
        //}

        // If type does not exist, we create it
        boolean mappingExist = isMappingExist(index, type);
        if (mappingExist) {
            logger.info("Mapping [" + index + "]/[" + type + "] doesn't exist. Creating it.");
            XContentBuilder xcontent = mapping;
            // Read the mapping json file if exists and use it
            if (xcontent != null) {
                //if (logger.isTraceEnabled()) {
                    logger.info("Mapping for [" + index + "]/[" + type + "]=" + xcontent.string());
                //}
                // Create type and mapping
                PutMappingResponse response = _client.admin().indices()
                        .preparePutMapping(index)
                        .setType(type)
                        .setSource(xcontent)
                        .execute().actionGet();
                if (!response.isAcknowledged()) {
                    throw new Exception("Could not define mapping for type [" + index + "]/[" + type + "].");
                } else {
                    //if (logger.isDebugEnabled()) {
                        logger.info("Mapping definition for [" + index + "]/[" + type + "] succesfully created.");
                   // }
                }
            } else {
                //if (logger.isDebugEnabled()) {
                    logger.info("No mapping definition for [" + index + "]/[" + type + "]. Ignoring.");
                //}
            }
        } else {
            //if (logger.isDebugEnabled()) {
                logger.info("Mapping [" + index + "]/[" + type + "] already exists.");
            //}
        }
        //if (logger.isTraceEnabled()) {
            logger.info("/pushMapping(" + index + "," + type + ")");
        //}
    }

    @Override
    public void init() {
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


        logger.warn(LOG_TAG + "River is creating mapping...");
        try {
            pushMapping(_indexName, PstMetadataTags.INDEX_TYPE_EMAIL_MESSAGE, PstMetadataTags.buildPstEmailMapping());
            pushMapping(_indexName, PstMetadataTags.INDEX_TYPE_CONTACT, PstMetadataTags.buildPstContactMapping());
            pushMapping(_indexName, PstMetadataTags.INDEX_TYPE_CALENDAR, PstMetadataTags.buildPstAppointmentMapping());
            pushMapping(_indexName, PstMetadataTags.INDEX_TYPE_ATTACHMENT, PstMetadataTags.buildPstAttachmentMapping());
        } catch (Exception e) {
            logger.warn("failed to create mapping for [{}], disabling river...",
                    e, _indexName);
            return;
        }
        logger.warn(LOG_TAG + "River has created mapping...");
    }
}
