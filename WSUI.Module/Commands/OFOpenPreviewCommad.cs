using System.Diagnostics;
using System.IO;
using System.Windows;
using OF.Core.Enums;
using OF.Core.Helpers;
using OF.Core.Logger;
using OF.Infrastructure.Service.Helpers;
using OF.Infrastructure.Services;
using OF.Module.Core;
using OF.Module.Interface;
using OF.Module.Interface.ViewModel;
using OF.Module.Service.Dialogs.Message;

namespace OF.Module.Commands
{
    public class OFOpenPreviewCommad : OFBaseFilePreviewCommand
    {
        public OFOpenPreviewCommad(IMainViewModel mainViewModel) : base(mainViewModel)
        {
        }

        protected override void OnExecute()
        {
            string fileName = string.Empty;
            var type = GetTypeOfCurrentItem();
            var currentItem = GetCurrentSearchObject();
            switch (type)
            {
                case OFTypeSearchItem.File:
                case OFTypeSearchItem.Picture:
                case OFTypeSearchItem.FileAll:
                    fileName = OFSearchItemHelper.GetFileName(currentItem,false);
                    break;
                case OFTypeSearchItem.Attachment:
                    fileName = MainViewModel.IsPreviewVisible ? OFTempFileManager.Instance.GenerateTempFileName(currentItem) ?? OFOutlookHelper.Instance.GetAttachmentTempFileName(currentItem) 
                        : OFSearchItemHelper.GetFileName(currentItem) ;
                    break;
                case OFTypeSearchItem.Email:
                    fileName = MainViewModel.IsPreviewVisible ? OFTempFileManager.Instance.GenerateTempFileName(currentItem)
                        : OFSearchItemHelper.GetFileName(currentItem);
                    break;

            }
            if (string.IsNullOrEmpty(fileName) ||
                FileService.IsDirectory(fileName))
                return;
            try
            {
                Process.Start(fileName);
            }
            catch (FileLoadException ex)
            {
                OFMessageBoxService.Instance.Show("Error",
                                                string.Format(
                                                    "File '{0}' is locked by another process.\nClose all programs and try again.",
                                                    Path.GetFileName(fileName)), MessageBoxButton.OK, MessageBoxImage.Error);
                OFLogger.Instance.LogError(string.Format("{0}: {1} - {2}", "Error", "Create Email - File Load", ex.Message));
            }
            catch (System.Exception ex)
            {
                OFLogger.Instance.LogError(string.Format("{0}: {1} - {2}", "Start error", fileName, ex.Message));
            }
        }

        protected override string GetCaption()
        {
            return "Open";
        }

        protected override string GetIcon()
        {
            return @"pack://application:,,,/OF.Module;component/Images/fileopen.png";
        }

        protected override string GetTooltip()
        {
            return "Open file";
        }

    }
}
