using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace OFPreview.PreviewHandler.Controls.Office
{
	public class OfficePreviewFactory
	{
		public static BaseOfficePreview CreatePreviewWindow(string filename)
		{
            string ext = Path.GetExtension(filename).ToLower();
            switch(ext)
            {
                case ".doc":
                case ".docx":
                    return new WordPreview();
                case ".xls":
                case ".xlsx":
                    return new ExcelPreview();
                default:
                    return null;
            }
		}
	}
}
