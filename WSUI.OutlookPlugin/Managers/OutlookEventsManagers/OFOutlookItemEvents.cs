
using OFOutlookPlugin.Interfaces;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace OFOutlookPlugin.Managers.OutlookEventsManagers
{
    public class OFOutlookItemEvents : IOFOutlookItemEvents
    {
        private IOFMailRemovingManager _removingManager;

        public OFOutlookItemEvents(IOFMailRemovingManager removingManager)
        {
            _removingManager = removingManager;
        }

        public void ConnectTo(Outlook.MailItem Mail)
        {
            //if (Mail == null)
            //{
            //    return;
            //}
            //this.ConnectTo(Mail, true);
        }

        public void Dispose ( )
        {
            throw new System.NotImplementedException();
        }
    }
}