using OF.Core.Interfaces;

namespace OF.Core.Data.ElasticSearch.Request.Base
{
    public class OFBaseTerm : ITerm
    {
        public OFBaseTerm()
        {
        }
        public virtual void SetValue(object value){}
    }
}