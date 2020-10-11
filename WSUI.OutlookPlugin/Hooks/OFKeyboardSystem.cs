using System;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Windows.Forms;
using Microsoft.Practices.Prism.Events;
using OF.Core.Win32;
using OF.Infrastructure.Events;
using OF.Infrastructure.Payloads;
using OFOutlookPlugin;
using OFOutlookPlugin.Hooks;

namespace SS.ShareScreen.Systems.Keyboard
{
    public class OFKeyboardSystem : OFBaseHookSystem
    {

        private static System.Windows.Forms.Keys LastKey;

        private readonly WindowsFunction.HookProc _proc = KeyboardHookProc;

        public OFKeyboardSystem(IEventAggregator eventAggregator)
            :base(eventAggregator)
        {
        }
        
        protected override Delegate GetCallback() => _proc;

        protected override int GetHookType() => WindowsFunction.WH_KEYBOARD;

        private static IntPtr KeyboardHookProc(int code, IntPtr wParam, IntPtr lParam)
        {
            if (code < 0)
                WindowsFunction.CallNextHookEx(((OFBaseHookSystem)GetHookSystem()).GetHookPtr(), code, wParam, lParam);

            bool ctrl = ((int)WindowsFunction.GetKeyState(162) & 256) == 256 || ((int)WindowsFunction.GetKeyState(163) & 256) == 256;
            bool shift = ((int)WindowsFunction.GetKeyState(160) & 256) == 256 || ((int)WindowsFunction.GetKeyState(161) & 256) == 256;
            bool alt = ((int)WindowsFunction.GetKeyState(164) & 256) == 256 || ((int)WindowsFunction.GetKeyState(165) & 256) == 256;

            if (code == 0)
            {
                long lp = Environment.Is64BitProcess ? lParam.ToInt64() : lParam.ToInt32(); 
                uint num = (uint)((lp & 1073741824) >> 30);
                if (num == 0)
                {
                    Keys key = (Keys)wParam.ToInt32();
                    GetHookSystem().HookEventAggregator.GetEvent<OFKeyDownEvent>().Publish(new OFKeyDownPayload(key, ctrl, shift, alt));
                }
            }

            return WindowsFunction.CallNextHookEx(((OFBaseHookSystem)GetHookSystem()).GetHookPtr(), code, wParam, lParam);
        }

    }
}