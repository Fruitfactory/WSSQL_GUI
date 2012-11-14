using System;
using System.Collections.Generic;
using System.Text;
using WSUI.Infrastructure.Service.Enums;
using WSUI.Module.Interface;

namespace WSUI.Module.Core
{
    public abstract class BaseCommandStrategy : ICommandStrategy
    {
        protected IKindItem _kindItem;
        protected List<IWSCommand> _listCommand;

        protected BaseCommandStrategy()
        {
            _listCommand = new List<IWSCommand>();
        }

        protected BaseCommandStrategy(IKindItem kindItem)
            :this()
        {
            _kindItem = kindItem;
        }

        protected virtual void OnInit()
        {
            
        }

        #region Implementation of ICommandStrategy

        public void Init()
        {
            OnInit();
        }

        public List<IWSCommand> Commands { get { return _listCommand; } }
        public TypeSearchItem Type { get; protected set; }

        #endregion
    }
}
