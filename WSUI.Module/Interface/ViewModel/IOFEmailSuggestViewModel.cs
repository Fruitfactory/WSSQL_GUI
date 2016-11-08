using System;

namespace OF.Module.Interface.ViewModel
{
    public interface IOFEmailSuggestViewModel
    {
        void Show(Tuple<IntPtr,string> hWndParent);
        void Hide();
    }
}