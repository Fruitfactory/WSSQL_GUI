using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace WSPreview.PreviewHandler.PInvoke
{
    public class WindowAPI
    {

        public const int WM_DESTROY = 0x0002;
        public const int WM_SIZE = 0x0005;
        public const int WM_KEYDOWN = 0x0100;
        public const int WM_COPY = 0x0301;
        public const int WM_CHAR = 0x0102;

        public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumChildWindows(IntPtr hwndParent, EnumWindowsProc lpEnumFunc, IntPtr lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);


        /// <summary>
        /// SetWindowPos Flags
        /// </summary>
        public enum SetWindowPosFlags : uint
        {

            NOSIZE = 0x0001,
            NOMOVE = 0x0002,
            NOZORDER = 0x0004,
            NOREDRAW = 0x0008,
            NOACTIVATE = 0x0010,
            DRAWFRAME = 0x0020,
            FRAMECHANGED = 0x0020,
            SHOWWINDOW = 0x0040,
            HIDEWINDOW = 0x0080,
            NOCOPYBITS = 0x0100,
            NOOWNERZORDER = 0x0200,
            NOREPOSITION = 0x0200,
            NOSENDCHANGING = 0x0400,
            DEFERERASE = 0x2000,
            ASYNCWINDOWPOS = 0x4000
        }

        public enum WindowLongFlags : int
        {
            GWL_EXSTYLE = -20,
            GWLP_HINSTANCE = -6,
            GWLP_HWNDPARENT = -8,
            GWL_ID = -12,
            GWL_STYLE = -16,
            GWL_USERDATA = -21,
            GWL_WNDPROC = -4,
            DWLP_USER = 0x8,
            DWLP_MSGRESULT = 0x0,
            DWLP_DLGPROC = 0x4
        }

        // Window Styles 
        public const UInt32 WS_OVERLAPPED = 0;
        public const UInt32 WS_POPUP = 0x80000000;
        public const UInt32 WS_CHILD = 0x40000000;
        public const UInt32 WS_MINIMIZE = 0x20000000;
        public const UInt32 WS_VISIBLE = 0x10000000;
        public const UInt32 WS_DISABLED = 0x8000000;
        public const UInt32 WS_CLIPSIBLINGS = 0x4000000;
        public const UInt32 WS_CLIPCHILDREN = 0x2000000;
        public const UInt32 WS_MAXIMIZE = 0x1000000;
        public const UInt32 WS_CAPTION = 0xC00000;      // WS_BORDER or WS_DLGFRAME  
        public const UInt32 WS_BORDER = 0x800000;
        public const UInt32 WS_DLGFRAME = 0x400000;
        public const UInt32 WS_VSCROLL = 0x200000;
        public const UInt32 WS_HSCROLL = 0x100000;
        public const UInt32 WS_SYSMENU = 0x80000;
        public const UInt32 WS_THICKFRAME = 0x40000;
        public const UInt32 WS_GROUP = 0x20000;
        public const UInt32 WS_TABSTOP = 0x10000;
        public const UInt32 WS_MINIMIZEBOX = 0x20000;
        public const UInt32 WS_MAXIMIZEBOX = 0x10000;
        public const UInt32 WS_TILED = WS_OVERLAPPED;
        public const UInt32 WS_ICONIC = WS_MINIMIZE;
        public const UInt32 WS_SIZEBOX = WS_THICKFRAME;

        // Extended Window Styles 
        public const UInt32 WS_EX_DLGMODALFRAME = 0x0001;
        public const UInt32 WS_EX_NOPARENTNOTIFY = 0x0004;
        public const UInt32 WS_EX_TOPMOST = 0x0008;
        public const UInt32 WS_EX_ACCEPTFILES = 0x0010;
        public const UInt32 WS_EX_TRANSPARENT = 0x0020;
        public const UInt32 WS_EX_MDICHILD = 0x0040;
        public const UInt32 WS_EX_TOOLWINDOW = 0x0080;
        public const UInt32 WS_EX_WINDOWEDGE = 0x0100;
        public const UInt32 WS_EX_CLIENTEDGE = 0x0200;
        public const UInt32 WS_EX_CONTEXTHELP = 0x0400;
        public const UInt32 WS_EX_RIGHT = 0x1000;
        public const UInt32 WS_EX_LEFT = 0x0000;
        public const UInt32 WS_EX_RTLREADING = 0x2000;
        public const UInt32 WS_EX_LTRREADING = 0x0000;
        public const UInt32 WS_EX_LEFTSCROLLBAR = 0x4000;
        public const UInt32 WS_EX_RIGHTSCROLLBAR = 0x0000;
        public const UInt32 WS_EX_CONTROLPARENT = 0x10000;
        public const UInt32 WS_EX_STATICEDGE = 0x20000;
        public const UInt32 WS_EX_APPWINDOW = 0x40000;
        public const UInt32 WS_EX_OVERLAPPEDWINDOW = (WS_EX_WINDOWEDGE | WS_EX_CLIENTEDGE);
        public const UInt32 WS_EX_PALETTEWINDOW = (WS_EX_WINDOWEDGE | WS_EX_TOOLWINDOW | WS_EX_TOPMOST);
        public const UInt32 WS_EX_LAYERED = 0x00080000;
        public const UInt32 WS_EX_NOINHERITLAYOUT = 0x00100000; // Disable inheritence of mirroring by children
        public const UInt32 WS_EX_LAYOUTRTL = 0x00400000; // Right to left mirroring
        public const UInt32 WS_EX_COMPOSITED = 0x02000000;
        public const UInt32 WS_EX_NOACTIVATE = 0x08000000;


        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className,  string windowTitle);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, SetWindowPosFlags uFlags);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);


    }
}
