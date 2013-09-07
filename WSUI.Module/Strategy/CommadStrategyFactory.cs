using WSUI.Core.Enums;
using WSUI.Module.Interface;

namespace WSUI.Module.Strategy
{
    public class CommadStrategyFactory
    {
        public static ICommandStrategy CreateStrategy(TypeSearchItem type, IKindItem kindItem)
        {
            ICommandStrategy strategy = null;
            switch (type)
            {
                case TypeSearchItem.Email:
                    strategy = new EmailCommandStrategy(kindItem);
                    break;
                case TypeSearchItem.File:
                case TypeSearchItem.Attachment:
                case TypeSearchItem.FileAll:
                case TypeSearchItem.Picture:
                    strategy = new FileCommandStrategy(kindItem);
                    break;
            }
            if(strategy != null)
                strategy.Init();

            return strategy;
        }
    }
}
