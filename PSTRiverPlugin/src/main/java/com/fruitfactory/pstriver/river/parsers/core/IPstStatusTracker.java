/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package com.fruitfactory.pstriver.river.parsers.core;

import com.fruitfactory.pstriver.helpers.PstRiverStatus;

/**
 *
 * @author Yariki
 */
public interface IPstStatusTracker {
    void setStatus(PstRiverStatus riverStatus);

    PstRiverStatus getStatus();

    PstRiverStatus getInitialStatus();
}
