using System;
using System.Runtime.InteropServices;

namespace OF.CA.Core
{
    public class CoreSetupApplication : IDisposable
    {
        protected readonly string ProductName;
        private IntPtr _mainWindowHandle;

        

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);


        public CoreSetupApplication(string productName)
        {
            ProductName = productName;
        }

        protected IntPtr GetMainWindowHandle()
        {
            _mainWindowHandle = FindWindow(null, ProductName + " Setup");
            if (_mainWindowHandle == IntPtr.Zero)
                _mainWindowHandle = FindWindow("#32770", ProductName);
            return _mainWindowHandle;
        }

        public virtual void Dispose()
        {
            _mainWindowHandle = IntPtr.Zero;
        }
    }
}