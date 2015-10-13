
namespace OF.Infrastructure.Controls.ProgressManager
{
    public interface IProgressManager
    {
        void StartOperation(OFProgressOperation operation);
        void StopOperation();
        void SetProgress(int value);
        bool InProgress { get; }
    }
}
