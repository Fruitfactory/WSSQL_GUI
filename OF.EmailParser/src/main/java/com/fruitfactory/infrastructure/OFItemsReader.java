package com.fruitfactory.infrastructure;

import com.google.gson.Gson;
import com.google.gson.GsonBuilder;
import com.fruitfactory.interfaces.IOFItemsReader;
import com.fruitfactory.models.OFItemsContainer;

/**
 * Created by Yariki on 1/29/2017.
 */
public class OFItemsReader implements IOFItemsReader {

    public OFItemsReader() {

    }

    @Override
    public OFItemsContainer parseContainer(String body) {
        Gson gson = new GsonBuilder().setDateFormat("yyyy-MM-dd'T'HH:mm:ss.SSS").create();
        OFItemsContainer container = gson.fromJson(body,OFItemsContainer.class);
        return container;
    }
}
