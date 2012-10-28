using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WSUI.Module.Interface
{
    interface IUView<T>
    {
        ISettingsView<T> SettingsView { get; set; }
        IDataView<T> DataView { get; set; }
    }
}
