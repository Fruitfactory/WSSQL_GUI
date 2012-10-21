using System.Collections.Generic;

namespace WSSQLGUI.Controllers
{
    internal interface IEmailSettings
    {
        List<string> GetFolders();
    }
}