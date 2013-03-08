using System;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Globalization;
using C4F.DevKit.PreviewHandler.PreviewHandlerFramework;

namespace C4F.DevKit.PreviewHandler.PreviewHandlers
{
    [PreviewHandler("WSSQL TXT Preview Handler", ".txt;.csv", "{24B7E73C-C49F-488A-86AD-2FA2E3232E06}")]
    [ProgId("C4F.DevKit.PreviewHandler.PreviewHandlers.TxtPreviewHandler")]
    [Guid("D12FC8ED-130D-4D85-9EA0-59AD9C843F63")]
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
                    txtBox.ScrollBars = ScrollBars.Both;
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
