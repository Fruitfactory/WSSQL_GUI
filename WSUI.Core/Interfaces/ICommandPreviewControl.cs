using System;
using WSUI.Core.EventArguments;

namespace WSUI.Core.Interfaces
{
    public interface ICommandPreviewControl
    {
        event EventHandler<WSUIPreviewCommandArgs> PreviewCommandExecuted;
    }
}