using WSUI.Core.Enums;
using WSUI.Core.Helpers;
using WSUI.Core.Logger;
using WSUI.Module.Interface.ViewModel;
using Outlook = Microsoft.Office.Interop.Outlook;
using WSUI.Module.Core;
using WSUI.Module.Interface;
using WSUI.Module.Service.Dialogs.Message;

namespace WSUI.Module.Commands
{
    public class ForwardCommand : BaseEmailPreviewCommand
    {
        public ForwardCommand(IMainViewModel mainViewModel) : base(mainViewModel)
        {
        }

        protected override void OnExecute()
        {
            var searchItem = GetCurrentSearchObject();
            var mail = OutlookHelper.Instance.GetEmailItem(searchItem);
            if (mail != null)
            {
                try
                {
                    if (mail is Outlook._MailItem)
                    {
                        var forwardmail = (mail as Outlook._MailItem).Forward();
                        forwardmail.Display(false);
                        (mail as Outlook._MailItem).Close(Outlook.OlInspectorClose.olDiscard);
                    }
                    else if (mail is Outlook._MeetingItem)
                    {
                        var Metting = (mail as Outlook._MeetingItem);
                        if (Metting != null)
                        {
                            var forward = Metting.Forward();
                            forward.Display(false);
                            Metting.Close(Outlook.OlInspectorClose.olDiscard);
                        }
                        
                    }
                    else if (mail is Outlook._AppointmentItem)
                    {
                        var Appoint = (mail as Outlook._AppointmentItem);
                        if (Appoint != null)
                        {
                            var forward = Appoint.ForwardAsVcal();
                            forward.Display(false);
                            Appoint.Close(Outlook.OlInspectorClose.olDiscard);
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    MessageBoxService.Instance.Show("Error", "Can't open Outlook item.", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Asterisk);
                    WSSqlLogger.Instance.LogError("Forward Command: " + ex.Message);
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
