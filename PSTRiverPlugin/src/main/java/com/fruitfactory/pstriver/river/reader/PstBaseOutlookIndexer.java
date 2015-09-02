package com.fruitfactory.pstriver.river.reader;

import com.fruitfactory.pstriver.helpers.PstReaderStatus;
import com.fruitfactory.pstriver.useractivity.IReaderControl;
import org.elasticsearch.action.bulk.BulkProcessor;
import org.elasticsearch.action.index.IndexRequest;
import org.elasticsearch.common.logging.ESLogger;
import org.elasticsearch.common.xcontent.XContentBuilder;

import java.util.logging.Level;
import java.util.logging.Logger;

import static com.fruitfactory.pstriver.river.PstRiver.LOG_TAG;

/**
 * Created by Yariki on 8/26/2015.
 */
public class PstBaseOutlookIndexer extends Thread  implements IReaderControl {
    protected ESLogger _logger;
    protected boolean _closed = false;
    protected PstReaderStatus _status;
    protected BulkProcessor _bulkProcessor;
    private Object LOCK = new Object();
    private boolean _paused = false;

    public PstBaseOutlookIndexer(String name, ESLogger _logger, BulkProcessor _bulkProcessor) {
        super(name);
        this._logger = _logger;
        this._bulkProcessor = _bulkProcessor;
    }

    public void pauseThread(){
        synchronized(LOCK){
            _paused = true;
            LOCK.notifyAll();
            _logger.info(LOG_TAG + "!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! Thread #"+ getName() + " was paused...");
        }
    }

    public void resumeThread(){
        synchronized(LOCK){
            _paused = false;
            LOCK.notifyAll();
            _logger.info(LOG_TAG + "!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! Thread #"+ getName() + " was resumed...");
        }
    }

    public boolean isStopped(){
        return _closed;
    }

    public boolean isPaused(){
        return _paused;
    }

    protected void tryToWait(){
        synchronized(LOCK){
            if(_paused){
                try {
                    LOCK.wait();
                } catch (InterruptedException ex) {
                    Logger.getLogger(PstOutlookFileReader.class.getName()).log(Level.SEVERE, null, ex);
                }
            }
        }
    }

    @Override
    public PstReaderStatus getStatus() {
        return _status;
    }

    protected void esIndex(String index, String type, String id,
                           XContentBuilder xb) throws Exception {
        if (_logger.isDebugEnabled()) {
            _logger.debug("Indexing in ES " + index + ", " + type + ", " + id);
        }
        if (_logger.isTraceEnabled()) {
            _logger.trace("JSon indexed : {}", xb.string());
        }

        if (!_closed) {
            _bulkProcessor.add(new IndexRequest(index, type, id).source(xb));
        } else {
            _logger.warn("trying to add new file while closing river. Document [{}]/[{}]/[{}] has been ignored", index, type, id);
        }
    }
}
