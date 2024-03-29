﻿
namespace OF.Infrastructure.Controls.ProgressManager
{
    public enum ProgressFormCommand
    {
        None,
        Settings,
        Activate,
        Progress
    }    

    public interface IProgressForm
    {
        void CloseExt();
        void ProcessCommand(ProgressFormCommand cmd, object arg);
        int Width { get; set; }
        int Height { get; set; }
    }
}
