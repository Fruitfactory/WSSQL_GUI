/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package com.fruitfactory.pstriver.useractivity;

import com.sun.jna.platform.win32.Kernel32;
import com.sun.jna.platform.win32.User32;
import com.sun.jna.platform.win32.WinUser;
import com.sun.jna.platform.win32.WinDef.HMODULE;
import com.sun.jna.platform.win32.WinDef.LRESULT;
import com.sun.jna.platform.win32.WinDef.WPARAM;
import com.sun.jna.platform.win32.WinUser.HHOOK;
import com.sun.jna.platform.win32.WinUser.KBDLLHOOKSTRUCT;
import com.sun.jna.platform.win32.WinUser.LowLevelKeyboardProc;
import com.sun.jna.platform.win32.WinUser.MSG;
import java.util.logging.Level;
import java.util.logging.Logger;
import org.elasticsearch.common.logging.ESLogger;

/**
 *
 * @author Yariki
 */
public class PstInputHook extends Thread implements IInputHookManage{

    private WinUser.HHOOK hhk;
    private WinUser.HHOOK hhm;
    private WinUser.LowLevelKeyboardProc keyboardHook;
    private HMODULE hMod;
    private int lastInputEventTime;
    private final User32 userLib = User32.INSTANCE;
    private ESLogger logger;

    public PstInputHook(ESLogger logger) {
        this.logger = logger;
    }
    
    @Override
    public void run() {
        lastInputEventTime = Kernel32.INSTANCE.GetTickCount();
        hMod = Kernel32.INSTANCE.GetModuleHandle(null);

        keyboardHook = new LowLevelKeyboardProc() {

            @Override
            public LRESULT callback(int i, WPARAM wparam, KBDLLHOOKSTRUCT k) {
                if (i >= 0) {
                    switch (wparam.intValue()) {
                        case WinUser.WM_KEYUP:
                        case WinUser.WM_KEYDOWN:
                        case WinUser.WM_SYSKEYDOWN:
                        case WinUser.WM_SYSKEYUP:
                            System.err.println("in callback, key=" + k.vkCode);
                            lastInputEventTime = Kernel32.INSTANCE.GetTickCount();
                            break;
                    }
                }
                logger.info(METHOD_NAME, "" + k.vkCode);
                return userLib.CallNextHookEx(hhk, i, wparam, k.getPointer());
            }
        };

        hhk = userLib.SetWindowsHookEx(WinUser.WH_KEYBOARD_LL, keyboardHook, hMod, 0);
        int result;
        MSG msg = new MSG();
        while((result = userLib.GetMessage(msg, null, 0, 0)) != 0){
            if(result == -1){
                break;
            }else{
                userLib.TranslateMessage(msg);
                userLib.DispatchMessage(msg);
            }
        }
        if (hhk != null) {
            userLib.UnhookWindowsHookEx(hhk);
        }
        
    }

    @Override
    public int getIdleTime() {
        return Kernel32.INSTANCE.GetTickCount() - lastInputEventTime;
    }

    @Override
    public void unRegisterHook(){
        if(hhk != null){
            userLib.UnhookWindowsHookEx(hhk);
            this.interrupt();
        }
    }
    
}
