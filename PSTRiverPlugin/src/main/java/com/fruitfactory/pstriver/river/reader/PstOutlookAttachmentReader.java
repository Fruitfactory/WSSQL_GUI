package com.fruitfactory.pstriver.river.reader;

import com.fruitfactory.pstriver.helpers.PstReaderStatus;
import com.fruitfactory.pstriver.rest.PstRESTRepository;
import com.fruitfactory.pstriver.rest.data.PstAttachmentContainer;
import com.fruitfactory.pstriver.rest.data.PstAttachmentContent;
import com.fruitfactory.pstriver.rest.data.PstAttachmentIndexProcess;
import com.fruitfactory.pstriver.utils.PstMetadataTags;
import com.fruitfactory.pstriver.utils.PstSignTool;
import com.pff.PSTAttachment;
import com.sun.org.apache.xerces.internal.impl.dv.util.Base64;
import org.apache.tika.Tika;
import org.apache.tika.metadata.Metadata;
import org.elasticsearch.action.bulk.BulkProcessor;
import org.elasticsearch.common.io.stream.BytesStreamInput;
import org.elasticsearch.common.logging.ESLogger;
import org.elasticsearch.common.xcontent.XContentBuilder;

import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.util.Date;
import java.util.UUID;

import static com.fruitfactory.pstriver.river.PstRiver.LOG_TAG;
import static org.elasticsearch.common.xcontent.XContentFactory.jsonBuilder;

/**
 * Created by Yariki on 8/26/2015.
 */
public class PstOutlookAttachmentReader extends PstBaseOutlookIndexer {


    private Date _lastUpdateDate;
    private String _indexName;
    private String _name;

    private final Tika _tika = new Tika();

    public PstOutlookAttachmentReader(String indexName, Date lastUpdated, BulkProcessor bulkProcessor,String name, ESLogger _logger) {
        super(name, _logger,bulkProcessor);
        this._bulkProcessor = bulkProcessor;
        this._lastUpdateDate = lastUpdated;
        this._indexName = indexName;
        setDaemon(true);
        setPriority(MIN_PRIORITY);
        _name = "attachment";
    }

    @Override
    public void run() {
        try {

            if(!PstRESTRepository.gettIsOFPluginRunning()){
                _status = PstReaderStatus.Finished;
                PstRESTRepository.setStatus(_name, PstReaderStatus.Finished);
                return;
            }
            _status = PstReaderStatus.Busy;
            PstRESTRepository.setStatus(_name, PstReaderStatus.Busy);
            while(true){
                tryToWait();
                PstAttachmentContainer container = PstRESTRepository.getAttachmentContainer();
                if(container == null){
                    continue;
                }
                if(PstAttachmentIndexProcess.getValue(container.getProcess()) == PstAttachmentIndexProcess.End){
                    _logger.info("!!!!!!!! Exit From Attachment Reader");
                    break;
                }
                for(PstAttachmentContent content : container.getAttachments()){
                    saveAttachment(content);
                }
            }
            _status = PstReaderStatus.Finished;
            PstRESTRepository.setStatus(_name, PstReaderStatus.Finished);

        }catch (Exception e){
            _logger.error(e.getMessage());
        }
    }

    private void saveAttachment(PstAttachmentContent attachment) throws IOException, Exception {
        if (attachment == null) {
            return;
        }
        String filename = attachment.getFilename();
        String pathname = attachment.getPath();
        long size = attachment.getSize();
        String mime = attachment.getMimetag();
        String entryid = attachment.getEntryid();
        StringBuilder strBuilder = new StringBuilder();
        String parsedContent = "";

        byte[] byteBuffer = Base64.decode(attachment.getContent());
        parsedContent = _tika.parseToString(new BytesStreamInput(byteBuffer), new Metadata());

        XContentBuilder source = jsonBuilder().startObject();

        if (_logger.isTraceEnabled()) {
            source.prettyPrint();
        }
        _logger.info(String.format("---- AttachmentFile => %s",filename));
        source
                .field(PstMetadataTags.Attachment.FILENAME, filename)
                .field(PstMetadataTags.Attachment.PATH, pathname)
                .field(PstMetadataTags.Attachment.SIZE, size)
                .field(PstMetadataTags.Attachment.MIME_TAG, mime)
                .field(PstMetadataTags.Attachment.ANALYZED_CONTENT,parsedContent)
                .field(PstMetadataTags.Attachment.CONTENT, attachment.getContent())
                .field(PstMetadataTags.Attachment.EMAIL_ID, attachment.getEmailid())
                .field(PstMetadataTags.Attachment.ENTRYID, entryid);
        source.endObject();
        esIndex(_indexName, PstMetadataTags.INDEX_TYPE_ATTACHMENT, PstSignTool.sign(UUID.randomUUID().toString()).toString(), source);
    }

}
