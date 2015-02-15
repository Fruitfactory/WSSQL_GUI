/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package com.fruitfactory.pstriver.helpers;

/**
 *
 * @author Yariki
 */
public class AttachmentHelper {
    private String _filename;
    private String _path;
    private long _size;
    private String _mimetype;

    public AttachmentHelper(String _filename, String _path, long _size, String _mimetype) {
        this._filename = _filename;
        this._path = _path;
        this._size = _size;
        this._mimetype = _mimetype;
    }

    public String getFilename() {
        return _filename;
    }

    public String getMimetype() {
        return _mimetype;
    }

    public String getPath() {
        return _path;
    }

    public long getSize() {
        return _size;
    }
    
}
