package com.fruitfactory.infrastructure.core;

import com.fruitfactory.interfaces.IOFDataProcessThread;
import com.fruitfactory.interfaces.IOFDataRepositoryPipe;
import com.fruitfactory.metadata.OFMetadataTags;
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
    public void run() {
        while (true){
            OFItemsContainer container = null;
            try {
                container = dataSource.popData();
                if(container != null && container.getProcess() == OFMetadataTags.END_PROCESS){
                    logger.info(String.format("Exit Thread (by closing): '%s'", this.getName()));
                    break;
                }
                if(container != null){
                    processData(container);
                }
            }catch(Exception ex){
                logger.error(ex.toString());
            }
        }
        logger.info(String.format("Exit Thread (another reason): '%s'", this.getName()));
    }

    protected abstract void processData(OFItemsContainer container);

    protected Logger getLogger(){
        return logger;
    }

    @Override
    public void close() {
        try {
            logger.info("Close thread...");
            dataSource.pushData(new OFItemsContainer(OFMetadataTags.END_PROCESS));
            dataSource.stopNotify();
        } catch (InterruptedException e) {
            e.printStackTrace();
        }
        try{
            join();
        }catch (InterruptedException e){
            logger.error(e.toString());
        }
    }
}
