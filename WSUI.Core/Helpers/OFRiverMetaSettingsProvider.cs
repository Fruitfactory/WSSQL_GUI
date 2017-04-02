using System;
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
        
        public OFRiverMeta GetCurrentSettings()
        {
            try
            {
                bool created;
                Mutex mutex = new Mutex(true, GlobalConst.RiverMetaMutex, out created);
                var settingsMeta =
                        OFObjectJsonSaveReadHelper.Instance.Read<OFRiverMeta>(GlobalConst.SettingsRiverFile);
                mutex.ReleaseMutex();
                if (settingsMeta.IsNull())
                {
                    settingsMeta = new OFRiverMeta(OFElasticSearchClientBase.DefaultInfrastructureName);
                }
                return settingsMeta;
            }
            catch (Exception e)
            {
                OFLogger.Instance.LogError(e.ToString());
            }

            return null;
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
                bool created;
                Mutex mutex = new Mutex(true, GlobalConst.RiverMetaMutex, out created);
                OFObjectJsonSaveReadHelper.Instance.Save(settings, GlobalConst.SettingsRiverFile);
                mutex.ReleaseMutex();
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
                bool created;
                Mutex mutex = new Mutex(true, GlobalConst.RiverMetaMutex, out created);
                OFObjectJsonSaveReadHelper.Instance.Save(riverMeta, GlobalConst.SettingsRiverFile);
                mutex.ReleaseMutex();
            }
            catch (Exception e)
            {
                OFLogger.Instance.LogError(e.ToString());
            }
        }

    }
}