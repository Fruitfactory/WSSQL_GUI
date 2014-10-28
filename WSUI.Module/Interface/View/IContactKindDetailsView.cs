using WSUI.Core.Interfaces;
using WSUI.Module.Interface.ViewModel;

namespace WSUI.Module.Interface.View
{
    public interface IContactKindDetailsView<T> where T : ISearchObject
    {
        IContactKindDetailsViewModel<T> Model { get; set; }
    }
}