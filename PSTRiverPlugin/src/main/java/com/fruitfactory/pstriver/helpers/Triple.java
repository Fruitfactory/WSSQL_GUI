/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package com.fruitfactory.pstriver.helpers;

/**
 *
 * @author Yariki
 */
public class Triple<T, T1, T2> {

    private T _item1;
    private T1 _item2;
    private T2 _item3;

    public Triple(T item1, T1 item2, T2 item3) {
        _item1 = item1;
        _item2 = item2;
        _item3 = item3;
    }

    public T getItem1() {
        return _item1;
    }

    public T1 getItem2() {
        return _item2;
    }
    
    public T2 getItem3(){
        return _item3;
    }
}
