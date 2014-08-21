using Microsoft.Office.Interop.Outlook;
using WSUI.Core.Enums;
using WSUI.Core.Helpers;
using WSUI.Core.Logger;
using WSUI.Module.Core;
using WSUI.Module.Interface;
using WSUI.Module.Interface.ViewModel;
using WSUI.Module.Service.Dialogs.Message;

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
                KindItem.Current.TypeItem == TypeSearchItem.Email)
                return true;
            return false;
        }

        protected override void OnExecute()
        {
            var itemSearch = KindItem.Current;
            var mail = OutlookHelper.Instance.GetEmailItem(itemSearch);
            if (mail != null)
            {
                try
                {


                    if (mail is _MailItem)
                    {
                        var replymail = (mail as _MailItem).Reply();
                        replymail.Display(false);
                        (mail as _MailItem).Close(OlInspectorClose.olDiscard);

                    }
                    else if (mail is _MeetingItem)
                    {
                        var replymail = (mail as _MeetingItem).Reply();
                        replymail.Display(false);
                        (mail as _MeetingItem).Close(OlInspectorClose.olDiscard);

                    }
                }
                catch (System.Exception ex)
                {
                    MessageBoxService.Instance.Show("Error", "Can't open Outlook item.", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Asterisk);
                    WSSqlLogger.Instance.LogError("Reply Command: " + ex.Message);
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
