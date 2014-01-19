using System.Windows.Input;

namespace WSUI.Module.Interface
{
    public interface IScrollableView
    {
        ICommand ScrollChangeCommand { get; }
    }
}
