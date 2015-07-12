package com.fruitfactory.pstriver.river.reader;

import com.fruitfactory.pstriver.helpers.PstEmailValidator;
import com.fruitfactory.pstriver.helpers.PstStringHelper;
import com.fruitfactory.pstriver.helpers.Triple;

import java.util.ArrayList;
import java.util.List;

/**
 * Created by Yariki on 7/8/2015.
 */
public class PstShortEmailData {
    private String subject;
    private String senderName;
    private String senderAddress;
    private List<Triple<String,String,String>> listTo;
    private List<Triple<String,String,String>> listCc;
    private List<Triple<String,String,String>> listBcc;



    public PstShortEmailData(String subject, String senderName, String senderAddress, List<Triple<String, String, String>> listTo, List<Triple<String, String, String>> listCc, List<Triple<String, String, String>> listBcc) {
        this.subject = subject;
        this.senderName = senderName;
        this.senderAddress = senderAddress;
        this.listTo = listTo;
        this.listCc = listCc;
        this.listBcc = listBcc;
    }

    public String getSubject() {
        return subject;
    }

    public String getSenderName() {
        return senderName;
    }

    public String getSenderAddress() {
        return senderAddress;
    }

    public String getEmail(String name){

        String email = "";
        email = getInternalEmail(listTo,name);
        if(!email.isEmpty()){
            return email;
        }
        email = getInternalEmail(listCc,name);
        if(!email.isEmpty()){
            return email;
        }
        email = getInternalEmail(listBcc,name);
        return email;
    }

    private String getInternalEmail(List<Triple<String,String,String>> list, String name){
        for(Triple<String,String,String> data : list){
            if(data.getItem1().toLowerCase().contains(name) && PstEmailValidator.isEmail(data.getItem2())){
                return data.getItem2();
            }
        }
        return "";
    }

}
