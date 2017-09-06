using System;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using OF.Core.Core.ElasticSearch;
using OF.Core.Data.ElasticSearch;
using OF.Core.Extensions;
using OF.Core.Interfaces;
using OF.Core.Logger;

namespace OF.Core.Helpers
{
    public class OFRiverMetaSettingsProvider : IOFRiverMetaSettingsProvider
    {
        private readonly string mutexId = "Global\\" + GlobalConst.RiverMetaMutex;
        private readonly int Timeout = 750;


        public OFRiverMeta GetCurrentSettings()
        {
            OFRiverMeta settingsMeta = null;
            try
            {
                settingsMeta = OFObjectJsonSaveReadHelper.Instance.ReadElasticSearchSettings<OFRiverMeta>();
            }
            catch (Exception e)
            {
                OFLogger.Instance.LogError(e.ToString());
            }
            if (settingsMeta.IsNull())
            {
                settingsMeta = new OFRiverMeta(OFElasticSearchClientBase.DefaultInfrastructureName);
            }
            return settingsMeta;
        }

        public DateTime? GetLastIndexingDateTime()
        {
            var settings = GetCurrentSettings();
            return settings.IsNotNull() ? settings.LastDate : null;
        }

        public void UpdateRiverMetaSettings(OFRiverMeta settings)
        {
            try
            {

                OFObjectJsonSaveReadHelper.Instance.SaveElasticSearchSettings(settings);
            }
            catch (Exception e)
            {
                OFLogger.Instance.LogError(e.ToString());
            }
        }

        public void UpdateLastIndexingDateTime(DateTime lastDateTime)
        {
            try
            {
                var riverMeta = GetCurrentSettings();
                riverMeta.LastDate = lastDateTime;
                OFObjectJsonSaveReadHelper.Instance.SaveElasticSearchSettings(riverMeta);
            }
            catch (Exception e)
            {
                OFLogger.Instance.LogError(e.ToString());
            }
        }

        public void UpdateServiceAplicationSettings(OFRiverMeta settings)
        {
            try
            {
                var riverMeta = GetCurrentSettings();
                riverMeta.Pst.Schedule = settings.Pst.Schedule;
                OFObjectJsonSaveReadHelper.Instance.SaveElasticSearchSettings(riverMeta);
            }
            catch (Exception e)
            {
                OFLogger.Instance.LogError(e.ToString());
            }
        }

    }
}