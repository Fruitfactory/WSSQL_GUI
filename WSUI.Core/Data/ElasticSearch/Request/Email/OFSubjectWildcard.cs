using OF.Core.Data.ElasticSearch.Request.Base;

namespace OF.Core.Data.ElasticSearch.Request.Email
{
    public class OFSubjectWildcard : OFBaseWildcard
    {
        public object subject { get; set; }

        public OFSubjectWildcard()
        {
            
        }

        public override void SetValue(object value)
        {
            subject = string.Format("{0}*", value);
        }
    }
}