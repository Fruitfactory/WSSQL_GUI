using System.Diagnostics;
using System.Windows;
using Microsoft.Office.Interop.Outlook;
using OF.Core.Enums;
using OF.Core.Extensions;
using OF.Core.Helpers;
using OF.Core.Logger;
using OF.Infrastructure.Service.Helpers;
using OF.Module.Core;
using OF.Module.Interface;
using OF.Module.Interface.ViewModel;
using OF.Module.Service.Dialogs.Message;
using Exception = System.Exception;

namespace OF.Module.Commands
{
    public class OFOpenEmailCommand :  OFBaseEmailPreviewCommand
    {
        public OFOpenEmailCommand(IMainViewModel mainViewModel) : base(mainViewModel)
        {
        }
        protected override void OnExecute()
        {
            var type = GetTypeOfCurrentItem();
            var currentItem = GetCurrentSearchObject();

            string filename = string.Empty;
            try
            {
                switch (type)
                {
                    case OFTypeSearchItem.Email:
                        filename = OFTempFileManager.Instance.GenerateTempFileName(currentItem);
                        var email = OFOutlookHelper.Instance.GetEmailItem(currentItem.EntryID) as MailItem;
                        if (email.IsNotNull() && !string.IsNullOrEmpty(filename))
                        {
                            email.SaveAs(filename);
                            Process.Start(filename);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                OFMessageBoxService.Instance.Show("Error",
                                                string.Format(
                                                    "Error: '{0}'",ex.Message) , MessageBoxButton.OK, MessageBoxImage.Error);
                OFLogger.Instance.LogError(ex.Message);
            }
        }

        protected override string GetIcon()
        {
            return @"pack://application:,,,/OF.Module;component/Images/openemail.png";
        }

        protected override string GetCaption()
        {
            return "Open";
        }

        protected override string GetTooltip()
        {
            return "Open Email";
        }
    }
}