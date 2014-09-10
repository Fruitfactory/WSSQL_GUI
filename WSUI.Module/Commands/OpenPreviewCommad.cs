using System.Diagnostics;
using System.IO;
using System.Windows;
using WSUI.Core.Enums;
using WSUI.Core.Helpers;
using WSUI.Core.Logger;
using WSUI.Infrastructure.Service.Helpers;
using WSUI.Infrastructure.Services;
using WSUI.Module.Core;
using WSUI.Module.Interface;
using WSUI.Module.Interface.ViewModel;
using WSUI.Module.Service.Dialogs.Message;

namespace WSUI.Module.Commands
{
    public class OpenPreviewCommad : BasePreviewCommand
    {
        public OpenPreviewCommad(IMainViewModel mainViewModel) : base(mainViewModel)
        {
        }

        protected override bool OnCanExecute()
        {

            if (MainViewModel != null && MainViewModel.Current != null &&
                ((MainViewModel.Current.TypeItem & TypeSearchItem.FileAll) == MainViewModel.Current.TypeItem))
                return true;
            return false;
        }

        protected override void OnExecute()
        {
            string fileName = string.Empty;
            switch (MainViewModel.Current.TypeItem)
            {
                case TypeSearchItem.File:
                case TypeSearchItem.Picture:
                case TypeSearchItem.FileAll:
                    fileName = SearchItemHelper.GetFileName(MainViewModel.Current,false);
                    break;
                case TypeSearchItem.Attachment:
                    fileName = TempFileManager.Instance.GenerateTempFileName(MainViewModel.Current) ?? OutlookHelper.Instance.GetAttachmentTempFileName(MainViewModel.Current);
                    break;
                case TypeSearchItem.Email:
                    fileName = TempFileManager.Instance.GenerateTempFileName(MainViewModel.Current);
                    break;

            }
            if (string.IsNullOrEmpty(fileName) ||
                FileService.IsDirectory(fileName))
                return;
            try
            {
                //if (FileExestensionsHelper.Instance.IsExternsionRequiredClosePreview(Path.GetExtension(fileName)))
                //{
                //    MainViewModel.Parent.ForceClosePreview();
                //}
                Process.Start(fileName);
            }
            catch (FileLoadException ex)
            {
                MessageBoxService.Instance.Show("Error",
                                                string.Format(
                                                    "File '{0}' is locked by another process.\nClose all programs and try again.",
                                                    Path.GetFileName(fileName)), MessageBoxButton.OK, MessageBoxImage.Error);
                WSSqlLogger.Instance.LogError(string.Format("{0}: {1} - {2}", "Error", "Create Email - File Load", ex.Message));
            }
            catch (System.Exception ex)
            {
                WSSqlLogger.Instance.LogError(string.Format("{0}: {1} - {2}", "Start error", fileName, ex.Message));
            }
        }

        protected override string GetCaption()
        {
            return "Open";
        }

        protected override string GetIcon()
        {
            return @"pack://application:,,,/WSUI.Module;component/Images/fileopen.png";
        }

        protected override string GetTooltip()
        {
            return "Open file";
        }

    }
}
