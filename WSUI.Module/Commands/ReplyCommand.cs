using Microsoft.Office.Interop.Outlook;
using WSUI.Core.Enums;
using WSUI.Core.Extensions;
using WSUI.Core.Helpers;
using WSUI.Core.Logger;
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
                        string filename = SearchItemHelper.GetFileName(itemSearch);
                        MailItem mailItem = OutlookHelper.Instance.CreateEmailFromTemplate("c:\\Users\\Yariki\\Downloads\\test-sample-message.eml");
                        if (mailItem.IsNotNull())
                        {
                            var reply = mailItem.Reply();
                            reply.Display(false);
                            mailItem.Close(OlInspectorClose.olDiscard);
                        }
                        break;
                }
            }
            catch (System.Exception ex)
            {
                MessageBoxService.Instance.Show("Error", "Can't open Outlook item.", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Asterisk);
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
