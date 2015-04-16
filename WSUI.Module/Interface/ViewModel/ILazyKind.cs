using System.Windows.Input;

namespace OF.Module.Interface.ViewModel
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

        bool IsVisibleByDefault { get; }
        ICommand ChooseCommand { get; }

    }
}
