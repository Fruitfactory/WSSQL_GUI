using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Documents;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Unity;
using OF.Core.Helpers;
using OF.Core.Logger;
using OF.Module.Interface.ViewModel;

namespace OF.Module.ViewModel.LogFileManager
{
    public class OFLogFilesSenderManager  : IOFLogFilesSenderManager
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
                var esBinPath = OFRegistryHelper.Instance.GetElasticSearchpath();
                var p = esBinPath.Substring(0, esBinPath.LastIndexOf('\\'));

                var elasticSearchLogsPath = Path.Combine(p, "logs");
                var outlookfinderlogPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "OutlookFinder", "Log");
#if DEBUG
                var toEmail = "iyariki.ya@gmail.com";
#else
                var toEmail = OF.Module.Properties.Settings.Default.LogFileEmail;
#endif


                var outlookfinderZipsPath =
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "OutlookFinder");

                ClearPreviousZipFiles(outlookfinderZipsPath);

                var elasticTask = CreateZipFileAsync(elasticSearchLogsPath,
                    Path.Combine(outlookfinderZipsPath, OF.Module.Properties.Settings.Default.ElasticSearchZipFileName));
                var outlookfinderTask = CreateZipFileAsync(outlookfinderlogPath,
                    Path.Combine(outlookfinderZipsPath, OF.Module.Properties.Settings.Default.OutlookFinderZipFileName));

                elasticTask.Wait();
                outlookfinderTask.Wait();

                var listZipFiles = new List<string>() { elasticTask.Result, outlookfinderTask.Result };

                InternalSendLogs(listZipFiles, toEmail);

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

        #endregion


        #region [private methods]

        private void InternalSendLogs(List<string> listZipFiles, string toEmail)
        {
            OFOutlookHelper.Instance.SendEmail("Logs",toEmail,listZipFiles);
        }

        private void ClearPreviousZipFiles(string outlookfinderZipsPath)
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
        }


        private Task<string> CreateZipFileAsync(string logFolder, string zipFilename)
        {
            var folder = logFolder;
            var filename = zipFilename;

            return Task<string>.Factory.StartNew(() => CreateZipFile(folder, filename));
        }

        private string CreateZipFile(string folder, string filename)
        {
            var zip = new FastZip();
            zip.CreateZip(filename, folder, false, null);
            return filename;
        }


        #endregion






    }
}