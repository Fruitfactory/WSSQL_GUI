package com.fruitfactory.interfaces;

import com.fruitfactory.models.OFItemsContainer;

/**
 * Created by Yariki on 1/29/2017.
 */
public interface IOFItemsReader {

    OFItemsContainer parseContainer(String body);

}
