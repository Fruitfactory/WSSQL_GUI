using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Documents;
using System.Windows.Forms;
using OF.Control;
using OF.Core.Data;
using OF.Core.Enums;
using OF.Core.Extensions;
using OF.Core.Win32;
using OF.Infrastructure.Helpers.AttachedProperty;
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
        private IPluginBootStraper pluginBootStraper;

        private WindowsFunction.LowLevelKeyboardProc _callback = null;

        private readonly object LOOK = new object();

        #endregion

        #region [ctor]

        public OFEmailSuggesterManager(IPluginBootStraper pluginBootStraper)
        {
            this.pluginBootStraper = pluginBootStraper;
        }

        #endregion
        

        public void SubscribeMailWindow()
        {
            var listWnd = new List<IntPtr>();
            WindowsFunction.GetAllObjectWithClass("rctrl_renwnd32", listWnd);
            foreach (var intPtr in listWnd)
            {
                var listChilds = new List<IntPtr>();
                WindowsFunction.GetAllChildWindowsWithClass(intPtr, "RichEdit20WPT", listChilds);
                var listControls = new List<int>();
                foreach (var listChild in listChilds)
                {
                    var ctrlId = WindowsFunction.GetDlgCtrlID(listChild);
                    if (_needIdList.Contains(ctrlId))
                    {
                        listControls.Add(listChild.ToInt32());
                    }
                }
                if (listControls.Any())
                {
                    //foreach (var listChild in listControls)
                    //{
                    //    System.Diagnostics.Debug.WriteLine(string.Format("{0} - {1}",intPtr,listChild));
                    //}
                    if (_needHwndCtrl.ContainsKey(intPtr.ToInt32()))
                    {
                        var temp = _needHwndCtrl[intPtr.ToInt32()];
                        temp.AddRange(listControls);
                        _needHwndCtrl[intPtr.ToInt32()] = temp.Distinct().ToList();
                    }
                    else
                        _needHwndCtrl.Add(intPtr.ToInt32(), listControls);
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
        }


        public void ProcessKeyDown(int Key)
        {
            var classStr = new StringBuilder(255);
            //var wnd = WindowsFunction.GetForegroundWindow();
            //WindowsFunction.GetClassName(wnd, classStr, 255);
            //System.Diagnostics.Debug.WriteLine("!!!! " + classStr + " HWND: " + wnd.ToInt32());
            var hWnd = WindowsFunction.GetFocus();
            //classStr = new StringBuilder(255);
            var ctrlId = WindowsFunction.GetDlgCtrlID(hWnd);
            WindowsFunction.GetClassName(hWnd, classStr, 255);
            //System.Diagnostics.Debug.WriteLine("!!!! " + classStr + " HWND: " + hWnd.ToInt32() + " CtrlID: " + ctrlId);
            

            foreach (var keyValuePair in _needHwndCtrl)
            {
                if (keyValuePair.Value.Contains(hWnd.ToInt32()))
                {
                    var key = (Keys) Key;
                    var text = WindowsFunction.GetRichEditText(hWnd);
                    if (key >= Keys.A && key <= Keys.Z)
                    {
                        var criteria = text.Append(key).ToString().ToLowerCase();
                        if (criteria.Length > 2)
                        {
                            System.Diagnostics.Debug.WriteLine("!!!! TEXT: " + text);
                            pluginBootStraper.PassAction(new OFAction(OFActionType.ShowSuggestEmail, new Tuple<IntPtr, string>(hWnd, text.ToString())));
                        }
                    }
                    else if (key == Keys.Down)
                    {
                        pluginBootStraper.PassAction(new OFAction(OFActionType.ShowSuggestEmail, new Tuple<IntPtr, string>(hWnd, text.ToString())));
                    }
                    else if (key == Keys.Escape)
                    {
                        pluginBootStraper.PassAction(new OFAction(OFActionType.HideSuggestEmail, null));
                    }
                }
            }
            
        }
    }
}