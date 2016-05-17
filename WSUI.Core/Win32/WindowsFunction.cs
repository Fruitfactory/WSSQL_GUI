using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
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

        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", EntryPoint = "SetActiveWindow", SetLastError = true)]
        static extern IntPtr SetActiveWindow(IntPtr hWnd);

        [DllImport("user32.dll", EntryPoint = "SetFocus", SetLastError = true)]
        static extern IntPtr SetFocus(IntPtr hWnd);





        const int WM_COMMAND = 0x111;
        const int MIN_ALL = 419;
        const int MIN_ALL_UNDO = 416;
        public const int WM_LBUTTONDOWN = 0x201;
        public const int WM_LBUTTONUP = 0x202;
        const int WM_ACTIVATE = 0x0006;

        const int GWL_HWNDPARENT = (-8);

        const int GWL_ID = (-12);
        const int GWL_STYLE = (-16);
        const int GWL_EXSTYLE = (-20);

// Window Styles 
        const UInt32 WS_OVERLAPPED = 0;
        const UInt32 WS_POPUP = 0x80000000;
        const UInt32 WS_CHILD = 0x40000000;
        const UInt32 WS_MINIMIZE = 0x20000000;
        const UInt32 WS_VISIBLE = 0x10000000;
        const UInt32 WS_DISABLED = 0x8000000;
        const UInt32 WS_CLIPSIBLINGS = 0x4000000;
        const UInt32 WS_CLIPCHILDREN = 0x2000000;
        const UInt32 WS_MAXIMIZE = 0x1000000;
        const UInt32 WS_CAPTION = 0xC00000;      // WS_BORDER or WS_DLGFRAME  
        const UInt32 WS_BORDER = 0x800000;
        const UInt32 WS_DLGFRAME = 0x400000;
        const UInt32 WS_VSCROLL = 0x200000;
        const UInt32 WS_HSCROLL = 0x100000;
        const UInt32 WS_SYSMENU = 0x80000;
        const UInt32 WS_THICKFRAME = 0x40000;
        const UInt32 WS_GROUP = 0x20000;
        const UInt32 WS_TABSTOP = 0x10000;
        const UInt32 WS_MINIMIZEBOX = 0x20000;
        const UInt32 WS_MAXIMIZEBOX = 0x10000;
        const UInt32 WS_TILED = WS_OVERLAPPED;
        const UInt32 WS_ICONIC = WS_MINIMIZE;
        const UInt32 WS_SIZEBOX = WS_THICKFRAME;
        const int SW_HIDE = 0;
        const int SW_SHOWNORMAL = 1;
        const int SW_NORMAL = 1;



        [DllImport("ole32.dll")]
        static extern int CreateBindCtx(
            uint reserved,
            out IBindCtx ppbc);

        [DllImport("ole32.dll")]
        public static extern void GetRunningObjectTable(
            int reserved,
            out IRunningObjectTable prot);


        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        static extern uint SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

        [DllImport("User32")]
        private static extern int ShowWindow(IntPtr hwnd, int nCmdShow);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWindowVisible(IntPtr hWnd);

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
                if (sb.ToString().StartsWith(data.Title) || sb.ToString().Contains(data.Title))
                {
                    data.hWnd = hWnd;
                    return false;    // Found the wnd, halt enumeration
                }
            }
            return true;
        }

        public static void HideWindow(IntPtr hWnd)
        {
            ShowWindow(hWnd, SW_HIDE);
        }

        public static void ShowWindow(IntPtr hWnd)
        {
            ShowWindow(hWnd, SW_SHOWNORMAL);
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

        private static List<object> GetRunningInstances(string[] progIds)
        {
            List<string> clsIds = new List<string>();

            // get the app clsid
            foreach (string progId in progIds)
            {
                Type type = Type.GetTypeFromProgID(progId);

                if (type != null)
                    clsIds.Add(type.GUID.ToString().ToUpper());
            }

            // get Running Object Table ...
            IRunningObjectTable Rot = null;
            GetRunningObjectTable(0, out Rot);
            if (Rot == null)
                return null;

            // get enumerator for ROT entries
            IEnumMoniker monikerEnumerator = null;
            Rot.EnumRunning(out monikerEnumerator);

            if (monikerEnumerator == null)
                return null;

            monikerEnumerator.Reset();

            List<object> instances = new List<object>();

            IntPtr pNumFetched = new IntPtr();
            IMoniker[] monikers = new IMoniker[1];

            // go through all entries and identifies app instances
            while (monikerEnumerator.Next(1, monikers, pNumFetched) == 0)
            {
                IBindCtx bindCtx;
                CreateBindCtx(0, out bindCtx);
                if (bindCtx == null)
                    continue;

                string displayName;
                monikers[0].GetDisplayName(bindCtx, null, out displayName);

                foreach (string clsId in clsIds)
                {
                    if (displayName.ToUpper().IndexOf(clsId) > 0)
                    {
                        object ComObject;
                        Rot.GetObject(monikers[0], out ComObject);


                        if (ComObject == null)
                            continue;

                        instances.Add(ComObject);
                        break;
                    }
                }
            }

            return instances;
        }

        public static bool IsOutlookRegisteredInROT()
        {
            string[] progIds =
            {
                "Outlook.Application"
            };

            List<object> instances = GetRunningInstances(progIds);

            return instances.Count > 0;
        }

        public static void SetShellWindowActive()
        {
            IntPtr lHwnd = FindWindow("Shell_TrayWnd", null);
            if (lHwnd != null)
            {
                SetFocus(lHwnd);
            }
        }

    }
}