using OF.Core.Data;

namespace OF.Core.Interfaces
{
    internal interface IOutlookHelper
    {
        string GetEMailTempFileName(BaseSearchObject itemsearch);

        string GetAttachmentTempFileName(BaseSearchObject itemsearch);
    }
}