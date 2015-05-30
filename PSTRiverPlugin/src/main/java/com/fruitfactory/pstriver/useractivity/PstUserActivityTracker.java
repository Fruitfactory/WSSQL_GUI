/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package com.fruitfactory.pstriver.useractivity;

import java.util.List;
import java.util.logging.Level;
import java.util.logging.Logger;

/**
 *
 * @author Yariki
 */
public class PstUserActivityTracker extends Thread {
    
    private volatile boolean stopped = false;
    private List<IReaderControl> readers;
    private int onlineTime;
    private int idleTime;
    private PstWin32IdleTime.State oldState = PstWin32IdleTime.State.UNKNOWN;

    public PstUserActivityTracker(List<IReaderControl> readers) {
        this.readers = readers;
        onlineTime = 120;
        idleTime = 180;
    }

    public PstUserActivityTracker(List<IReaderControl> readers, int onlineTime, int idleTime) {
        this.readers = readers;
        this.onlineTime = onlineTime;
        this.idleTime = idleTime;
    }
    
    public void stopTracking(){
        readers = null;
        stopped = true;
    }

    @Override
    public void run() {
        while(!stopped){
            try{
                
                int idleSec = PstWin32IdleTime.getIdleTimeMillisWin32() / 1000;
                PstWin32IdleTime.State newState = idleSec < onlineTime ? PstWin32IdleTime.State.ONLINE:
                        idleSec > idleTime ? PstWin32IdleTime.State.AWAY : PstWin32IdleTime.State.IDLE;
                if(newState != oldState){
                    oldState = newState;
                    processState(newState);
                }
                Thread.sleep(1000);
            }catch(Exception ex){
                Logger.getGlobal().log(Level.SEVERE, ex.getMessage());
            }
        }
    }
    
    private void processState(PstWin32IdleTime.State state){
        if(readers == null || readers.isEmpty()){
            return;
        }
        switch(state){
            case ONLINE:
                processOnlineState(readers);
                break;
            case AWAY:
                processAwayState(readers);
                break;
        }
    }

    private void processOnlineState(List<IReaderControl> readers) {
        for(IReaderControl r : readers){
            if(!r.isPaused() && !r.isStopped()){
                r.pauseThread();
            }
        }
    }

    private void processAwayState(List<IReaderControl> readers) {
        for(IReaderControl r : readers){
            if(r.isPaused() && !r.isStopped()){
                r.resumeThread();
            }
        }
    }
    
}
