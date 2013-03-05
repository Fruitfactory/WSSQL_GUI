using Microsoft.Office.Interop.Outlook;
using WSUI.Infrastructure.Service.Enums;
using WSUI.Infrastructure.Service.Helpers;
using WSUI.Module.Core;
using WSUI.Module.Interface;

namespace WSUI.Module.Commands
{
    public class ReplyAllCommand : BasePreviewCommand
    {
        public ReplyAllCommand(IKindItem kindItem) : base(kindItem)
        {
        }


        protected override bool OnCanExecute()
        {
            if (KindItem != null && KindItem.Current != null &&
                KindItem.Current.Type == TypeSearchItem.Email)
                return true;
            return false;
        }

        protected override void OnExecute()
        {
            var itemSearch = KindItem.Current;
            var mail = OutlookHelper.Instance.GetEmailItem(itemSearch);
            if (mail != null)
            {
                var replymail = mail.ReplyAll();
                replymail.Display(false);
                mail.Close(OlInspectorClose.olDiscard);
            }
        }

        protected override string GetCaption()
        {
            return "Reply All";
        }

        protected override string GetIcon()
        {
            return @"pack://application:,,,/WSUI.Module;component/Images/replayall.png";
        }

        protected override string GetTooltip()
        {
            return "Reply All";
        }

    }
}