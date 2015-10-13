using OF.Core.Enums;
using OF.Module.Commands;
using OF.Module.Core;
using OF.Module.Interface;
using OF.Module.Interface.ViewModel;

namespace OF.Module.Strategy
{
    public class FileCommandStrategy : OFBaseCommandStrategy
    {
        public FileCommandStrategy(IMainViewModel mainViewModel)
            :base(mainViewModel)
        {
            Type = OFTypeSearchItem.File | OFTypeSearchItem.Attachment;
        }

        protected override void OnInit()
        {
            base.OnInit();
            _listCommand.Add(new OFOpenPreviewCommad(_MainViewModel));
            _listCommand.Add(new OFEmailCommand(_MainViewModel));
            _listCommand.Add(new OFOpenFolderCommand(_MainViewModel));
        }
    }
}
