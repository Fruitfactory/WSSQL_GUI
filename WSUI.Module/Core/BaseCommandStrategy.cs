using System.Collections.ObjectModel;
using OF.Core.Enums;
using OF.Module.Interface;
using OF.Module.Interface.Service;
using OF.Module.Interface.ViewModel;

namespace OF.Module.Core
{
    public abstract class BaseCommandStrategy : ICommandStrategy
    {
        protected IMainViewModel _MainViewModel;
        protected ObservableCollection<IOFCommand> _listCommand;

        protected BaseCommandStrategy()
        {
            _listCommand = new ObservableCollection<IOFCommand>();
        }

        protected BaseCommandStrategy(IMainViewModel mainViewModel)
            :this()
        {
            _MainViewModel = mainViewModel;
        }

        protected virtual void OnInit()
        {
            
        }

        #region Implementation of ICommandStrategy

        public void Init()
        {
            OnInit();
        }

        public ObservableCollection<IOFCommand> Commands { get { return _listCommand; } }
        public TypeSearchItem Type { get; protected set; }

        #endregion
    }
}
