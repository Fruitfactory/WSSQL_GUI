using Microsoft.Office.Interop.Outlook;
using WSUI.Core.Enums;
using WSUI.Core.Logger;
using WSUI.Infrastructure.Service.Helpers;
using WSUI.Module.Core;
using WSUI.Module.Interface;
using WSUI.Module.Service.Dialogs.Message;

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
                    if (mail is MailItem)
                    {
                        var replyallmail = (mail as MailItem).ReplyAll();
                        replyallmail.Display(false);
                        (mail as MailItem).Close(OlInspectorClose.olDiscard);

                    }
                    else if (mail is MeetingItem)
                    {
                        var replyallmail = (mail as MeetingItem).ReplyAll();
                        replyallmail.Display(false);
                        (mail as MeetingItem).Close(OlInspectorClose.olDiscard);

                    }
                }
                catch (System.Exception ex)
                {
                    MessageBoxService.Instance.Show("Error", "Can't open Outlook item.", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Asterisk);
                    WSSqlLogger.Instance.LogError("ReplyAll Command: " + ex.Message);
                }
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