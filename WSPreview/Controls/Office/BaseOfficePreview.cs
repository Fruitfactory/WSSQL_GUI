using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace WSPreview.PreviewHandler.Controls.Office
{
	public abstract class BaseOfficePreview
	{
        protected CultureInfo _threadCulture;

		public virtual void CreateApp()
		{
			SaveCurrentCulture();
            SetEnUsCulture();
		}

		public virtual void UnloadApp()
		{
			RestoreCurrentCulture();
		}

		public virtual void LoadFile(string filename)
		{
			
		}

        public string Filename { get; set; }

	    protected void SetEnUsCulture()
	    {
	        Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
	    }

	    private void SaveCurrentCulture()
	    {
	        _threadCulture = Thread.CurrentThread.CurrentCulture;
	    }

	    private void RestoreCurrentCulture()
	    {
	        Thread.CurrentThread.CurrentCulture = _threadCulture;
	    }
	}
}
