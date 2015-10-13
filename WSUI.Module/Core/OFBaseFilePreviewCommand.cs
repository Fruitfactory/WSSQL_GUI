using OF.Core.Enums;
using OF.Module.Interface.ViewModel;

namespace OF.Module.Core
{
    public class OFBaseFilePreviewCommand : OFBasePreviewCommand
    {
        public OFBaseFilePreviewCommand(IMainViewModel mainViewModel) : base(mainViewModel)
        {
        }

        protected override bool OnCanExecute()
        {

            if ( MainViewModel != null && 
                
                (
                (MainViewModel.IsPreviewVisible && MainViewModel.Current != null && ((MainViewModel.Current.TypeItem & OFTypeSearchItem.FileAll) == MainViewModel.Current.TypeItem))
                ||
                (!MainViewModel.IsPreviewVisible && MainViewModel.CurrentTracked != null && ((MainViewModel.CurrentTracked.TypeItem & OFTypeSearchItem.FileAll) == MainViewModel.CurrentTracked.TypeItem))
                )
                )
                return true;
            return false;
        }

    }
}