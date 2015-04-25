using Microsoft.Office.Interop.Outlook;
using OF.Core.Data;
using OF.Core.Enums;
using OF.Core.Extensions;
using OF.Core.Helpers;
using OF.Core.Logger;
using OF.Infrastructure.Helpers;
using OF.Infrastructure.Service.Helpers;
using OF.Module.Core;
using OF.Module.Interface;
using OF.Module.Interface.ViewModel;
using OF.Module.Service.Dialogs.Message;

namespace OF.Module.Commands
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
                OFLogger.Instance.LogError("Reply Command: " + ex.Message);
            }
        }

        protected override string GetCaption()
        {
            return "Reply";
        }

        protected override string GetIcon()
        {
            return @"pack://application:,,,/OF.Module;component/Images/reply.png";
        }

        protected override string GetTooltip()
        {
            return "Reply email";
        }

    }
}
