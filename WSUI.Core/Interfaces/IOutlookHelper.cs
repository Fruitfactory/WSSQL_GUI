
using WSUI.Core.Core;
using WSUI.Core.Data;

namespace WSUI.Core.Interfaces 
{
    interface IOutlookHelper
    {
        string GetEMailTempFileName(BaseSearchObject itemsearch);

        string GetAttachmentTempFileName(BaseSearchObject itemsearch);
    }
}
