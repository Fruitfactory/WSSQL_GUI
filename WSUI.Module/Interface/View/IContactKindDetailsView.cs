using OF.Core.Interfaces;
using OF.Module.Interface.ViewModel;

namespace OF.Module.Interface.View
{
    public interface IContactKindDetailsView<T> where T : ISearchObject
    {
        IContactKindDetailsViewModel<T> Model { get; set; }
    }
}