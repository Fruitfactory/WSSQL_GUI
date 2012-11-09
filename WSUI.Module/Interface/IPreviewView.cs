using System;
using System.Collections.Generic;
using System.Text;
using WSUI.Module.ViewModel;

namespace WSUI.Module.Interface
{
    public interface IPreviewView
    {
        MainViewModel Model
        {
            get;
            set;
        }

        bool SetPreviewFile(string filename);
        void Init();
    }
}
