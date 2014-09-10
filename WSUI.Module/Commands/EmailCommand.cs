using System;
using System.IO;
using System.Windows;
using WSUI.Core.Enums;
using WSUI.Core.Helpers;
using WSUI.Core.Logger;
using WSUI.Infrastructure.Service.Helpers;
using WSUI.Module.Core;
using WSUI.Module.Interface.ViewModel;
using WSUI.Module.Service.Dialogs.Message;

namespace WSUI.Module.Commands
{
    public class EmailCommand : BasePreviewCommand
    {
        public EmailCommand(IMainViewModel mainViewModel)
            : base(mainViewModel)
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
            string filename = string.Empty;
            try
            {
                switch (MainViewModel.Current.TypeItem)
                {
                    case TypeSearchItem.File:
                    case TypeSearchItem.Picture:
                    case TypeSearchItem.FileAll:

                        filename = SearchItemHelper.GetFileName(MainViewModel.Current);
                        break;

                    case TypeSearchItem.Attachment:
                        filename = TempFileManager.Instance.GenerateTempFileName(MainViewModel.Current) ?? OutlookHelper.Instance.GetAttachmentTempFileName(MainViewModel.Current);
                        break;
                }

                if (string.IsNullOrEmpty(filename))
                    return;
                //if (FileExestensionsHelper.Instance.IsExternsionRequiredClosePreview(Path.GetExtension(filename)))
                //{
                //    MainViewModel.Parent.ForceClosePreview();
                //}
                var mail = OutlookHelper.Instance.CreateNewEmail();
                mail.Attachments.Add(filename);
                mail.BodyFormat = Microsoft.Office.Interop.Outlook.OlBodyFormat.olFormatHTML;
                mail.Display(false);
            }
            catch (FileLoadException ex)
            {
                MessageBoxService.Instance.Show("Error",
                                                string.Format(
                                                    "File '{0}' is locked by another process.\nClose all programs and try again.",
                                                    Path.GetFileName(filename)), MessageBoxButton.OK, MessageBoxImage.Error);
                WSSqlLogger.Instance.LogError(string.Format("{0}: {1} - {2}", "Error", "Create Email - File Load", ex.Message));
            }
            catch (Exception ex)
            {
                WSSqlLogger.Instance.LogError(string.Format("{0}: {1} - {2}", "Error", "Create Email", ex.Message));
            }
        }

        protected override string GetCaption()
        {
            return "Email";
        }

        protected override string GetIcon()
        {
            return @"pack://application:,,,/WSUI.Module;component/Images/send.png";
        }

        protected override string GetTooltip()
        {
            return "Send email";
        }
    }
}