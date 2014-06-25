using System.Windows.Input;

namespace WSUI.Module.Interface
{
    public interface ILazyKind
    {
        IKindItem Kind { get; }
        string UIName { get; set; }
        string Icon{get;}
        string Data { get;}
        int ID { get; }
        bool Toggle { get; set; }
        void Initialize();
        ICommand ChooseCommand { get; }

    }
}
