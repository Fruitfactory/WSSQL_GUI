using System;
using OF.Core.Data;
using OF.Core.Interfaces;
using OF.Module.ViewModel;

namespace OF.Module.Interface.View
{
    public interface IPreviewView
    {
        MainViewModel Model
        {
            get;
            set;
        }

        bool SetPreviewFile(string filename);
        bool SetPreviewObject(OFBaseSearchObject searchObject);
        void SetSearchPattern(string pattern);
        void SetFullFolderPath(string path);
        void ClearPreview();
        void PassActionForPreview(IWSAction action);

        event EventHandler StartLoad;
        event EventHandler StopLoad;

        void Init();
    }
}
