using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Office.Interop.Outlook;
using WSUI.Infrastructure.Service.Enums;
using WSUI.Module.Core;
using WSUI.Module.Interface;
using WSUI.Infrastructure.Service.Helpers;

namespace WSUI.Module.Commands
{
    public class ForwardCommand : BasePreviewCommand
    {
        public ForwardCommand(IKindItem kindItem) : base(kindItem)
        {
        }

        protected override bool OnCanExecute()
        {
            if (_kindItem != null && _kindItem.Current != null &&
                _kindItem.Current.Type == TypeSearchItem.Email)
                return true;
            return false;
        }

        protected override void OnExecute()
        {
            var searchItem = _kindItem.Current;
            var mail = OutlookHelper.Instance.GetEmailItem(searchItem);
            if (mail != null)
            {
                var forwardmail = mail.Forward();
                forwardmail.Display(false);
                mail.Close(OlInspectorClose.olDiscard);
            }
        }

        protected override string GetCaption()
        {
            return "Forward";
        }

        protected override string GetIcon()
        {
            return @"pack://application:,,,/WSUI.Module;component/Images/forward.png";
        }

        protected override string GetTooltip()
        {
            return "Forward email";
        }

    }
}
