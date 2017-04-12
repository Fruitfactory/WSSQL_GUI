package com.fruitfactory.controllers;

import com.fruitfactory.infrastructure.OFDataParse;
import com.fruitfactory.infrastructure.OFDataSender;
import com.fruitfactory.infrastructure.OFDateRepositoryPipe;
import com.fruitfactory.infrastructure.OFItemsReader;
import com.fruitfactory.interfaces.IOFDataProcessThread;
import com.fruitfactory.interfaces.IOFDataRepositoryPipe;
import com.fruitfactory.interfaces.IOFItemsReader;
import com.fruitfactory.models.OFItemsContainer;
import com.fruitfactory.models.response.OFEmailResponse;
import org.apache.log4j.Logger;
import org.springframework.stereotype.Controller;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestMethod;
import org.springframework.web.bind.annotation.ResponseBody;

/**
 * Created by Yariki on 1/29/2017.
 */
@Controller
@RequestMapping(value = "/parse")
public class OFEmailController {

    private final IOFItemsReader reader = new OFItemsReader();
    private final Logger logger = Logger.getLogger(OFEmailController.class);
    private IOFDataProcessThread dataParser = null;
    private IOFDataProcessThread dataSender = null;
    private IOFDataRepositoryPipe dataIncome = null;
    private IOFDataRepositoryPipe dataOutcome = null;

    public OFEmailController() {
        dataIncome = new OFDateRepositoryPipe();
        dataOutcome = new OFDateRepositoryPipe();
        dataParser = new OFDataParse(dataIncome,dataOutcome,"dataParser");
        dataSender = new OFDataSender(dataOutcome,"dataSender");
        dataParser.start();
        dataSender.start();
    }

    @RequestMapping(value = "/items",method = RequestMethod.POST,produces = "application/json")
    @ResponseBody
    public OFEmailResponse putEmailContainer(@RequestBody String body){
        OFEmailResponse response = new OFEmailResponse(false,"");
        try {
            //logger.info(body);
            OFItemsContainer container = reader.parseContainer(body);
            if(container != null){
                dataIncome.pushData(container);
                response.setStatus(true);
            }
        }catch(Exception ex){
            response.setMessage(ex.getMessage());
            logger.error(ex.getMessage());
        }
        return response;
    }

}
