using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace OFPreview.PreviewHandler.Controls.Office.WebUtils
{
    [ComImport]
    [Guid("34A715A0-6587-11D0-924A-0020AFC7AC4D")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    [TypeLibType(TypeLibTypeFlags.FHidden)]
    public interface DWebBrowserEvents2
    {
        [PreserveSig]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [DispId(250)]
        void BeforeNavigate2([In] [MarshalAs(UnmanagedType.IDispatch)] object pDisp,
                             [In] [MarshalAs(UnmanagedType.Struct)] ref object URL,
                             [In] [MarshalAs(UnmanagedType.Struct)] ref object Flags,
                             [In] [MarshalAs(UnmanagedType.Struct)] ref object TargetFrameName,
                             [In] [MarshalAs(UnmanagedType.Struct)] ref object PostData,
                             [In] [MarshalAs(UnmanagedType.Struct)] ref object Headers,
                             [In] [Out] ref bool Cancel);
    }
}