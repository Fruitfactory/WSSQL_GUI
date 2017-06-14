package com.fruitfactory.infrastructure;

import com.fruitfactory.interfaces.IOFDataRepositoryPipe;
import com.fruitfactory.models.OFItemsContainer;

/**
 * Created by Yariki on 2/1/2017.
 */
public class OFDataParser extends com.fruitfactory.infrastructure.core.OFDataProcess {

    private OFItemsParser itemsParser;
    private OFDataSender dataSender;



    public OFDataParser(IOFDataRepositoryPipe dataSource, String name) {
        super(dataSource, name);
        itemsParser = new OFItemsParser(getLogger());
        dataSender = new OFDataSender("sender",getLogger());
    }

    @Override
    protected void processData(OFItemsContainer container) {
        try {
            getLogger().info("Process data...");
            itemsParser.processEmail(container);
            itemsParser.processAttachments(container);

            dataSender.sendData(container);

        }catch (Exception ex){
            getLogger().error(ex.toString());
        }
    }

}
