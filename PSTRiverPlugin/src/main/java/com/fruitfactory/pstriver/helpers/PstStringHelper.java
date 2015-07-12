/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package com.fruitfactory.pstriver.helpers;

import org.apache.commons.lang3.StringUtils;

/**
 *
 * @author Yariki
 */
public class PstStringHelper {
    
    public static int pecentageOfTextMatch(String text1, String text2){
        int percentage = 0;
        text1 = text1.trim().replaceAll("\\s+", " ");
        text2 = text2.trim().replaceAll("\\s+", " ");
        percentage=(int) (100 - (float) StringUtils.getLevenshteinDistance(text1, text2) * 100 / (float) (text1.length() + text2.length()));
        return percentage;
    }
    
}
