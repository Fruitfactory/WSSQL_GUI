/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package com.fruitfactory.pstriver.river.parsers.core;

import com.fruitfactory.pstriver.interfaces.IPstRestAttachmentClient;
import com.fruitfactory.pstriver.interfaces.IPstRiverInitializer;
import com.fruitfactory.pstriver.helpers.PstRiverStatus;
import com.fruitfactory.pstriver.helpers.PstRiverStatusInfo;
import com.fruitfactory.pstriver.rest.PstRESTRepository;
import com.fruitfactory.pstriver.rest.PstRestClient;
import com.fruitfactory.pstriver.river.reader.PstOutlookAttachmentReader;
import com.fruitfactory.pstriver.river.reader.PstOutlookFileReader;
import static com.fruitfactory.pstriver.river.PstRiver.LOG_TAG;

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
import org.elasticsearch.common.unit.TimeValue;
import org.elasticsearch.common.xcontent.XContentBuilder;
import static org.elasticsearch.common.xcontent.XContentFactory.jsonBuilder;
import org.elasticsearch.river.RiverName;

/**
 *
 * @author Yariki
 */
public abstract class PstParserBase implements IPstParser, IPstStatusTracker {

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
    private PstOutlookAttachmentReader _attachmentReader;
    private IPstRestAttachmentClient _restAttachmentClient;

    private int countEmails;
    private int countAttachments;


    protected PstParserBase(PstFeedDefinition def, Client client, BulkProcessor bulkProcessor, RiverName riverName, String indexName, ESLogger logger,IPstRiverInitializer riverInitializer) {
        _def = def;
        _readers = new ArrayList<Thread>();
        this.logger = logger;
        this._client = client;
        this._bulkProcessor = bulkProcessor;
        this.riverName = riverName;
        this._indexName = indexName;
        this._riverInitializer = riverInitializer;
        this._restAttachmentClient = new PstRestClient(logger);
        parseSettings();
        
    }

    public void close() {
        _closed = true;
    }

    @Override
    public void run() {

        if(this._riverInitializer != null){
            this._riverInitializer.init();
        }

        while (true) {
            if (_closed) {
                logger.warn(LOG_TAG + "River pst was closed...");
                break;
            }
            try {
                _lastUpdatedDate = getLastDateFromRiver();
                riverStatus = getStatusFromRiver();

                countEmails = getLastCountIndexedCountOfEmails();
                countAttachments = getLastCountIndexedOfAttachments();

                if (riverStatus != PstRiverStatus.InitialIndexing) {
                    riverStatus = PstRiverStatus.Busy;
                }
                setRiverStatus(riverStatus);
                
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
                _attachmentReader = new PstOutlookAttachmentReader(_indexName,_lastUpdatedDate,_bulkProcessor,"pst_attachment",logger);

                int delayTimeOut = onProcess(_readers);

                for (Thread thr : _readers){
                    if(!(thr instanceof PstOutlookFileReader)){
                        continue;
                    }
                    PstOutlookFileReader reader = (PstOutlookFileReader)thr;
                    countEmails += reader.getCountOfIndexedEmails();
                    countAttachments += reader.getCountOfIndexedAttachments();
                }

                try {
                    _lastUpdatedDate = new Date();
                    _readers.clear();
                    updateLastDateRiver(_lastUpdatedDate);
                    setLastCountIndexedOfEmails(countEmails);
                    setLastCountIndexedOfAttachments(countAttachments);
                    
                    setRiverStatus(PstRiverStatus.StandBy);
                    flush();
                    
                    if(delayTimeOut > 0){
                        Thread.sleep(TimeValue.timeValueHours(delayTimeOut).millis());
                    }

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

    public void setStatus(PstRiverStatus riverStatus){
        setRiverStatus(riverStatus);
    }

    public PstRiverStatus getStatus(){
        return this.riverStatus;
    }

    protected abstract int onProcess(List<Thread> readers) throws Exception;

    protected PstOutlookAttachmentReader getAttachmentReader(){
        return _attachmentReader;
    }

    protected IPstRestAttachmentClient getRestAttachmentClient() { return _restAttachmentClient; }

    protected PstRiverStatus getRiverStatus(){
        return this.riverStatus;
    }
    
    protected void setRiverStatus(PstRiverStatus status){
        this.riverStatus = status;
        updateStatusRiver(this.riverStatus);
        PstRESTRepository.setRiverStatus(new PstRiverStatusInfo(riverStatus, _lastUpdatedDate, countEmails, countAttachments));

    }
    
    protected ESLogger getLogger(){
        return logger;
    }
    
    protected PstFeedDefinition getDefinition(){
        return _def;
    }

    protected void flush(){
        try{
            _bulkProcessor.flush();
        }catch(Exception ex){
            logger.error(ex.getMessage());
        }
    }
    
    private void esIndex(String index, String type, String id,
            XContentBuilder xb) throws Exception {
        //if (logger.isDebugEnabled()) logger.debug("Indexing in ES " + index + ", " + type + ", " + id);
        //logger.warn("Indexing in ES " + index + ", " + type + ", " + id);
        if (logger.isTraceEnabled()) {
            logger.trace("JSon indexed : {}", xb.string());
        }
        //logger.warn("JSon indexed : {}", xb.string());

        if (!_closed) {
            _bulkProcessor.add(new IndexRequest(index, type, id).source(xb));
        } else {
            logger.warn("trying to add new file while closing river. Document [{}]/[{}]/[{}] has been ignored", index, type, id);
        }
    }

    @SuppressWarnings("unchecked")
    protected Date getLastDateFromRiver() {
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
                        .getSourceAsMap().get(PstGlobalConst.LAST_UPDATED_FIELD);

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

    private void updateLastDateRiver(Date scanDate){
        // We store the lastupdate date and some stats

        try{
            scanDate = new DateTime(scanDate).secondOfDay().roundFloorCopy().minusSeconds(2).toDate();

            XContentBuilder xb = jsonBuilder()
                    .startObject()
                    .startObject(PstGlobalConst.LAST_UPDATED_FIELD)
                    .field("feedname", riverName.getName())
                    .field("lastdate", scanDate)
                    .endObject()
                    .endObject();
            esIndex("_river", riverName.name(), PstGlobalConst.LAST_UPDATED_FIELD, xb);

        }catch (Exception ex){
            logger.error(ex.getMessage());
        }

    }

    @SuppressWarnings("unchecked")
    private PstRiverStatus getStatusFromRiver() {
        PstRiverStatus riverStatus = PstRiverStatus.None;

        Object value = getSavedValue(null,PstGlobalConst.RIVER_STATUS,"riverstatus");
        if(value == null){
            return PstRiverStatus.InitialIndexing;
        }
        return PstRiverStatus.valueOf(value.toString());
    }

    protected void updateStatusRiver(PstRiverStatus riverStatus) {
        setSavedValue(PstGlobalConst.RIVER_STATUS,"riverstatus",riverStatus);
    }

    protected int getLastCountIndexedCountOfEmails(){
        int countEmails = Integer.parseInt(getSavedValue(0,PstGlobalConst.COUNT_INDEXED_EMAIL,"indexedemails").toString());
        return countEmails;
    }

    protected void setLastCountIndexedOfEmails(int count){
        setSavedValue(PstGlobalConst.COUNT_INDEXED_EMAIL,"indexedemails",count);
    }

    protected int getLastCountIndexedOfAttachments(){
        int count = Integer.parseInt(getSavedValue(0,PstGlobalConst.COUNT_INDEXED_ATTACHMENT,"indexedattachments").toString());
        return count;
    }

    protected void setLastCountIndexedOfAttachments(int count){
        setSavedValue(PstGlobalConst.COUNT_INDEXED_ATTACHMENT,"indexedattachments",count);
    }

    private Object getSavedValue(Object defaultValue,String id, String fieldName){
        Object value = defaultValue;
        try {
            // Do something
            // If the river is being closed, we return
            if (_closed) {
                return value;
            }

            _client.admin().indices().prepareRefresh("_river").execute()
                    .actionGet();

            // If the river is being closed, we return
            if (_closed) {
                return value;
            }
            GetResponse lastSeqGetResponse = _client
                    .prepareGet("_river", riverName.name(),
                            id).execute().actionGet();
            if (lastSeqGetResponse.isExists()) {
                Map<String, Object> fsState = (Map<String, Object>) lastSeqGetResponse
                        .getSourceAsMap().get(PstGlobalConst.PST_PREFIX);

                if (fsState != null) {
                    value = fsState.get(fieldName);
                }
            } else {
                // First call
                if (logger.isDebugEnabled()) {
                    logger.debug("{} doesn't exist", PstGlobalConst.RIVER_STATUS);
                }
            }
        } catch (Exception e) {
            logger.warn("failed to get _lastupdate, throttling....", e);
        }
        return value;
    }

    private void setSavedValue(String id, String fieldName, Object value){
        try {
            XContentBuilder xb = jsonBuilder()
                    .startObject()
                    .startObject(PstGlobalConst.PST_PREFIX)
                    .field("feedname", riverName.getName())
                    .field(fieldName, value)
                    .endObject()
                    .endObject();
            esIndex("_river", riverName.name(), id, xb);

        } catch (Exception ex) {
            logger.error(ex.getMessage());
        }

    }

    protected void parseSettings(){

    }

}
