using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Newtonsoft.Json;
using OF.Core.Data.Settings;
using OF.Core.Enums;
using OF.Core.Extensions;
using OF.Core.Helpers;
using OF.Core.Interfaces;
using OF.Core.Logger;
using OF.Infrastructure.Implements.Inspections;

namespace OF.Inspection
{
    class Program
    {
        private static readonly IDictionary<OFSettingsType, IOFSettingInspection> InspectionInstances = new Dictionary<OFSettingsType, IOFSettingInspection>()
        {
            {OFSettingsType.CheckAndFixJvmServivePath, new OFESServiceSettingInspection()},
            {OFSettingsType.AutoComplete, new OFESAutoCompleteSettingsInspection() }
        };

        static void Main(string[] args)
        {
            Console.WriteLine("Starting...");

            try
            {
                var listSettings =
                    OFObjectJsonSaveReadHelper.Instance.ReadApplicationSettings<List<OFTypeInspectionPayloadSettings>>();
                if (listSettings.IsNull() || !listSettings.Any())
                {
                    Console.WriteLine("There is no arguments...");
                    return;
                }

                foreach (var ofSettingsType in listSettings)
                {
                    Console.WriteLine(string.Format("Type: {0}; Settings: {1}", ofSettingsType.Type, ofSettingsType.SettingsPayload));

                    if (InspectionInstances.ContainsKey(ofSettingsType.Type)
                        && InspectionInstances[ofSettingsType.Type].IsValidValueOfSetting.HasValue
                        && !InspectionInstances[ofSettingsType.Type].IsValidValueOfSetting.Value)
                    {
                        Console.WriteLine("Write settins 1....");
                        InspectionInstances[ofSettingsType.Type].FixSetting(ofSettingsType.SettingsPayload);
                    }
                    else if (InspectionInstances.ContainsKey(ofSettingsType.Type)
                             && !InspectionInstances[ofSettingsType.Type].IsValidValueOfSetting.HasValue)
                    {
                        Console.WriteLine("Write settins 2....");
                        InspectionInstances[ofSettingsType.Type].FixSetting(ofSettingsType.SettingsPayload);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
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
