using System;
using System.Windows.Input;
using WSUI.Module.Interface;

namespace WSUI.Module.Core
{
    public abstract class BasePreviewCommand : IWSCommand
    {
        protected IKindItem KindItem;

        protected BasePreviewCommand(IKindItem kindItem)
        {
            KindItem = kindItem;
        }

        protected virtual bool OnCanExecute()
        {
            return KindItem != null;
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
