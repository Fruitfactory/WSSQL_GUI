using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using WSUI.Infrastructure.Service.Enums;
using WSUI.Module.Interface;

namespace WSUI.Module.Core
{
    public abstract class BaseCommandStrategy : ICommandStrategy
    {
        protected IKindItem _kindItem;
        protected ObservableCollection<IWSCommand> _listCommand;

        protected BaseCommandStrategy()
        {
            _listCommand = new ObservableCollection<IWSCommand>();
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

        public ObservableCollection<IWSCommand> Commands { get { return _listCommand; } }
        public TypeSearchItem Type { get; protected set; }

        #endregion
    }
}
