using System;
using System.Windows.Forms;
using Microsoft.Practices.Prism.Events;
using OF.Core.Win32;
using OFOutlookPlugin.Hooks;

namespace SS.ShareScreen.Systems.Keyboard
{
    public class OFKeyboardSystem : OFBaseHookSystem
    {

        private static System.Windows.Forms.Keys LastKey;

        const int WH_KEYBOARD_LL = 13; 
        const int WM_KEYDOWN = 0x100; 

        private readonly WindowsFunction.LowLevelKeyboardProc _proc = KeyboardHookProc;

        public OFKeyboardSystem(IEventAggregator eventAggregator)
            :base(eventAggregator)
        {
        }
        
        protected override Delegate GetCallback() => _proc;

        protected override int GetHookType() => WH_KEYBOARD_LL;

        private static IntPtr KeyboardHookProc(int code, int wParam, ref WindowsFunction.KeyboardHookStruct lParam)
        {
            if (code >= 0 && (wParam == WindowsFunction.WM_KEYDOWN || wParam == WindowsFunction.WM_SYSKEYDOWN))
            {
                var key = (System.Windows.Forms.Keys)Enum.Parse(typeof (System.Windows.Forms.Keys), lParam.vkCode.ToString());
            }
            return WindowsFunction.CallNextHookEx( ((OFBaseHookSystem)GetHookSystem()).GetHookPtr() , code, (int)wParam, WindowsFunction.StructToPtr(lParam));
        }
    }
}