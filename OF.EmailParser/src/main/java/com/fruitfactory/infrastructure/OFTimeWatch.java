/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package com.fruitfactory.infrastructure;

import java.util.concurrent.TimeUnit;

/**
 *
 * @author Yariki
 */
public class OFTimeWatch {
    
    
    long starts;

    public OFTimeWatch() {
    }
    
    public OFTimeWatch reset(){
        starts = System.currentTimeMillis();
        return this;
    }
    
    public long time(){
        long ends =  System.currentTimeMillis();
        return ends - starts;
    }
    
    public long time(TimeUnit unit){
        return unit.convert(time(), TimeUnit.MILLISECONDS);
    }
    
    public long timeInMiliSeconds(){
        return time(TimeUnit.MILLISECONDS);
    }
    
    public long timeInSeconds(){
        return time(TimeUnit.SECONDS);
    }
   
    
}
