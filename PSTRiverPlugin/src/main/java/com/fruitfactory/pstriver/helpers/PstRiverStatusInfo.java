/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package com.fruitfactory.pstriver.helpers;

import java.util.Date;

/**
 *
 * @author Yariki
 */
public class PstRiverStatusInfo {
    private PstRiverStatus _status;
    private Date _lastDateUpdated;
    private int _countEmails;
    private int _countAttachments;

    public PstRiverStatusInfo(PstRiverStatus _status, Date _lastDateUpdated, int countEmails, int countAttchments) {
        this._status = _status;
        this._lastDateUpdated = _lastDateUpdated;
        this._countEmails = countEmails;
        this._countAttachments = countAttchments;
    }

    public PstRiverStatus getStatus() {
        return _status;
    }

    public void setStatus(PstRiverStatus _status) {
        this._status = _status;
    }

    public Date getLastDateUpdated() {
        return _lastDateUpdated;
    }

    public void setLastDateUpdated(Date _lastDateUpdated) {
        this._lastDateUpdated = _lastDateUpdated;
    }

    public int getCountEmails() {
        return _countEmails;
    }

    public void setCountEmails(int _countEmails) {
        this._countEmails = _countEmails;
    }

    public int getCountAttachments() {
        return _countAttachments;
    }

    public void setCountAttachments(int _countAttachments) {
        this._countAttachments = _countAttachments;
    }
}
