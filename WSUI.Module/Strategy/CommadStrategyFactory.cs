using System;
using System.Collections.Generic;
using System.Text;
using WSUI.Infrastructure.Service.Enums;
using WSUI.Module.Core;
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
                    strategy = new FileCommandStrategy(kindItem);
                    break;
            }
            if(strategy != null)
                strategy.Init();

            return strategy;
        }
    }
}
