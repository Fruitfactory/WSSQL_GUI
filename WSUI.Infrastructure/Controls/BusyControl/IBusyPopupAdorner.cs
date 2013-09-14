namespace WSUI.Infrastructure.Controls.BusyControl
{
    public interface IBusyPopupAdorner
    {
        bool IsBusy { get; set; }
        string Message { get; set; }
    }
}