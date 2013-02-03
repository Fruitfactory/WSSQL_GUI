using System.Windows;
namespace WSUI.Module.Service.Dialogs.Interfaces
{
    public interface IMessageModel
    {
        string Title { get; set; }
        string Message { get; set; }
        MessageBoxImage Icon { get; set; }
        MessageBoxButton Button { get; set; }
    }
}