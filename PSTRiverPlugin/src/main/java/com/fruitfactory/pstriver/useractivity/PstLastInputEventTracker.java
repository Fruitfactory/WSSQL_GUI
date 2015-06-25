/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package com.fruitfactory.pstriver.useractivity;

import com.fruitfactory.pstriver.rest.PstStatusRepository;
import java.util.Arrays;
import java.util.List;
import java.util.logging.Level;
import java.util.logging.Logger;
import org.elasticsearch.common.logging.ESLogger;

/**
 *
 * @author Yariki
 */
public class PstLastInputEventTracker implements IInputHookManage {
 
    @Override
    public void start() {
       
    }

    @Override
    public void unRegisterHook() {
        
    }
    
    private ESLogger logger;

    public PstLastInputEventTracker(ESLogger logger) {
        this.logger = logger;
    }
    
    @Override
    public long getIdleTime() {
        int idleTime = PstStatusRepository.getLastUserActivity();
        return idleTime;
    }
    
}
