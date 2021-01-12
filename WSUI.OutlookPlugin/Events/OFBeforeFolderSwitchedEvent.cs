using System.Runtime.InteropServices;
using Microsoft.Practices.Prism.Events;
using OF.Infrastructure.Events;

namespace OFOutlookPlugin.Events
{
	[ClassInterface(ClassInterfaceType.None)]
	public class OFBeforeFolderSwitchedEvent : CompositePresentationEvent<OFBeforeFolderSwitchedPayload>

	{
		
	}
}