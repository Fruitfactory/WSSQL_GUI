package com.fruitfactory.pstriver.helpers;

import java.util.concurrent.TimeUnit;

/**
 * Created by Yariki on 1/8/2016.
 */
public class PstTimeWatch {
    long starts;

    public static PstTimeWatch start() {
        return new PstTimeWatch();
    }

    private PstTimeWatch() {
        reset();
    }

    public PstTimeWatch reset() {
        starts = System.currentTimeMillis();
        return this;
    }

    public long time() {
        long ends = System.currentTimeMillis();
        return ends - starts;
    }

    public long time(TimeUnit unit) {
        return unit.convert(time(), TimeUnit.MILLISECONDS);
    }

}
