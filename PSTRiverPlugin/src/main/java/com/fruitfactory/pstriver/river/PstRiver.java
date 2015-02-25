/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

package com.fruitfactory.pstriver.river;

import com.fruitfactory.pstriver.helpers.AttachmentHelper;
import com.fruitfactory.pstriver.helpers.Pair;
import com.fruitfactory.pstriver.utils.PstFeedDefinition;
import com.fruitfactory.pstriver.utils.PstGlobalConst;
import com.fruitfactory.pstriver.utils.PstMetadataTags;
import com.fruitfactory.pstriver.utils.PstSignTool;
import com.pff.PSTAttachment;
import com.pff.PSTConversationIndexData;
import com.pff.PSTException;
import com.pff.PSTFile;
import com.pff.PSTFolder;
import com.pff.PSTMessage;
import com.pff.PSTRecipient;
import example.TestGui;
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
public class PstRiver extends AbstractRiverComponent implements River{
    
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
        this._indexName = riverName.name();
        this._typeName = PstMetadataTags.INDEX_TYPE_EMAIL_MESSAGE;
        
        if(settings.settings().containsKey(PstGlobalConst.PST_PREFIX)){
            Map<String,Object> feed = (Map<String,Object>)settings.settings().get(PstGlobalConst.PST_PREFIX);
            TimeValue updateRate = XContentMapValues.nodeTimeValue(feed.get(PstFeedDefinition.UPDATE_RATE), TimeValue.timeValueMinutes(60));
            
            String[] pstList = PstFeedDefinition.getListOfPst(settings.settings(), PstFeedDefinition.PST_LIST_PATH);
            _definition = new PstFeedDefinition(riverName.getName(), pstList, updateRate);
        }else{
            _definition = new PstFeedDefinition(riverName.getName(), null, TimeValue.timeValueMinutes(60));
        }
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
                .newThread(new PstParser(_definition));
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
        private static final String FILE_NAME = "c:\\Users\\Yariki\\AppData\\Local\\Microsoft\\Outlook\\iyariki.ost";
        private Date _lastUpdatedDate;
        private PstFeedDefinition _def;
                 
        
        public PstParser(PstFeedDefinition def) {
            _def = def;
        }
        
        @Override
        public void run() {
            if(_closed){
                logger.warn(LOG_TAG + "River pst was closed...");
                return;
            }
            try {
                Date scanDateNew = new Date();
                _lastUpdatedDate = getLastDateFromRiver();
                
                String[] psts = _def.getDataArray();
                
                for(String file : psts){
                    if(!Files.exists(Paths.get(file), LinkOption.NOFOLLOW_LINKS)){
                        continue;
                    }
                    PSTFile pstFile = new PSTFile(file);
                    processFolder(pstFile.getRootFolder());    
                }
                updateFsRiver(scanDateNew);
                
            } catch (Exception e) {
                logger.error(LOG_TAG + e.getMessage());
            }
            try{
               Thread.sleep(_def.getUpdateRate().getMillis());
            }catch(Exception ex){
                logger.error(LOG_TAG + ex.getMessage());
            }
            
        }
        
        public void processFolder(PSTFolder pstFolder){
            
            try {
                String folderName = pstFolder.getDisplayName();
                
                logger.warn(LOG_TAG + " Folder: " + folderName);
                
                
                if(pstFolder.hasSubfolders() ){
                    Vector<PSTFolder> folders = pstFolder.getSubFolders();
                    for(PSTFolder folder : folders){
                        processFolder(folder);
                    }
                }
                
                if(pstFolder.getContentCount() > 0 ){
                    PSTMessage message = (PSTMessage)pstFolder.getNextChild();
                    while(message  != null){
                        
                        if(_lastUpdatedDate  != null && message.getLastModificationTime().getTime() > _lastUpdatedDate.getTime()){
                            continue;
                        }
                        
                        String subject = message.getSubject();
                        String sender = message.getSentRepresentingName();
                        String senderEmail = message.getSentRepresentingEmailAddress();
                        String body = message.getBody();
                        if(body.isEmpty()){
                            body = message.getBodyHTML();
                        }
                        boolean hasAttachment = message.hasAttachments();
                        Date dateCreated = message.getCreationTime();
                        Date dateReceived = message.getMessageDeliveryTime();
                        long size = message.getMessageSize();
                        PSTConversationIndexData indexData = message.getConversationIndexData();
                        UUID id = null;
                        try {
                            id = indexData.getConversationUUID(message.getConversationTopic(),message.getConversationIndexTracking());
                        } catch (NoSuchAlgorithmException ex) {
                            Logger.getLogger(TestGui.class.getName()).log(Level.SEVERE, null, ex);
                        }
                        String conversationId = id != null ? id.toString() : "";
                        
                        List<Pair<String,String>> listTo = new ArrayList<Pair<String,String>>();
                        List<Pair<String,String>> listCc = new ArrayList<Pair<String,String>>();
                        List<Pair<String,String>> listBcc = new ArrayList<Pair<String,String>>();
                        List<AttachmentHelper> listAttachments = new ArrayList<AttachmentHelper>();
                        int count = 0;
                        try {
                            count = message.getNumberOfRecipients();

                            for(int i = 0; i < count; i++){
                                PSTRecipient recipient = message.getRecipient(i);
                                Pair<String,String> pairRecipient = new Pair<String,String>(recipient.getDisplayName(),recipient.getEmailAddress());
                                int flag = recipient.getRecipientType();
                                switch(flag){
                                    case PSTRecipient.MAPI_TO:
                                        listTo.add(pairRecipient);
                                        break;
                                    case PSTRecipient.MAPI_CC:
                                        listCc.add(pairRecipient);
                                        break;
                                    case PSTRecipient.MAPI_BCC:
                                        listBcc.add(pairRecipient);
                                        break;
                                }
                            }
                        } catch (PSTException ex) {
                            Logger.getLogger(TestGui.class.getName()).log(Level.SEVERE, null, ex);
                        } catch (IOException ex) {
                            Logger.getLogger(TestGui.class.getName()).log(Level.SEVERE, null, ex);
                        }
                        try{
                            int countAttachments  = message.getNumberOfAttachments();
                            for(int i= 0; i < countAttachments; i++){
                                PSTAttachment attachment = message.getAttachment(i);
                                if(attachment == null){
                                    continue;
                                }
                                AttachmentHelper helper = new AttachmentHelper(attachment.getLongFilename(),attachment.getLongPathname(),attachment.getFilesize(),attachment.getMimeTag());
                                listAttachments.add(helper);
                            }
                        }catch(Exception ex){
                            System.out.println(ex.getMessage());
                        }
                        
                        XContentBuilder source = jsonBuilder().startObject();
                        
                        if (logger.isTraceEnabled()) {
                            source.prettyPrint();
                        }
                        
                        source
                                .field(PstMetadataTags.Email.ITEM_NAME,subject)
                                .field(PstMetadataTags.Email.ITEM_URL,subject)
                                .field(PstMetadataTags.Email.ITEM_NAME_DISPLAY,subject)
                                .field(PstMetadataTags.Email.FOLDER,folderName)
                                .field(PstMetadataTags.Email.DATE_CREATED,dateCreated)
                                .field(PstMetadataTags.Email.DATE_RECEIVED,dateReceived)
                                .field(PstMetadataTags.Email.SIZE,size)
                                .field(PstMetadataTags.Email.CONVERSATION_ID,conversationId)
                                .field(PstMetadataTags.Email.CONVERSATION_INDEX,"")
                                .field(PstMetadataTags.Email.SUBJECT, subject)
                                .field(PstMetadataTags.Email.CONTENT,body)
                                .field(PstMetadataTags.Email.HAS_ATTACHMENTS, Boolean.toString(hasAttachment))
                                .field(PstMetadataTags.Email.FROM_NAME,sender)
                                .field(PstMetadataTags.Email.FROM_ADDRESS, senderEmail);
                        
                        AddArrayOfEmails(source, listTo, 
                                PstMetadataTags.Email.TO, 
                                PstMetadataTags.Email.To.NAME, 
                                PstMetadataTags.Email.To.ADDRESS);
                        AddArrayOfEmails(source, listCc, 
                                PstMetadataTags.Email.CC, 
                                PstMetadataTags.Email.Cc.NAME, 
                                PstMetadataTags.Email.Cc.ADDRESS);
                        AddArrayOfEmails(source, listBcc, 
                                PstMetadataTags.Email.BCC, 
                                PstMetadataTags.Email.Bcc.NAME, 
                                PstMetadataTags.Email.Bcc.ADDRESS);
                        
                        if(!listAttachments.isEmpty()){
                            source.field(PstMetadataTags.Email.ATTACHMENTS).startArray();
                            for (AttachmentHelper listAttachment : listAttachments) {
                                source.startObject();
                                source.field(PstMetadataTags.Email.Attachments.FILENAME,
                                        listAttachment.getFilename());
                                source.field(PstMetadataTags.Email.Attachments.PATH,
                                        listAttachment.getPath());
                                source.field(PstMetadataTags.Email.Attachments.SIZE,
                                        listAttachment.getSize());
                                source.field(PstMetadataTags.Email.Attachments.MIME_TAG,
                                        listAttachment.getMimetype());
                                source.endObject();
                            }
                            source.endArray();
                        }
                        
                        source.endObject();
                        
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
        
        private void AddArrayOfEmails(XContentBuilder builder, List<Pair<String,String>> list, String objectName, String nameTag, String addressTag) throws IOException{
            if(list.isEmpty()){
                return;
            }
            builder.field(objectName).startArray();
            for (Pair<String, String> item : list) {
                builder.startObject();
                builder.field(nameTag,item.getItem1());
                builder.field(addressTag,item.getItem2());
                builder.endObject();
            }
            builder.endArray();
        }
        
        private void esIndex(String index, String type, String id,
                             XContentBuilder xb) throws Exception {
            //if (logger.isDebugEnabled()) logger.debug("Indexing in ES " + index + ", " + type + ", " + id);
            logger.warn("Indexing in ES " + index + ", " + type + ", " + id);
            if (logger.isTraceEnabled()) logger.trace("JSon indexed : {}", xb.string());
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
                    if (logger.isDebugEnabled())
                        logger.debug("{} doesn't exist", PstGlobalConst.LAST_UPDATED_FIELD);
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
        
        
        
    }
    
}