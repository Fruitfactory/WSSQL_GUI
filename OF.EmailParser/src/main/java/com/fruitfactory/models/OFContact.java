package com.fruitfactory.models;

import com.carrotsearch.ant.tasks.junit4.dependencies.com.google.gson.annotations.SerializedName;

import java.util.Date;

/**
 * Created by Yariki on 1/29/2017.
 */
public class OFContact {

    @SerializedName("entryid")
    public String entryid;

    @SerializedName("firstname")
    public String firstname;

    @SerializedName("lastname")
    public String lastname;

    @SerializedName("emailaddress1")
    public String emailaddress1;

    @SerializedName("emailaddress2")
    public String emailaddress2;

    @SerializedName("emailaddress3")
    public String emailaddress3;

    @SerializedName("businesstelephone")
    public String businesstelephone;

    @SerializedName("hometelephone")
    public String hometelephone;

    @SerializedName("mobiletelephone")
    public String mobiletelephone;

    @SerializedName("homeaddresscity")
    public String homeaddresscity;

    @SerializedName("homeaddresscountry")
    public String homeaddresscountry;

    @SerializedName("homeaddresspostalcode")
    public String homeaddresspostalcode;

    @SerializedName("homeaddressstate")
    public String homeaddressstate;

    @SerializedName("homeaddressstreet")
    public String homeaddressstreet;

    @SerializedName("homeaddresspostofficebox")
    public String homeaddresspostofficebox;

    @SerializedName("businessaddresscity")
    public String businessaddresscity ;

    @SerializedName("businessaddresscountry")
    public String businessaddresscountry;

    @SerializedName("businessaddressstate")
    public String businessaddressstate ;

    @SerializedName("businessaddressstreet")
    public String businessaddressstreet ;

    @SerializedName("businessaddresspostofficebox")
    public String businessaddresspostofficebox ;

    @SerializedName("keyword")
    public String keyword;

    @SerializedName("location")
    public String location;

    @SerializedName("companyname")
    public String companyname;

    @SerializedName("title")
    public String title;

    @SerializedName("departmentname")
    public String departmentname;

    @SerializedName("middlename")
    public String middlename;

    @SerializedName("displynameprefix")
    public String displynameprefix;

    @SerializedName("profession")
    public String profession;

    @SerializedName("note")
    public String note;

    @SerializedName("homeaddress")
    public String homeaddress;

    @SerializedName("workaddress")
    public String workaddress;

    @SerializedName("otheraddress")
    public String otheraddress;

    @SerializedName("birthday")
    public Date birthday;

    @SerializedName("addresstype")
    public String addresstype;

    @SerializedName("storeid")
    public String storeid;


    public OFContact() {
    }

    public String getEntryid() {
        return entryid;
    }

    public void setEntryid(String entryid) {
        this.entryid = entryid;
    }

    public String getFirstname() {
        return firstname;
    }

    public void setFirstname(String firstname) {
        this.firstname = firstname;
    }

    public String getLastname() {
        return lastname;
    }

    public void setLastname(String lastname) {
        this.lastname = lastname;
    }

    public String getEmailaddress1() {
        return emailaddress1;
    }

    public void setEmailaddress1(String emailaddress1) {
        this.emailaddress1 = emailaddress1;
    }

    public String getEmailaddress2() {
        return emailaddress2;
    }

    public void setEmailaddress2(String emailaddress2) {
        this.emailaddress2 = emailaddress2;
    }

    public String getEmailaddress3() {
        return emailaddress3;
    }

    public void setEmailaddress3(String emailaddress3) {
        this.emailaddress3 = emailaddress3;
    }

    public String getBusinesstelephone() {
        return businesstelephone;
    }

    public void setBusinesstelephone(String businesstelephone) {
        this.businesstelephone = businesstelephone;
    }

    public String getHometelephone() {
        return hometelephone;
    }

    public void setHometelephone(String hometelephone) {
        this.hometelephone = hometelephone;
    }

    public String getMobiletelephone() {
        return mobiletelephone;
    }

    public void setMobiletelephone(String mobiletelephone) {
        this.mobiletelephone = mobiletelephone;
    }

    public String getHomeaddresscity() {
        return homeaddresscity;
    }

    public void setHomeaddresscity(String homeaddresscity) {
        this.homeaddresscity = homeaddresscity;
    }

    public String getHomeaddresscountry() {
        return homeaddresscountry;
    }

    public void setHomeaddresscountry(String homeaddresscountry) {
        this.homeaddresscountry = homeaddresscountry;
    }

    public String getHomeaddresspostalcode() {
        return homeaddresspostalcode;
    }

    public void setHomeaddresspostalcode(String homeaddresspostalcode) {
        this.homeaddresspostalcode = homeaddresspostalcode;
    }

    public String getHomeaddressstate() {
        return homeaddressstate;
    }

    public void setHomeaddressstate(String homeaddressstate) {
        this.homeaddressstate = homeaddressstate;
    }

    public String getHomeaddressstreet() {
        return homeaddressstreet;
    }

    public void setHomeaddressstreet(String homeaddressstreet) {
        this.homeaddressstreet = homeaddressstreet;
    }

    public String getHomeaddresspostofficebox() {
        return homeaddresspostofficebox;
    }

    public void setHomeaddresspostofficebox(String homeaddresspostofficebox) {
        this.homeaddresspostofficebox = homeaddresspostofficebox;
    }

    public String getBusinessaddresscity() {
        return businessaddresscity;
    }

    public void setBusinessaddresscity(String businessaddresscity) {
        this.businessaddresscity = businessaddresscity;
    }

    public String getBusinessaddresscountry() {
        return businessaddresscountry;
    }

    public void setBusinessaddresscountry(String businessaddresscountry) {
        this.businessaddresscountry = businessaddresscountry;
    }

    public String getBusinessaddressstate() {
        return businessaddressstate;
    }

    public void setBusinessaddressstate(String businessaddressstate) {
        this.businessaddressstate = businessaddressstate;
    }

    public String getBusinessaddressstreet() {
        return businessaddressstreet;
    }

    public void setBusinessaddressstreet(String businessaddressstreet) {
        this.businessaddressstreet = businessaddressstreet;
    }

    public String getBusinessaddresspostofficebox() {
        return businessaddresspostofficebox;
    }

    public void setBusinessaddresspostofficebox(String businessaddresspostofficebox) {
        this.businessaddresspostofficebox = businessaddresspostofficebox;
    }

    public String getKeyword() {
        return keyword;
    }

    public void setKeyword(String keyword) {
        this.keyword = keyword;
    }

    public String getLocation() {
        return location;
    }

    public void setLocation(String location) {
        this.location = location;
    }

    public String getCompanyname() {
        return companyname;
    }

    public void setCompanyname(String companyname) {
        this.companyname = companyname;
    }

    public String getTitle() {
        return title;
    }

    public void setTitle(String title) {
        this.title = title;
    }

    public String getDepartmentname() {
        return departmentname;
    }

    public void setDepartmentname(String departmentname) {
        this.departmentname = departmentname;
    }

    public String getMiddlename() {
        return middlename;
    }

    public void setMiddlename(String middlename) {
        this.middlename = middlename;
    }

    public String getDisplynameprefix() {
        return displynameprefix;
    }

    public void setDisplynameprefix(String displynameprefix) {
        this.displynameprefix = displynameprefix;
    }

    public String getProfession() {
        return profession;
    }

    public void setProfession(String profession) {
        this.profession = profession;
    }

    public String getNote() {
        return note;
    }

    public void setNote(String note) {
        this.note = note;
    }

    public String getHomeaddress() {
        return homeaddress;
    }

    public void setHomeaddress(String homeaddress) {
        this.homeaddress = homeaddress;
    }

    public String getWorkaddress() {
        return workaddress;
    }

    public void setWorkaddress(String workaddress) {
        this.workaddress = workaddress;
    }

    public String getOtheraddress() {
        return otheraddress;
    }

    public void setOtheraddress(String otheraddress) {
        this.otheraddress = otheraddress;
    }

    public Date getBirthday() {
        return birthday;
    }

    public void setBirthday(Date birthday) {
        this.birthday = birthday;
    }

    public String getAddresstype() {
        return addresstype;
    }

    public void setAddresstype(String addresstype) {
        this.addresstype = addresstype;
    }

    public String getStoreid() {
        return storeid;
    }

    public void setStoreid(String storeid) {
        this.storeid = storeid;
    }



}
