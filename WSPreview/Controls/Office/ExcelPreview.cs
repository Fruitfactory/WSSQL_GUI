using System;
using OF.Core.Helpers;
using Excel = Microsoft.Office.Interop.Excel;
using Core = Microsoft.Office.Core;
using System.Runtime.InteropServices;

namespace OFPreview.PreviewHandler.Controls.Office
{
	public class ExcelPreview : BaseOfficePreview
	{
		protected Excel.Application _app = null;
        
		public override void CreateApp()
		{
            base.CreateApp();
		    _app = new Excel.Application();
		    _app.Visible = false;
		}

		public override void UnloadApp()
		{
            
            if (_app != null)
            {
                _app.Quit();
                Marshal.ReleaseComObject(_app);
            }
            base.UnloadApp();
		}

	    public override void LoadFile(string filename)
	    {
	        try
	        {
                object mis = System.Reflection.Missing.Value;
                object oldafFileName = (object)filename;
                object readOnly = (object)false;
                Excel.Workbook wB = _app.Workbooks.Open(filename, mis, readOnly, mis, mis, mis, mis, mis, mis, mis,
                    mis, mis, mis, mis, mis);
                Filename = OFTempFileManager.Instance.GenerateHtmlTempFileName(Guid.NewGuid());
                object newFilename = (object)Filename;
                wB.SaveAs(newFilename, Microsoft.Office.Interop.Excel.XlFileFormat.xlHtml,
                    mis, mis, mis, mis, Excel.XlSaveAsAccessMode.xlExclusive, mis, mis, mis,
                    mis, mis);
	        }
	        catch (Exception ex)
	        {
                OF.Core.Logger.OFLogger.Instance.LogError(ex.Message);	            
	            
	        }
	    }
	}
}
