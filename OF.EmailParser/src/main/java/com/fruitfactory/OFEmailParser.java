package com.fruitfactory;

import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;

/**
 * Created by Yariki on 1/29/2017.
 */
@SpringBootApplication
public class OFEmailParser {

    public static void main(String[] args){
        System.setProperty("userApp.logFolder",System.getenv("APPDATA") + "/Outlookfinder/Log");
        SpringApplication.run(OFEmailParser.class,args);
    }

}
