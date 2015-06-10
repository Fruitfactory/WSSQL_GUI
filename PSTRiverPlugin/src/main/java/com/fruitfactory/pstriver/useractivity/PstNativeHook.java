/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package com.fruitfactory.pstriver.useractivity;

import com.sun.jna.platform.win32.Kernel32;
import com.sun.jna.platform.win32.User32;
import java.util.logging.Handler;
import java.util.logging.Level;
import java.util.logging.Logger;
import org.elasticsearch.common.logging.ESLogger;
import org.jnativehook.GlobalScreen;
import org.jnativehook.NativeHookException;
import org.jnativehook.keyboard.NativeKeyEvent;
import org.jnativehook.keyboard.NativeKeyListener;
import org.jnativehook.mouse.NativeMouseEvent;
import org.jnativehook.mouse.NativeMouseInputListener;
import org.jnativehook.mouse.NativeMouseWheelEvent;
import org.jnativehook.mouse.NativeMouseWheelListener;

/**
 *
 * @author Yariki
 */
public class PstNativeHook implements IInputHookManage{

    class GlobalKeyListener implements NativeKeyListener{

        @Override
        public void nativeKeyPressed(NativeKeyEvent nke) {
            trackEventTime();
            logger.info("HOOK: Key = " + nke.getKeyChar());
        }

        @Override
        public void nativeKeyReleased(NativeKeyEvent nke) {
            trackEventTime();
            logger.info("HOOK: Key = " + nke.getKeyChar());
        }

        @Override
        public void nativeKeyTyped(NativeKeyEvent nke) {
            trackEventTime();
            logger.info("HOOK: Key = " + nke.getKeyChar());
        }
    }
    
    class GlobalMouseInputListener implements NativeMouseInputListener{

        @Override
        public void nativeMouseClicked(NativeMouseEvent nme) {
            trackEventTime();
        }

        @Override
        public void nativeMousePressed(NativeMouseEvent nme) {
            trackEventTime();
        }

        @Override
        public void nativeMouseReleased(NativeMouseEvent nme) {
            trackEventTime();
        }

        @Override
        public void nativeMouseMoved(NativeMouseEvent nme) {
            trackEventTime();
        }

        @Override
        public void nativeMouseDragged(NativeMouseEvent nme) {
            trackEventTime();
        }
        
    }
    
    class GlobalMouseWheelListener implements NativeMouseWheelListener{

        @Override
        public void nativeMouseWheelMoved(NativeMouseWheelEvent nmwe) {
            trackEventTime();
        }
        
    }
    
    private ESLogger logger;
    private int lastInputEventTime;
    private GlobalKeyListener keyListener;
    private GlobalMouseInputListener mouseInputListener;
    private GlobalMouseWheelListener mouseWheelListener;

    public PstNativeHook(ESLogger logger) {
        keyListener = new GlobalKeyListener();
        mouseInputListener = new GlobalMouseInputListener();
        mouseWheelListener = new GlobalMouseWheelListener();
        this.logger = logger;
        disableLogging();
    }
    
    @Override
    public void start() {
        try {
            GlobalScreen.registerNativeHook();
            registerListeners();
        } catch (NativeHookException ex) {
            Logger.getLogger(PstNativeHook.class.getName()).log(Level.SEVERE, null, ex);
        }
    }

    @Override
    public void unRegisterHook() {
        try {
            GlobalScreen.unregisterNativeHook();
        } catch (NativeHookException ex) {
            Logger.getLogger(PstNativeHook.class.getName()).log(Level.SEVERE, null, ex);
        }
    }

    @Override
    public synchronized int getIdleTime() {
        return Kernel32.INSTANCE.GetTickCount() - lastInputEventTime;
    }
    
    private void registerListeners(){
        GlobalScreen.addNativeKeyListener(keyListener);
//        GlobalScreen.addNativeMouseListener(mouseInputListener);
//        GlobalScreen.addNativeMouseWheelListener(mouseWheelListener);
    }
    
    private void unregisterListeners(){
        GlobalScreen.removeNativeKeyListener(keyListener);
//        GlobalScreen.removeNativeMouseListener(mouseInputListener);
//        GlobalScreen.removeNativeMouseWheelListener(mouseWheelListener);
    }
    
    private synchronized void trackEventTime(){
        lastInputEventTime = Kernel32.INSTANCE.GetTickCount();
    }
    
    private void disableLogging(){
        Logger logger = Logger.getLogger(GlobalScreen.class.getPackage().getName());
        logger.setLevel(Level.OFF);
        Handler[] handlers = Logger.getLogger("").getHandlers();
        for (int i = 0; i < handlers.length; i++) {
            handlers[i].setLevel(Level.OFF);
        }
    }
    
}
