using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WSUI.Infrastructure.Service.Interfaces
{
    interface IOutlookHelper
    {
        string GetEMailTempFileName(WSUI.Infrastructure.Core.BaseSearchData itemsearch);

        string GetAttachmentTempFileName(WSUI.Infrastructure.Core.BaseSearchData itemsearch);
    }
}
