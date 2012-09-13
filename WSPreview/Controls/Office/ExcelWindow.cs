using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;
using Core = Microsoft.Office.Core;
using System.Runtime.InteropServices;
using C4F.DevKit.PreviewHandler.PInvoke;

namespace C4F.DevKit.PreviewHandler.Controls.Office
{
	public class ExcelWindow : BaseOfficeWindow
	{
		protected Excel.Application _app = null;
        protected Excel.Workbook _book = null;
		protected const string ClassExcelWindowName = "XLMAIN";

		public override void CreateApp()
		{
            _app = new Excel.Application();
            _app.Visible = false;
		}

		public override void UnloadApp()
		{
            if (_app != null)
            {
                _app.Visible = false;
                _book.Close();
                _app.Quit();
                Marshal.ReleaseComObject(_book);
                Marshal.ReleaseComObject(_app);
                _app = null;
                _book = null;
            }
		}

		public override void SetParentControl(UserControl parent)
		{
            _childHandle = WindowAPI.FindWindow(ClassExcelWindowName, null);
            base.SetParentControl(parent);
		}

		protected override void SaveOfficeSettings()
		{
            _app.CommandBars.AdaptiveMenus = false;
            _app.CommandBars.OfType<Core.CommandBar>().ToList().ForEach(cmd =>
            {
                try
                {
                    _dictSettings.Add(cmd.Name, cmd.Visible);
                    cmd.Visible = false;
                }
                catch{}
            });
		}

		public override void RestoreOfficeSettings()
		{
            foreach (var cmd in _app.CommandBars.OfType<Core.CommandBar>())
            {
                try
                {
                    if (_dictSettings.ContainsKey(cmd.Name))
                        cmd.Visible = _dictSettings[cmd.Name];
                }
                catch { }
            }
		}

		public override void LoadFile(string filename)
		{
            _book = _app.Workbooks.Open(filename);
            SaveOfficeSettings();
            _book.Activate();
            _app.Visible = true;
		}
	}
}
