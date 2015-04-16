using OF.Core.Interfaces;
using OF.Module.Interface.ViewModel;

namespace OF.Module.Interface.View
{
    public interface IElasticSearchView
    {
        IElasticSearchViewModel Model { get; set; }
    }
}