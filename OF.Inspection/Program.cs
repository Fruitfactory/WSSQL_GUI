using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Newtonsoft.Json;
using OF.Core.Data.Settings;
using OF.Core.Enums;
using OF.Core.Extensions;
using OF.Core.Helpers;
using OF.Core.Interfaces;
using OF.Infrastructure.Implements.Inspections;

namespace OF.Inspection
{
    class Program
    {
        private static readonly IDictionary<OFSettingsType,IOFSettingInspection> InspectionInstances = new Dictionary<OFSettingsType, IOFSettingInspection>()
        {
            {OFSettingsType.CheckAndFixJvmServivePath, new OFESServiceSettingInspection()},
            {OFSettingsType.AutoComplete, new OFESAutoCompleteSettingsInspection() }
        };
         
        static void Main(string[] args)
        {
            try
            {
                var listSettings =
                    OFObjectJsonSaveReadHelper.Instance.ReadApplicationSettings<List<OFTypeInspectionPayloadSettings>>();
                if (listSettings.IsNull() || !listSettings.Any())
                {
                    return;
                }

                foreach (var ofSettingsType in listSettings)
                {
                    if (InspectionInstances.ContainsKey(ofSettingsType.Type)
                        && InspectionInstances[ofSettingsType.Type].IsValidValueOfSetting.HasValue
                        && !InspectionInstances[ofSettingsType.Type].IsValidValueOfSetting.Value)
                    {
                        InspectionInstances[ofSettingsType.Type].FixSetting(ofSettingsType.SettingsPayload);
                    }
                    else if (InspectionInstances.ContainsKey(ofSettingsType.Type)
                             && !InspectionInstances[ofSettingsType.Type].IsValidValueOfSetting.HasValue)
                    {
                        InspectionInstances[ofSettingsType.Type].FixSetting(ofSettingsType.SettingsPayload);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                OFIsolatedStorageHelper.Instance.RemoveApplicationSettings();
            }
        }

        private static string[] ParseArguments(string[] inputArgs)
        {
            var args = Encoding.UTF8.GetString(Convert.FromBase64String(inputArgs[0]));
            return args.Split(';');
        }


    }
}
