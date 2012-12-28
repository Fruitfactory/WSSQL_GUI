using System;
using System.Windows;

namespace WSUI.Module.Service
{
    public class MainWindowInfo
    {
        public MainWindowInfo()
        {
            MainWindowRect = new Rect();
            MainWindowHandle = IntPtr.Zero;
        }

        public Rect MainWindowRect { get; set; }
        public IntPtr MainWindowHandle { get; set; }
    }
}
