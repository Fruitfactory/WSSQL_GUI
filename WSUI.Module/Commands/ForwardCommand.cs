using Outlook = Microsoft.Office.Interop.Outlook;
using WSUI.Infrastructure.Service.Enums;
using WSUI.Module.Core;
using WSUI.Module.Interface;
using WSUI.Infrastructure.Service.Helpers;
using WSPreview.PreviewHandler.Service.Logger;

namespace WSUI.Module.Commands
{
    public class ForwardCommand : BasePreviewCommand
    {
        public ForwardCommand(IKindItem kindItem) : base(kindItem)
        {
        }

        protected override bool OnCanExecute()
        {
            if (KindItem != null && KindItem.Current != null &&
                KindItem.Current.Type == TypeSearchItem.Email)
                return true;
            return false;
        }

        protected override void OnExecute()
        {
            var searchItem = KindItem.Current;
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
