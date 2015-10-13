using System;
using System.IO;
using System.Windows;
using OF.Core.Enums;
using OF.Core.Helpers;
using OF.Core.Logger;
using OF.Infrastructure.Service.Helpers;
using OF.Module.Core;
using OF.Module.Interface.ViewModel;
using OF.Module.Service.Dialogs.Message;

namespace OF.Module.Commands
{
    public class OFEmailCommand : OFBaseFilePreviewCommand
    {
        public OFEmailCommand(IMainViewModel mainViewModel)
            : base(mainViewModel)
        {
        }

        protected override void OnExecute()
        {
            string filename = string.Empty;
            try
            {
                var currentItem = GetCurrentSearchObject();
                var type = GetTypeOfCurrentItem();
                switch (type)
                {
                    case OFTypeSearchItem.File:
                    case OFTypeSearchItem.Picture:
                    case OFTypeSearchItem.FileAll:

                        filename = OFSearchItemHelper.GetFileName(currentItem);
                        break;

                    case OFTypeSearchItem.Attachment:
                        filename = MainViewModel.IsPreviewVisible ? OFTempFileManager.Instance.GenerateTempFileName(currentItem) ?? OFOutlookHelper.Instance.GetAttachmentTempFileName(currentItem)
                            : OFSearchItemHelper.GetFileName(currentItem);
                        break;
                }

                if (string.IsNullOrEmpty(filename))
                    return;
                var mail = OFOutlookHelper.Instance.CreateNewEmail();
                mail.Attachments.Add(filename);
                mail.BodyFormat = Microsoft.Office.Interop.Outlook.OlBodyFormat.olFormatHTML;
                mail.Display(false);
            }
            catch (FileLoadException ex)
            {
                OFMessageBoxService.Instance.Show("Error",
                                                string.Format(
                                                    "File '{0}' is locked by another process.\nClose all programs and try again.",
                                                    Path.GetFileName(filename)), MessageBoxButton.OK, MessageBoxImage.Error);
                OFLogger.Instance.LogError(string.Format("{0}: {1} - {2}", "Error", "Create Email - File Load", ex.Message));
            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(string.Format("{0}: {1} - {2}", "Error", "Create Email", ex.Message));
            }
        }

        protected override string GetCaption()
        {
            return "Email";
        }

        protected override string GetIcon()
        {
            return @"pack://application:,,,/OF.Module;component/Images/send.png";
        }

        protected override string GetTooltip()
        {
            return "Send email";
        }
    }
}