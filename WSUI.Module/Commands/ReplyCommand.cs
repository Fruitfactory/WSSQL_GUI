using Microsoft.Office.Interop.Outlook;
using WSUI.Core.Data;
using WSUI.Core.Enums;
using WSUI.Core.Extensions;
using WSUI.Core.Helpers;
using WSUI.Core.Logger;
using WSUI.Infrastructure.Helpers;
using WSUI.Infrastructure.Service.Helpers;
using WSUI.Module.Core;
using WSUI.Module.Interface;
using WSUI.Module.Interface.ViewModel;
using WSUI.Module.Service.Dialogs.Message;

namespace WSUI.Module.Commands
{
    public class ReplyCommand : BaseEmailPreviewCommand
    {
        public ReplyCommand(IMainViewModel mainViewModel)
            : base(mainViewModel)
        {
        }

        protected override void OnExecute()
        {
            var itemSearch = GetCurrentSearchObject();
            try
            {

                switch (itemSearch.TypeItem)
                {
                    case TypeSearchItem.Email:
                        MailItem reply = EmailCommandPreviewHelper.Instance.CreateReplyEmail(itemSearch as EmailSearchObject);
                        if (reply.IsNotNull())
                        {
                            reply.Display(false);
                        }
                        break;
                }
            }
            catch (System.Exception ex)
            {
                MessageBoxService.Instance.Show("Error", "Can't crete Reply email.", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Asterisk);
                WSSqlLogger.Instance.LogError("Reply Command: " + ex.Message);
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
