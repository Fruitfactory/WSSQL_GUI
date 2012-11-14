using System;
using System.Collections.Generic;
using System.Text;
using WSUI.Infrastructure.Service.Enums;
using WSUI.Infrastructure.Service.Helpers;
using WSUI.Module.Core;
using WSUI.Module.Interface;

namespace WSUI.Module.Commands
{
    public class EmailCommand : BasePreviewCommand
    {
        public EmailCommand(IKindItem kindItem) : base(kindItem)
        {
        }

        protected override bool OnCanExecute()
        {
            if (_kindItem != null && _kindItem.Current != null && _kindItem.Current.Type == TypeSearchItem.File)
                return true;
            return false;
        }

        protected override void OnExecute()
        {
            string filename = SearchItemHelper.GetFileName(_kindItem.Current);
            if(string.IsNullOrEmpty(filename))
                return;
            var mail = OutlookHelper.Instance.CreateNewEmail();
            mail.Attachments.Add(filename);
            mail.BodyFormat = Microsoft.Office.Interop.Outlook.OlBodyFormat.olFormatHTML;
            mail.Display(false);

        }

        protected override string GetCaption()
        {
            return "Email";
        }

    }
}
