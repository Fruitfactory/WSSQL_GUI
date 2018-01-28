using System;
using OF.Core.Win32;

namespace OFOutlookPlugin.Managers
{
    internal class OFWindowPositionManager : IDisposable
    {
        private WindowsFunction.WinEventDelegate _callback;
        private const uint EVENT_OBJECT_LOCATIONCHANGE = 0x800B;
        private const uint EVENT_SYSTEM_MOVESIZESTART = 0x000A;
        private IntPtr _hhook;

        public OFWindowPositionManager(WindowsFunction.WinEventDelegate callback )
        {
            _callback = callback;
        }

        internal void Subscribe()
        {
            _hhook = WindowsFunction.SetWinEventHook(EVENT_SYSTEM_MOVESIZESTART, EVENT_SYSTEM_MOVESIZESTART,
                IntPtr.Zero, _callback, 0, 0, WindowsFunction.WINEVENT_OUTOFCONTEXT);
        }
        
        public void Dispose()
        {
            WindowsFunction.UnhookWinEvent(_hhook);
        }
    }
}