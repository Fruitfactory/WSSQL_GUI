using WSUI.Core.Data;

namespace WSUI.Core.Interfaces
{
    internal interface IOutlookHelper
    {
        string GetEMailTempFileName(BaseSearchObject itemsearch);

        string GetAttachmentTempFileName(BaseSearchObject itemsearch);
    }
}