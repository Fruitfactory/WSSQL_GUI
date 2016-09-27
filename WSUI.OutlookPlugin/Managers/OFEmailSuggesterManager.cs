using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Documents;
using System.Windows.Forms;
using OF.Core.Win32;
using OFOutlookPlugin.Hooks;
using OFOutlookPlugin.Interfaces;

namespace OFOutlookPlugin.Managers
{
    public class OFEmailSuggesterManager : IOFEmailSuggesterManager
    {
        #region [needs]

        private readonly List<int> _needIdList = new List<int>() { 4099, 4100, 4103 };
        private readonly IDictionary<int, List<int>> _needHwndCtrl = new Dictionary<int, List<int>>();

        private IntPtr _hook;
        private IntPtr _mainWindowHandle;

        private WindowsFunction.LowLevelKeyboardProc _callback = null;

        private readonly object LOOK = new object();

        #endregion

        #region [ctor]

        public OFEmailSuggesterManager(IntPtr mainWindowHandle)
        {
            //_callback = new WindowsFunction.LowLevelKeyboardProc(KeyboardHookProc);
            //using (var process = Process.GetCurrentProcess())
            //{
            //    using (var module = process.MainModule)
            //    {

            //        _hook = WindowsFunction.SetWindowsHookEx(WindowsFunction.WH_KEYBOARD_LL, _callback,IntPtr.Zero, 0);
            //    }
            //}

            //HookManager.KeyDown += HookManagerOnKeyDown;

            _mainWindowHandle = mainWindowHandle;
        }

        private void HookManagerOnKeyDown(object sender, KeyEventArgs keyEventArgs)
        {
            var hWnd = WindowsFunction.GetFocusedControl(_mainWindowHandle).ToInt32();
            foreach (var keyValuePair in _needHwndCtrl)
            {
                if (keyValuePair.Value.Contains(hWnd))
                {

                    switch ((Keys)keyEventArgs.KeyCode)
                    {
                        case Keys.Down:
                            System.Diagnostics.Debug.WriteLine("!!!!!! Down pressed...");
                            break;
                        case Keys.Escape:
                            System.Diagnostics.Debug.WriteLine("!!!!!! Escape pressed...");
                            break;
                    }
                }
            }
            System.Diagnostics.Debug.WriteLine("!!!! It's here");
        }

        #endregion
        

        public void SubscribeMailWindow()
        {
            var listWnd = new List<IntPtr>();
            WindowsFunction.GetAllObjectWithClass("rctrl_renwnd32", listWnd);
            foreach (var intPtr in listWnd)
            {
                if (_needHwndCtrl.ContainsKey(intPtr.ToInt32()))
                {
                    continue;
                }
                var listChilds = new List<IntPtr>();
                WindowsFunction.GetAllChildWindowsWithClass(intPtr, "RichEdit20WPT", listChilds);
                var listChildsInt = listChilds.Select(c => c.ToInt32()).ToList();
                var listRemove = new List<int>();
                foreach (var listChild in listChilds)
                {
                    var ctrlId = WindowsFunction.GetDlgCtrlID(listChild);
                    if (!_needIdList.Contains(ctrlId))
                    {
                        listRemove.Add(listChild.ToInt32());
                    }
                }
                listRemove.ForEach(c => listChildsInt.Remove(c));
                if (listChildsInt.Any())
                {
                    foreach (var listChild in listChildsInt)
                    {
                        System.Diagnostics.Debug.WriteLine(string.Format("{0} - {1}",intPtr,listChild));
                    }

                    _needHwndCtrl.Add(intPtr.ToInt32(), listChildsInt);
                }
            }
        }

        public void UnsubscribeMailWindow()
        {
            var listWnd = new List<IntPtr>();
            WindowsFunction.GetAllObjectWithClass("rctrl_renwnd32", listWnd);
            var listWndInt = listWnd.Select(w => w.ToInt32()).ToList();
            if (listWndInt.Any())
            {
                var listRemove = new List<KeyValuePair<int, List<int>>>();
                foreach (var pair in _needHwndCtrl)
                {
                    if (!listWndInt.Contains(pair.Key))
                    {
                        listRemove.Add(pair);
                    }
                }
                listRemove.ForEach(p => _needHwndCtrl.Remove(p.Key));
            }
            else
            {
                _needHwndCtrl.Clear();
            }
        }

        public void Dispose()
        {
            HookManager.KeyDown -= HookManagerOnKeyDown;
            //WindowsFunction.UnhookWindowsHookEx(_hook);
        }


        public void ProcessKeyDown(int Key)
        {
            var classStr = new StringBuilder(255);
            var wnd = WindowsFunction.GetForegroundWindow();
            WindowsFunction.GetClassName(wnd, classStr, 255);
            System.Diagnostics.Debug.WriteLine("!!!! " + classStr);
            var hWnd = WindowsFunction.GetFocus();
            classStr = new StringBuilder(255);
            WindowsFunction.GetClassName(hWnd, classStr, 255);
            System.Diagnostics.Debug.WriteLine("!!!! " + classStr);

            foreach (var keyValuePair in _needHwndCtrl)
            {
                if (keyValuePair.Value.Contains(hWnd.ToInt32()))
                {

                    switch ((Keys)Key)
                    {
                        case Keys.Down:
                            System.Diagnostics.Debug.WriteLine("!!!!!! Down pressed...");
                            break;
                        case Keys.Escape:
                            System.Diagnostics.Debug.WriteLine("!!!!!! Escape pressed...");
                            break;
                    }
                }
            }
            
        }

        private IntPtr KeyboardHookProc(int code, int wParam, ref WindowsFunction.KeyboardHookStruct lParam)
        {
            if (code >= 0 && wParam == WindowsFunction.WM_KEYDOWN)
            {
                var hWnd = WindowsFunction.GetFocus();
                foreach (var keyValuePair in _needHwndCtrl)
                {
                    if (keyValuePair.Value.Contains(hWnd.ToInt32()))
                    {
                        switch ((Keys)lParam.vkCode)
                        {
                            case Keys.Down:
                                System.Diagnostics.Debug.WriteLine("!!!!!! Down pressed...");
                                break;
                            case Keys.Escape:
                                System.Diagnostics.Debug.WriteLine("!!!!!! Escape pressed...");
                                break;
                        }
                    }
                }
            }
            System.Diagnostics.Debug.WriteLine("!!!! It's here");
            return WindowsFunction.CallNextHookEx(_hook, code, wParam, WindowsFunction.StructToPtr(lParam));
        }

    }
}