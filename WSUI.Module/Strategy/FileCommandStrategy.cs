using WSUI.Core.Enums;
using WSUI.Module.Commands;
using WSUI.Module.Core;
using WSUI.Module.Interface;

namespace WSUI.Module.Strategy
{
    public class FileCommandStrategy : BaseCommandStrategy
    {
        public FileCommandStrategy(IKindItem kindItem)
            :base(kindItem)
        {
            Type = TypeSearchItem.File | TypeSearchItem.Attachment;
        }

        protected override void OnInit()
        {
            base.OnInit();
            _listCommand.Add(new OpenPreviewCommad(_kindItem));
            _listCommand.Add(new EmailCommand(_kindItem));
        }
    }
}
