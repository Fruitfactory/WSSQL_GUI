using System.Runtime.InteropServices;
using Microsoft.Practices.Prism.Events;

namespace OFOutlookPlugin.Events
{
    [ClassInterface(ClassInterfaceType.None)]
    public class OFOpenWindow : CompositePresentationEvent<bool>
    {
    }
}