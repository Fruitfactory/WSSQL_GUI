using Nest.Resolvers;
using OF.Core.Data.ElasticSearch.Request.Base;

namespace OF.Core.Data.ElasticSearch.Request
{
    public class OFQuerySimpleTerm<T> where T : OFBaseTerm, new()
    {
        public T term { get; set; }

        public OFQuerySimpleTerm(object value)
        {
            term = new T();
            term.SetValue(value);
        }
    }
}