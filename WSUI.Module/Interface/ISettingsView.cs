using System;
using System.Collections.Generic;
using System.Text;

namespace WSUI.Module.Interface
{
    public interface ISettingsView<T>
    {
        T Model
        {
            get;
            set;
        }
    }
}
