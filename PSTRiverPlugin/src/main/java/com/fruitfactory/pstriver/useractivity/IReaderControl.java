/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package com.fruitfactory.pstriver.useractivity;

import com.fruitfactory.pstriver.helpers.PstReaderStatus;

/**
 *
 * @author Yariki
 */
public interface IReaderControl {
    boolean isPaused();

    boolean isStopped();

    void pauseThread();

    void resumeThread();

    void stopThread();
    
    PstReaderStatus getStatus();
    
}
