using System;
using System.Collections.Generic;
using System.Text;
using WSUI.Infrastructure.Service.Enums;
using WSUI.Infrastructure.Service.Helpers;
using WSUI.Module.Core;
using WSUI.Module.Interface;
using C4F.DevKit.PreviewHandler.Service.Logger;

namespace WSUI.Module.Commands
{
    public class EmailCommand : BasePreviewCommand
    {
        public EmailCommand(IKindItem kindItem) : base(kindItem)
        {
        }

        protected override bool OnCanExecute()
        {
            if (_kindItem != null && _kindItem.Current != null &&
                ((_kindItem.Current.Type & TypeSearchItem.FileAll) == _kindItem.Current.Type))
                return true;
            return false;
        }

        protected override void OnExecute()
        {
            try
            {
                string filename = string.Empty;
                switch (_kindItem.Current.Type)
                {
                    case TypeSearchItem.File:
                    case TypeSearchItem.Picture:
                    case TypeSearchItem.FileAll:

                        filename = SearchItemHelper.GetFileName(_kindItem.Current);
                        break;
                    case TypeSearchItem.Attachment:
                        filename = TempFileManager.Instance.GenerateTempFileName(_kindItem.Current) ?? OutlookHelper.Instance.GetAttachmentTempFileName(_kindItem.Current);
                        break;
                }
                
                if (string.IsNullOrEmpty(filename))
                    return;
                var mail = OutlookHelper.Instance.CreateNewEmail();
                mail.Attachments.Add(filename);
                mail.BodyFormat = Microsoft.Office.Interop.Outlook.OlBodyFormat.olFormatHTML;
                mail.Display(false);
            }
            catch(Exception ex)
            {
                WSSqlLogger.Instance.LogError(string.Format("{0}: {1} - {2}","Error","Create Email",ex.Message));
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
