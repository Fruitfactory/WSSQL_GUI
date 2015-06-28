using System;

namespace OF.Core.Interfaces
{
    public interface IElasticSearchClient
    {
        OF.Core.Interfaces.IRawSearchResult<T> RawSearch<T>(object body) where T : class, new();
        Nest.ISearchResponse<T> Search<T>(Func<Nest.SearchDescriptor<T>, Nest.SearchDescriptor<T>> searchSelector) where T : class;
    }
}
