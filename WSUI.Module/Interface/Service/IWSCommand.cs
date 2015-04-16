using System.Windows.Input;

namespace OF.Module.Interface.Service
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
