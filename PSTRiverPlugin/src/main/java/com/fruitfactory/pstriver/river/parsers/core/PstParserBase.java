/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package com.fruitfactory.pstriver.river.parsers.core;

import com.fruitfactory.pstriver.interfaces.IPstRiverInitializer;
import com.fruitfactory.pstriver.river.parsers.core.IPstParser;
import com.fruitfactory.pstriver.helpers.PstRiverStatus;
import com.fruitfactory.pstriver.helpers.PstRiverStatusInfo;
import com.fruitfactory.pstriver.rest.PstStatusRepository;
import com.fruitfactory.pstriver.river.reader.PstOutlookFileReader;
import static com.fruitfactory.pstriver.river.PstRiver.LOG_TAG;
import com.fruitfactory.pstriver.useractivity.IInputHookManage;
import com.fruitfactory.pstriver.useractivity.IReaderControl;
import com.fruitfactory.pstriver.useractivity.PstLastInputEventTracker;
import com.fruitfactory.pstriver.useractivity.PstUserActivityTracker;
import com.fruitfactory.pstriver.utils.PstFeedDefinition;
import com.fruitfactory.pstriver.utils.PstGlobalConst;
import java.nio.file.Files;
import java.nio.file.LinkOption;
import java.nio.file.Paths;
import java.util.ArrayList;
import java.util.Date;
import java.util.List;
import java.util.Map;
import org.elasticsearch.action.bulk.BulkProcessor;
import org.elasticsearch.action.get.GetResponse;
import org.elasticsearch.action.index.IndexRequest;
import org.elasticsearch.client.Client;
import org.elasticsearch.common.joda.time.DateTime;
import org.elasticsearch.common.joda.time.format.ISODateTimeFormat;
import org.elasticsearch.common.logging.ESLogger;
import org.elasticsearch.common.xcontent.XContentBuilder;
import static org.elasticsearch.common.xcontent.XContentFactory.jsonBuilder;
import org.elasticsearch.river.RiverName;

/**
 *
 * @author Yariki
 */
public abstract class PstParserBase implements IPstParser {

    private Date _lastUpdatedDate;
    private PstFeedDefinition _def;
    private List<Thread> _readers;
    private ESLogger logger;
    private volatile boolean _closed = false;
    private final Client _client;
    private volatile BulkProcessor _bulkProcessor;
    private RiverName riverName;
    private String _indexName;
    private PstRiverStatus riverStatus;
    private IPstRiverInitializer _riverInitializer;

    protected PstParserBase(PstFeedDefinition def, Client client, BulkProcessor bulkProcessor, RiverName riverName, String indexName, ESLogger logger,IPstRiverInitializer riverInitializer) {
        _def = def;
        _readers = new ArrayList<Thread>();
        this.logger = logger;
        this._client = client;
        this._bulkProcessor = bulkProcessor;
        this.riverName = riverName;
        this._indexName = indexName;
        this._riverInitializer = riverInitializer;
        parseSettings();
        
    }

    public void close() {
        _closed = true;
    }

    @Override
    public void run() {

        if(this._riverInitializer  != null){
            this._riverInitializer.init();
        }

        while (true) {
            if (_closed) {
                logger.warn(LOG_TAG + "River pst was closed...");
                break;
            }
            Date scanDateNew = new Date();
            try {
                _lastUpdatedDate = getLastDateFromRiver();
                riverStatus = getStatusFromRiver();

                if (riverStatus != PstRiverStatus.InitialIndexing) {
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

                onProcess(_readers);

                try {
                    _readers.clear();
                    updateFsRiver(scanDateNew);
                    updateStatusRiver(PstRiverStatus.StandBy);
                    PstStatusRepository.setRiverStatus(new PstRiverStatusInfo(PstRiverStatus.StandBy, scanDateNew));
                } catch (Exception ex) {
                    logger.error(LOG_TAG + ex.getMessage() + " " + ex.toString());
                }

            } catch (InterruptedException e) {
                logger.error(LOG_TAG + e.getMessage() + " " + e.toString());
            } catch (Exception e) {
                logger.error(LOG_TAG + e.getMessage() + " " + e.toString());
            }

        }
       
    }

    protected abstract void onProcess(List<Thread> readers) throws Exception;
    
    protected PstRiverStatus getRiverStatus(){
        return riverStatus;
    }
    
    protected void setRiverStatus(PstRiverStatus status){
        riverStatus = status;
    }
    
    protected ESLogger getLogger(){
        return logger;
    }
    
    protected PstFeedDefinition getDefinition(){
        return _def;
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
                    .prepareGet("_river", riverName.name(),
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
                    .prepareGet("_river", riverName.name(),
                            PstGlobalConst.RIVER_STATUS).execute().actionGet();
            if (lastSeqGetResponse.isExists()) {
                Map<String, Object> fsState = (Map<String, Object>) lastSeqGetResponse
                        .getSourceAsMap().get(PstGlobalConst.PST_PREFIX);

                if (fsState != null) {
                    Object status = fsState.get("riverstatus");
                    if (status != null) {
                        riverStatus = (PstRiverStatus) status;
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

    protected void updateStatusRiver(PstRiverStatus riverStatus)
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

    protected void parseSettings(){

    }

}
