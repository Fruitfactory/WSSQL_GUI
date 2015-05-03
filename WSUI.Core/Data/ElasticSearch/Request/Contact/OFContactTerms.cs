using System.Collections.Generic;
using OF.Core.Data.ElasticSearch.Request.Base;

namespace OF.Core.Data.ElasticSearch.Request.Contact
{
    public class OFFirstNameTerm : OFBaseDictionaryTerm
    {
        protected override string GetKey()
        {
            return "firstname";
        }

    }

    public class OFLastNameTerm : OFBaseDictionaryTerm
    {
        protected override string GetKey()
        {
            return "lastname";
        }
    }

    public class OFEmailaddress1Term : OFBaseDictionaryTerm
    {
        protected override string GetKey()
        {
            return "emailaddress1";
        }
    }

    public class OFEmailaddress2Term : OFBaseDictionaryTerm
    {
        protected override string GetKey()
        {
            return "emailaddress2";
        }
    }

    public class OFEmailaddress3Term : OFBaseDictionaryTerm
    {
        protected override string GetKey()
        {
            return "emailaddress3";
        }
    }

}