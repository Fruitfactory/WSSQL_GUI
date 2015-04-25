using OF.Core.Data;
using OF.Core.Enums;
using OF.Core.Extensions;
using OF.Core.Helpers;
using OF.Core.Logger;
using OF.Infrastructure.Helpers;
using OF.Module.Interface.ViewModel;
using Outlook = Microsoft.Office.Interop.Outlook;
using OF.Module.Core;
using OF.Module.Interface;
using OF.Module.Service.Dialogs.Message;

namespace OF.Module.Commands
{
    public class ForwardCommand : BaseEmailPreviewCommand
    {
        public ForwardCommand(IMainViewModel mainViewModel)
            : base(mainViewModel)
        {
        }

        protected override void OnExecute()
        {
            var searchItem = GetCurrentSearchObject();
            if(searchItem.IsNull())
                return;
            
            try
            {
                switch (searchItem.TypeItem)
                {
                    case TypeSearchItem.Email:
                        Outlook.MailItem reply = EmailCommandPreviewHelper.Instance.CreateForwardEmail(searchItem as EmailSearchObject);
                        if (reply.IsNotNull())
                        {
                            reply.Display(false);
                        }
                        break;
                }
            }
            catch (System.Exception ex)
            {
                MessageBoxService.Instance.Show("Error", "Can't create Forward item.", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Asterisk);
                OFLogger.Instance.LogError("Forward Command: " + ex.Message);
            }
        }

        protected override string GetCaption()
        {
            return "Forward";
        }

        protected override string GetIcon()
        {
            return @"pack://application:,,,/OF.Module;component/Images/forward.png";
        }

        protected override string GetTooltip()
        {
            return "Forward email";
        }

    }
}
