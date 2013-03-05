using WSUI.Infrastructure.Service.Enums;
using WSUI.Module.Commands;
using WSUI.Module.Core;
using WSUI.Module.Interface;

namespace WSUI.Module.Strategy
{
    public class EmailCommandStrategy : BaseCommandStrategy
    {
        public EmailCommandStrategy(IKindItem kindItem)
            :base(kindItem)
        {
            Type = TypeSearchItem.Email;
        }

        protected override void OnInit()
        {
            base.OnInit();
            _listCommand.Add(new OpenEmailCommand(_kindItem));
            _listCommand.Add(new ReplyCommand(_kindItem));
            _listCommand.Add(new ReplyAllCommand(_kindItem));
            _listCommand.Add(new ForwardCommand(_kindItem));
        }

    }
}
