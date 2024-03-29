﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security;
using Newtonsoft.Json;
using OF.Core.Data.Settings;
using OF.Core.Enums;
using OF.Core.Helpers;
using OF.Core.Interfaces;
using OF.Core.Logger;
using OF.Infrastructure.Implements.Inspections;

namespace OF.Infrastructure.Helpers
{
    public class OFInspectionHelper
    {

        private readonly JsonSerializerSettings Settings = new JsonSerializerSettings() { Formatting = Formatting.None };

        #region [static]

        private static readonly Lazy<OFInspectionHelper> instance = new Lazy<OFInspectionHelper>(() => new OFInspectionHelper());

        public static OFInspectionHelper Instance
        {
            get { return instance.Value; }
        }

        #endregion

        #region [public]

        public bool IsESServiceSettingsValid()
        {
            var esSettingsInspection = new OFESServiceSettingInspection();
            return esSettingsInspection.IsValidValueOfSetting.HasValue && esSettingsInspection.IsValidValueOfSetting.Value;
        }

        public void RunFixSettings(IEnumerable<OFTypeInspectionPayloadSettings> types)
        {
            try
            {
                var path = OFRegistryHelper.Instance.GetOfPath();
                path = Path.Combine(path, "ofinspection.exe");
                if (!File.Exists(path))
                {
                    throw new FileNotFoundException(path);
                }

                OFObjectJsonSaveReadHelper.Instance.SaveApplicationSettings(types);

                ProcessStartInfo si = new ProcessStartInfo();
                si.FileName = path;
                si.WindowStyle = ProcessWindowStyle.Hidden;
                si.Verb = "runas";
                Process process = new Process { StartInfo = si };
                process.Start();
            }
            catch (Exception e)
            {
                OFLogger.Instance.LogError(e.ToString());
            }
        }

        #endregion

        #region [private]
        #endregion
    }
}