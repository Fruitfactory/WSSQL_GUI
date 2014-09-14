using System.ComponentModel;
using System.Windows.Input;

namespace WSUI.Core.Utils.Dialog.Interfaces
{
    public interface IWSUIViewModel : IDataErrorInfo, INotifyPropertyChanged
    {
        string Title { get; }

        IWSUIView View { get; }

        ICommand OKCommand { get; }

        ICommand CancelCommand { get; }
    }
}