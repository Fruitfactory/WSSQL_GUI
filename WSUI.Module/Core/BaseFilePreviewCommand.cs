using OF.Core.Enums;
using OF.Module.Interface.ViewModel;

namespace OF.Module.Core
{
    public class BaseFilePreviewCommand : BasePreviewCommand
    {
        public BaseFilePreviewCommand(IMainViewModel mainViewModel) : base(mainViewModel)
        {
        }

        protected override bool OnCanExecute()
        {

            if ( MainViewModel != null && 
                
                (
                (MainViewModel.IsPreviewVisible && MainViewModel.Current != null && ((MainViewModel.Current.TypeItem & TypeSearchItem.FileAll) == MainViewModel.Current.TypeItem))
                ||
                (!MainViewModel.IsPreviewVisible && MainViewModel.CurrentTracked != null && ((MainViewModel.CurrentTracked.TypeItem & TypeSearchItem.FileAll) == MainViewModel.CurrentTracked.TypeItem))
                )
                )
                return true;
            return false;
        }

    }
}