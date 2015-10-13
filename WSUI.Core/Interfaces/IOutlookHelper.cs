using OF.Core.Data;

namespace OF.Core.Interfaces
{
    internal interface IOutlookHelper
    {
        string GetEMailTempFileName(OFBaseSearchObject itemsearch);

        string GetAttachmentTempFileName(OFBaseSearchObject itemsearch);
    }
}