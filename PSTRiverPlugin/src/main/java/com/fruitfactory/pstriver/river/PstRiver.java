/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package com.fruitfactory.pstriver.river;

import com.fruitfactory.pstriver.helpers.AttachmentHelper;
import com.fruitfactory.pstriver.helpers.PstRiverStatus;
import com.fruitfactory.pstriver.helpers.PstRiverStatusInfo;
import com.fruitfactory.pstriver.rest.PstStatusRepository;
import com.fruitfactory.pstriver.useractivity.IReaderControl;
import com.fruitfactory.pstriver.useractivity.PstUserActivityTracker;

import com.fruitfactory.pstriver.utils.PstFeedDefinition;
import com.fruitfactory.pstriver.utils.PstGlobalConst;
import com.fruitfactory.pstriver.utils.PstMetadataTags;
import com.fruitfactory.pstriver.utils.PstSignTool;
import com.pff.*;
import java.io.IOException;
import java.nio.file.Files;
import java.nio.file.LinkOption;
import java.nio.file.Paths;
import java.security.NoSuchAlgorithmException;
import java.util.ArrayList;
import java.util.Date;
import java.util.List;
import java.util.Map;
import java.util.UUID;
import java.util.Vector;
import java.util.logging.Level;
import java.util.logging.Logger;
import org.apache.tika.Tika;
import org.elasticsearch.ExceptionsHelper;
import org.elasticsearch.action.admin.indices.mapping.put.PutMappingResponse;
import org.elasticsearch.action.bulk.BulkItemResponse;
import org.elasticsearch.action.bulk.BulkProcessor;
import org.elasticsearch.action.bulk.BulkRequest;
import org.elasticsearch.action.bulk.BulkResponse;
import org.elasticsearch.action.get.GetResponse;
import org.elasticsearch.action.index.IndexRequest;
import org.elasticsearch.client.Client;
import org.elasticsearch.cluster.ClusterState;
import org.elasticsearch.cluster.block.ClusterBlockException;
import org.elasticsearch.cluster.metadata.IndexMetaData;
import org.elasticsearch.cluster.metadata.MappingMetaData;
import org.elasticsearch.common.inject.Inject;
import org.elasticsearch.common.joda.time.DateTime;
import org.elasticsearch.common.joda.time.format.ISODateTimeFormat;
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
public class PstRiver extends AbstractRiverComponent implements River {

    public static final String LOG_TAG = "PST-RIVER: ";

    private final Client _client;

    private final String _indexName;

    private final String _typeName;

    private volatile BulkProcessor _bulkProcessor;

    private volatile Thread _pstParseThread;
    private volatile boolean _closed = false;
    private PstFeedDefinition _definition;

    @Inject
    public PstRiver(RiverName riverName, RiverSettings settings, Client client) {
        super(riverName, settings);
        this._client = client;
        this._typeName = PstMetadataTags.INDEX_TYPE_EMAIL_MESSAGE;

        if (settings.settings().containsKey(PstGlobalConst.PST_PREFIX)) {
            Map<String, Object> feed = (Map<String, Object>) settings.settings().get(PstGlobalConst.PST_PREFIX);
            TimeValue updateRate = XContentMapValues.nodeTimeValue(feed.get(PstFeedDefinition.UPDATE_RATE), TimeValue.timeValueMinutes(60));
            this._indexName = XContentMapValues.nodeStringValue(feed.get(PstFeedDefinition.INDEX_NAME), riverName.name());
            logger.info(LOG_TAG + " _indexName" + this._indexName );

            String[] pstList = PstFeedDefinition.getListOfPst(settings.settings(), PstFeedDefinition.PST_LIST_PATH);
            _definition = new PstFeedDefinition(riverName.getName(), pstList, updateRate);
        } else {
            _definition = new PstFeedDefinition(riverName.getName(), null, TimeValue.timeValueMinutes(60));
            this._indexName = riverName.getName();
        }
        logger.warn(LOG_TAG + "River was created...");
    }

    @Override
    public void start() {

        logger.warn(LOG_TAG + "River is starting...");
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
                    .setBulkActions(100)
                    .setConcurrentRequests(1)
                    .setFlushInterval(TimeValue.timeValueSeconds(5))
                    .build();
        
        
        }catch(Exception ex){
            logger.error(LOG_TAG, ex.getMessage());
        }
        logger.warn(LOG_TAG + "River has created BulkProcessor...");        
        logger.warn(LOG_TAG + "River is creating parse thread...");

        try{
            _pstParseThread = EsExecutors.daemonThreadFactory(settings.globalSettings(), "pst_slurper")
                    .newThread(new PstParser(_definition));
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

        if (_pstParseThread != null) {
            _pstParseThread.interrupt();
        }
        if (_bulkProcessor != null) {
            _bulkProcessor.close();
        }
    }

    private boolean isMappingExist(String index, String type) {
        logger.info(LOG_TAG + " Is mapping exist...");
        ClusterState cs = _client.admin().cluster().prepareState().setIndices(index).execute().actionGet().getState();
        IndexMetaData imd = cs.getMetaData().index(index);

        if (imd == null) {
            return false;
        }

        MappingMetaData mdd = imd.mapping(type);

        if (mdd != null) {
            return true;
        }
        return false;
    }

    private void pushMapping(String index, String type, XContentBuilder mapping) throws Exception {
        //if (logger.isTraceEnabled()) {
            logger.trace("pushMapping(" + index + "," + type + ")");
        //}

        // If type does not exist, we create it
        boolean mappingExist = isMappingExist(index, type);
        if (!mappingExist) {
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

    class PstParser implements Runnable {

        private Date _lastUpdatedDate;
        private PstFeedDefinition _def;
        private List<Thread> _readers;
        

        public PstParser(PstFeedDefinition def) {
            _def = def;
            _readers = new ArrayList<Thread>();
        }

        @Override
        public void run() {
            while(true){
                if (_closed) {
                    logger.warn(LOG_TAG + "River pst was closed...");
                    return;
                }
                Date scanDateNew = new Date();
                try {
                    _lastUpdatedDate = getLastDateFromRiver();
                    PstRiverStatus riverStatus = getStatusFromRiver();
                    
                    if(riverStatus != PstRiverStatus.InitialIndexing){
                        riverStatus = PstRiverStatus.Busy;
                    }
                    updateStatusRiver(riverStatus);
                    PstStatusRepository.setRiverStatus(new PstRiverStatusInfo(riverStatus, _lastUpdatedDate));
                    String[] psts = _def.getDataArray();
                    int index = 1;
                    for (String file : psts) {
                        if (!Files.exists(Paths.get(file), LinkOption.NOFOLLOW_LINKS)) {
                            continue;
                        }
                        PstOutlookFileReader reader = new PstOutlookFileReader(_indexName, file, _lastUpdatedDate, logger, _bulkProcessor, "pst_" + Integer.toString(index));
                        reader.init();
                        reader.prepareStatusInfo();
                        _readers.add(reader);
                        index++;
                    }
                    List<IReaderControl> readerControls = new ArrayList<>();
                    for(Thread r : _readers){
                        readerControls.add((IReaderControl)r);
                    }
                    PstUserActivityTracker tracker = new PstUserActivityTracker(readerControls);
                    tracker.start();
                    
                    for(Thread reader : _readers){
                        reader.start();
                        reader.join();
                        ((PstOutlookFileReader)reader).close();
                    }
                    tracker.stopTracking();
                    tracker.join();
                    readerControls.clear();

                } catch (InterruptedException e) {
                    logger.error(LOG_TAG + e.getMessage() + " " + e.toString());
                } catch (Exception e) {
                    logger.error(LOG_TAG + e.getMessage()  + " " + e.toString());
                }

                try {
                    _readers.clear();
                    updateFsRiver(scanDateNew);
                    updateStatusRiver(PstRiverStatus.StandBy);
                    PstStatusRepository.setRiverStatus(new PstRiverStatusInfo(PstRiverStatus.StandBy, scanDateNew));
                    Thread.sleep(_def.getUpdateRate().getMillis());
                } catch (Exception ex) {
                    logger.error(LOG_TAG + ex.getMessage()  + " " + ex.toString());
                }

            }
        }

        private void esIndex(String index, String type, String id,
                XContentBuilder xb) throws Exception {
            //if (logger.isDebugEnabled()) logger.debug("Indexing in ES " + index + ", " + type + ", " + id);
            logger.warn("Indexing in ES " + index + ", " + type + ", " + id);
            if (logger.isTraceEnabled()) {
                logger.trace("JSon indexed : {}", xb.string());
            }
            logger.warn("JSon indexed : {}", xb.string());

            if (!_closed) {
                _bulkProcessor.add(new IndexRequest(index, type, id).source(xb));
            } else {
                logger.warn("trying to add new file while closing river. Document [{}]/[{}]/[{}] has been ignored", index, type, id);
            }
        }

        @SuppressWarnings("unchecked")
        private Date getLastDateFromRiver() {
            Date lastDate = null;
            try {
                // Do something
                // If the river is being closed, we return
                if (_closed) {
                    return lastDate;
                }

                _client.admin().indices().prepareRefresh("_river").execute()
                        .actionGet();

                // If the river is being closed, we return
                if (_closed) {
                    return lastDate;
                }
                GetResponse lastSeqGetResponse = _client
                        .prepareGet("_river", riverName().name(),
                                PstGlobalConst.LAST_UPDATED_FIELD).execute().actionGet();
                if (lastSeqGetResponse.isExists()) {
                    Map<String, Object> fsState = (Map<String, Object>) lastSeqGetResponse
                            .getSourceAsMap().get(PstGlobalConst.PST_PREFIX);

                    if (fsState != null) {
                        Object lastupdate = fsState.get("lastdate");
                        if (lastupdate != null) {
                            String strLastDate = lastupdate.toString();
                            lastDate = ISODateTimeFormat
                                    .dateOptionalTimeParser()
                                    .parseDateTime(strLastDate).toDate();
                        }
                    }
                } else {
                    // First call
                    if (logger.isDebugEnabled()) {
                        logger.debug("{} doesn't exist", PstGlobalConst.LAST_UPDATED_FIELD);
                    }
                }
            } catch (Exception e) {
                logger.warn("failed to get _lastupdate, throttling....", e);
            }
            return lastDate;
        }

        private void updateFsRiver(Date scanDate)
                throws Exception {
            // We store the lastupdate date and some stats
            scanDate = new DateTime(scanDate).secondOfDay().roundFloorCopy().minusSeconds(2).toDate();

            XContentBuilder xb = jsonBuilder()
                    .startObject()
                    .startObject(PstGlobalConst.PST_PREFIX)
                    .field("feedname", riverName.getName())
                    .field("lastdate", scanDate)
                    .endObject()
                    .endObject();
            esIndex("_river", riverName.name(), PstGlobalConst.LAST_UPDATED_FIELD, xb);
        }
        
        @SuppressWarnings("unchecked")
        private PstRiverStatus getStatusFromRiver() {
            PstRiverStatus riverStatus = PstRiverStatus.None;
            try {
                // Do something
                // If the river is being closed, we return
                if (_closed) {
                    return riverStatus;
                }

                _client.admin().indices().prepareRefresh("_river").execute()
                        .actionGet();

                // If the river is being closed, we return
                if (_closed) {
                    return riverStatus;
                }
                GetResponse lastSeqGetResponse = _client
                        .prepareGet("_river", riverName().name(),
                                PstGlobalConst.RIVER_STATUS).execute().actionGet();
                if (lastSeqGetResponse.isExists()) {
                    Map<String, Object> fsState = (Map<String, Object>) lastSeqGetResponse
                            .getSourceAsMap().get(PstGlobalConst.PST_PREFIX);

                    if (fsState != null) {
                        Object status = fsState.get("riverstatus");
                        if (status != null) {
                            riverStatus = (PstRiverStatus)status;
                        }
                    }
                } else {
                    // First call
                    if (logger.isDebugEnabled()) {
                        logger.debug("{} doesn't exist", PstGlobalConst.RIVER_STATUS);
                    }
                    riverStatus = PstRiverStatus.InitialIndexing;
                }
            } catch (Exception e) {
                logger.warn("failed to get _lastupdate, throttling....", e);
            }
            return riverStatus;
        }
        
        private void updateStatusRiver(PstRiverStatus riverStatus)
                throws Exception {

            XContentBuilder xb = jsonBuilder()
                    .startObject()
                    .startObject(PstGlobalConst.PST_PREFIX)
                    .field("feedname", riverName.getName())
                    .field("riverstatus", riverStatus)
                    .endObject()
                    .endObject();
            esIndex("_river", riverName.name(), PstGlobalConst.RIVER_STATUS, xb);
        }

    }

}
