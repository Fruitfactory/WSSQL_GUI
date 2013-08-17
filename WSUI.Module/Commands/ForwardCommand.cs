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
            if (KindItem != null && KindItem.Current != null &&
                KindItem.Current.Type == TypeSearchItem.Email)
                return true;
            return false;
        }

        protected override void OnExecute()
        {
            var searchItem = KindItem.Current;
            var mail = OutlookHelper.Instance.GetEmailItem(searchItem);
            if (mail != null)
            {
                if (mail is MailItem)
                {
                    var forwardmail = (mail as MailItem).Forward();
                    forwardmail.Display(false);
                    (mail as MailItem).Close(OlInspectorClose.olDiscard);    
                }
                else if(mail is MeetingItem)
                {
                    var forwardMetting = (mail as MeetingItem).Forward();
                    forwardMetting.Display(false);
                    (mail as MeetingItem).Close(OlInspectorClose.olDiscard);
                }
                else if (mail is AppointmentItem)
                {
                    var forwardMetting = (mail as AppointmentItem).ForwardAsVcal();
                    forwardMetting.Display(false);
                    (mail as AppointmentItem).Close(OlInspectorClose.olDiscard);
                }
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
