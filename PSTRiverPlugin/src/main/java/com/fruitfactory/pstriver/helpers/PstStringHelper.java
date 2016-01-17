/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package com.fruitfactory.pstriver.helpers;

import org.apache.commons.io.FilenameUtils;
import org.apache.commons.lang3.StringUtils;

import java.util.Arrays;
import java.util.List;
import java.util.regex.Matcher;
import java.util.regex.Pattern;
import com.sun.org.apache.xerces.internal.impl.dv.util.Base64;

/**
 *
 * @author Yariki
 */
public class PstStringHelper {

    private static List<String> AllowedExtensions = Arrays.asList("DOC", "DOCX", "XLS", "XLSX", "PPT", "PPTX", "PDF", "TXT", "LOG" );
    private final static Pattern patternMime = Pattern.compile("(?:=\\?)([^\\?]+)(?:\\?B\\?)([^\\?]*)(?:\\?=)");

    public static int pecentageOfTextMatch(String text1, String text2){
        int percentage = 0;
        text1 = text1.trim().replaceAll("\\s+", " ");
        text2 = text2.trim().replaceAll("\\s+", " ");
        percentage=(int) (100 - (float) StringUtils.getLevenshteinDistance(text1, text2) * 100 / (float) (text1.length() + text2.length()));
        return percentage;
    }

    public static int hashCode(String value) {
        int h = 0;
        if (h == 0 && value.length() > 0) {
            char val[] = value.toCharArray();
            for (int i = 0; i < value.length(); i++) {
                h = 31 * h + val[i];
            }
        }
        return h;
    }

    public static boolean isFileAllowed(String filename){
        String ext = FilenameUtils.getExtension(filename).toUpperCase();
        return AllowedExtensions.indexOf(ext) > -1;
    }

    public static String DecodeMimeString(String original){
        try{
            Matcher m = patternMime.matcher(original);
            if (m.find()) {
                String charset =m.group(1);
                String data = m.group(2);
                byte[] b = Base64.decode(data);
                String res = new String(b,charset);
                return res;
            }
            return original;
        }catch (Exception ex){
            return original;
        }
    }

    
}
