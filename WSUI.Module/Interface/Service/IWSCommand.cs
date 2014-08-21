using System.Windows.Input;

namespace WSUI.Module.Interface.Service
{
    public interface IWSCommand : ICommand
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
