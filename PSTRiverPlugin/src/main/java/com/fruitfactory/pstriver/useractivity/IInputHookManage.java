/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package com.fruitfactory.pstriver.useractivity;

/**
 *
 * @author Yariki
 */
public interface IInputHookManage extends IInputHookIdle {
    void start();
    void unRegisterHook();
}
