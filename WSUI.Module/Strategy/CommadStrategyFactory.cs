using OF.Core.Enums;
using OF.Module.Interface;
using OF.Module.Interface.Service;
using OF.Module.Interface.ViewModel;

namespace OF.Module.Strategy
{
    public class CommadStrategyFactory
    {
        public static ICommandStrategy CreateStrategy(TypeSearchItem type, IMainViewModel mainViewModel)
        {
            ICommandStrategy strategy = null;
            switch (type)
            {
                case TypeSearchItem.Email:
                    strategy = new EmailCommandStrategy(mainViewModel);
                    break;
                case TypeSearchItem.File:
                case TypeSearchItem.Attachment:
                case TypeSearchItem.FileAll:
                case TypeSearchItem.Picture:
                    strategy = new FileCommandStrategy(mainViewModel);
                    break;
            }
            if(strategy != null)
                strategy.Init();

            return strategy;
        }
    }
}
