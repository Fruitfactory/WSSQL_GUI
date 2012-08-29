using System;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Globalization;
using C4F.DevKit.PreviewHandler.PreviewHandlerFramework;

namespace C4F.DevKit.PreviewHandler.PreviewHandlers
{
    [PreviewHandler("WSSQL TXT Preview Handler", ".txt", "{FE6E2141-FF49-4AD7-9B63-C5024F86CE9D}")]
    [ProgId("C4F.DevKit.PreviewHandler.PreviewHandlers.TxtPreviewHandler")]
    [Guid("F5335413-19DB-4C39-90C4-D45680621B76")]
    [ClassInterface(ClassInterfaceType.None)]
    [ComVisible(true)]
    public sealed class TxtPreviewHandler : FileBasedPreviewHandler
    {
        protected override PreviewHandlerControl CreatePreviewHandlerControl()
        {
            return new TxtPreviewHandlerControl();
        }

        private sealed class TxtPreviewHandlerControl : FileBasedPreviewHandlerControl
        {

            public override void Load(FileInfo file)
            {
                StreamReader reader = null;
                try
                {
                    reader = File.OpenText(file.FullName);
                    TextBox txtBox = new TextBox();
                    txtBox.Multiline = true;
                    txtBox.Dock = DockStyle.Fill;
                    txtBox.Text = reader.ReadToEnd();
                    Controls.Add(txtBox);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error Rendering Preview - this handler requires Txt Preview", ex);
                }
                finally
                {
                    if (reader != null)
                        reader.Close();
                }
            }
        }
    }
}
