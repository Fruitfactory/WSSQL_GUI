/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package com.fruitfactory.pstriver.useractivity;

import com.fruitfactory.pstriver.rest.PstRESTRepository;
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
        int idleTime = PstRESTRepository.getLastUserActivity();
        return idleTime;
    }
    
}
