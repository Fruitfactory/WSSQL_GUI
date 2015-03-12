using WSUI.Core.Interfaces;

namespace WSUI.Core.Data.ElasticSearch
{
    public class WSUIElasticSearchBaseEntity : IElasticSearchObject
    {
        public string EntryID
        {
            get; set;
        }
    }
}