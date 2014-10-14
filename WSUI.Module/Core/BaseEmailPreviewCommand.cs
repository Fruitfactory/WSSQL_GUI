using WSUI.Core.Data;
using WSUI.Core.Enums;
using WSUI.Module.Interface.ViewModel;

namespace WSUI.Module.Core
{
    public class BaseEmailPreviewCommand : BasePreviewCommand
    {
        public BaseEmailPreviewCommand(IMainViewModel mainViewModel) : base(mainViewModel)
        {
        }

        protected override bool OnCanExecute()
        {
            if (MainViewModel != null &&
                (
                (MainViewModel.IsPreviewVisible && MainViewModel.Current != null && (MainViewModel.Current.TypeItem & TypeSearchItem.Email) == TypeSearchItem.Email)
                ||
                (!MainViewModel.IsPreviewVisible && MainViewModel.CurrentTracked != null && (MainViewModel.CurrentTracked.TypeItem & TypeSearchItem.Email) == TypeSearchItem.Email)
                )
                )
                return true;
            return false;
        }


    }
}