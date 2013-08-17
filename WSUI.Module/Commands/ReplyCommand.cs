using Microsoft.Office.Interop.Outlook;
using WSUI.Infrastructure.Service.Enums;
using WSUI.Infrastructure.Service.Helpers;
using WSUI.Module.Core;
using WSUI.Module.Interface;

namespace WSUI.Module.Commands
{
    public class ReplyCommand : BasePreviewCommand
    {
        public ReplyCommand(IKindItem kindItem) : base(kindItem)
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
                if (mail is MailItem)
                {
                    var replymail = (mail as MailItem).Reply();
                    replymail.Display(false);
                    (mail as MailItem).Close(OlInspectorClose.olDiscard);
                    
                }
                else if(mail is MeetingItem)
                {
                    var replymail = (mail as MeetingItem).Reply();
                    replymail.Display(false);
                    (mail as MeetingItem).Close(OlInspectorClose.olDiscard);
                    
                }
            }
        }

        protected override string GetCaption()
        {
            return "Reply";
        }

        protected override string GetIcon()
        {
            return @"pack://application:,,,/WSUI.Module;component/Images/reply.png";
        }

        protected override string GetTooltip()
        {
            return "Reply email";
        }

    }
}
