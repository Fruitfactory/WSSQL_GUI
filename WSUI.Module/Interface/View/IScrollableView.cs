using System.Windows.Input;

namespace OF.Module.Interface.View
{
    public interface IScrollableView
    {
        ICommand ScrollChangeCommand { get; }
    }

    public interface IScrollableViewExtended : IScrollableView
    {
        ICommand ScrollChangedCommand2 { get; }
    }

}
