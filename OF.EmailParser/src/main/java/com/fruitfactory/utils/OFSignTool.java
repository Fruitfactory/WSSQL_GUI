package com.fruitfactory.utils;

import java.security.MessageDigest;
import java.security.NoSuchAlgorithmException;

/**
 * Created by Yariki on 2/5/2017.
 */
public class OFSignTool {
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
