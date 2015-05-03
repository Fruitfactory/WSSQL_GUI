using OF.Core.Data.ElasticSearch.Request.Base;

namespace OF.Core.Data.ElasticSearch.Request
{
    public class OFTerm<T> where T : OFBaseTerm, new()
    {
        public T term { get; set; }

        public OFTerm()
        {
            term = new T();
        }

        public OFTerm(object value)
            :this()
        {
            term.SetValue(value);
        }
    }
}