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
public class PstReaderStatusInfo {

    private String _name;
    private int _count;
    private int _proccedCount;
    private int _proccedAttachmentCount;
    private PstReaderStatus _status;
    private String _folderName;

    public String getFolderName() {
        return _folderName;
    }

    public void setFolderName(String _folderName) {
        this._folderName = _folderName;
    }

    public PstReaderStatus getStatus() {
        return _status;
    }

    public void setStatus(PstReaderStatus _status) {
        this._status = _status;
    }

    public PstReaderStatusInfo(String _name, int _count) {
        this._name = _name;
        this._count = _count;
    }

    public String getName() {
        return _name;
    }

    public void setName(String _name) {
        this._name = _name;
    }

    public int getCount() {
        return _count;
    }

    public void setCount(int _count) {
        this._count = _count;
    }

    public int getProccedCount() {
        return _proccedCount;
    }

    public void setProccedCount(int _proccedCount) {
        this._proccedCount = _proccedCount;
    }

    public int getProccedAttachmentCount() {
        return _proccedAttachmentCount;
    }

    public void setProccedAttachmentCount(int _proccedAttachmentCount) {
        this._proccedAttachmentCount = _proccedAttachmentCount;
    }


}
