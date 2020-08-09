using System;
using Nest;
using OF.Core.Data.ElasticSearch;

namespace OF.Core.Interfaces
{
    public interface IElasticSearchClient<T> where T : class, IElasticSearchObject, new()
    {
        OF.Core.Interfaces.IRawSearchResult<T> RawSearch(object body);
        Nest.ISearchResponse<T> Search(Func<Nest.SearchDescriptor<T>, Nest.SearchDescriptor<T>> searchSelector);

        ElasticClient ElasticClient { get; }
    }
}
