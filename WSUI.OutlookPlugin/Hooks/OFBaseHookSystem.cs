using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using Microsoft.Practices.Prism.Events;
using OF.Core.Win32;
using OFOutlookPlugin.Interfaces;

namespace OFOutlookPlugin.Hooks
{
    public abstract class OFBaseHookSystem : IOFHookSystem
    {
        protected static IntPtr _hhook = IntPtr.Zero;

        private static OFBaseHookSystem _this;
        private IEventAggregator _eventAggregator;


        protected OFBaseHookSystem(IEventAggregator eventAggregator)
        {
            _this = this;
            _eventAggregator = eventAggregator;
        }


        public virtual void StartSystem()
        {
            SetHook();
        }

        public virtual void StopSystem()
        {
            ReleaseHook();
        }

        private void SetHook()
        {
            //IntPtr ptrUser = SSWindowsFunctions.LoadLibrary("User32");
            using (ProcessModule module = Process.GetCurrentProcess().MainModule)
            {
                _hhook = WindowsFunction.SetWindowsHookEx(GetHookType(), GetCallback(), WindowsFunction.GetModuleHandle(module.ModuleName), 0);
            }
        }

        private void ReleaseHook()
        {
            Contract.Requires(_hhook != IntPtr.Zero);
            WindowsFunction.UnhookWindowsHookEx(_hhook);
        }

        public IntPtr GetHookPtr() => _hhook;

        protected abstract Delegate GetCallback();

        protected abstract int GetHookType();

        protected static OFBaseHookSystem GetHookSystem() => _this;

        protected IEventAggregator EventAggregator => _eventAggregator;

    }
}