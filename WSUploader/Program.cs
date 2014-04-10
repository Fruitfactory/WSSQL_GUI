using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Management.Instrumentation;
using System.Net;
using System.Text;
using System.Xml.Linq;



namespace WSUploader
{
    class Program
    {
        const string FileConfigName = "wsgen.exe.config";
        private const string FolderPattern = "{0}\\1033\\{1}";
        private const string FolderPatternFtp = "{0}/1033/{1}";

        static void Main(string[] args)
        {
            string buildNumber = string.Empty;
            if (string.IsNullOrEmpty((buildNumber = GetBuildNumber())))
                return;
            try
            {
                string remotePath = string.Format(FolderPatternFtp, Properties.Settings.Default.RemoteFullPath, "OutlookFinderSetup.msi");//, buildNumber
                string rem = "ftp://213.108.40.45:8888/shares/NetShare/11111111";
                var request = (FtpWebRequest)WebRequest.Create(rem);
                request.Method = WebRequestMethods.Ftp.UploadFile;
                request.UseBinary = true;
                request.UsePassive = false;
                request.KeepAlive = true;
                request.Credentials = new NetworkCredential(Properties.Settings.Default.Username, Properties.Settings.Default.Password);
                string locaPath = string.Format(FolderPattern, Properties.Settings.Default.LocalFullPath, buildNumber);
                locaPath = Path.Combine(locaPath, "OutlookFinderSetup.msi");

                //using (FtpConnection ftp = new FtpConnection(Properties.Settings.Default.FtpServer, Properties.Settings.Default.Username, Properties.Settings.Default.Password))
                //{
                //    ftp.Open();
                //    ftp.Login();
                //    if (ftp.DirectoryExists("/shares"))
                //    {
                //        Console.WriteLine("shares is exist");
                //    }
                //    else
                //        Console.WriteLine("shares isn't exist");
                //}

                ftp f = new ftp("ftp://readyshare.routerlogin.net/", Properties.Settings.Default.Username, Properties.Settings.Default.Password);

                f.upload("shares/NetShare",locaPath);

                //using (var stream = new FileStream(locaPath, FileMode.Open, FileAccess.Read))
                //using (var reader = new BinaryReader(stream))
                //using (Stream requestStream = request.GetRequestStream())
                {
                    //const int BufferSize = 1024 * 400;
                    //byte[] buffer = new byte[BufferSize];
                    //int bytes;
                    //while ((bytes = reader.Read(buffer, 0, BufferSize)) > 0)
                    //{
                    //    requestStream.Write(buffer, 0, bytes);
                    //}
                }

                
                Console.WriteLine("Done.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

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
    }
}
