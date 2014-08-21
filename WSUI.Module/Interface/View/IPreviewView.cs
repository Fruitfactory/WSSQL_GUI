using System;
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
        void SetSearchPattern(string pattern);
        void SetFullFolderPath(string path);
        void SetPreviewObject(object previewData);
        void ClearPreview();
        void PassActionForPreview(IWSAction action);

        event EventHandler StartLoad;
        event EventHandler StopLoad;

        void Init();
    }
}
