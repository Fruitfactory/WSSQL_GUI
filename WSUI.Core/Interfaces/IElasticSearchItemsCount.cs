using OF.Core.Data.ElasticSearch;

namespace OF.Core.Interfaces
{
    public interface IElasticSearchItemsCount
    {
        long GetTypeCount<T>() where T : OFElasticSearchBaseEntity; 
    }
}