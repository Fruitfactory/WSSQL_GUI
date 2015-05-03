using OF.Core.Data.ElasticSearch.Request.Base;

namespace OF.Core.Data.ElasticSearch.Request.Contact
{
    public class OFToNameTerm : OFBaseDictionaryTerm
    {
        protected override string GetKey()
        {
            return "to.name";
        }
    }

    public class OFToAddressTerm : OFBaseDictionaryTerm
    {
        protected override string GetKey()
        {
            return "to.address";
        }
    }

    public class OFCcNameTerm : OFBaseDictionaryTerm
    {
        protected override string GetKey()
        {
            return "cc.name";
        }
    }

    public class OFCcAddressTerm : OFBaseDictionaryTerm
    {
        protected override string GetKey()
        {
            return "cc.address";
        }
    }

    public class OFBccNameTerm : OFBaseDictionaryTerm
    {
        protected override string GetKey()
        {
            return "bcc.name";
        }
    }

    public class OFBccAddressTerm : OFBaseDictionaryTerm
    {
        protected override string GetKey()
        {
            return "bcc.address";
        }
    }

    public class OFFromNameTerm : OFBaseDictionaryTerm
    {
        protected override string GetKey()
        {
            return "fromname";
        }
    }

    public class OFFromAddressTerm : OFBaseDictionaryTerm
    {
        protected override string GetKey()
        {
            return "fromaddress";
        }
    }

    public class OFContentTerm : OFBaseDictionaryTerm
    {
        protected override string GetKey()
        {
            return "analyzedcontent";
        }
    }

    public class OFSubjectTerm : OFBaseDictionaryTerm
    {
        protected override string GetKey()
        {
            return "subject";
        }
    }

}