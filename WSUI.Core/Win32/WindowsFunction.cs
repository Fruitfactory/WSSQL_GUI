using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace OF.Core.Win32
{
    public static class WindowsFunction
    {
        #region [struct]

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;        // x position of upper-left corner
            public int Top;         // y position of upper-left corner
            public int Right;       // x position of lower-right corner
            public int Bottom;      // y position of lower-right corner
        }

        [StructLayout(LayoutKind.Sequential)]
        public class POINT
        {
            public int x = 0;
            public int y = 0;
        }

        public struct LASTINPUTINFO
        {
            public uint cbSize;

            public uint dwTime;
        }


        public class SearchData
        {
            // You can put any dicks or Doms in here...
            public string Wndclass;
            public string Title;
            public IntPtr hWnd;
        }


        #endregion [struct]

        #region [function]

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

        [DllImport("User32", EntryPoint = "ClientToScreen", SetLastError = true, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern int ClientToScreen(IntPtr hWnd, [In, Out] POINT pt);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetActiveWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetFocus();

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("User32.dll")]
        private static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

        [DllImport("Kernel32.dll")]
        private static extern uint GetTickCount();

        [DllImport("Kernel32.dll")]
        public static extern uint GetLastError();

        private delegate bool EnumWindowsProc(IntPtr hWnd, ref SearchData data);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, ref SearchData data);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        public const int BN_CLICKED = 245;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SendMessage(int hWnd, int msg, int wParam, IntPtr lParam);

       
        #endregion [function]


        public static IntPtr SearchForWindow(string wndclass, string title)
        {
            SearchData sd = new SearchData { Wndclass = wndclass, Title = title };
            EnumWindows(new EnumWindowsProc(EnumProc), ref sd);
            return sd.hWnd;
        }

        public static bool EnumProc(IntPtr hWnd, ref SearchData data)
        {
            StringBuilder sb = new StringBuilder(1024);
            GetClassName(hWnd, sb, sb.Capacity);
            if (sb.ToString().StartsWith(data.Wndclass))
            {
                sb = new StringBuilder(1024);
                GetWindowText(hWnd, sb, sb.Capacity);
                if (sb.ToString().StartsWith(data.Title))
                {
                    data.hWnd = hWnd;
                    return false;    // Found the wnd, halt enumeration
                }
            }
            return true;
        }

        public static Point TransformToScreen(Point point, Visual relativeTo)
        {
            HwndSource hwndSource = PresentationSource.FromVisual(relativeTo) as HwndSource;
            Visual root = hwndSource.RootVisual;
            // Translate the point from the visual to the root.
            GeneralTransform transformToRoot = relativeTo.TransformToAncestor(root);
            Point pointRoot = transformToRoot.Transform(point);

            // Transform the point from the root to client coordinates.
            Matrix m = Matrix.Identity;
            Transform transform = VisualTreeHelper.GetTransform(root);
            if (transform != null)
            {
                m = Matrix.Multiply(m, transform.Value);
            }
            Vector offset = VisualTreeHelper.GetOffset(root);
            m.Translate(offset.X, offset.Y);
            Point pointClient = m.Transform(pointRoot);
            // Convert from “device-independent pixels” into pixels.
            pointClient = hwndSource.CompositionTarget.TransformToDevice.Transform(pointClient);
            POINT pointClientPixels = new POINT();
            pointClientPixels.x = (0 < pointClient.X) ? (int)(pointClient.X + 0.5) : (int)(pointClient.X - 0.5);
            pointClientPixels.y = (0 < pointClient.Y) ? (int)(pointClient.Y + 0.5) : (int)(pointClient.Y - 0.5);

            // Transform the point into screen coordinates.
            POINT pointScreenPixels = pointClientPixels;
            ClientToScreen(hwndSource.Handle, pointScreenPixels);
            return new Point(pointScreenPixels.x, pointScreenPixels.y);
        }
 
        public static uint GetIdleTime()
        {
            LASTINPUTINFO lastInPut = new LASTINPUTINFO();
            lastInPut.cbSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf(lastInPut);
            GetLastInputInfo(ref lastInPut);
            var tickCount = GetTickCount();
            return (tickCount - lastInPut.dwTime);
        }
    }
}