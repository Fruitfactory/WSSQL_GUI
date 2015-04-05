using WSUI.Core.Data;
using WSUI.Core.Enums;
using WSUI.Core.Extensions;
using WSUI.Core.Helpers;
using WSUI.Core.Logger;
using WSUI.Infrastructure.Helpers;
using WSUI.Module.Interface.ViewModel;
using Outlook = Microsoft.Office.Interop.Outlook;
using WSUI.Module.Core;
using WSUI.Module.Interface;
using WSUI.Module.Service.Dialogs.Message;

namespace WSUI.Module.Commands
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
                WSSqlLogger.Instance.LogError("Forward Command: " + ex.Message);
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
