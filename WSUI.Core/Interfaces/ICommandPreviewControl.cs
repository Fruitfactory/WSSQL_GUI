using System;
using OF.Core.EventArguments;

namespace OF.Core.Interfaces
{
    public interface ICommandPreviewControl
    {
        event EventHandler<OFPreviewCommandArgs> PreviewCommandExecuted;
    }
}