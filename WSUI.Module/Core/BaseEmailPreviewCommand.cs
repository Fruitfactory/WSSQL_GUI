using OF.Core.Data;
using OF.Core.Enums;
using OF.Module.Interface.ViewModel;

namespace OF.Module.Core
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