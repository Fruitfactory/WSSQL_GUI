/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package com.fruitfactory.pstriver.useractivity;

import com.fruitfactory.pstriver.helpers.PstReaderStatus;

import java.util.Date;
import java.util.List;
import java.util.logging.Level;
import java.util.logging.Logger;

import com.fruitfactory.pstriver.helpers.PstRiverStatus;
import com.fruitfactory.pstriver.interfaces.IPstRestAttachmentClient;
import com.fruitfactory.pstriver.rest.PstRESTRepository;
import com.fruitfactory.pstriver.river.parsers.core.IPstStatusTracker;
import org.elasticsearch.common.logging.ESLogger;
import org.elasticsearch.common.unit.TimeValue;
import org.joda.time.*;

/**
 *
 * @author Yariki
 */
public class PstUserActivityTracker extends Thread {
    
    public enum State {

        UNKNOWN, ONLINE, IDLE, AWAY, NIGHT
    };
    
    private volatile boolean stopped = false;
    private List<IReaderControl> readers;
    private int onlineTime;
    private int idleTime;
    private State oldState = State.UNKNOWN;
    private ESLogger logger;
    private IInputHookIdle hookIdleTime;
    private LocalTime timeBeginNight = new LocalTime(0,0,0);
    private LocalTime timeFinishNight = new LocalTime(6,0,0);
    private final Object lock = new Object();
    private Date _lastUpdated;
    private boolean isWorking = false;

    private IPstStatusTracker statusTracker;
    private IPstRestAttachmentClient restAttachmentClient;
    
    public PstUserActivityTracker(IInputHookIdle hookIdleTime, IPstStatusTracker statusTracker, List<IReaderControl> readers, IPstRestAttachmentClient restAttachmentClient, ESLogger logger) {
        this.readers = readers;
        onlineTime = (int)TimeValue.timeValueMinutes(2).getSeconds();
        idleTime = (int)TimeValue.timeValueMinutes(2).getSeconds();
        this.logger = logger;
        this.hookIdleTime = hookIdleTime;
        this.statusTracker = statusTracker;
        this.restAttachmentClient = restAttachmentClient;
    }

    public PstUserActivityTracker(IInputHookIdle hookIdleTime, IPstStatusTracker statusTracker, List<IReaderControl> readers, IPstRestAttachmentClient restAttachmentClient, Date lastDate, int onlineTime, int idleTime, ESLogger logger) {
        this.readers = readers;
        this.onlineTime = onlineTime;
        this.idleTime = idleTime;
        this.logger = logger;
        this.hookIdleTime = hookIdleTime;
        this.statusTracker = statusTracker;
        this.restAttachmentClient = restAttachmentClient;
        this._lastUpdated = lastDate;
    }
    
    public  void startTracking(){
        start();
        isWorking = true;
    }
    
    public void stopTracking(){
        synchronized (lock){
            readers = null;
            restAttachmentClient = null;
            stopped = true;
            isWorking = false;
        }
    }

    public void setReaders(List<IReaderControl> readers){
        synchronized (lock){
            this.readers = readers;
        }
    }

    public synchronized boolean isWorking(){
        return isWorking;
    }

    public synchronized State getLastState(){
        return oldState;
    }

    @Override
    public void run() {
        while(true){
            synchronized (lock){
                if(stopped){
                    break;
                }
            }
            try{
                State state = State.UNKNOWN;
                long idleSec = this.hookIdleTime.getIdleTime();// / 1000; // PstWin32IdleTime.getIdleTimeMillisWin32(logger) / 1000;
                boolean isForce = PstRESTRepository.isForce();
                State newState = IsHight() || isForce ? State.NIGHT : idleSec < onlineTime ? State.ONLINE: idleSec > idleTime ? State.AWAY : State.IDLE;
                if(newState != getLastState()){
                    processState(newState);
                }
                //System.out.println(String.format("Time = %s, State = %s", idleSec, newState));
                //logger.info(String.format("Time = %s, State = %s, OldState = %s", idleSec, newState,oldState));
                setLastState(newState);
                Thread.sleep(1000);
            }catch(Exception ex){
                Logger.getGlobal().log(Level.SEVERE, ex.getMessage());
                logger.error("Tracker: " + ex.getMessage());
            }
        }
    } 

    private synchronized void setLastState(State newState){
        oldState = newState;
    }

    private void processState(State state){
        if(readers == null || readers.isEmpty()){
            return;
        }
        switch(state){
            case ONLINE:
                processOnlineState(readers);
                break;
            case AWAY:
            case NIGHT:
                processAwayState(readers);
                break;
            case IDLE:
                break;
        }
    }

    private void processOnlineState(List<IReaderControl> readers) {
        synchronized (lock){
            for(IReaderControl r : readers){
                if(r.getStatus() == PstReaderStatus.Busy && !r.isPaused() && !r.isStopped()){
                    r.pauseThread();
                }
            }
        }
        if(restAttachmentClient != null){
            restAttachmentClient.suspentRead();
        }
        statusTracker.setStatus(PstRiverStatus.StandBy);
    }
    
    private boolean IsHight(){
        LocalTime currentTime = (new DateTime()).toLocalTime();
        return timeBeginNight.getMillisOfDay() < currentTime.getMillisOfDay() && currentTime.getMillisOfDay() < timeFinishNight.getMillisOfDay();
    }

    private void processAwayState(List<IReaderControl> readers) {
        synchronized (lock){
            for(IReaderControl r : readers){
                if(r.getStatus() == PstReaderStatus.Busy && r.isPaused() && !r.isStopped()){
                    r.resumeThread();
                }
            }
        }
        if(restAttachmentClient != null){
            restAttachmentClient.resumeRead(_lastUpdated);
        }
        statusTracker.setStatus(PstRiverStatus.Busy);
    }
    
}
