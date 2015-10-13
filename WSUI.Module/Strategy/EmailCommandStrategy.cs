using OF.Core.Enums;
using OF.Module.Commands;
using OF.Module.Core;
using OF.Module.Interface;
using OF.Module.Interface.ViewModel;

namespace OF.Module.Strategy
{
    public class EmailCommandStrategy : OFBaseCommandStrategy
    {
        public EmailCommandStrategy(IMainViewModel mainViewModel)
            :base(mainViewModel)
        {
            Type = OFTypeSearchItem.Email;
        }

        protected override void OnInit()
        {
            base.OnInit();
            _listCommand.Add(new OFOpenEmailCommand(_MainViewModel));
            _listCommand.Add(new OFReplyCommand(_MainViewModel));
            _listCommand.Add(new OFReplyAllCommand(_MainViewModel));
            _listCommand.Add(new ForwardCommand(_MainViewModel));
        }

    }
}
