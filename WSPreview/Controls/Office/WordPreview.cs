using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using OF.Core.Helpers;
using Word = Microsoft.Office.Interop.Word;
using Core = Microsoft.Office.Core;
using OFPreview.PreviewHandler.PInvoke;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace OFPreview.PreviewHandler.Controls.Office
{
	public class WordPreview : BaseOfficePreview
	{
		protected Word._Application _app;

		public override void CreateApp()
		{
            base.CreateApp();
            _app = new Word.Application();
            _app.Visible = false;
		}

		public override void UnloadApp()
		{
            if (_app != null)
            {
                _app.Quit();
                Marshal.FinalReleaseComObject(_app);
            }
            base.UnloadApp();
		}


		public override void LoadFile(string filename)
		{
		    try
		    {

		        object fileName = (object) filename;
		        object m = System.Reflection.Missing.Value;
		        object readOnly = (object) false;
		        Word.Document doc = _app.Documents.Open(ref fileName, ref m, ref readOnly,
		            ref m, ref m, ref m, ref m, ref m, ref m, ref m,
		            ref m, ref m, ref m, ref m, ref m, ref m);
		        Filename = TempFileManager.Instance.GenerateHtmlTempFileName(Guid.NewGuid());
		        object newFilename = (object) Filename;
		        object format = (object) Word.WdSaveFormat.wdFormatHTML;
		        doc.SaveAs(ref newFilename, ref format,
		            ref m, ref m, ref m, ref m, ref m, ref m, ref m,
		            ref m, ref m, ref m, ref m, ref m, ref m, ref m);
		    }
		    catch (Exception ex)
		    {
		        OF.Core.Logger.OFLogger.Instance.LogError(ex.Message);
		    }
		}
    }
}
