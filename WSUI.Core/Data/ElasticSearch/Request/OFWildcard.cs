using Nest;
using OF.Core.Data.ElasticSearch.Request.Base;
using OF.Core.Interfaces;

namespace OF.Core.Data.ElasticSearch.Request
{
    public class OFWildcard<T> : OFBaseWildcard, IOFWildcard<T> where T : IWildcard, new()
    {
        public T wildcard { get; set; }

        public OFWildcard(object value)
        {
            wildcard = new T();
            wildcard.SetValue(value);
        }

    }
}