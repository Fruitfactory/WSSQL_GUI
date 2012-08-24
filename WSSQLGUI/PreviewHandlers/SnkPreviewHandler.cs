// Stephen Toub
// Coded and published in January 2007 issue of MSDN Magazine 
// http://msdn.microsoft.com/msdnmag/issues/07/01/PreviewHandlers/default.aspx

using System;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Text;
using C4F.DevKit.PreviewHandler.PreviewHandlerFramework;

namespace C4F.DevKit.PreviewHandler.PreviewHandlers
{
    [PreviewHandler("MSDN Magazine Strong Name Key Preview Handler", ".snk;.keys", "{D7CAD297-1D39-478d-8BCC-585D74B234DA}")]
    [ProgId("C4F.DevKit.PreviewHandler.PreviewHandlers.SnkPreviewHandler")]
    [Guid("2D6DAD69-F296-4be0-AB9E-6ED642AEF76B")]
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