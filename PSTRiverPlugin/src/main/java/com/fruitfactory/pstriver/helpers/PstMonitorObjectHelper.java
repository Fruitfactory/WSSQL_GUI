package com.fruitfactory.pstriver.helpers;

/**
 * Created by yariki on 5/10/2016.
 */
public class PstMonitorObjectHelper {

    private Object _look = new Object();
    private PstMonitorObject _monitor = new PstMonitorObject();
    private boolean _wasSignaled = false;

    public final static PstMonitorObjectHelper INSTANCE = new PstMonitorObjectHelper();

    public void doWait(long milisec){
        synchronized (_look){
            while(!_wasSignaled){
                try {
                    _monitor.wait(milisec);
                } catch (InterruptedException e) {
                    e.printStackTrace();
                }
            }
            _wasSignaled = false;
        }
    }

    public void doNotify(){
        synchronized (_look){
            _wasSignaled = true;
            _monitor.notify();
        }
    }

    public void resetSignaledState(){
        synchronized (_look){
            _wasSignaled = false;
        }
    }
}
