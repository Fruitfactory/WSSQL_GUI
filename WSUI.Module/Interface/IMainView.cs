using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WSUI.Module.ViewModel;

namespace WSUI.Module.Interface
{
    public interface IMainView
    {
        IMainViewModel Model { get; set; }
    }
}
