using OF.Core.Enums;

namespace OF.Core.Interfaces
{
    public interface IOFIOutlookItemsReaderObserver
    {
        void UpdateStatus(PstReaderStatus newStatus);
    }
}