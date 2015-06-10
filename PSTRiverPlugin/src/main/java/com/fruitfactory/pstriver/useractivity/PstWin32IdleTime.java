/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package com.fruitfactory.pstriver.useractivity;

import com.sun.jna.*;
import com.sun.jna.win32.*;
import com.sun.jna.platform.win32.*;
import java.util.Arrays;
import java.util.List;
import java.util.logging.Level;
import java.util.logging.Logger;
import org.elasticsearch.common.logging.ESLogger;
/**
 *
 * @author Yariki
 */
public class PstWin32IdleTime {

    /**
     * Get the amount of milliseconds that have elapsed since the last input
     * event (mouse or keyboard)
     *
     * @return idle time in milliseconds
     */
    public static long getIdleTimeMillisWin32(ESLogger logger) {
        try{
            WinUser.LASTINPUTINFO lastInputInfo = new WinUser.LASTINPUTINFO();
            User32.INSTANCE.GetLastInputInfo(lastInputInfo);
            int tickCount = Kernel32.INSTANCE.GetTickCount();
            int dwTime = lastInputInfo.dwTime;
            
            logger.info(String.format("TickCount = %d, dwTime = %d", tickCount,dwTime));
            
            return tickCount - dwTime;
        }catch(Exception ex){
            Logger.getGlobal().log(Level.SEVERE,ex.getMessage());
        }
        return 0;
    }

    enum State {

        UNKNOWN, ONLINE, IDLE, AWAY
    };
}
