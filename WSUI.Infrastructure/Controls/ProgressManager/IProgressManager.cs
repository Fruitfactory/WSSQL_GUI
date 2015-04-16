﻿
namespace OF.Infrastructure.Controls.ProgressManager
{
    public interface IProgressManager
    {
        void StartOperation(ProgressOperation operation);
        void StopOperation();
        void SetProgress(int value);
        bool InProgress { get; }
    }
}
