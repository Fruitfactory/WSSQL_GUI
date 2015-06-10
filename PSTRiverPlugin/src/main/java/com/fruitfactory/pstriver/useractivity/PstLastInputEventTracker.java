/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package com.fruitfactory.pstriver.useractivity;

import com.sun.jna.Native;
import com.sun.jna.Structure;
import com.sun.jna.platform.win32.Kernel32;
import com.sun.jna.platform.win32.WinDef;
import com.sun.jna.win32.StdCallLibrary;
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

        public interface Kernel32 extends StdCallLibrary {

        Kernel32 INSTANCE = (Kernel32) Native.loadLibrary("kernel32", Kernel32.class);

        public int GetTickCount();
    };

    public interface User32 extends StdCallLibrary {

        User32 INSTANCE = (User32) Native.loadLibrary("user32", User32.class);

        public static class LASTINPUTINFO extends Structure {

            public int cbSize = 8;

            public int dwTime;

            @Override
            protected List getFieldOrder() {
                return Arrays.asList(new String[]{"cbSize", "dwTime"});
            }
        }

        public boolean GetLastInputInfo(LASTINPUTINFO result);
    };

    private int lastInputTime;
    private ESLogger logger;
    private Object lock = new Object();

    public PstLastInputEventTracker(ESLogger logger) {
        this.logger = logger;
    }
    
    @Override
    public int getIdleTime() {
        User32.LASTINPUTINFO info = new User32.LASTINPUTINFO();
        User32.INSTANCE.GetLastInputInfo(info);
        
        logger.info(String.format("Last Input Info = %d", info.dwTime));
        
        return Kernel32.INSTANCE.GetTickCount() - info.dwTime;
    }
    
}
