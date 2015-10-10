using System.Collections.ObjectModel;
using OF.Core.Enums;

namespace OF.Module.Interface.Service
{
    public interface ICommandStrategy
    {
        void Init();

        ObservableCollection<IOFCommand> Commands
        {
            get;
        }

        TypeSearchItem Type
        {
            get;
        }
    }
}
