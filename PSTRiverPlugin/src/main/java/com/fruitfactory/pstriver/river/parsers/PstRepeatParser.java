/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package com.fruitfactory.pstriver.river.parsers;

import com.fruitfactory.pstriver.helpers.PstRiverStatus;
import com.fruitfactory.pstriver.river.parsers.core.PstParserBase;
import com.fruitfactory.pstriver.river.parsers.settings.PstEveryHourPeriodSettings;
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

    public PstRepeatParser(PstFeedDefinition def, Client client, BulkProcessor bulkProcessor, RiverName riverName, String indexName, ESLogger logger) {
        super(def, client, bulkProcessor, riverName, indexName, logger);
    }

    @Override
    protected void onProcess(List<Thread> readers) throws Exception {
        updateStatusRiver(PstRiverStatus.Busy);

        for(Thread reader : readers){
            try{
                reader.start();
                reader.join();
                ((PstOutlookFileReader)reader).close();
            }catch(Exception ex){
                getLogger().error(PstGlobalConst.PST_PREFIX + " " + ex.getMessage() );
            }
        }
        updateStatusRiver(PstRiverStatus.StandBy);
        TimeValue time = TimeValue.timeValueHours(_repeatSettings != null ? _repeatSettings.getHourPeriod() : DEFAULT_PERIOD);
        Thread.sleep(time.millis());
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
