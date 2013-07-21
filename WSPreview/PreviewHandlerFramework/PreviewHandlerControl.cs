// Stephen Toub
// Coded and published in January 2007 issue of MSDN Magazine 
// http://msdn.microsoft.com/msdnmag/issues/07/01/PreviewHandlers/default.aspx

using System;
using System.IO;
using System.Windows.Forms;
using WSPreview.PreviewHandler.Controls.CsvControl;
using WSPreview.PreviewHandler.Service.Preview;

namespace WSPreview.PreviewHandler.PreviewHandlerFramework
{
    public abstract class PreviewHandlerControl : Control
    {
        protected PreviewHandlerControl()
        {
            
        }

        public virtual void Load(FileInfo file)
        {
            try
            {
                Control ctrl = GetPreviewControl();
                if (ctrl != null)
                {
                    if (ctrl is IPreviewControl)
                    {
                        (ctrl as IPreviewControl).LoadFile(file.FullName);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public virtual void Load(Stream stream)
        {
            try
            {
                Control ctrl = GetPreviewControl();
                if (ctrl != null)
                {
                    if (ctrl is IPreviewControl)
                    {
                        (ctrl as IPreviewControl).LoadFile(stream);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public virtual void Unload()
        {
            foreach (Control c in Controls)
            {
                if (!(c is IPreviewControl))
                {
                    c.Dispose();
                }
                else
                {
                    (c as IPreviewControl).Clear();
                }
            }
            Controls.Clear();
        }

        protected static string CreateTempPath(string extension)
        {
            return Path.GetTempPath() + Guid.NewGuid().ToString("N") + extension;
        }

        protected virtual Control GetPreviewControl()
        {
            ControlsKey key = GetControlsKey();
            Control ctrl = null;
            switch (key)
            {
                case ControlsKey.None:
                    ctrl = GetCustomerPreviewControl();
                    break;
                default:
                    IPreviewControl previewControl = HelperPreviewHandlers.Instance.GetPreviewControl(key);
                    if (previewControl != null)
                    {
                        ctrl = (Control)previewControl;
                    }
                    break;
            }
            if (ctrl != null)
            {
                ctrl.Dock = DockStyle.Fill;
                Controls.Add(ctrl);
            }
            return ctrl;
        }

        protected virtual ControlsKey GetControlsKey()
        {
            return ControlsKey.None;
        }

        protected virtual Control GetCustomerPreviewControl()
        {
            return null;
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
