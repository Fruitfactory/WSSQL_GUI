package com.fruitfactory.pstriver.helpers;

import org.apache.commons.validator.routines.EmailValidator;


/**
 * Created by Yariki on 7/8/2015.
 */
public class PstEmailValidator {


    public static boolean isEmail(String email){
        if(email == null || email.isEmpty()){
            return false;
        }

        boolean result = EmailValidator.getInstance().isValid(email);
        return result;
    }


}
