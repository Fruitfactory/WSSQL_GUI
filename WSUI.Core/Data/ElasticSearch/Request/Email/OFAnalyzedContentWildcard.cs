using OF.Core.Data.ElasticSearch.Request.Base;

namespace OF.Core.Data.ElasticSearch.Request.Email
{
    public class OFAnalyzedContentWildcard : OFBaseWildcard
    {
        public object analyzedcontent { get; set; }

        public OFAnalyzedContentWildcard()
        {
            
        }

        public override void SetValue(object value)
        {
            analyzedcontent = string.Format("{0}*", value);
        }
    }
}