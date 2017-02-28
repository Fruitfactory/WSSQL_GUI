using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Newtonsoft.Json;
using OF.Core.Data.Settings;
using OF.Core.Enums;
using OF.Core.Extensions;
using OF.Core.Interfaces;
using OF.Infrastructure.Implements.Inspections;

namespace OF.Inspection
{
    class Program
    {
        private static readonly IDictionary<OFSettingsType,IOFSettingInspection> InspectionInstances = new Dictionary<OFSettingsType, IOFSettingInspection>()
        {
            {OFSettingsType.CheckAndFixJvmServivePath, new OFESServiceSettingInspection()}
        };

        static void Main(string[] args)
        {
            if (args.IsEmpty())
            {
                return;
            }
            try
            {
                var listSettings = new List<OFTypeInspectionPayloadSettings>();

                foreach (var s in args)
                {
                    listSettings.Add( JsonConvert.DeserializeObject<OFTypeInspectionPayloadSettings>(s));
                }

                if (!listSettings.Any())
                {
                    return;
                }

                foreach (var ofSettingsType in listSettings)
                {
                    if (InspectionInstances.ContainsKey(ofSettingsType.Type) && !InspectionInstances[ofSettingsType.Type].IsValidValueOfSetting)
                    {
                        InspectionInstances[ofSettingsType.Type].FixSetting(ofSettingsType.SettingsPayload);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

    }
}
