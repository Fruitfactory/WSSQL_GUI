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
        void SetSearchPattern(string pattern);
        void ClearPreview();

        void Init();
    }
}
