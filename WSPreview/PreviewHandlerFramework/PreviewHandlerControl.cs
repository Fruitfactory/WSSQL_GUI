// Stephen Toub
// Coded and published in January 2007 issue of MSDN Magazine 
// http://msdn.microsoft.com/msdnmag/issues/07/01/PreviewHandlers/default.aspx

using System;
using System.IO;
using System.Windows.Forms;

namespace C4F.DevKit.PreviewHandler.PreviewHandlerFramework
{
    public abstract class PreviewHandlerControl : Control
    {
        protected PreviewHandlerControl()
        {
            
        }

        public abstract void Load(FileInfo file);
        public abstract void Load(Stream stream);

        public virtual void Unload()
        {
            foreach (Control c in Controls) c.Dispose();
            Controls.Clear();
        }

        protected static string CreateTempPath(string extension)
        {
            return Path.GetTempPath() + Guid.NewGuid().ToString("N") + extension;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (Controls.Count > 0)
            {
                Controls[0].Bounds = this.ClientRectangle;
            }
        }

    }
}
