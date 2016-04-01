using AddinExpress.MSO;
using OFOutlookPlugin.Interfaces;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace OFOutlookPlugin.Managers.OutlookEventsManagers
{
    public class OFOutlookItemEvents : AddinExpress.MSO.ADXOutlookItemEvents, IOFOutlookItemEvents
    {
        private IOFMailRemovingManager _removingManager;

        public OFOutlookItemEvents(ADXAddinModule module, IOFMailRemovingManager removingManager) : base(module)
        {
            _removingManager = removingManager;
        }

        public override void ProcessAttachmentAdd(object attachment)
        {
            
        }

        public override void ProcessAttachmentRead(object attachment)
        {
            
        }

        public override void ProcessBeforeAttachmentSave(object attachment, ADXCancelEventArgs e)
        {
            
        }

        public override void ProcessBeforeCheckNames(ADXCancelEventArgs e)
        {
            
        }

        public override void ProcessClose(ADXCancelEventArgs e)
        {
            
        }

        public override void ProcessCustomAction(object action, object response, ADXCancelEventArgs e)
        {
            
        }

        public override void ProcessCustomPropertyChange(string name)
        {
            
        }

        public override void ProcessForward(object forward, ADXCancelEventArgs e)
        {
            
        }

        public override void ProcessOpen(ADXCancelEventArgs e)
        {
            
        }

        public override void ProcessPropertyChange(string name)
        {
            
        }

        public override void ProcessRead()
        {
            
        }

        public override void ProcessReply(object response, ADXCancelEventArgs e)
        {
            
        }

        public override void ProcessReplyAll(object response, ADXCancelEventArgs e)
        {
            
        }

        public override void ProcessSend(ADXCancelEventArgs e)
        {
            
        }

        public override void ProcessWrite(ADXCancelEventArgs e)
        {
            
        }

        public override void ProcessBeforeDelete(object item, ADXCancelEventArgs e)
        {
            _removingManager.RemoveMail(item as Outlook.MailItem);    
        }

        public override void ProcessAttachmentRemove(object attachment)
        {
            
        }

        public override void ProcessBeforeAttachmentAdd(object attachment, ADXCancelEventArgs e)
        {
        }

        public override void ProcessBeforeAttachmentPreview(object attachment, ADXCancelEventArgs e)
        {
        }

        public override void ProcessBeforeAttachmentRead(object attachment, ADXCancelEventArgs e)
        {
        }

        public override void ProcessBeforeAttachmentWriteToTempFile(object attachment, ADXCancelEventArgs e)
        {
        }

        public override void ProcessUnload()
        {
        }

        public override void ProcessBeforeAutoSave(ADXCancelEventArgs e)
        {
        }

        public void ConnectTo(Outlook.MailItem Mail)
        {
            if (Mail == null)
            {
                return;
            }
            this.ConnectTo(Mail, true);
        }
    }
}