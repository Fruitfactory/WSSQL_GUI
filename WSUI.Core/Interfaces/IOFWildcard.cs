using OF.Core.Data.ElasticSearch.Request.Base;

namespace OF.Core.Interfaces
{
    public interface IOFWildcard<out T> where T: IWildcard, new()
    {
        
    }
}