using OF.Core.Enums;
using OF.Module.Interface;
using OF.Module.Interface.Service;
using OF.Module.Interface.ViewModel;

namespace OF.Module.Strategy
{
    public class CommadStrategyFactory
    {
        public static ICommandStrategy CreateStrategy(OFTypeSearchItem type, IMainViewModel mainViewModel)
        {
            ICommandStrategy strategy = null;
            switch (type)
            {
                case OFTypeSearchItem.Email:
                    strategy = new EmailCommandStrategy(mainViewModel);
                    break;
                case OFTypeSearchItem.File:
                case OFTypeSearchItem.Attachment:
                case OFTypeSearchItem.FileAll:
                case OFTypeSearchItem.Picture:
                    strategy = new FileCommandStrategy(mainViewModel);
                    break;
            }
            if(strategy != null)
                strategy.Init();

            return strategy;
        }
    }
}
