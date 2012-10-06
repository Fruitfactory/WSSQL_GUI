using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WSSQLGUI.Core;

namespace WSSQLGUI.Views
{
    internal interface IContactSettingsView : ISettingsView
    {
        string FolderContact { get; set; }
    }

}
