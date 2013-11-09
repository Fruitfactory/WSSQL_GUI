using System;

namespace WSUI.Core.Enums
{
    [Flags]
    public enum ActivationState
    {
        None = 0,
        Activated,
        NonActivated,
        Error
    }
}