using System.Runtime.InteropServices;
using Microsoft.Practices.Prism.Events;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace OFOutlookPlugin.Events
{
    [ClassInterface(ClassInterfaceType.None)]
    public class OFActivatedInspectorEvent : CompositePresentationEvent<Outlook.Inspector>
    {
        
    }
}