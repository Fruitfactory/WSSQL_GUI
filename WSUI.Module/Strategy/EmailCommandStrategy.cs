using OF.Core.Enums;
using OF.Module.Commands;
using OF.Module.Core;
using OF.Module.Interface;
using OF.Module.Interface.ViewModel;

namespace OF.Module.Strategy
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
