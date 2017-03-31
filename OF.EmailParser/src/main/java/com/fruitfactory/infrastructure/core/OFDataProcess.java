package com.fruitfactory.infrastructure.core;

import com.fruitfactory.interfaces.IOFDataProcessThread;
import com.fruitfactory.interfaces.IOFDataRepositoryPipe;
import com.fruitfactory.models.OFItemsContainer;
import org.apache.log4j.Logger;

/**
 * Created by Yariki on 2/1/2017.
 */
public abstract class OFDataProcess  extends Thread  implements IOFDataProcessThread{

    private final IOFDataRepositoryPipe dataSource;
    private volatile boolean stop = false;

    private final Logger logger = Logger.getLogger(OFDataProcess.class);

    public OFDataProcess(IOFDataRepositoryPipe dataSource, String name) {
        super(name);
        this.dataSource = dataSource;
    }

    @Override
    public synchronized void run() {
        while (!stop){
            OFItemsContainer container = null;
            try {
                container = dataSource.popData();
                processData(container);
            }catch(Exception ex){
                logger.error(ex.toString());
            }
        }
    }

    protected abstract void processData(OFItemsContainer container);

    protected Logger getLogger(){
        return logger;
    }

    @Override
    public void close() {
        dataSource.stopNotify();
        stop = true;
    }
}
