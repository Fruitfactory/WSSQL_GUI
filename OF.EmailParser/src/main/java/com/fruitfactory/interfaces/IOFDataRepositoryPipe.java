package com.fruitfactory.interfaces;

import com.fruitfactory.models.OFItemsContainer;

/**
 * Created by Yariki on 1/29/2017.
 */
public interface IOFDataRepositoryPipe {

    void pushData(OFItemsContainer container) throws InterruptedException;

    OFItemsContainer popData();

    void stopNotify();

}
