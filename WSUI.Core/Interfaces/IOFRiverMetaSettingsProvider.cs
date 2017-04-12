using System;
using OF.Core.Data.ElasticSearch;

namespace OF.Core.Interfaces
{
    public interface IOFRiverMetaSettingsProvider
    {
        OFRiverMeta GetCurrentSettings();

        DateTime? GetLastIndexingDateTime();

        void UpdateLastIndexingDateTime(DateTime lastDateTime);

        void UpdateRiverMetaSettings(OFRiverMeta settings);

        void UpdateServiceAplicationSettings(OFRiverMeta settings);
    }
}