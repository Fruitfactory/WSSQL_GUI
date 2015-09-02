/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package com.fruitfactory.pstriver.river.parsers;

import com.fruitfactory.pstriver.helpers.PstRiverStatus;
import com.fruitfactory.pstriver.interfaces.IPstRiverInitializer;
import com.fruitfactory.pstriver.river.parsers.core.PstParserBase;
import com.fruitfactory.pstriver.river.parsers.settings.PstEveryHourPeriodSettings;
import com.fruitfactory.pstriver.river.reader.PstOutlookAttachmentReader;
import com.fruitfactory.pstriver.river.reader.PstOutlookFileReader;
import com.fruitfactory.pstriver.utils.PstFeedDefinition;
import java.util.List;

import com.fruitfactory.pstriver.utils.PstGlobalConst;
import com.google.gson.Gson;
import com.google.gson.GsonBuilder;
import org.elasticsearch.action.bulk.BulkProcessor;
import org.elasticsearch.client.Client;
import org.elasticsearch.common.logging.ESLogger;
import org.elasticsearch.common.unit.TimeValue;
import org.elasticsearch.river.RiverName;

/**
 *
 * @author Yariki
 */
public class PstRepeatParser extends PstParserBase{

    private static final int DEFAULT_PERIOD = 1;

    PstEveryHourPeriodSettings _repeatSettings;

    public PstRepeatParser(PstFeedDefinition def, Client client, BulkProcessor bulkProcessor, RiverName riverName, String indexName, ESLogger logger, IPstRiverInitializer riverInitializer) {
        super(def, client, bulkProcessor, riverName, indexName, logger,riverInitializer);
    }

    @Override
    protected int onProcess(List<Thread> readers) throws Exception {
        setRiverStatus(PstRiverStatus.Busy);
        PstOutlookAttachmentReader attachmentReader = getAttachmentReader();
        attachmentReader.start();
        for(Thread reader : readers){
            try{
                reader.start();
                reader.join();
                flush();
                ((PstOutlookFileReader)reader).close();
            }catch(Exception ex){
                getLogger().error(PstGlobalConst.PST_PREFIX + " " + ex.getMessage() );
            }
        }
        attachmentReader.join(1500);
        return _repeatSettings.getHourPeriod();
    }

    @Override
    protected void parseSettings() {
        if(getDefinition().getScheduleSettings().getSettings().isEmpty()){
            return;
        }
        try {
            Gson gson = new GsonBuilder().create();
            _repeatSettings = gson.fromJson(getDefinition().getScheduleSettings().getSettings(),PstEveryHourPeriodSettings.class);
        }catch(Exception ex){
            getLogger().error(PstGlobalConst.PST_PREFIX + " " + ex.getMessage());
        }
    }
}
