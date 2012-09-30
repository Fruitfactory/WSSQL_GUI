using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WSSQLGUI.Services.Interfaces
{
    interface IOutlookHelper
    {
        string GetEMailTempFileName(WSSQLGUI.Core.BaseSearchData itemsearch);

        string GetAttachmentTempFileName(WSSQLGUI.Core.BaseSearchData itemsearch);
    }
}
