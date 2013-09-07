using System.Collections.ObjectModel;
using WSUI.Core.Enums;

namespace WSUI.Module.Interface
{
    public interface ICommandStrategy
    {
        void Init();

        ObservableCollection<IWSCommand> Commands
        {
            get;
        }

        TypeSearchItem Type
        {
            get;
        }
    }
}
