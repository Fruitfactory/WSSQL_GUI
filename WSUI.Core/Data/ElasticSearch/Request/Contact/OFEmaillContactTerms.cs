using OF.Core.Data.ElasticSearch.Request.Base;

namespace OF.Core.Data.ElasticSearch.Request.Contact
{
    public class OFToNameTerm : OFBaseDictionaryWildcardTerm
    {
        protected override string GetKey()
        {
            return "to.name";
        }
    }

    public class OFToAddressTerm : OFBaseDictionaryWildcardTerm
    {
        protected override string GetKey()
        {
            return "to.address";
        }
    }

    public class OFCcNameTerm : OFBaseDictionaryWildcardTerm
    {
        protected override string GetKey()
        {
            return "cc.name";
        }
    }

    public class OFCcAddressTerm : OFBaseDictionaryWildcardTerm
    {
        protected override string GetKey()
        {
            return "cc.address";
        }
    }

    public class OFBccNameTerm : OFBaseDictionaryWildcardTerm
    {
        protected override string GetKey()
        {
            return "bcc.name";
        }
    }

    public class OFBccAddressTerm : OFBaseDictionaryWildcardTerm
    {
        protected override string GetKey()
        {
            return "bcc.address";
        }
    }

    public class OFFromNameTerm : OFBaseDictionaryWildcardTerm
    {
        protected override string GetKey()
        {
            return "fromname";
        }
    }

    public class OFFromAddressTerm : OFBaseDictionaryWildcardTerm
    {
        protected override string GetKey()
        {
            return "fromaddress";
        }
    }

    public class OFContactContentTerm : OFBaseDictionaryWildcardTerm
    {
        protected override string GetKey()
        {
            return "analyzedcontent";
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