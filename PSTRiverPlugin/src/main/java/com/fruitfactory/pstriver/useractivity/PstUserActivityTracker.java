/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package com.fruitfactory.pstriver.useractivity;

import com.fruitfactory.pstriver.helpers.PstReaderStatus;
import java.util.List;
import java.util.logging.Level;
import java.util.logging.Logger;
import org.elasticsearch.common.logging.ESLogger;
import org.elasticsearch.common.unit.TimeValue;

/**
 *
 * @author Yariki
 */
public class PstUserActivityTracker extends Thread {
    
     enum State {

        UNKNOWN, ONLINE, IDLE, AWAY
    };
    
    private volatile boolean stopped = false;
    private List<IReaderControl> readers;
    private int onlineTime;
    private int idleTime;
    private State oldState = State.UNKNOWN;
    private ESLogger logger;
    private IInputHookIdle hookIdleTime;

    public PstUserActivityTracker(IInputHookIdle hookIdleTime, List<IReaderControl> readers,ESLogger logger) {
        this.readers = readers;
        onlineTime = (int)TimeValue.timeValueMinutes(2).getSeconds();
        idleTime = (int)TimeValue.timeValueMinutes(2).getSeconds();
        this.logger = logger;
        this.hookIdleTime = hookIdleTime;
    }

    public PstUserActivityTracker(IInputHookIdle hookIdleTime, List<IReaderControl> readers, int onlineTime, int idleTime,ESLogger logger) {
        this.readers = readers;
        this.onlineTime = onlineTime;
        this.idleTime = idleTime;
        this.logger = logger;
        this.hookIdleTime = hookIdleTime;
    }
    
    public  void startTracking(){
        start();
    }
    
    public void stopTracking(){
        readers = null;
        stopped = true;
    }

    @Override
    public void run() {
        while(!stopped){
            try{
                State state = State.UNKNOWN;
                long idleSec = this.hookIdleTime.getIdleTime();// / 1000; // PstWin32IdleTime.getIdleTimeMillisWin32(logger) / 1000;
                
                System.out.println(String.format("Idle Sec = %d", idleSec));
                logger.info("Tracker: " + String.format("Idle Sec = %d", idleSec));
                State newState = idleSec < onlineTime ? State.ONLINE: idleSec > idleTime ? State.AWAY : State.IDLE;
                if(newState != state){
                    processState(newState);
                }
                Thread.sleep(1000);
            }catch(Exception ex){
                Logger.getGlobal().log(Level.SEVERE, ex.getMessage());
                logger.error("Tracker: " + ex.getMessage());
            }
        }
    } 
    
    private void processState(State state){
        if(readers == null || readers.isEmpty()){
            return;
        }
        switch(state){
            case ONLINE:
                processOnlineState(readers);
                System.out.println("Online time!!");
                logger.info("Tracker: " + "User Online!!" );
                break;
            case AWAY:
                processAwayState(readers);
                System.out.println("Away time!!");
                logger.info("Tracker: " + "User Away!!" );
                break;
            case IDLE:
                System.out.println("Idle time!!");
                logger.info("Tracker: " + "User Idle!!" );
                break;
        }
    }

    private void processOnlineState(List<IReaderControl> readers) {
        for(IReaderControl r : readers){
            if(r.getStatus() == PstReaderStatus.Busy && !r.isPaused() && !r.isStopped()){
                r.pauseThread();
            }
        }
    }

    private void processAwayState(List<IReaderControl> readers) {
        for(IReaderControl r : readers){
            if(r.getStatus() == PstReaderStatus.Busy && r.isPaused() && !r.isStopped()){
                r.resumeThread();
            }
        }
    }
    
}
