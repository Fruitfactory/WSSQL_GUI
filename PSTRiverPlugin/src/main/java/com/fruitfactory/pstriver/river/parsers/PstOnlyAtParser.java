/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package com.fruitfactory.pstriver.river.parsers;

import com.fruitfactory.pstriver.helpers.PstRiverStatus;
import com.fruitfactory.pstriver.interfaces.IPstRiverInitializer;
import com.fruitfactory.pstriver.river.parsers.core.PstParserBase;
import com.fruitfactory.pstriver.river.parsers.settings.PstOnlyAtSettings;
import com.fruitfactory.pstriver.river.reader.PstOutlookAttachmentReader;
import com.fruitfactory.pstriver.river.reader.PstOutlookFileReader;
import com.fruitfactory.pstriver.utils.PstFeedDefinition;
import com.fruitfactory.pstriver.utils.PstGlobalConst;
import com.google.gson.Gson;
import com.google.gson.GsonBuilder;


import java.text.SimpleDateFormat;
import java.util.Calendar;
import java.util.Date;
import java.util.GregorianCalendar;
import java.util.List;
import java.util.logging.Level;

import org.elasticsearch.action.bulk.BulkProcessor;
import org.elasticsearch.client.Client;
import org.elasticsearch.common.logging.ESLogger;
import org.elasticsearch.river.RiverName;
import org.joda.time.DateTime;

/**
 *
 * @author Yariki
 */
public class PstOnlyAtParser extends PstParserBase {

    private final String AM_CONST = "am";
    private final String PM_CONST = "pm";
    private final int HOUR_SHIFT = 12;

    private PstOnlyAtSettings _onlyAtSettings = null;
    private int _hour;

    public PstOnlyAtParser(PstFeedDefinition def, Client client, BulkProcessor bulkProcessor, RiverName riverName, String indexName, ESLogger logger, IPstRiverInitializer riverInitializer) {
        super(def, client, bulkProcessor, riverName, indexName, logger,riverInitializer);
    }

    @Override
    protected int onProcess(List<Thread> readers) throws Exception {
        
        setRiverStatus(PstRiverStatus.StandBy);

        while(true){
            DateTime date = new DateTime();
            int hour = date.getHourOfDay();
            int minutes = date.getMinuteOfHour();
            if(hour == _hour && minutes == 0){
                break;
            }
            Thread.sleep(1250);
        }

        setRiverStatus(PstRiverStatus.Busy);
        PstOutlookAttachmentReader attachmentReader = getAttachmentReader();
        attachmentReader.start();
        for (Thread reader : readers){
            try {
                reader.start();
                reader.join();
                flush();
                ((PstOutlookFileReader) reader).close();
            } catch (InterruptedException ex) {
                getLogger().error(Level.SEVERE.toString() +  ex.getMessage());
            }
        }

        attachmentReader.join(1500);
        
        return 0;
    }


    @Override
    protected void parseSettings(){
        if(getDefinition().getScheduleSettings().getSettings().isEmpty()){
            return;
        }
        try{
            Gson gson = new GsonBuilder().create();
            _onlyAtSettings = gson.fromJson(getDefinition().getScheduleSettings().getSettings(), PstOnlyAtSettings.class);
            if(_onlyAtSettings != null){
                _hour = getTimeAccordingSettings(_onlyAtSettings);
            }
        }catch(Exception ex){
            getLogger().error(PstGlobalConst.PST_PREFIX +  " " + ex.getMessage());
        }
    }

    private int getTimeAccordingSettings(PstOnlyAtSettings settings){
        String type = settings.getHourType().toLowerCase();
        if(settings.getHourOnlyAt() == 12 && type.equals(AM_CONST)){
            return 0;
        }
        if(settings.getHourOnlyAt() == 12 && type.equals(PM_CONST)){
            return settings.getHourOnlyAt();
        }
        switch (type) {
            case AM_CONST:
                return settings.getHourOnlyAt();
            case PM_CONST:
                return settings.getHourOnlyAt() + HOUR_SHIFT;
        }
        return 0;
    }

    
}
