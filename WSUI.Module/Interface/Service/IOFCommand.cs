using System.Windows.Input;

namespace OF.Module.Interface.Service
{
    public interface IOFCommand : ICommand
    {
        string Icon
        {
            get;
        }

        string Caption
        {
            get;
        }

        string Tooltip { get; }
    }
}
