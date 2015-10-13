using System;
using System.Windows;

namespace OF.Module.Service
{
    public class OFMainWindowInfo
    {
        public OFMainWindowInfo()
        {
            MainWindowRect = new Rect();
            MainWindowHandle = IntPtr.Zero;
        }

        public Rect MainWindowRect { get; set; }
        public IntPtr MainWindowHandle { get; set; }
    }
}
