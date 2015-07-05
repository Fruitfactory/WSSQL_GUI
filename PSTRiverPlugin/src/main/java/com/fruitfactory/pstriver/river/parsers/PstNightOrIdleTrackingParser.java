/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package com.fruitfactory.pstriver.river.parsers;

import com.fruitfactory.pstriver.river.parsers.core.PstParserBase;
import com.fruitfactory.pstriver.helpers.PstRiverStatus;
import static com.fruitfactory.pstriver.river.PstRiver.LOG_TAG;

import com.fruitfactory.pstriver.river.parsers.settings.PstNightIdleTimeSettings;
import com.fruitfactory.pstriver.river.reader.PstOutlookFileReader;
import com.fruitfactory.pstriver.useractivity.IInputHookManage;
import com.fruitfactory.pstriver.useractivity.IReaderControl;
import com.fruitfactory.pstriver.useractivity.PstLastInputEventTracker;
import com.fruitfactory.pstriver.useractivity.PstUserActivityTracker;
import com.fruitfactory.pstriver.utils.PstFeedDefinition;
import java.util.ArrayList;
import java.util.List;
import java.util.logging.Level;
import java.util.logging.Logger;

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
public class PstNightOrIdleTrackingParser extends PstParserBase {

    private PstNightIdleTimeSettings _settings;
    private IInputHookManage _inputHookManage;

    public PstNightOrIdleTrackingParser(PstFeedDefinition def, Client client, BulkProcessor bulkProcessor, RiverName riverName, String indexName, ESLogger logger) {
        super(def, client, bulkProcessor, riverName, indexName, logger);
        _inputHookManage = new PstLastInputEventTracker(logger);
    }

    @Override
    protected void onProcess(List<Thread> readers) throws InterruptedException, Exception {

        List<IReaderControl> readerControls = new ArrayList<>();
        for (Thread r : readers) {
            readerControls.add((IReaderControl) r);
        }

        PstUserActivityTracker tracker = null;

        tracker = new PstUserActivityTracker(this._inputHookManage, readerControls, _settings.getIdleTime(), _settings.getIdleTime(), getLogger());
        tracker.startTracking();
        getLogger().info(LOG_TAG + "User activity tracker was created...");
        getLogger().info(LOG_TAG + "Start parsing files...");
        for (Thread reader : readers) {
            reader.start();
            getLogger().info(((PstOutlookFileReader)reader).getFilename());
        }
        for(Thread reader: readers){
            try {
                reader.join();
            } catch (InterruptedException ex) {
                getLogger().error(Level.SEVERE.toString() +  ex.getMessage());
            }
        }
        for(Thread reader: readers){
            ((PstOutlookFileReader) reader).close();
        }
        if (tracker != null) {
            try {
                tracker.stopTracking();
                tracker.join();
            } catch (InterruptedException ex) {
                getLogger().error(Level.SEVERE.toString() +  ex.getMessage());
            }
        }
        readerControls.clear();
        updateStatusRiver(PstRiverStatus.StandBy);
        TimeValue time = TimeValue.timeValueHours(1);
        Thread.sleep(time.millis());
    }

    @Override
    protected void parseSettings() {
        if(getDefinition().getScheduleSettings().getSettings().isEmpty()){
            return;
        }
        try{
            Gson gson = new GsonBuilder().create();
            _settings = gson.fromJson(getDefinition().getScheduleSettings().getSettings(),PstNightIdleTimeSettings.class);
        }catch(Exception ex){
            getLogger().error(PstGlobalConst.LOG_TAG + " " + ex.getMessage());
        }

    }
}
