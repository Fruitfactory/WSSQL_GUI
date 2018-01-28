package com.fruitfactory.infrastructure;

import com.fruitfactory.interfaces.IOFDataRepositoryPipe;
import com.fruitfactory.models.OFItemsContainer;

/**
 * Created by Yariki on 2/1/2017.
 */
public class OFDataParser extends com.fruitfactory.infrastructure.core.OFDataProcess {

    private OFItemsParser itemsParser;
    private IOFDataRepositoryPipe dataTarget;

    public OFDataParser(IOFDataRepositoryPipe dataSource, IOFDataRepositoryPipe dataTarget, String name) {
        super(dataSource, name);
        itemsParser = new OFItemsParser(getLogger());
        this.dataTarget = dataTarget;
    }

    @Override
    protected void processData(OFItemsContainer container) {
        try {
            getLogger().info("Process data...");
            itemsParser.processEmail(container);
            itemsParser.processAttachments(container);

            dataTarget.pushData(container);
            if(container != null && container.getEmail()  != null){
                getLogger().info(String.format("Parse: %s",container.getEmail().getSubject()));
            }

        }catch (Exception ex){
            getLogger().error(ex.toString());
        }
    }

}
