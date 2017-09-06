using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Concurrency;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Automation;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Threading;
using AddinExpress.MSO;
using OF.Control;
using OF.Core.Data;
using OF.Core.Enums;
using OF.Core.Extensions;
using OF.Core.Logger;
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
        private OFWindowPositionManager _positionManager;

        private WindowsFunction.LowLevelKeyboardProc _callback = null;

        private readonly object LOOK = new object();

        private readonly char EmailSeparator = ';';

        private readonly string TemplateInsert = "{0}; ";

        private IDisposable _token;

        private readonly WindowsFunction.WinEventDelegate _proc;

        #endregion

        #region [ctor]

        public OFEmailSuggesterManager(IPluginBootStraper pluginBootStraper)
        {
            this.pluginBootStraper = pluginBootStraper;
            _proc = new WindowsFunction.WinEventDelegate(WinEventProc);
            _positionManager = new OFWindowPositionManager(_proc);
            _positionManager.Subscribe();
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
            if (_positionManager.IsNotNull())
            {
                _positionManager.Dispose();
            }
        }


        public void ProcessKeyDown(ADXKeyDownEventArgs Args)
        {
            var classStr = new StringBuilder(255);
            var hWnd = WindowsFunction.GetFocus();
            var ctrlId = WindowsFunction.GetDlgCtrlID(hWnd);
            WindowsFunction.GetClassName(hWnd, classStr, 255);

            Dispatcher.CurrentDispatcher.BeginInvoke((Action) (() =>
            {
                ProcessKeyPressing(Args,hWnd);

            }));
        }

        private void ProcessKeyPressing(ADXKeyDownEventArgs Args, IntPtr hWnd)
        {
            foreach (var keyValuePair in _needHwndCtrl)
            {
                if (keyValuePair.Value.Contains(hWnd.ToInt32()))
                {
                    var key = (Keys) Args.VirtualKey;
                    var text = WindowsFunction.GetRichEditText(hWnd);

                    if ((key >= Keys.A && key <= Keys.Z) || key == Keys.Back || key == Keys.Delete)
                    {
                        //var criteria = key != Keys.Back && key != Keys.Delete
                        //    ? text.Append(key).ToString().ToLowerCase()
                        //    : text.ToString().ToLowerCase();

                        var criteria = text.ToString();

                        if (criteria.IndexOf(EmailSeparator) > -1)
                        {
                            var arr = criteria.Split(EmailSeparator);
                            criteria = arr[arr.Length - 1];
                        }
                        if (_token.IsNotNull())
                        {
                            _token.Dispose();
                            _token = null;
                        }
                        _token = Scheduler.ThreadPool.Schedule(() =>
                        {
                            pluginBootStraper.PassAction(new OFAction(OFActionType.ShowSuggestEmail,
                                new Tuple<IntPtr, string>(hWnd, criteria.Trim())));
                        }, TimeSpan.FromMilliseconds(50));
                    }
                    else if (key == Keys.Escape)
                    {
                        pluginBootStraper.PassAction(new OFAction(OFActionType.HideSuggestEmail, null));
                    }
                    else
                    {
                        OFActionType actionType = OFActionType.None;
                        switch (key)
                        {
                            case Keys.Return:
                                actionType = OFActionType.SelectSuggestEmail;
                                Args.Handled = true;
                                break;
                            case Keys.Down:
                                actionType = OFActionType.DownSuggestEmail;
                                break;
                            case Keys.Up:
                                actionType = OFActionType.UpSuggestEmail;
                                break;
                        }
                        pluginBootStraper.PassAction(new OFAction(actionType, null));
                    }
                }
            }
        }

        private static IEnumerable<string> GetTextFromFocusedTextControl(IntPtr hWnd)
        {
            var focusedAutomation = AutomationElement.FromHandle(hWnd);
            var list = new List<string>();
            if (focusedAutomation.IsNotNull())
            {
                var childrens = focusedAutomation.FindAll(TreeScope.Children,
                    new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Button));
                if (childrens.Count > 0)
                {
                    foreach (AutomationElement automationElement in childrens)
                    {
                        list.Add(automationElement.Current.Name);
                    }
                }
            }
            return list;
        }

        public void SuggestedEmail(Tuple<IntPtr, string> data)
        {
            if (data.IsNull() || data.Item1.IsNull() || data.Item1 == IntPtr.Zero || string.IsNullOrEmpty(data.Item2))
            {
                return;
            }
            foreach (var keyValuePair in _needHwndCtrl)
            {
                if (keyValuePair.Value.Contains(data.Item1.ToInt32()))
                {
                    var text = GetTextFromFocusedTextControl(data.Item1);
                    var sb = WindowsFunction.GetRichEditText(data.Item1);
                    if (text.IsNotNull() && sb.IsNotNull())
                    {
                        var str = sb.ToString();
                        if (str.Contains(EmailSeparator))
                        {
                            var arr = str.Split(EmailSeparator);

                            for (int i = 0; i < text.Count(); i++)
                            {
                                if (i >= arr.Length)
                                    continue;
                                arr[i] = text.ElementAt(i);
                            }
                            if (!arr[arr.Length - 1].IsEmail())
                            {
                                arr[arr.Length - 1] = data.Item2;
                            }
                            str = String.Join(EmailSeparator.ToString() + ' ', arr);
                            str = string.Format(TemplateInsert, str);
                            WindowsFunction.SetRichEditText(data.Item1, str);
                            WindowsFunction.SendMessage(data.Item1.ToInt32(), (int)WindowsFunction.EM.SETSEL, str.Length,
                                new IntPtr(str.Length));

                        }
                        else
                        {

                            var email = string.Format(TemplateInsert, data.Item2);
                            WindowsFunction.SetRichEditText(data.Item1, email);
                            WindowsFunction.SendMessage(data.Item1.ToInt32(), (int)WindowsFunction.EM.SETSEL, email.Length,
                                new IntPtr(email.Length));

                        }
                    }
                }
            }
        }

        private void WinEventProc(IntPtr hWinEventHook, uint eventType,
            IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            // filter out non-HWND namechanges... (eg. items within a listbox)
            if (idObject != 0 || idChild != 0)
            {
                return;
            }
            if (_needHwndCtrl.ContainsKey(hwnd.ToInt32()))
            {
                OFLogger.Instance.LogDebug($"!!!!! Window was moved: {hwnd.ToInt32()}");

                pluginBootStraper.PassAction(new OFAction(OFActionType.HideSuggestEmail, null));
            }

        }

    }
}