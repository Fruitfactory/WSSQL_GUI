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
            if (KindItem != null && KindItem.Current != null &&
                ((KindItem.Current.Type & TypeSearchItem.FileAll) == KindItem.Current.Type))
                return true;
            return false;
        }

        protected override void OnExecute()
        {
            try
            {
                string filename = string.Empty;
                switch (KindItem.Current.Type)
                {
                    case TypeSearchItem.File:
                    case TypeSearchItem.Picture:
                    case TypeSearchItem.FileAll:

                        filename = SearchItemHelper.GetFileName(KindItem.Current);
                        break;
                    case TypeSearchItem.Attachment:
                        filename = TempFileManager.Instance.GenerateTempFileName(KindItem.Current) ?? OutlookHelper.Instance.GetAttachmentTempFileName(KindItem.Current);
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
