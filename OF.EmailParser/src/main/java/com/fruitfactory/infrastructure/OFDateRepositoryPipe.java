package com.fruitfactory.infrastructure;

import com.fruitfactory.interfaces.IOFDataRepositoryPipe;
import com.fruitfactory.models.OFItemsContainer;
import org.apache.log4j.Logger;

import java.util.concurrent.LinkedBlockingQueue;

/**
 * Created by Yariki on 1/29/2017.
 */
public class OFDateRepositoryPipe implements IOFDataRepositoryPipe {

    private LinkedBlockingQueue<OFItemsContainer> dataContainer;
    private final Object syncObject = new Object();
    private volatile boolean available = false;
    private String name;

    private final Logger logger = Logger.getLogger(OFDateRepositoryPipe.class);


    public OFDateRepositoryPipe(String name) {
        dataContainer = new LinkedBlockingQueue<>();
        this.name = name;
    }

    @Override
    public  void pushData(OFItemsContainer container)  {
        try{
            synchronized (syncObject){
                dataContainer.put(container);
                available = true;
                syncObject.notifyAll();
                logger.info(String.format("Push data into '%s'",name));
                logger.info(String.format("Count ('%s') after push: %s",name,dataContainer.size()));
            }
        }catch(InterruptedException ie){
            logger.error(ie.getMessage());
        }catch (Exception ex){
            logger.error(ex.getMessage());
        }
    }

    @Override
    public void stopNotify() {
        synchronized (syncObject){
            syncObject.notifyAll();
        }
    }

    @Override
    public OFItemsContainer popData() throws InterruptedException {
        if(!available){
            synchronized (syncObject){
                try {
                    syncObject.wait();
                } catch (InterruptedException e) {
                    logger.error(e.getMessage());
                }
            }
        }
        logger.info(String.format("Pop data: %s",name));
        OFItemsContainer container = dataContainer.take();
        logger.info(String.format("Count ('%s') after pop: %s, Available (%s)",name,dataContainer.size(),available));
        available = !dataContainer.isEmpty();
        return container;
    }

    @Override
    public int count() {
        return dataContainer.size();
    }
}
