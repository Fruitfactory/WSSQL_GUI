using Elasticsearch.Net;
using Microsoft.Office.Interop.Outlook;

namespace OF.Infrastructure.Events
{
	public class OFBeforeFolderSwitchedPayload
	{
		public OFBeforeFolderSwitchedPayload(Explorer explorer, object folder, bool cancel)
		{
			Explorer = explorer;
			Folder = folder;
			Cancel = cancel;
		}
		public Explorer Explorer { get; }

		public object Folder { get; }

		public bool Cancel { get; private set;}

		public void CancelSwitching()
		{
			Cancel = true;
		}
		
	}
}