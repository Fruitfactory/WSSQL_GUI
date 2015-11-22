namespace OF.Core.Interfaces
{
    public interface IElasticSearchItemsCount
    {
        long GetTypeCount<T>() where T : class; 
    }
}