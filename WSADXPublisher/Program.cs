﻿using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace WSADXPublisher
{
    class Program
    {

        enum TypeInstaller : byte
        {
            Full,
            Trial
        }


        private const string SetArg = "/set";
        private const string ClearArg = "/clear";
        private const string TrialArg = "/trial";

        private const string TempFileNamePattern = "{0}.tmp";
        private const string ClickTwiceSettings = "clickTwice.Settings";

        #region settings name

        private const string InstallerFile = "installerFile";
        private const string PublishingLocation = "publishingLocation";
        private const string InstallationUrl = "installationUrl";
        private const string CertificateFile = "certificateFile";
        private const string IconFileName = "iconFileName";
        private const string QuietModeDuringInstall = "quietModeDuringInstall";
        private const string QuietModeDuringUninstall = "quietModeDuringUninstall";
        private const string ShowDownloaderWindow = "showDownloaderWindow";
        private const string DownloaderFileName = "downloaderFileName";
        private const string TargetApplicationNames = "targetApplicationNames";
        private const string TargetFrameworkVersion = "targetFrameworkVersion";
        private const string ShowRunningApplicationsWarning = "showRunningApplicationsWarning";
        private const string CertificatePassword = "certificatePassword";
        private const string TimestampUrl = "timestampUrl";

        #endregion

        private const string KeyAttribute = "key";
        private const string ValueAttribute = "value";



        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("You should pass at least one argument. (/set or /clear)");
                return;
            }
            switch(args[0].ToLowerInvariant())
            {
                case SetArg:
                    TypeInstaller type = GetTypeInstaller(args);
                    SetConfiguration(type);
                    break;
                case ClearArg:
                    ClearConfiguration();
                    break;
                default:
                    Console.WriteLine("Wrong argument.");
                    break;
            }

        }

        private static TypeInstaller GetTypeInstaller(string[] args)
        {
            if (args.Length == 1)
            {
                return TypeInstaller.Full;
            }
            return args[1].ToLowerInvariant() == TrialArg ? TypeInstaller.Trial : TypeInstaller.Full;
        }


        static void SetConfiguration(TypeInstaller type)
        {
            if(!CheckSettings())
                return;
            XDocument doc = LoadSettingsFileAndStoreTempCopySettingsFile();
            XElement clickTwiceSet = null;
            try
            {
                clickTwiceSet = doc.Descendants(ClickTwiceSettings).First();
            }
            catch (Exception)
            {
                Console.WriteLine("Couldn't find ClickTwice settings.");
                return;
            }

            SetSettings(clickTwiceSet,type);
            doc.Save(Path.Combine(Properties.Settings.Default.AddInExpressBinFolderPath,
                                            Properties.Settings.Default.ADXPublisherConfigFilename));
        }

        static void SetSettings(XElement el,TypeInstaller type)
        {
            if (el.Descendants().Any(e => e.Attribute(KeyAttribute).Value == InstallerFile))
            {
                XElement val = el.Descendants().Where(e => e.Attribute(KeyAttribute).Value == InstallerFile).First();
                if (val != null)
                {
                    val.Attribute(ValueAttribute).Value = Properties.Settings.Default.installerFile;
                }
            }
            if (el.Descendants().Any(e => e.Attribute(KeyAttribute).Value == PublishingLocation))
            {
                XElement val = el.Descendants().Where(e => e.Attribute(KeyAttribute).Value == PublishingLocation).First();
                if (val != null)
                {
                    val.Attribute(ValueAttribute).Value = type == TypeInstaller.Full ? Properties.Settings.Default.publishingLocation : Properties.Settings.Default.publishingLocationTrial;
                }
            }
            if (el.Descendants().Any(e => e.Attribute(KeyAttribute).Value == InstallationUrl))
            {
                XElement val = el.Descendants().Where(e => e.Attribute(KeyAttribute).Value == InstallationUrl).First();
                if (val != null)
                {
                    val.Attribute(ValueAttribute).Value = type == TypeInstaller.Full ? Properties.Settings.Default.installationUrl : Properties.Settings.Default.installationUrlTrial;
                }
            }
            if (el.Descendants().Any(e => e.Attribute(KeyAttribute).Value == CertificateFile))
            {
                XElement val = el.Descendants().Where(e => e.Attribute(KeyAttribute).Value == CertificateFile).First();
                if (val != null)
                {
                    val.Attribute(ValueAttribute).Value = Properties.Settings.Default.certificateFile;
                }
            }
            if (el.Descendants().Any(e => e.Attribute(KeyAttribute).Value == CertificatePassword))
            {
                XElement val = el.Descendants().Where(e => e.Attribute(KeyAttribute).Value == CertificatePassword).First();
                if (val != null)
                {
                    val.Attribute(ValueAttribute).Value = Properties.Settings.Default.certificatePassword;
                }
            }
            if (el.Descendants().Any(e => e.Attribute(KeyAttribute).Value == TimestampUrl))
            {
                XElement val = el.Descendants().Where(e => e.Attribute(KeyAttribute).Value == TimestampUrl).First();
                if (val != null)
                {
                    val.Attribute(ValueAttribute).Value = Properties.Settings.Default.timestampUrl;
                }
            }
            if (el.Descendants().Any(e => e.Attribute(KeyAttribute).Value == IconFileName))
            {
                XElement val = el.Descendants().Where(e => e.Attribute(KeyAttribute).Value == IconFileName).First();
                if (val != null)
                {
                    val.Attribute(ValueAttribute).Value = Properties.Settings.Default.iconFileName;
                }
            }
            if (el.Descendants().Any(e => e.Attribute(KeyAttribute).Value == QuietModeDuringInstall))
            {
                XElement val = el.Descendants().Where(e => e.Attribute(KeyAttribute).Value == QuietModeDuringInstall).First();
                if (val != null)
                {
                    val.Attribute(ValueAttribute).Value = Properties.Settings.Default.quietModeDuringInstall.ToString();
                }
            }
            if (el.Descendants().Any(e => e.Attribute(KeyAttribute).Value == QuietModeDuringUninstall))
            {
                XElement val = el.Descendants().Where(e => e.Attribute(KeyAttribute).Value == QuietModeDuringUninstall).First();
                if (val != null)
                {
                    val.Attribute(ValueAttribute).Value = Properties.Settings.Default.quietModeDuringUninstall.ToString();
                }
            }
            if (el.Descendants().Any(e => e.Attribute(KeyAttribute).Value == ShowDownloaderWindow))
            {
                XElement val = el.Descendants().Where(e => e.Attribute(KeyAttribute).Value == ShowDownloaderWindow).First();
                if (val != null)
                {
                    val.Attribute(ValueAttribute).Value = Properties.Settings.Default.showDownloaderWindow.ToString();
                }
            }
            if (el.Descendants().Any(e => e.Attribute(KeyAttribute).Value == DownloaderFileName))
            {
                XElement val = el.Descendants().Where(e => e.Attribute(KeyAttribute).Value == DownloaderFileName).First();
                if (val != null)
                {
                    val.Attribute(ValueAttribute).Value = Properties.Settings.Default.downloaderFileName.ToString();
                }
            }
            if (el.Descendants().Any(e => e.Attribute(KeyAttribute).Value == TargetApplicationNames))
            {
                XElement val = el.Descendants().Where(e => e.Attribute(KeyAttribute).Value == TargetApplicationNames).First();
                if (val != null)
                {
                    val.Attribute(ValueAttribute).Value = Properties.Settings.Default.targetApplicationNames.ToString();
                }
            }
            if (el.Descendants().Any(e => e.Attribute(KeyAttribute).Value == TargetFrameworkVersion))
            {
                XElement val = el.Descendants().Where(e => e.Attribute(KeyAttribute).Value == TargetFrameworkVersion).First();
                if (val != null)
                {
                    val.Attribute(ValueAttribute).Value = Properties.Settings.Default.targetFrameworkVersion.ToString();
                }
            }
            if (el.Descendants().Any(e => e.Attribute(KeyAttribute).Value == ShowRunningApplicationsWarning))
            {
                XElement val = el.Descendants().Where(e => e.Attribute(KeyAttribute).Value == ShowRunningApplicationsWarning).First();
                if (val != null)
                {
                    val.Attribute(ValueAttribute).Value = Properties.Settings.Default.showRunningApplicationsWarning.ToString();
                }
            }
        }

        static XDocument LoadSettingsFileAndStoreTempCopySettingsFile()
        {
            XDocument doc =
                XDocument.Load(Path.Combine(Properties.Settings.Default.AddInExpressBinFolderPath,
                                            Properties.Settings.Default.ADXPublisherConfigFilename));
            doc.Save(Path.Combine(Path.GetTempPath(),string.Format(TempFileNamePattern,Properties.Settings.Default.ADXPublisherConfigFilename)));
            return doc;
        }

        static bool CheckSettings()
        {
            bool result = true;
            if (string.IsNullOrEmpty(Properties.Settings.Default.AddInExpressBinFolderPath))
            {
                Console.WriteLine("Add-In Express folder is empty.");
                result = false;
            }
            if (!Directory.Exists(Properties.Settings.Default.AddInExpressBinFolderPath))
            {
                Console.WriteLine("Add-In Express folder isn't exist.");
                result = false;
            }
            if (!File.Exists(Properties.Settings.Default.installerFile))
            {
                Console.WriteLine("Installer file isn't exist");
                result = false;
            }
            if (!File.Exists(Properties.Settings.Default.certificateFile))
            {
                Console.WriteLine("Certificate file isn't exist");
                result = false;
            }
            if (!File.Exists(Properties.Settings.Default.iconFileName))
            {
                Console.WriteLine("Icon file isn't exist");
                result = false;
            }
            if (!Directory.Exists(Properties.Settings.Default.publishingLocation))
            {
                try
                {
                    Directory.CreateDirectory(Properties.Settings.Default.publishingLocation);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error while creating directory: {0}\nException: {1}", Properties.Settings.Default.publishingLocation,ex.Message);
                    result = false;
                }
            }
            if (string.IsNullOrEmpty(Properties.Settings.Default.downloaderFileName))
            {
                Console.WriteLine("downloaderFileName property is empty");
                result = false;
            }
            //if (string.IsNullOrEmpty(Properties.Settings.Default.targetApplicationNames))
            //{
            //    Console.WriteLine("targetApplicationNames property is empty");
            //    result = false;
            //}
            if (string.IsNullOrEmpty(Properties.Settings.Default.targetFrameworkVersion))
            {
                Console.WriteLine("targetFrameworkVersion property is empty");
                result = false;
            }

            return result;
        }


        static void ClearConfiguration()
        {
            string tempFile = Path.Combine(Path.GetTempPath(),
                                           string.Format(TempFileNamePattern,
                                                         Properties.Settings.Default.ADXPublisherConfigFilename));
            if (File.Exists(tempFile))
            {
                XDocument doc = XDocument.Load(tempFile);
                doc.Save(Path.Combine(Properties.Settings.Default.AddInExpressBinFolderPath,
                                                Properties.Settings.Default.ADXPublisherConfigFilename));    
            }
            else
            {
                XDocument doc =
                XDocument.Load(Path.Combine(Properties.Settings.Default.AddInExpressBinFolderPath,
                                            Properties.Settings.Default.ADXPublisherConfigFilename));

                XElement clickTwiceSet = null;
                try
                {
                    clickTwiceSet = doc.Descendants(ClickTwiceSettings).First();
                }
                catch (Exception)
                {
                    Console.WriteLine("Couldn't find ClickTwice settings.");
                    return;
                }

                ClearSettings(clickTwiceSet);
                doc.Save(Path.Combine(Properties.Settings.Default.AddInExpressBinFolderPath,
                                                Properties.Settings.Default.ADXPublisherConfigFilename));
            }
        }

        static void ClearSettings(XElement el)
        {
            if (el.Descendants().Any(e => e.Attribute(KeyAttribute).Value == InstallerFile))
            {
                XElement val = el.Descendants().Where(e => e.Attribute(KeyAttribute).Value == InstallerFile).First();
                if (val != null)
                {
                    val.Attribute(ValueAttribute).Value = string.Empty;
                }
            }
            if (el.Descendants().Any(e => e.Attribute(KeyAttribute).Value == PublishingLocation))
            {
                XElement val = el.Descendants().Where(e => e.Attribute(KeyAttribute).Value == PublishingLocation).First();
                if (val != null)
                {
                    val.Attribute(ValueAttribute).Value = string.Empty;
                }
            }
            if (el.Descendants().Any(e => e.Attribute(KeyAttribute).Value == InstallationUrl))
            {
                XElement val = el.Descendants().Where(e => e.Attribute(KeyAttribute).Value == InstallationUrl).First();
                if (val != null)
                {
                    val.Attribute(ValueAttribute).Value = string.Empty;
                }
            }
            if (el.Descendants().Any(e => e.Attribute(KeyAttribute).Value == CertificateFile))
            {
                XElement val = el.Descendants().Where(e => e.Attribute(KeyAttribute).Value == CertificateFile).First();
                if (val != null)
                {
                    val.Attribute(ValueAttribute).Value = string.Empty;
                }
            }
            if (el.Descendants().Any(e => e.Attribute(KeyAttribute).Value == CertificatePassword))
            {
                XElement val = el.Descendants().Where(e => e.Attribute(KeyAttribute).Value == CertificatePassword).First();
                if (val != null)
                {
                    val.Attribute(ValueAttribute).Value = string.Empty;
                }
            }
            if (el.Descendants().Any(e => e.Attribute(KeyAttribute).Value == TimestampUrl))
            {
                XElement val = el.Descendants().Where(e => e.Attribute(KeyAttribute).Value == TimestampUrl).First();
                if (val != null)
                {
                    val.Attribute(ValueAttribute).Value = string.Empty;
                }
            }
            if (el.Descendants().Any(e => e.Attribute(KeyAttribute).Value == IconFileName))
            {
                XElement val = el.Descendants().Where(e => e.Attribute(KeyAttribute).Value == IconFileName).First();
                if (val != null)
                {
                    val.Attribute(ValueAttribute).Value = string.Empty;
                }
            }
            if (el.Descendants().Any(e => e.Attribute(KeyAttribute).Value == QuietModeDuringInstall))
            {
                XElement val = el.Descendants().Where(e => e.Attribute(KeyAttribute).Value == QuietModeDuringInstall).First();
                if (val != null)
                {
                    val.Attribute(ValueAttribute).Value = bool.FalseString;
                }
            }
            if (el.Descendants().Any(e => e.Attribute(KeyAttribute).Value == QuietModeDuringUninstall))
            {
                XElement val = el.Descendants().Where(e => e.Attribute(KeyAttribute).Value == QuietModeDuringUninstall).First();
                if (val != null)
                {
                    val.Attribute(ValueAttribute).Value = bool.TrueString;
                }
            }
            if (el.Descendants().Any(e => e.Attribute(KeyAttribute).Value == ShowDownloaderWindow))
            {
                XElement val = el.Descendants().Where(e => e.Attribute(KeyAttribute).Value == ShowDownloaderWindow).First();
                if (val != null)
                {
                    val.Attribute(ValueAttribute).Value = bool.TrueString;
                }
            }
            if (el.Descendants().Any(e => e.Attribute(KeyAttribute).Value == DownloaderFileName))
            {
                XElement val = el.Descendants().Where(e => e.Attribute(KeyAttribute).Value == DownloaderFileName).First();
                if (val != null)
                {
                    val.Attribute(ValueAttribute).Value = bool.TrueString;
                }
            }
            if (el.Descendants().Any(e => e.Attribute(KeyAttribute).Value == TargetApplicationNames))
            {
                XElement val = el.Descendants().Where(e => e.Attribute(KeyAttribute).Value == TargetApplicationNames).First();
                if (val != null)
                {
                    val.Attribute(ValueAttribute).Value = bool.TrueString;
                }
            }
            if (el.Descendants().Any(e => e.Attribute(KeyAttribute).Value == TargetFrameworkVersion))
            {
                XElement val = el.Descendants().Where(e => e.Attribute(KeyAttribute).Value == TargetFrameworkVersion).First();
                if (val != null)
                {
                    val.Attribute(ValueAttribute).Value = bool.TrueString;
                }
            }
            if (el.Descendants().Any(e => e.Attribute(KeyAttribute).Value == ShowRunningApplicationsWarning))
            {
                XElement val = el.Descendants().Where(e => e.Attribute(KeyAttribute).Value == ShowRunningApplicationsWarning).First();
                if (val != null)
                {
                    val.Attribute(ValueAttribute).Value = bool.FalseString;
                }
            }
        }

    }
}
