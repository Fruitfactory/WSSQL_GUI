using System.Collections.ObjectModel;
using OF.Core.Enums;
using OF.Module.Interface;
using OF.Module.Interface.Service;
using OF.Module.Interface.ViewModel;

namespace OF.Module.Core
{
    public abstract class OFBaseCommandStrategy : ICommandStrategy
    {
        protected IMainViewModel _MainViewModel;
        protected ObservableCollection<IOFCommand> _listCommand;

        protected OFBaseCommandStrategy()
        {
            _listCommand = new ObservableCollection<IOFCommand>();
        }

        protected OFBaseCommandStrategy(IMainViewModel mainViewModel)
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
        public OFTypeSearchItem Type { get; protected set; }

        #endregion
    }
}
