using WSUI.Core.Interfaces;
using WSUI.Module.Interface.ViewModel;

namespace WSUI.Module.Interface.Service
{
    internal interface INavigationService
    {
        INavigationView CurrentView { get; }
        void SetMainViewModel(IMainViewModel main);
        void ShowSelectedKind(object kindItem);
        void MoveToLeft(INavigationView newView);
        void MoveToRight();
        bool IsBackButtonVisible { get; }

        void MoveToFirstDataView();

    }
}