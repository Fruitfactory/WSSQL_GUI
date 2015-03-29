using WSUI.Core.Interfaces;
using WSUI.Module.Interface.ViewModel;

namespace WSUI.Module.Interface.View
{
    public interface IElasticSearchView
    {
        IElasticSearchViewModel Model { get; set; }
    }
}