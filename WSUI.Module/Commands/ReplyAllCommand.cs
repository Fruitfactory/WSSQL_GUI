using Microsoft.Office.Interop.Outlook;
using OF.Core.Data;
using OF.Core.Enums;
using OF.Core.Extensions;
using OF.Core.Helpers;
using OF.Core.Logger;
using OF.Infrastructure.Helpers;
using OF.Module.Core;
using OF.Module.Interface;
using OF.Module.Interface.ViewModel;
using OF.Module.Service.Dialogs.Message;

namespace OF.Module.Commands
{
    public class ReplyAllCommand : BaseEmailPreviewCommand
    {
        public ReplyAllCommand(IMainViewModel mainViewModel)
            : base(mainViewModel)
        {
        }

        protected override void OnExecute()
        {
            var itemSearch = GetCurrentSearchObject();
            if (itemSearch.IsNull())
                return;
            try
            {
                switch (itemSearch.TypeItem)
                {
                    case TypeSearchItem.Email:
                        MailItem reply = EmailCommandPreviewHelper.Instance.CreateReplyAllEmail(itemSearch as EmailSearchObject);
                        if (reply.IsNotNull())
                        {
                            reply.Display(false);
                        }
                        break;
                }
            }
            catch (System.Exception ex)
            {
                MessageBoxService.Instance.Show("Error", "Can't open Outlook item.", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Asterisk);
                WSSqlLogger.Instance.LogError("ReplyAll Command: " + ex.Message);
            }
        }

        protected override string GetCaption()
        {
            return "Reply All";
        }

        protected override string GetIcon()
        {
            return @"pack://application:,,,/OF.Module;component/Images/replayall.png";
        }

        protected override string GetTooltip()
        {
            return "Reply All";
        }

    }
}