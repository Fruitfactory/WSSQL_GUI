using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Documents;
using System.Windows.Forms;
using OF.Core.Win32;
using OFOutlookPlugin.Interfaces;

namespace OFOutlookPlugin.Managers
{
    public class OFEmailSuggesterManager : IOFEmailSuggesterManager
    {
        #region [needs]

        private readonly List<int> _needIdList = new List<int>() { 4099, 4100, 4103 };
        private readonly IDictionary<IntPtr, List<IntPtr>> _needHwndCtrl = new Dictionary<IntPtr, List<IntPtr>>();

        private IntPtr _hook;
        private IntPtr _mainWindowHandle;

        private WindowsFunction.LowLevelKeyboardProc _callback = null;

        private readonly object LOOK = new object();

        #endregion

        #region [ctor]

        public OFEmailSuggesterManager(IntPtr mainWindowHandle)
        {
            _callback = new WindowsFunction.LowLevelKeyboardProc(KeyboardHookProc);
            _hook = WindowsFunction.SetWindowsHookEx(WindowsFunction.WH_KEYBOARD_LL, _callback, IntPtr.Zero,0);
            _mainWindowHandle = mainWindowHandle;
        }

        #endregion
        

        public void SubscribeMailWindow()
        {
            var listWnd = new List<IntPtr>();
            WindowsFunction.GetAllObjectWithClass("rctrl_renwnd32", listWnd);
            foreach (var intPtr in listWnd)
            {
                if (_needHwndCtrl.ContainsKey(intPtr))
                {
                    continue;
                }
                var listChilds = new List<IntPtr>();
                WindowsFunction.GetAllChildWindowsWithClass(intPtr, "RichEdit20WPT", listChilds);
                var listRemove = new List<IntPtr>();
                foreach (var listChild in listChilds)
                {
                    var ctrlId = WindowsFunction.GetDlgCtrlID(listChild);
                    if (!_needIdList.Contains(ctrlId))
                    {
                        listRemove.Add(listChild);
                    }
                }
                listRemove.ForEach(c => listChilds.Remove(c));
                if (listChilds.Any())
                {
                    _needHwndCtrl.Add(intPtr,listChilds);
                }
            }
        }

        public void UnsubscribeMailWindow()
        {
            var listWnd = new List<IntPtr>();
            WindowsFunction.GetAllObjectWithClass("rctrl_renwnd32", listWnd);

            if (listWnd.Any())
            {
                var listRemove = new List<KeyValuePair<IntPtr, List<IntPtr>>>();
                foreach (var pair in _needHwndCtrl)
                {
                    if (!listWnd.Contains(pair.Key))
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
            WindowsFunction.UnhookWindowsHookEx(_hook);
        }


        private IntPtr KeyboardHookProc(int code, int wParam, ref WindowsFunction.KeyboardHookStruct lParam)
        {
            if (code >= 0 && wParam == WindowsFunction.WM_KEYDOWN)
            {
                var hWnd = WindowsFunction.GetFocusedControl(_mainWindowHandle);
                foreach (var keyValuePair in _needHwndCtrl)
                {
                    if (keyValuePair.Value.Contains(hWnd))
                    {
                        System.Diagnostics.Debug.WriteLine("!!!! It's here");
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
            return WindowsFunction.CallNextHookEx(_hook, code, wParam, WindowsFunction.StructToPtr(lParam));
        }

    }
}