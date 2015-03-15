using System;
using WSUI.Core.Data;
using WSUI.Core.Interfaces;
using WSUI.Module.ViewModel;

namespace WSUI.Module.Interface.View
{
    public interface IPreviewView
    {
        MainViewModel Model
        {
            get;
            set;
        }

        bool SetPreviewFile(string filename);
        bool SetPreviewObject(BaseSearchObject searchObject);
        void SetSearchPattern(string pattern);
        void SetFullFolderPath(string path);
        void ClearPreview();
        void PassActionForPreview(IWSAction action);

        event EventHandler StartLoad;
        event EventHandler StopLoad;

        void Init();
    }
}
