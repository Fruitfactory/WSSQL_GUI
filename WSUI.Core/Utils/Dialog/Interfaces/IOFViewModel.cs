using System.ComponentModel;
using System.Windows.Input;

namespace OF.Core.Utils.Dialog.Interfaces
{
    public interface IOFViewModel : IDataErrorInfo, INotifyPropertyChanged
    {
        string Title { get; }

        IOFView View { get; }

        ICommand OKCommand { get; }

        ICommand CancelCommand { get; }
    }
}