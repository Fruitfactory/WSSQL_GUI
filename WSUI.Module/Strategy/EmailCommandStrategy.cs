using WSUI.Core.Enums;
using WSUI.Module.Commands;
using WSUI.Module.Core;
using WSUI.Module.Interface;
using WSUI.Module.Interface.ViewModel;

namespace WSUI.Module.Strategy
{
    public class EmailCommandStrategy : BaseCommandStrategy
    {
        public EmailCommandStrategy(IMainViewModel mainViewModel)
            :base(mainViewModel)
        {
            Type = TypeSearchItem.Email;
        }

        protected override void OnInit()
        {
            base.OnInit();
            _listCommand.Add(new OpenEmailCommand(_MainViewModel));
            _listCommand.Add(new ReplyCommand(_MainViewModel));
            _listCommand.Add(new ReplyAllCommand(_MainViewModel));
            _listCommand.Add(new ForwardCommand(_MainViewModel));
        }

    }
}
