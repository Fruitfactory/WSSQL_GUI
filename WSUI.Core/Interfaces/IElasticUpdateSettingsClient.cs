using OF.Core.Data.ElasticSearch;

namespace OF.Core.Interfaces
{
    public interface IElasticUpdateSettingsClient 
    {
        void UpdateSettings(OFRiverMeta settings);
    }
}