
using WSUI.Core.Core;

namespace WSUI.Infrastructure.Service.Interfaces
{
    interface IOutlookHelper
    {
        string GetEMailTempFileName(BaseSearchData itemsearch);

        string GetAttachmentTempFileName(BaseSearchData itemsearch);
    }
}
