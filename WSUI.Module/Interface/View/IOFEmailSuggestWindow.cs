using System;

namespace OF.Module.Interface.View
{
    public interface IOFEmailSuggestWindow : IDisposable
    {
        object Model { get; set; }
        void ShowSuggestings(IntPtr hWnd);
        void HideSuggestings();

        bool IsVisible { get; }
        void JumpToEmailList();

        bool IsClosed { get; }
    }
}