using System.Windows.Forms;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace OFPreview.PreviewHandler.Controls.Office.WebUtils
{
    public partial class ExtWebBrowser
    {
        class ExtWebBrowserEventHelper : StandardOleMarshalObject, DWebBrowserEvents2
        {
            private readonly ExtWebBrowser _parent;

            public ExtWebBrowserEventHelper(ExtWebBrowser parent)
            {
                _parent = parent;
            }

            public void BeforeNavigate2(object pDisp, ref object URL, ref object Flags, ref object TargetFrameName, ref object PostData, ref object Headers, ref bool Cancel)
            {
                if (!ReferenceEquals(_parent, null))
                {
                    _parent.OnBeforeNavigate2(pDisp,
                        ref URL,
                        ref Flags,
                        ref TargetFrameName,
                        ref PostData,
                        ref Headers,
                        ref Cancel
                        );
                }
            }
        }    
    }
    
}