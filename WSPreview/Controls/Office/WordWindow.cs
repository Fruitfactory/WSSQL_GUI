using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Word = Microsoft.Office.Interop.Word;
using Core = Microsoft.Office.Core;
using C4F.DevKit.PreviewHandler.PInvoke;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace C4F.DevKit.PreviewHandler.Controls.Office
{
	public class WordWindow : BaseOfficeWindow
	{
		protected Word._Application _app;
        protected Word._Document _doc;
		protected const string ClassWordWindowName = "OpusApp";

		public override void CreateApp()
		{
            

            _app = new Word.Application();
            _app.Visible = false;
            
		}

		public override void UnloadApp()
		{
            if (_app != null)
            {
                var list = Process.GetProcesses().Where(p => p.ProcessName.ToUpper().StartsWith("WINWORD")).ToList();
                if (list != null && list.Count > 0)
                {
                    list[0].Kill();
                }
                Marshal.FinalReleaseComObject(_doc);
                Marshal.FinalReleaseComObject(_app);
                _doc = null;
                _app = null;
            }
            base.UnloadApp();
		}

		public override void SetParentControl(UserControl parent)
		{
            _childHandle = WindowAPI.FindWindow(ClassWordWindowName, null);
            base.SetParentControl(parent);
		}

		protected override void SaveOfficeSettings()
		{
            _app.CommandBars.AdaptiveMenus = false;
            var list = _app.CommandBars.OfType<Core.CommandBar>().ToList();
            foreach (var cmd in list)
            {
                try
                {
                    _dictSettings[cmd.Name] = cmd.Enabled;
                    cmd.Enabled = false;
                }
                catch { }
            }
            
		}

		public override void RestoreOfficeSettings()
		{

            foreach (var cmd in _app.CommandBars.OfType<Core.CommandBar>())
            {
                if (_dictSettings.ContainsKey(cmd.Name))
                {
                    try
                    {
                        cmd.Enabled = _dictSettings[cmd.Name];
                    }
                    catch { }
                }
            }
            WindowAPI.SetParent(_childHandle, IntPtr.Zero);
		}

		public override void LoadFile(string filename)
		{
            if (_doc != null)
            {
                _doc.Close();
                Marshal.ReleaseComObject(_doc);
                _doc = null;
            }
            object oTrue = true;
            _doc = _app.Documents.Open(filename,Type.Missing,ref oTrue);
            _doc.ActiveWindow.WindowState = Microsoft.Office.Interop.Word.WdWindowState.wdWindowStateMaximize;
            _doc.Activate();
            _app.ActiveWindow.DisplayRightRuler = false;
            _app.ActiveWindow.DisplayScreenTips = false;
            _app.ActiveWindow.DisplayVerticalRuler = false;
            _app.ActiveWindow.ActivePane.DisplayRulers = false;
            _app.ActiveWindow.ActivePane.View.Type = Microsoft.Office.Interop.Word.WdViewType.wdWebView;
            SaveOfficeSettings();
            _app.Visible = true;
        }

    }
}
