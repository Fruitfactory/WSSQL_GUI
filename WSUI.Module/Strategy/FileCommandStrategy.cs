using OF.Core.Enums;
using OF.Module.Commands;
using OF.Module.Core;
using OF.Module.Interface;
using OF.Module.Interface.ViewModel;

namespace OF.Module.Strategy
{
    public class FileCommandStrategy : BaseCommandStrategy
    {
        public FileCommandStrategy(IMainViewModel mainViewModel)
            :base(mainViewModel)
        {
            Type = TypeSearchItem.File | TypeSearchItem.Attachment;
        }

        protected override void OnInit()
        {
            base.OnInit();
            _listCommand.Add(new OpenPreviewCommad(_MainViewModel));
            _listCommand.Add(new EmailCommand(_MainViewModel));
            _listCommand.Add(new OpenFolderCommand(_MainViewModel));
        }
    }
}
