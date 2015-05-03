using System.Collections.Generic;
using System.ComponentModel;
using OF.Core.Interfaces;

namespace OF.Core.Data.ElasticSearch.Response
{
    public class OFRawSearchResponse<T> : IRawSearchResult<T> where T : class, new ()
    {

        public OFRawSearchResponse(int took, int total, List<T> documents)
        {
            Total = total;
            Took = took;
            Documents = documents;
        }

        public int Total { get; private set; }

        public int Took { get; private set; }

        public List<T> Documents { get; private set; }
    }
}