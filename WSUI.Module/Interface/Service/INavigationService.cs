using OF.Core.Interfaces;
using OF.Module.Interface.View;
using OF.Module.Interface.ViewModel;

namespace OF.Module.Interface.Service
{
    internal interface INavigationService
    {
        INavigationView CurrentView { get; }
        void SetMainViewModel(IMainViewModel main);
        void ShowSelectedKind(object kindItem);
        void MoveToLeft(INavigationView newView,bool useTransaction);
        void MoveToRight();
        bool IsBackButtonVisible { get; }
        bool IsPreviewVisible { get; }
        IPreviewView PreviewView { get; }
        bool IsContactDetailsVisible { get; }
        IContactDetailsViewModel ContactDetailsViewModel { get; }
        void MoveToFirstDataView(bool useTransaction);

    }
}