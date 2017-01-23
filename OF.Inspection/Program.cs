using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
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
                var listSettings = new List<OFSettingsType>();

                foreach (var s in args)
                {
                    listSettings.Add((OFSettingsType)Enum.Parse(typeof(OFSettingsType), s));
                }

                if (!listSettings.Any())
                {
                    return;
                }

                foreach (var ofSettingsType in listSettings)
                {
                    if (InspectionInstances.ContainsKey(ofSettingsType) && !InspectionInstances[ofSettingsType].IsValidValueOfSetting)
                    {
                        InspectionInstances[ofSettingsType].FixSetting(null);
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
