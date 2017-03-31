namespace OF.Core.Interfaces
{
    public interface IUserActivityTracker
    {
        void Start(IOutlookItemsReader reader);
        void Stop();
    }
}