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
            bool created;
            OFRiverMeta settingsMeta = null;
            using (Mutex mutex = CreateMutex(out created))
            {
                var hasHandled = false;

                try
                {

                    try
                    {
                        hasHandled = mutex.WaitOne(Timeout, false);
                        if (!hasHandled)
                        {
                            throw new TimeoutException("Timeout waiting for exlussive access while getting current settings.");
                        }
                    }
                    catch (AbandonedMutexException e)
                    {
                        OFLogger.Instance.LogError(e.ToString());
                        hasHandled = true;
                    }
                    settingsMeta = OFObjectJsonSaveReadHelper.Instance.Read<OFRiverMeta>(GlobalConst.SettingsRiverFile);

                }
                finally 
                {
                    if(hasHandled)
                        mutex.ReleaseMutex();                   
                }
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
                bool created;
                using (Mutex mutex = CreateMutex(out created))
                {
                    var hasHandled = false;

                    try
                    {

                        try
                        {
                            hasHandled = mutex.WaitOne(Timeout, false);
                            if (!hasHandled)
                            {
                                throw new TimeoutException("Timeout waiting for exlussive access while updating settings.");
                            }
                        }
                        catch (AbandonedMutexException e)
                        {
                            OFLogger.Instance.LogError(e.ToString());
                            hasHandled = true;
                        }
                        OFObjectJsonSaveReadHelper.Instance.Save(settings, GlobalConst.SettingsRiverFile);

                    }
                    finally
                    {
                        if (hasHandled)
                            mutex.ReleaseMutex();
                    }
                }

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
                using (Mutex mutex = CreateMutex(out created))
                {
                    var hasHandled = false;

                    try
                    {

                        try
                        {
                            hasHandled = mutex.WaitOne(Timeout, false);
                            if (!hasHandled)
                            {
                                throw new TimeoutException("Timeout waiting for exlussive access while updating last reading date.");
                            }
                        }
                        catch (AbandonedMutexException e)
                        {
                            OFLogger.Instance.LogError(e.ToString());
                            hasHandled = true;
                        }
                        OFObjectJsonSaveReadHelper.Instance.Save(riverMeta, GlobalConst.SettingsRiverFile);

                    }
                    finally
                    {
                        if (hasHandled)
                            mutex.ReleaseMutex();
                    }
                }



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

                bool created;
                using (Mutex mutex = CreateMutex(out created))
                {
                    var hasHandled = false;

                    try
                    {

                        try
                        {
                            hasHandled = mutex.WaitOne(Timeout, false);
                            if (!hasHandled)
                            {
                                throw new TimeoutException("Timeout waiting for exlussive access while updating last reading date.");
                            }
                        }
                        catch (AbandonedMutexException e)
                        {
                            OFLogger.Instance.LogError(e.ToString());
                            hasHandled = true;
                        }
                        OFObjectJsonSaveReadHelper.Instance.Save(riverMeta, GlobalConst.SettingsRiverFile);

                    }
                    finally
                    {
                        if (hasHandled)
                            mutex.ReleaseMutex();
                    }
                }



            }
            catch (Exception e)
            {
                OFLogger.Instance.LogError(e.ToString());
            }
        }
        


        private Mutex CreateMutex(out bool createdNew)
        {
            var allowedEveryoneRule = new MutexAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), MutexRights.FullControl, AccessControlType.Allow);
            var securitySettings = new MutexSecurity();
            securitySettings.AddAccessRule(allowedEveryoneRule);
            return new Mutex(false, mutexId, out createdNew, securitySettings);
        }

    }
}