using System;

namespace OF.Module.Interface.View
{
    public interface IOFEmailSuggestWindow
    {
        object Model { get; set; }
        void ShowSuggestings(IntPtr hWnd);
        void HideSuggestings();

        bool IsVisible { get; }
    }
}