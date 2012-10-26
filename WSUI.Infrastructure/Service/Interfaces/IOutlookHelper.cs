using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WSUI.Service.Interfaces
{
    interface IOutlookHelper
    {
        string GetEMailTempFileName(WSUI.Core.BaseSearchData itemsearch);

        string GetAttachmentTempFileName(WSUI.Core.BaseSearchData itemsearch);
    }
}
