using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Documents;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Unity;
using OF.Core.Extensions;
using OF.Core.Helpers;
using OF.Core.Logger;
using OF.Module.Interface.ViewModel;

namespace OF.Module.ViewModel.LogFileManager
{
    public class OFLogFilesSenderManager : IOFLogFilesSenderManager
    {
        private IUnityContainer _container;

        public OFLogFilesSenderManager(IUnityContainer container)
        {
            _container = container;
        }


        #region [methods]

        public bool SendLogFiles()
        {
            try
            {
                OFLogger.Instance.LogDebug("Entering SendLogFiles...");

                var esBinPath = OFRegistryHelper.Instance.GetElasticSearchpath();
                var p = esBinPath.Substring(0, esBinPath.LastIndexOf('\\'));

                OFLogger.Instance.LogDebug("esBinPath: {0}", esBinPath);
                OFLogger.Instance.LogDebug("p: {0}", p);

                var elasticSearchLogsPath = Path.Combine(p, "logs");
                var outlookfinderlogPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "OutlookFinder", "Log");

                OFLogger.Instance.LogDebug("elasticSearchLogsPath: {0}", elasticSearchLogsPath);
                OFLogger.Instance.LogDebug("outlookfinderlogPath: {0}", outlookfinderlogPath);


#if DEBUG
                var toEmail = "iyariki.ya@gmail.com";
#else
                var toEmail = OF.Module.Properties.Settings.Default.LogFileEmail;
#endif
                var outlookfinderZipsPath =
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "OutlookFinder");

                OFLogger.Instance.LogDebug("outlookfinderZipsPath: {0}", outlookfinderZipsPath);

                CopyFiles(elasticSearchLogsPath, GetTempFolder(outlookfinderZipsPath));
                CopyFiles(outlookfinderlogPath, GetTempFolder(outlookfinderZipsPath));

                var outlookfinderTask = CreateZipFileAsync(GetTempFolder(outlookfinderZipsPath),
                    Path.Combine(outlookfinderZipsPath, OF.Module.Properties.Settings.Default.OutlookFinderZipFileName));

                outlookfinderTask.Wait();

                var listZipFiles = new List<string>() {outlookfinderTask.Result};

                InternalSendLogs(listZipFiles, toEmail);

                ClearTempFiles(outlookfinderZipsPath);

                return true;

            }
            catch (AggregateException agg)
            {
                OFLogger.Instance.LogError(agg.ToString());
                if (agg.InnerExceptions.Any())
                {
                    foreach (var aggInnerException in agg.InnerExceptions)
                    {
                        OFLogger.Instance.LogError(aggInnerException.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(ex.ToString());
            }

            return false;
        }

        private void CopyFiles(string folder, string getTempFolder)
        {
            try
            {
                var di = new DirectoryInfo(folder);
                var files = di.GetFiles();
                foreach (var fileInfo in files)
                {
                    try
                    {
                        var destName = Path.GetFileName(fileInfo.FullName);
                        fileInfo.CopyTo(Path.Combine(getTempFolder, destName));
                    }
                    catch (Exception ex)
                    {
                        OFLogger.Instance.LogError(ex.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(ex.ToString());
            }
        }

        #endregion


        #region [private methods]

        private string GetTempFolder(string ofZippingFolder)
        {
            var path = Path.Combine(ofZippingFolder, "Temp");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }

        private void InternalSendLogs(List<string> listZipFiles, string toEmail)
        {
            OFOutlookHelper.Instance.SendEmail("Logs", toEmail, listZipFiles);
        }

        private void ClearTempFiles(string outlookfinderZipsPath)
        {
            if (!Directory.Exists(outlookfinderZipsPath))
            {
                return;
            }

            var dirInfo = new DirectoryInfo(outlookfinderZipsPath);
            foreach (var enumerateFile in dirInfo.EnumerateFiles("*.zip"))
            {
                enumerateFile.Delete();
            }
            var tempFolder = GetTempFolder(outlookfinderZipsPath);
            if (Directory.Exists(tempFolder))
            {
                Directory.Delete(tempFolder,true);
            }
        }


        private Task<string> CreateZipFileAsync(string logFolder, string zipFilename)
        {
            var folder = logFolder;
            var filename = zipFilename;

            return Task<string>.Factory.StartNew(() => CreateZipFile(folder, filename));
        }

        private string CreateZipFile(string folder, string filename)
        {

            //FileStream fsOut = null;
            //ZipOutputStream zipStream = null;

            //try
            //{
            //    fsOut = File.Create(filename);
            //    zipStream = new ZipOutputStream(fsOut);

            //    int folderOffset = folder.Length + (folder.EndsWith("\\") ? 0 : 1);

            //    CompressFolder(folder, zipStream, folderOffset);

            //    zipStream.IsStreamOwner = true;
            //    zipStream.Close();

            //}
            //catch (Exception ex)
            //{
            //    OFLogger.Instance.LogError(ex.ToString());
            //}
            //finally
            //{
            //    if (zipStream.IsNotNull())
            //    {
            //        zipStream.Close();
            //    }
            //    if (fsOut.IsNotNull())
            //    {
            //        fsOut.Close();
            //    }
            //}
            FastZip zip = null;
            try
            {
                zip = new FastZip();
                zip.CreateZip(filename, folder, false, null);
            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(ex.ToString());
            }
            return filename;
        }

        private void CompressFolder(string folder, ZipOutputStream ZipStream, int folderOffset)
        {
            var files = Directory.GetFiles(folder);

            foreach (var file in files)
            {
                try
                {
                    var fileInfo = new FileInfo(file);
                    var entryName = file.Substring(folderOffset);
                    entryName = ZipEntry.CleanName(entryName);
                    var newEntry = new ZipEntry(entryName);
                    newEntry.DateTime = fileInfo.LastWriteTime;
                    newEntry.Size = fileInfo.Length;

                    ZipStream.PutNextEntry(newEntry);

                    var buffer = new byte[4096];
                    using (var reader = File.OpenRead(file))
                    {
                        StreamUtils.Copy(reader, ZipStream, buffer);
                    }
                    ZipStream.CloseEntry();
                }
                catch (Exception ex)
                {
                    OFLogger.Instance.LogError(ex.ToString());
                }
            }
        }


        #endregion






    }
}