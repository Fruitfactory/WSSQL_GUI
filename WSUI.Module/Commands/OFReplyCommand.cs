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
    public class OFReplyCommand : OFBaseEmailPreviewCommand
    {
        public OFReplyCommand(IMainViewModel mainViewModel)
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
                    case OFTypeSearchItem.Email:
                        MailItem reply = OFEmailCommandPreviewHelper.Instance.CreateReplyEmail(itemSearch as OFEmailSearchObject);
                        if (reply.IsNotNull())
                        {
                            InsertAttachments(reply,itemSearch as OFEmailSearchObject);
                            reply.Display(false);
                        }
                        break;
                }
            }
            catch (System.Exception ex)
            {
                OFMessageBoxService.Instance.Show("Error", "Can't crete Reply email.", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Asterisk);
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
