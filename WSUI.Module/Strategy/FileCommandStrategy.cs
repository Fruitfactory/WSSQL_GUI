using WSUI.Core.Enums;
using WSUI.Module.Commands;
using WSUI.Module.Core;
using WSUI.Module.Interface;
using WSUI.Module.Interface.ViewModel;

namespace WSUI.Module.Strategy
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
