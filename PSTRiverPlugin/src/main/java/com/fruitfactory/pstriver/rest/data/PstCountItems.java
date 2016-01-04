package com.fruitfactory.pstriver.rest.data;

import com.carrotsearch.ant.tasks.junit4.dependencies.com.google.gson.annotations.SerializedName;

/**
 * Created by Yariki on 1/1/2016.
 */
public class PstCountItems {

    @SerializedName("count")
    public int count;

    public PstCountItems() {
    }

    public int getCount() {
        return count;
    }

    public void setCount(int count) {
        this.count = count;
    }
}
