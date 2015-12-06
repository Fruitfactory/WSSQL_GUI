package com.fruitfactory.pstriver.interfaces;

import java.util.Date;

/**
 * Created by Yariki on 12/5/2015.
 */
public interface IPstRestAttachmentClient {

    void startRead(Date date);

    void stopRead();

    void suspentRead();

    void resumeRead();

}
