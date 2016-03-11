package com.fruitfactory.pstriver.river.reader;

import org.elasticsearch.action.bulk.BulkProcessor;
import org.elasticsearch.common.logging.ESLogger;

/**
 * Created by yariki on 3/11/2016.
 */
public class PstCleaner extends PstBaseOutlookIndexer {

    public PstCleaner(String name, ESLogger _logger) {
        super(name, _logger, null);
    }

    @Override
    public void run() {

        while(!_closed){
            try {
                tryToWait();
                Thread.sleep(1500);
            }catch(Exception ex){
                _logger.error(getReaderName().toUpperCase() + " " + ex.toString());
            }finally {
                System.gc();
            }
        }
    }

    @Override
    protected String getReaderName() {
        return "cleaner";
    }
}
