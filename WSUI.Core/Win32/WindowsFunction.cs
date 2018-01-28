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
            public List<string> list;
            public List<IntPtr> listPtr;
        }

        /// <summary>
        /// contains information about the current state of both physical and virtual memory, including extended memory
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class MEMORYSTATUSEX
        {
            /// <summary>
            /// Size of the structure, in bytes. You must set this member before calling GlobalMemoryStatusEx. 
            /// </summary>
            public uint dwLength;

            /// <summary>
            /// Number between 0 and 100 that specifies the approximate percentage of physical memory that is in use (0 indicates no memory use and 100 indicates full memory use). 
            /// </summary>
            public uint dwMemoryLoad;

            /// <summary>
            /// Total size of physical memory, in bytes.
            /// </summary>
            public ulong ullTotalPhys;

            /// <summary>
            /// Size of physical memory available, in bytes. 
            /// </summary>
            public ulong ullAvailPhys;

            /// <summary>
            /// Size of the committed memory limit, in bytes. This is physical memory plus the size of the page file, minus a small overhead. 
            /// </summary>
            public ulong ullTotalPageFile;

            /// <summary>
            /// Size of available memory to commit, in bytes. The limit is ullTotalPageFile. 
            /// </summary>
            public ulong ullAvailPageFile;

            /// <summary>
            /// Total size of the user mode portion of the virtual address space of the calling process, in bytes. 
            /// </summary>
            public ulong ullTotalVirtual;

            /// <summary>
            /// Size of unreserved and uncommitted memory in the user mode portion of the virtual address space of the calling process, in bytes. 
            /// </summary>
            public ulong ullAvailVirtual;

            /// <summary>
            /// Size of unreserved and uncommitted memory in the extended portion of the virtual address space of the calling process, in bytes. 
            /// </summary>
            public ulong ullAvailExtendedVirtual;

            /// <summary>
            /// Initializes a new instance of the <see cref="T:MEMORYSTATUSEX"/> class.
            /// </summary>
            public MEMORYSTATUSEX()
            {
                this.dwLength = (uint)Marshal.SizeOf(typeof(MEMORYSTATUSEX));
            }
        }


        #endregion [struct]

        #region [function]

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

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

        [DllImport("user32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EnumChildWindows(IntPtr window, EnumWindowsProc callback, ref SearchData i);

        [DllImport("user32.dll")]
        public static extern int GetDlgCtrlID(IntPtr hwndCtl);

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
        public static extern IntPtr SetFocus(IntPtr hWnd);

        [DllImport("User32.dll")]
        public static extern IntPtr SendDlgItemMessage(IntPtr hWnd, uint IDDlgItem, int uMsg, int nMaxCount, StringBuilder lpString);

        [DllImport("User32.dll")]
        public static extern IntPtr GetParent(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, IntPtr ProcessId);

        [DllImport("user32.dll")]
        public static extern IntPtr AttachThreadInput(IntPtr idAttach,
                             IntPtr idAttachTo, bool fAttach);
        
        [DllImport("user32.dll")]
        public static extern bool ScreenToClient(IntPtr hWnd, ref POINT lpPoint);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, SetWindowPosFlags uFlags);

        [DllImport("user32.dll")]
        public static extern bool SetDlgItemText(IntPtr hDlg, int nIDDlgItem,string lpString);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool SetCaretPos(int x, int y);

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetDlgItemText(IntPtr hDlg, int nIDDlgItem, [Out] StringBuilder lpString, int nMaxCount);


        /// <summary>
        ///     Special window handles
        /// </summary>
        public enum SpecialWindowHandles
        {
            // ReSharper disable InconsistentNaming
            /// <summary>
            ///     Places the window at the top of the Z order.
            /// </summary>
            HWND_TOP = 0,
            /// <summary>
            ///     Places the window at the bottom of the Z order. If the hWnd parameter identifies a topmost window, the window loses its topmost status and is placed at the bottom of all other windows.
            /// </summary>
            HWND_BOTTOM = 1,
            /// <summary>
            ///     Places the window above all non-topmost windows. The window maintains its topmost position even when it is deactivated.
            /// </summary>
            HWND_TOPMOST = -1,
            /// <summary>
            ///     Places the window above all non-topmost windows (that is, behind all topmost windows). This flag has no effect if the window is already a non-topmost window.
            /// </summary>
            HWND_NOTOPMOST = -2
            // ReSharper restore InconsistentNaming
        }

        [Flags]
        public enum SetWindowPosFlags : uint
        {
            // ReSharper disable InconsistentNaming

            /// <summary>
            ///     If the calling thread and the thread that owns the window are attached to different input queues, the system posts the request to the thread that owns the window. This prevents the calling thread from blocking its execution while other threads process the request.
            /// </summary>
            SWP_ASYNCWINDOWPOS = 0x4000,

            /// <summary>
            ///     Prevents generation of the WM_SYNCPAINT message.
            /// </summary>
            SWP_DEFERERASE = 0x2000,

            /// <summary>
            ///     Draws a frame (defined in the window's class description) around the window.
            /// </summary>
            SWP_DRAWFRAME = 0x0020,

            /// <summary>
            ///     Applies new frame styles set using the SetWindowLong function. Sends a WM_NCCALCSIZE message to the window, even if the window's size is not being changed. If this flag is not specified, WM_NCCALCSIZE is sent only when the window's size is being changed.
            /// </summary>
            SWP_FRAMECHANGED = 0x0020,

            /// <summary>
            ///     Hides the window.
            /// </summary>
            SWP_HIDEWINDOW = 0x0080,

            /// <summary>
            ///     Does not activate the window. If this flag is not set, the window is activated and moved to the top of either the topmost or non-topmost group (depending on the setting of the hWndInsertAfter parameter).
            /// </summary>
            SWP_NOACTIVATE = 0x0010,

            /// <summary>
            ///     Discards the entire contents of the client area. If this flag is not specified, the valid contents of the client area are saved and copied back into the client area after the window is sized or repositioned.
            /// </summary>
            SWP_NOCOPYBITS = 0x0100,

            /// <summary>
            ///     Retains the current position (ignores X and Y parameters).
            /// </summary>
            SWP_NOMOVE = 0x0002,

            /// <summary>
            ///     Does not change the owner window's position in the Z order.
            /// </summary>
            SWP_NOOWNERZORDER = 0x0200,

            /// <summary>
            ///     Does not redraw changes. If this flag is set, no repainting of any kind occurs. This applies to the client area, the nonclient area (including the title bar and scroll bars), and any part of the parent window uncovered as a result of the window being moved. When this flag is set, the application must explicitly invalidate or redraw any parts of the window and parent window that need redrawing.
            /// </summary>
            SWP_NOREDRAW = 0x0008,

            /// <summary>
            ///     Same as the SWP_NOOWNERZORDER flag.
            /// </summary>
            SWP_NOREPOSITION = 0x0200,

            /// <summary>
            ///     Prevents the window from receiving the WM_WINDOWPOSCHANGING message.
            /// </summary>
            SWP_NOSENDCHANGING = 0x0400,

            /// <summary>
            ///     Retains the current size (ignores the cx and cy parameters).
            /// </summary>
            SWP_NOSIZE = 0x0001,

            /// <summary>
            ///     Retains the current Z order (ignores the hWndInsertAfter parameter).
            /// </summary>
            SWP_NOZORDER = 0x0004,

            /// <summary>
            ///     Displays the window.
            /// </summary>
            SWP_SHOWWINDOW = 0x0040,

            // ReSharper restore InconsistentNaming
        }


        public enum EM : int
        {
            GETSEL = 0x00B0,
            SETSEL = 0x00B1,
        }



        public static IntPtr GetFocusedControl(IntPtr thisHandle)
        {
            var activeWnd = GetForegroundWindow();
            var activeWndThread = GetWindowThreadProcessId(activeWnd, IntPtr.Zero);
            var thisWndThread = GetWindowThreadProcessId(thisHandle, IntPtr.Zero);
            AttachThreadInput(activeWndThread, thisWndThread, true);
            var focused = GetFocus();
            AttachThreadInput(activeWnd, thisWndThread, false);
            return focused;
        }

        public static StringBuilder GetRichEditText(IntPtr hWndRichEdit)
        {
            var dwID = GetDlgCtrlID(hWndRichEdit);
            IntPtr hWndParent = GetParent(hWndRichEdit);
            StringBuilder title = new StringBuilder(128);
            GetDlgItemText(hWndParent, dwID, title, title.Capacity);
            return title;
        }

        public static void SetRichEditText(IntPtr hWndRichEdit, string text)
        {
            uint dwID = GetWindowLong(hWndRichEdit, GWL_ID);
            IntPtr hWndParent = GetParent(hWndRichEdit);

            SendDlgItemMessage(hWndParent, dwID, WM_SETTEXT, 0, new StringBuilder(text));
        }

        public static void GetAllObjectWithClass(string wndClass, List<IntPtr> listObjects)
        {
            SearchData sd = new SearchData { Wndclass = wndClass, listPtr = listObjects };
            EnumWindows(new EnumWindowsProc(EnumProc1), ref sd);
        }

        public static void GetAllChildWindowsWithClass(IntPtr hWndParent, string wndClass, List<IntPtr> listObjects)
        {
            SearchData sd = new SearchData { Wndclass = wndClass, listPtr = listObjects };
            EnumChildWindows(hWndParent, new EnumWindowsProc(EnumProcChild), ref sd);
        }

        private static bool EnumProc1(IntPtr hWnd, ref SearchData data)
        {
            StringBuilder sb = new StringBuilder(1024);
            GetClassName(hWnd, sb, sb.Capacity);
            if (sb.ToString().StartsWith(data.Wndclass))
            {
                data.listPtr.Add(hWnd);
            }
            return true;
        }

        private static bool EnumProcChild(IntPtr hWnd, ref SearchData data)
        {
            StringBuilder sb = new StringBuilder(1024);
            GetClassName(hWnd, sb, sb.Capacity);
            if (sb.ToString().StartsWith(data.Wndclass))
            {
                data.listPtr.Add(hWnd);
            }
            return true;
        }


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

        public const int WM_GETTEXT = 0x000D;
        public const int WM_SETTEXT = 0x000C;

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
        
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool GlobalMemoryStatusEx([In, Out] MEMORYSTATUSEX lpBuffer); //Used to use ref with comment below

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, EntryPoint = "GlobalMemoryStatusEx", SetLastError = true)]
        public static extern bool _GlobalMemoryStatusEx(ref MEMORYSTATUSEX lpBuffer);

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


        #region [raw input]


        public const int WM_INPUT = 0x00FF;


        /// <summary>
        /// Enumeration containing HID usage page flags.
        /// </summary>
        public enum HIDUsagePage : ushort
        {
            /// <summary>Unknown usage page.</summary>
            Undefined = 0x00,
            /// <summary>Generic desktop controls.</summary>
            Generic = 0x01,
            /// <summary>Simulation controls.</summary>
            Simulation = 0x02,
            /// <summary>Virtual reality controls.</summary>
            VR = 0x03,
            /// <summary>Sports controls.</summary>
            Sport = 0x04,
            /// <summary>Games controls.</summary>
            Game = 0x05,
            /// <summary>Keyboard controls.</summary>
            Keyboard = 0x07,
            /// <summary>LED controls.</summary>
            LED = 0x08,
            /// <summary>Button.</summary>
            Button = 0x09,
            /// <summary>Ordinal.</summary>
            Ordinal = 0x0A,
            /// <summary>Telephony.</summary>
            Telephony = 0x0B,
            /// <summary>Consumer.</summary>
            Consumer = 0x0C,
            /// <summary>Digitizer.</summary>
            Digitizer = 0x0D,
            /// <summary>Physical interface device.</summary>
            PID = 0x0F,
            /// <summary>Unicode.</summary>
            Unicode = 0x10,
            /// <summary>Alphanumeric display.</summary>
            AlphaNumeric = 0x14,
            /// <summary>Medical instruments.</summary>
            Medical = 0x40,
            /// <summary>Monitor page 0.</summary>
            MonitorPage0 = 0x80,
            /// <summary>Monitor page 1.</summary>
            MonitorPage1 = 0x81,
            /// <summary>Monitor page 2.</summary>
            MonitorPage2 = 0x82,
            /// <summary>Monitor page 3.</summary>
            MonitorPage3 = 0x83,
            /// <summary>Power page 0.</summary>
            PowerPage0 = 0x84,
            /// <summary>Power page 1.</summary>
            PowerPage1 = 0x85,
            /// <summary>Power page 2.</summary>
            PowerPage2 = 0x86,
            /// <summary>Power page 3.</summary>
            PowerPage3 = 0x87,
            /// <summary>Bar code scanner.</summary>
            BarCode = 0x8C,
            /// <summary>Scale page.</summary>
            Scale = 0x8D,
            /// <summary>Magnetic strip reading devices.</summary>
            MSR = 0x8E
        }

        /// <summary>Enumeration containing the HID usage values.</summary>
        public enum HIDUsage : ushort
        {
            /// <summary></summary>
            Pointer = 0x01,
            /// <summary></summary>
            Mouse = 0x02,
            /// <summary></summary>
            Joystick = 0x04,
            /// <summary></summary>
            Gamepad = 0x05,
            /// <summary></summary>
            Keyboard = 0x06,
            /// <summary></summary>
            Keypad = 0x07,
            /// <summary></summary>
            SystemControl = 0x80,
            /// <summary></summary>
            X = 0x30,
            /// <summary></summary>
            Y = 0x31,
            /// <summary></summary>
            Z = 0x32,
            /// <summary></summary>
            RelativeX = 0x33,
            /// <summary></summary>    
            RelativeY = 0x34,
            /// <summary></summary>
            RelativeZ = 0x35,
            /// <summary></summary>
            Slider = 0x36,
            /// <summary></summary>
            Dial = 0x37,
            /// <summary></summary>
            Wheel = 0x38,
            /// <summary></summary>
            HatSwitch = 0x39,
            /// <summary></summary>
            CountedBuffer = 0x3A,
            /// <summary></summary>
            ByteCount = 0x3B,
            /// <summary></summary>
            MotionWakeup = 0x3C,
            /// <summary></summary>
            VX = 0x40,
            /// <summary></summary>
            VY = 0x41,
            /// <summary></summary>
            VZ = 0x42,
            /// <summary></summary>
            VBRX = 0x43,
            /// <summary></summary>
            VBRY = 0x44,
            /// <summary></summary>
            VBRZ = 0x45,
            /// <summary></summary>
            VNO = 0x46,
            /// <summary></summary>
            SystemControlPower = 0x81,
            /// <summary></summary>
            SystemControlSleep = 0x82,
            /// <summary></summary>
            SystemControlWake = 0x83,
            /// <summary></summary>
            SystemControlContextMenu = 0x84,
            /// <summary></summary>
            SystemControlMainMenu = 0x85,
            /// <summary></summary>
            SystemControlApplicationMenu = 0x86,
            /// <summary></summary>
            SystemControlHelpMenu = 0x87,
            /// <summary></summary>
            SystemControlMenuExit = 0x88,
            /// <summary></summary>
            SystemControlMenuSelect = 0x89,
            /// <summary></summary>
            SystemControlMenuRight = 0x8A,
            /// <summary></summary>
            SystemControlMenuLeft = 0x8B,
            /// <summary></summary>
            SystemControlMenuUp = 0x8C,
            /// <summary></summary>
            SystemControlMenuDown = 0x8D,
            /// <summary></summary>
            KeyboardNoEvent = 0x00,
            /// <summary></summary>
            KeyboardRollover = 0x01,
            /// <summary></summary>
            KeyboardPostFail = 0x02,
            /// <summary></summary>
            KeyboardUndefined = 0x03,
            /// <summary></summary>
            KeyboardaA = 0x04,
            /// <summary></summary>
            KeyboardzZ = 0x1D,
            /// <summary></summary>
            Keyboard1 = 0x1E,
            /// <summary></summary>
            Keyboard0 = 0x27,
            /// <summary></summary>
            KeyboardLeftControl = 0xE0,
            /// <summary></summary>
            KeyboardLeftShift = 0xE1,
            /// <summary></summary>
            KeyboardLeftALT = 0xE2,
            /// <summary></summary>
            KeyboardLeftGUI = 0xE3,
            /// <summary></summary>
            KeyboardRightControl = 0xE4,
            /// <summary></summary>
            KeyboardRightShift = 0xE5,
            /// <summary></summary>
            KeyboardRightALT = 0xE6,
            /// <summary></summary>
            KeyboardRightGUI = 0xE7,
            /// <summary></summary>
            KeyboardScrollLock = 0x47,
            /// <summary></summary>
            KeyboardNumLock = 0x53,
            /// <summary></summary>
            KeyboardCapsLock = 0x39,
            /// <summary></summary>
            KeyboardF1 = 0x3A,
            /// <summary></summary>
            KeyboardF12 = 0x45,
            /// <summary></summary>
            KeyboardReturn = 0x28,
            /// <summary></summary>
            KeyboardEscape = 0x29,
            /// <summary></summary>
            KeyboardDelete = 0x2A,
            /// <summary></summary>
            KeyboardPrintScreen = 0x46,
            /// <summary></summary>
            LEDNumLock = 0x01,
            /// <summary></summary>
            LEDCapsLock = 0x02,
            /// <summary></summary>
            LEDScrollLock = 0x03,
            /// <summary></summary>
            LEDCompose = 0x04,
            /// <summary></summary>
            LEDKana = 0x05,
            /// <summary></summary>
            LEDPower = 0x06,
            /// <summary></summary>
            LEDShift = 0x07,
            /// <summary></summary>
            LEDDoNotDisturb = 0x08,
            /// <summary></summary>
            LEDMute = 0x09,
            /// <summary></summary>
            LEDToneEnable = 0x0A,
            /// <summary></summary>
            LEDHighCutFilter = 0x0B,
            /// <summary></summary>
            LEDLowCutFilter = 0x0C,
            /// <summary></summary>
            LEDEqualizerEnable = 0x0D,
            /// <summary></summary>
            LEDSoundFieldOn = 0x0E,
            /// <summary></summary>
            LEDSurroundFieldOn = 0x0F,
            /// <summary></summary>
            LEDRepeat = 0x10,
            /// <summary></summary>
            LEDStereo = 0x11,
            /// <summary></summary>
            LEDSamplingRateDirect = 0x12,
            /// <summary></summary>
            LEDSpinning = 0x13,
            /// <summary></summary>
            LEDCAV = 0x14,
            /// <summary></summary>
            LEDCLV = 0x15,
            /// <summary></summary>
            LEDRecordingFormatDet = 0x16,
            /// <summary></summary>
            LEDOffHook = 0x17,
            /// <summary></summary>
            LEDRing = 0x18,
            /// <summary></summary>
            LEDMessageWaiting = 0x19,
            /// <summary></summary>
            LEDDataMode = 0x1A,
            /// <summary></summary>
            LEDBatteryOperation = 0x1B,
            /// <summary></summary>
            LEDBatteryOK = 0x1C,
            /// <summary></summary>
            LEDBatteryLow = 0x1D,
            /// <summary></summary>
            LEDSpeaker = 0x1E,
            /// <summary></summary>
            LEDHeadset = 0x1F,
            /// <summary></summary>
            LEDHold = 0x20,
            /// <summary></summary>
            LEDMicrophone = 0x21,
            /// <summary></summary>
            LEDCoverage = 0x22,
            /// <summary></summary>
            LEDNightMode = 0x23,
            /// <summary></summary>
            LEDSendCalls = 0x24,
            /// <summary></summary>
            LEDCallPickup = 0x25,
            /// <summary></summary>
            LEDConference = 0x26,
            /// <summary></summary>
            LEDStandBy = 0x27,
            /// <summary></summary>
            LEDCameraOn = 0x28,
            /// <summary></summary>
            LEDCameraOff = 0x29,
            /// <summary></summary>    
            LEDOnLine = 0x2A,
            /// <summary></summary>
            LEDOffLine = 0x2B,
            /// <summary></summary>
            LEDBusy = 0x2C,
            /// <summary></summary>
            LEDReady = 0x2D,
            /// <summary></summary>
            LEDPaperOut = 0x2E,
            /// <summary></summary>
            LEDPaperJam = 0x2F,
            /// <summary></summary>
            LEDRemote = 0x30,
            /// <summary></summary>
            LEDForward = 0x31,
            /// <summary></summary>
            LEDReverse = 0x32,
            /// <summary></summary>
            LEDStop = 0x33,
            /// <summary></summary>
            LEDRewind = 0x34,
            /// <summary></summary>
            LEDFastForward = 0x35,
            /// <summary></summary>
            LEDPlay = 0x36,
            /// <summary></summary>
            LEDPause = 0x37,
            /// <summary></summary>
            LEDRecord = 0x38,
            /// <summary></summary>
            LEDError = 0x39,
            /// <summary></summary>
            LEDSelectedIndicator = 0x3A,
            /// <summary></summary>
            LEDInUseIndicator = 0x3B,
            /// <summary></summary>
            LEDMultiModeIndicator = 0x3C,
            /// <summary></summary>
            LEDIndicatorOn = 0x3D,
            /// <summary></summary>
            LEDIndicatorFlash = 0x3E,
            /// <summary></summary>
            LEDIndicatorSlowBlink = 0x3F,
            /// <summary></summary>
            LEDIndicatorFastBlink = 0x40,
            /// <summary></summary>
            LEDIndicatorOff = 0x41,
            /// <summary></summary>
            LEDFlashOnTime = 0x42,
            /// <summary></summary>
            LEDSlowBlinkOnTime = 0x43,
            /// <summary></summary>
            LEDSlowBlinkOffTime = 0x44,
            /// <summary></summary>
            LEDFastBlinkOnTime = 0x45,
            /// <summary></summary>
            LEDFastBlinkOffTime = 0x46,
            /// <summary></summary>
            LEDIndicatorColor = 0x47,
            /// <summary></summary>
            LEDRed = 0x48,
            /// <summary></summary>
            LEDGreen = 0x49,
            /// <summary></summary>
            LEDAmber = 0x4A,
            /// <summary></summary>
            LEDGenericIndicator = 0x3B,
            /// <summary></summary>
            TelephonyPhone = 0x01,
            /// <summary></summary>
            TelephonyAnsweringMachine = 0x02,
            /// <summary></summary>
            TelephonyMessageControls = 0x03,
            /// <summary></summary>
            TelephonyHandset = 0x04,
            /// <summary></summary>
            TelephonyHeadset = 0x05,
            /// <summary></summary>
            TelephonyKeypad = 0x06,
            /// <summary></summary>
            TelephonyProgrammableButton = 0x07,
            /// <summary></summary>
            SimulationRudder = 0xBA,
            /// <summary></summary>
            SimulationThrottle = 0xBB
        }

        /// <summary>Enumeration containing flags for a raw input device.</summary>
        [Flags()]
        public enum RawInputDeviceFlags
        {
            /// <summary>No flags.</summary>
            None = 0,
            /// <summary>If set, this removes the top level collection from the inclusion list. This tells the operating system to stop reading from a device which matches the top level collection.</summary>
            Remove = 0x00000001,
            /// <summary>If set, this specifies the top level collections to exclude when reading a complete usage page. This flag only affects a TLC whose usage page is already specified with PageOnly.</summary>
            Exclude = 0x00000010,
            /// <summary>If set, this specifies all devices whose top level collection is from the specified usUsagePage. Note that Usage must be zero. To exclude a particular top level collection, use Exclude.</summary>
            PageOnly = 0x00000020,
            /// <summary>If set, this prevents any devices specified by UsagePage or Usage from generating legacy messages. This is only for the mouse and keyboard.</summary>
            NoLegacy = 0x00000030,
            /// <summary>If set, this enables the caller to receive the input even when the caller is not in the foreground. Note that WindowHandle must be specified.</summary>
            InputSink = 0x00000100,
            /// <summary>If set, the mouse button click does not activate the other window.</summary>
            CaptureMouse = 0x00000200,
            /// <summary>If set, the application-defined keyboard device hotkeys are not handled. However, the system hotkeys; for example, ALT+TAB and CTRL+ALT+DEL, are still handled. By default, all keyboard hotkeys are handled. NoHotKeys can be specified even if NoLegacy is not specified and WindowHandle is NULL.</summary>
            NoHotKeys = 0x00000200,
            /// <summary>If set, application keys are handled.  NoLegacy must be specified.  Keyboard only.</summary>
            AppKeys = 0x00000400
        }

        /// <summary>Value type for raw input devices.</summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct RAWINPUTDEVICE
        {
            /// <summary>Top level collection Usage page for the raw input device.</summary>
            public HIDUsagePage UsagePage;
            /// <summary>Top level collection Usage for the raw input device. </summary>
            public HIDUsage Usage;
            /// <summary>Mode flag that specifies how to interpret the information provided by UsagePage and Usage.</summary>
            public RawInputDeviceFlags Flags;
            /// <summary>Handle to the target device. If NULL, it follows the keyboard focus.</summary>
            public IntPtr WindowHandle;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct RawData
        {
            [FieldOffset(0)]
            internal Rawmouse mouse;
            [FieldOffset(0)]
            internal Rawkeyboard keyboard;
            [FieldOffset(0)]
            internal Rawhid hid;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct InputData
        {
            public Rawinputheader header;           // 64 bit header size: 24  32 bit the header size: 16
            public RawData data;                    // Creating the rest in a struct allows the header size to align correctly for 32/64 bit
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Rawinputheader
        {
            public uint dwType;                     // Type of raw input (RIM_TYPEHID 2, RIM_TYPEKEYBOARD 1, RIM_TYPEMOUSE 0)
            public uint dwSize;                     // Size in bytes of the entire input packet of data. This includes RAWINPUT plus possible extra input reports in the RAWHID variable length array. 
            public IntPtr hDevice;                  // A handle to the device generating the raw input data. 
            public IntPtr wParam;                   // RIM_INPUT 0 if input occurred while application was in the foreground else RIM_INPUTSINK 1 if it was not.

            public override string ToString()
            {
                return string.Format("RawInputHeader\n dwType : {0}\n dwSize : {1}\n hDevice : {2}\n wParam : {3}", dwType, dwSize, hDevice, wParam);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct Rawhid
        {
            public uint dwSizHid;
            public uint dwCount;
            public byte bRawData;

            public override string ToString()
            {
                return string.Format("Rawhib\n dwSizeHid : {0}\n dwCount : {1}\n bRawData : {2}\n", dwSizHid, dwCount, bRawData);
            }
        }

        [StructLayout(LayoutKind.Explicit)]
        internal struct Rawmouse
        {
            [FieldOffset(0)]
            public ushort usFlags;
            [FieldOffset(4)]
            public uint ulButtons;
            [FieldOffset(4)]
            public ushort usButtonFlags;
            [FieldOffset(6)]
            public ushort usButtonData;
            [FieldOffset(8)]
            public uint ulRawButtons;
            [FieldOffset(12)]
            public int lLastX;
            [FieldOffset(16)]
            public int lLastY;
            [FieldOffset(20)]
            public uint ulExtraInformation;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct Rawkeyboard
        {
            public ushort Makecode;                 // Scan code from the key depression
            public ushort Flags;                    // One or more of RI_KEY_MAKE, RI_KEY_BREAK, RI_KEY_E0, RI_KEY_E1
            private readonly ushort Reserved;       // Always 0    
            public ushort VKey;                     // Virtual Key Code
            public uint Message;                    // Corresponding Windows message for exmaple (WM_KEYDOWN, WM_SYASKEYDOWN etc)
            public uint ExtraInformation;           // The device-specific addition information for the event (seems to always be zero for keyboards)

            public override string ToString()
            {
                return string.Format("Rawkeyboard\n Makecode: {0}\n Makecode(hex) : {0:X}\n Flags: {1}\n Reserved: {2}\n VKeyName: {3}\n Message: {4}\n ExtraInformation {5}\n",
                                                    Makecode, Flags, Reserved, VKey, Message, ExtraInformation);
            }
        }

        public enum DataCommand : uint
        {
            RID_HEADER = 0x10000005, // Get the header information from the RAWINPUT structure.
            RID_INPUT = 0x10000003   // Get the raw data from the RAWINPUT structure.
        }

        /// <summary>Function to register a raw input device.</summary>
        /// <param name="pRawInputDevices">Array of raw input devices.</param>
        /// <param name="uiNumDevices">Number of devices.</param>
        /// <param name="cbSize">Size of the RAWINPUTDEVICE structure.</param>
        /// <returns>TRUE if successful, FALSE if not.</returns>
        [DllImport("user32.dll")]
        public static extern bool RegisterRawInputDevices([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] RAWINPUTDEVICE[] pRawInputDevices, int uiNumDevices, int cbSize);

        [DllImport("User32.dll", SetLastError = true)]
        internal static extern int GetRawInputData(IntPtr hRawInput, DataCommand command, [Out] out InputData buffer, [In, Out] ref int size, int cbSizeHeader);

        [DllImport("User32.dll", SetLastError = true)]
        internal static extern int GetRawInputData(IntPtr hRawInput, DataCommand command, [Out] IntPtr pData, [In, Out] ref int size, int sizeHeader);


        #endregion

        #region [hooks]


        public const int WH_KEYBOARD_LL = 13;
        public const int WH_MOUSE_LL = 14;
        public const int WH_MOUSE = 7;

        public const int WM_KEYDOWN = 0x100;
        public const int WM_KEYUP = 0x101;
        public const int WM_SYSKEYDOWN = 0x104;
        public const int WM_SYSKEYUP = 0x105;

        public const int WM_MOUSEMOVE = 0x0200;
        public const int WM_RBUTTONDOWN = 0x0204;
        public const int WM_RBUTTONUP = 0x0205;


        [StructLayout(LayoutKind.Sequential)]
        public struct KeyboardHookStruct
        {
            public int vkCode;
            public int scanCode;
            public int flags;
            public int time;
            public int dwExtraInfo;
        }

        [DllImport("user32.dll")]
        public static extern IntPtr SetWindowsHookEx(int idHook, Delegate callback, IntPtr hInstance, uint threadId);

        [DllImport("user32.dll")]
        public static extern bool UnhookWindowsHookEx(IntPtr hInstance);

        [DllImport("user32.dll")]
        public static extern IntPtr CallNextHookEx(IntPtr idHook, int nCode, int wParam, IntPtr lParam);

        [DllImport("kernel32.dll")]
        public static extern IntPtr LoadLibrary(string lpFileName);

        public delegate IntPtr LowLevelKeyboardProc(int nCode, int wParam, ref KeyboardHookStruct lParam);

        public static IntPtr StructToPtr(object obj)
        {
            var ptr = Marshal.AllocHGlobal(Marshal.SizeOf(obj));
            Marshal.StructureToPtr(obj, ptr, false);
            return ptr;
        }


        #endregion


        #region [win event hook]

        public delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType,
            IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

        [DllImport("user32.dll")]
        public static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr
                hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess,
            uint idThread, uint dwFlags);

        [DllImport("user32.dll")]
        public static extern bool UnhookWinEvent(IntPtr hWinEventHook);
        
        public const uint WINEVENT_OUTOFCONTEXT = 0;
        
        #endregion

        public static ulong GetAvailableMemory()
        {
            var mem = new WindowsFunction.MEMORYSTATUSEX();
            WindowsFunction.GlobalMemoryStatusEx(mem);

            var memoryInMb = mem.ullTotalPhys / (1024 * 1024);
            return memoryInMb;
        }

    }
}