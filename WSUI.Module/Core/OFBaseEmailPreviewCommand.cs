using OF.Core.Data;
using OF.Core.Enums;
using OF.Module.Interface.ViewModel;

namespace OF.Module.Core
{
    public class OFBaseEmailPreviewCommand : OFBasePreviewCommand
    {
        public OFBaseEmailPreviewCommand(IMainViewModel mainViewModel) : base(mainViewModel)
        {
        }

        protected override bool OnCanExecute()
        {
            if (MainViewModel != null &&
                (
                (MainViewModel.IsPreviewVisible && MainViewModel.Current != null && (MainViewModel.Current.TypeItem & OFTypeSearchItem.Email) == OFTypeSearchItem.Email)
                ||
                (!MainViewModel.IsPreviewVisible && MainViewModel.CurrentTracked != null && (MainViewModel.CurrentTracked.TypeItem & OFTypeSearchItem.Email) == OFTypeSearchItem.Email)
                )
                )
                return true;
            return false;
        }


    }
}