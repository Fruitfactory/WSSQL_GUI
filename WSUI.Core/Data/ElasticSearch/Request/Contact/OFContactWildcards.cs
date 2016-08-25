using OF.Core.Data.ElasticSearch.Request.Base;

namespace OF.Core.Data.ElasticSearch.Request.Contact
{
    public class OFFirstNameWildcard : OFBaseWildcard
    {
        public object firstname { get; set; }


        public override void SetValue(object value)
        {
            firstname = string.Format("*{0}*", value);
        }
    }


    public class OFLastNameWildcard : OFBaseWildcard
    {
        public object lastname { get; set; }

        public override void SetValue(object value)
        {
            lastname = string.Format("*{0}*", value);
        }
    }


    public class OFEmailaddress1Wildcard : OFBaseWildcard
    {
        public object emailaddress1 { get; set; }

        public override void SetValue(object value)
        {
            emailaddress1 = string.Format("*{0}*", value);
        }
    }

    public class OFEmailaddress2Wildcard : OFBaseWildcard
    {
        public object emailaddress2 { get; set; }

        public override void SetValue(object value)
        {
            emailaddress2 = string.Format("*{0}*", value);
        }
    }

    public class OFEmailaddress3Wildcard : OFBaseWildcard
    {
        public object emailaddress3 { get; set; }

        public override void SetValue(object value)
        {
            emailaddress3 = string.Format("*{0}*", value);
        }
    }

}