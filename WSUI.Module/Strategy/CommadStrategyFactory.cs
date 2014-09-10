using WSUI.Core.Enums;
using WSUI.Module.Interface;
using WSUI.Module.Interface.Service;
using WSUI.Module.Interface.ViewModel;

namespace WSUI.Module.Strategy
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
