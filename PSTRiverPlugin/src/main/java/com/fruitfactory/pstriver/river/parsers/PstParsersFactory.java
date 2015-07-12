/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package com.fruitfactory.pstriver.river.parsers;

import com.fruitfactory.pstriver.river.parsers.core.IPstParser;
import com.fruitfactory.pstriver.helpers.PstRiverSchedule;
import com.fruitfactory.pstriver.utils.PstFeedDefinition;
import org.elasticsearch.action.bulk.BulkProcessor;
import org.elasticsearch.client.Client;
import org.elasticsearch.common.logging.ESLogger;
import org.elasticsearch.river.RiverName;

/**
 *
 * @author Yariki
 */
public class PstParsersFactory {

    private PstParsersFactory(){
        
    }
    
    private static PstParsersFactory _instance = new PstParsersFactory();
    
    public static PstParsersFactory getInstance(){
        return _instance;
    }
    
    public IPstParser getParser(PstRiverSchedule type,PstFeedDefinition def, Client client, BulkProcessor bulkProcessor, RiverName riverName, String indexName, ESLogger logger){
        
        logger.info(String.format("Current schedule type %s....", type.toString()));
        
        switch(type){
            case EveryNightOrIdle:
                return new PstNightOrIdleTrackingParser(def,client,bulkProcessor,riverName,indexName,logger);
            case OnlyAt:
                return new PstOnlyAtParser(def, client, bulkProcessor, riverName, indexName, logger);
            case EveryHours:
                return new PstRepeatParser(def, client, bulkProcessor, riverName, indexName, logger);
            case Never:
                return null;
        }
        return null;
    }
    
}
