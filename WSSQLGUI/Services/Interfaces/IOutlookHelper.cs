using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WSSQLGUI.Services.Interfaces
{
    interface IOutlookHelper
    {
        string GetEMailTempFileName(WSSQLGUI.Models.SearchItem itemsearch);

        string GetAttachmentTempFileName(WSSQLGUI.Models.SearchItem itemsearch);
    }
}
