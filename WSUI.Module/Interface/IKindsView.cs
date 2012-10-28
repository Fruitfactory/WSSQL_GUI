using System;
using System.Collections.Generic;
using System.Text;
using WSUI.Module.ViewModel;

namespace WSUI.Module.Interface
{
    public interface IKindsView
    {
        MainViewModel Model
        {
            get;
            set;
        }
    }
}
