using System;
using System.Windows.Input;
using WSUI.Core.Data;
using WSUI.Core.Enums;
using WSUI.Module.Interface;
using WSUI.Module.Interface.Service;
using WSUI.Module.Interface.ViewModel;

namespace WSUI.Module.Core
{
    public abstract class BasePreviewCommand : IWSCommand
    {
        protected IMainViewModel MainViewModel;

        protected BasePreviewCommand(IMainViewModel mainViewModel)
        {
            MainViewModel = mainViewModel;
        }

        protected virtual bool OnCanExecute()
        {
            return MainViewModel != null;
        }

        protected virtual void OnExecute()
        {
            
        }

        protected virtual string GetIcon()
        {
            return null;
        }

        protected virtual string GetCaption()
        {
            return string.Empty;
        }

        protected virtual string GetTooltip()
        {
            return string.Empty;
        }

        protected BaseSearchObject GetCurrentSearchObject()
        {
            return MainViewModel.IsPreviewVisible ? MainViewModel.Current : MainViewModel.CurrentTracked;
        }

        protected TypeSearchItem GetTypeOfCurrentItem()
        {
            return MainViewModel.IsPreviewVisible ? MainViewModel.Current.TypeItem : MainViewModel.CurrentTracked.TypeItem;
        }

        #region Implementation of ICommand

        public void Execute(object parameter)
        {
            OnExecute();
        }

        public bool CanExecute(object parameter)
        {
            return OnCanExecute();
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        #endregion

        #region Implementation of IWSCommand

        public string Icon 
        { 
            get { return GetIcon(); } 
        }

        public string Caption
        {
            get { return GetCaption(); }
        }

        public string Tooltip
        {
            get { return GetTooltip(); }
        }

        #endregion
    }
}
