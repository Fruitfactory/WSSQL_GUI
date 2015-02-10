/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package com.fruitfactory.pstriver.utils;

import java.security.MessageDigest;
import java.security.NoSuchAlgorithmException;

/**
 *
 * @author Yariki
 */
public class PstSignTool {
    
    public static String sign(String toSign) throws NoSuchAlgorithmException {

        MessageDigest md = MessageDigest.getInstance("MD5");
        
        md.update(toSign.getBytes());

        String key = "";
        byte b[] = md.digest();
        for (int i = 0; i < b.length; i++) {
            long t = b[i] < 0 ? 256 + b[i] : b[i];
            key += Long.toHexString(t);
        }

        return key;
    }
}
