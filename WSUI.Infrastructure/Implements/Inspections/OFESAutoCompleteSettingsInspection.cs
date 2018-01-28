using System;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using OF.Core.Core.Inspections;
using OF.Core.Data.Settings.SettingsPayload;
using OF.Core.Enums;
using OF.Core.Extensions;
using OF.Core.Helpers;
using OF.Core.Logger;

namespace OF.Infrastructure.Implements.Inspections
{
    public class OFESAutoCompleteSettingsInspection : OFSettingInspection
    {

        public override void FixSetting(object value)
        {
            var strValue = value as string;
            if (string.IsNullOrEmpty(strValue))
            {
                return;
            }
            try
            {
                var payload = JsonConvert.DeserializeObject(strValue, typeof(OFAutoCompleteSettingsPayload)) as OFAutoCompleteSettingsPayload;
                if (payload.IsNull())
                {
                    return;
                }
                var officeVersion = OFRegistryHelper.Instance.GetOutlookVersion().Item1;
                if (payload.IsAutoCompleateDisabled)
                {
                    OFRegistryHelper.Instance.DisableOutlookAutoCompleateEmailsToCcBcc(officeVersion);
                }
                else
                {
                    OFRegistryHelper.Instance.EnableOutlookAutoCompleateEmailsToCcBcc(officeVersion);
                }
            }
            catch (Exception e)
            {
                OFLogger.Instance.LogError(e.ToString());
            }
        }

        public override OFSettingsType SettingsType { get {return OFSettingsType.AutoComplete;} }
    }
}