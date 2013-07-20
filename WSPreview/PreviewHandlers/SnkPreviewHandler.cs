// Stephen Toub
// Coded and published in January 2007 issue of MSDN Magazine 
// http://msdn.microsoft.com/msdnmag/issues/07/01/PreviewHandlers/default.aspx

using System;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Text;
using WSPreview.PreviewHandler.PreviewHandlerFramework;

namespace WSPreview.PreviewHandler.PreviewHandlers
{
    [PreviewHandler("MSDN Magazine Strong Name Key Preview Handler", ".snk;.keys", "{27145BEA-45DC-4E09-BDC1-7847AEC8AE6B}")]
    [ProgId("WSPreview.PreviewHandler.PreviewHandlers.SnkPreviewHandler")]
    [Guid("00F574FF-7474-4FDF-B1E5-DA7D6F83BDC9")]
    [ClassInterface(ClassInterfaceType.None)]
    [ComVisible(true)]
    public sealed class SnkPreviewHandler : FileBasedPreviewHandler
    {
        protected override PreviewHandlerControl CreatePreviewHandlerControl()
        {
            return new SnkPreviewHandlerControl();
        }

        private sealed class SnkPreviewHandlerControl : StreamBasedPreviewHandlerControl
        {
            public override void Load(Stream stream)
            {
                StrongNameKeyPair snk = new StrongNameKeyPair((FileStream)stream);
                TextBox text = new TextBox();
                text.Dock = DockStyle.Fill;
                text.ReadOnly = true;
                text.Multiline = true;
                text.Text = "Public key:" + Environment.NewLine + ToHexString(snk.PublicKey);
                Controls.Add(text);
            }

            private static string ToHexString(byte[] bytes)
            {
                StringBuilder sb = new StringBuilder(bytes.Length * 2);
                for (int i = 0; i < bytes.Length; i++)
                {
                    int val = bytes[i];
                    sb.Append(GetHexValue(val >> 4));
                    sb.Append(GetHexValue(val & 0x0F));
                }
                return sb.ToString();
            }

            private static char GetHexValue(int val)
            {
                if (val < 10) return (char)((int)'0' + val);
                else if (val >= 10 && val <= 15) return (char)((int)'A' + (val - 10));
                else throw new ArgumentOutOfRangeException("val");
            }
        }
    }
}