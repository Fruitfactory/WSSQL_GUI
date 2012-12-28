
namespace WSUI.Infrastructure.Service.Interfaces
{
    interface IOutlookHelper
    {
        string GetEMailTempFileName(WSUI.Infrastructure.Core.BaseSearchData itemsearch);

        string GetAttachmentTempFileName(WSUI.Infrastructure.Core.BaseSearchData itemsearch);
    }
}
