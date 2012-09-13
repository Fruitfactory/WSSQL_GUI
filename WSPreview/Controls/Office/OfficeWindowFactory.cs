using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace C4F.DevKit.PreviewHandler.Controls.Office
{
	public class OfficeWindowFactory
	{
		public static BaseOfficeWindow CreatePreviewWindow(string filename)
		{
            string ext = Path.GetExtension(filename).ToLower();
            switch(ext)
            {
                case ".doc":
                case ".docx":
                    return new WordWindow();
                case ".xls":
                    return new ExcelWindow();
                default:
                    return null;
            }
		}
	}
}
