using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Management.Instrumentation;
using System.Net;
using System.Reflection.Emit;
using System.Text;
using System.Xml.Linq;



namespace WSUploader
{
    class Program
    {
        const string FileConfigName = "wsgen.exe.config";
        private const string FolderPattern = "{0}\\1033\\{1}";
        private const string FolderPatternFtp = "{0}/1033/{1}";
        private const string versionInfoFile = "version_info.xml";

        static void Main(string[] args)
        {
            string buildNumber = string.Empty;
            if (string.IsNullOrEmpty((buildNumber = GetBuildNumber())))
                return;
            try
            {
                UploadFullVersion(buildNumber);
                UploadTrialVersion(buildNumber);
                Console.WriteLine("Done.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.WriteLine("Please, press ENTER to exit");
            Console.ReadLine();
        }

        private static string GetBuildNumber()
        {
            XDocument configMap = XDocument.Load(FileConfigName);
            var appSettings = configMap.Root.Elements().Where(e => e.Name == "applicationSettings");
            if (appSettings == null || !appSettings.Any())
                return string.Empty;
            var wsgen = appSettings.Elements().Where(e => e.Name == "WSGen.Properties.Settings");
            if (!wsgen.Any())
                return string.Empty;
            var buildNumber = wsgen.Elements().Where(e => e.Name == "setting" && e.Attribute("name").Value == "BuildNumber");
            if (!buildNumber.Any())
                return string.Empty;
            var build = buildNumber.ElementAt(0).Value;
            Console.WriteLine(build);
            return build;
        }


        static ftp GetFtp()
        {
            return new ftp(Properties.Settings.Default.FtpServer, Properties.Settings.Default.Username, Properties.Settings.Default.Password);
        }

        static bool FolderFullExists(string folder)
        {
            return FolderExists(Properties.Settings.Default.RemoteFullPath, folder);
        }

        static bool FolderTrialExists(string folder)
        {
            return FolderExists(Properties.Settings.Default.RemoteTrialPath, folder);
        }

        private static bool FolderExists(string baseFolder, string folder)
        {
            string fullPath = baseFolder + "/1033/";
            var ftp = GetFtp();
            var list = ftp.directoryListSimple(fullPath);
            string lowerFolder = folder.ToLowerInvariant();
            return list != null && list.Length > 0 && list.Any(s => s.ToLowerInvariant() == lowerFolder);
        }

        static string CreateFullFolder(string folder)
        {
            return CreateFolder(Properties.Settings.Default.RemoteFullPath, folder);
        }

        static string CreateTrialFolder(string folder)
        {
            return CreateFolder(Properties.Settings.Default.RemoteTrialPath, folder);
        }

        static string CreateFolder(string baseFolder, string folder)
        {
            string fullPath = baseFolder + "/1033/" + folder;
            var ftp = GetFtp();
            ftp.createDirectory(fullPath);
            return fullPath;
        }

        static void UploadFullVersion(string buildVersion)
        {
            string locaPathFolder = string.Format(FolderPattern, Properties.Settings.Default.LocalFullPath, buildVersion);
            string remoteFolder = string.Empty;
            try
            {
                if (!FolderFullExists(buildVersion))
                {
                    remoteFolder = CreateFullFolder(buildVersion);
                }
                if (string.IsNullOrEmpty(remoteFolder))
                {
                    Console.WriteLine("Remote '{0}' folder haven't been created",buildVersion);
                    return;
                }
                    
                var localFiles = (new DirectoryInfo(locaPathFolder)).EnumerateFiles();
                if (!localFiles.Any())
                {
                    Console.WriteLine("Couldn't find local files");
                    return;
                }
                foreach (var localFile in localFiles)
                {
                    Upload(remoteFolder + "/" + localFile.Name,localFile.FullName);
                }
                UploadFullVersionInfo();
                    
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static void UploadTrialVersion(string buildVersion)
        {
            string locaPathFolder = string.Format(FolderPattern, Properties.Settings.Default.LocalTrialPath, buildVersion);
            string remoteFolder = string.Empty;
            try
            {
                if (!FolderTrialExists(buildVersion))
                {
                    remoteFolder = CreateTrialFolder(buildVersion);
                }
                if (string.IsNullOrEmpty(remoteFolder))
                {
                    Console.WriteLine("Remote '{0}' folder haven't been created", buildVersion);
                    return;
                }

                var localFiles = (new DirectoryInfo(locaPathFolder)).EnumerateFiles();
                if (!localFiles.Any())
                {
                    Console.WriteLine("Couldn't find local files");
                    return;
                }
                foreach (var localFile in localFiles)
                {
                    Upload(remoteFolder + "/" + localFile.Name, localFile.FullName);
                }
                UploadTrialVersionInfo();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static void UploadFullVersionInfo()
        {
            var fi = new FileInfo(Properties.Settings.Default.LocalFullPath + "\\" + versionInfoFile);
            if (fi.Exists)
            {
                Upload(Properties.Settings.Default.RemoteFullPath + "/" + fi.Name, fi.FullName);
            }
        }

        static void UploadTrialVersionInfo()
        {
            var fi = new FileInfo(Properties.Settings.Default.LocalTrialPath + "\\" + versionInfoFile);
            if (fi.Exists)
            {
                Upload(Properties.Settings.Default.RemoteTrialPath + "/" + fi.Name, fi.FullName);
            }
        }

        static void Upload(string remoteFile, string localFile)
        {
            var ftp = GetFtp();
            ftp.upload(remoteFile,localFile);
        }


    }
}
