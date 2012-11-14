using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using WSUI.Module.Interface;

namespace WSUI.Module.Core
{
    public abstract class BasePreviewCommand : IWSCommand
    {
        protected IKindItem _kindItem;

        protected BasePreviewCommand(IKindItem kindItem)
        {
            _kindItem = kindItem;
        }

        protected virtual bool OnCanExecute()
        {
            return _kindItem != null;
        }

        protected virtual void OnExecute()
        {
            
        }

        protected virtual Image GetIcon()
        {
            return null;
        }

        protected virtual string GetCaption()
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

        public event EventHandler CanExecuteChanged;

        #endregion

        #region Implementation of IWSCommand

        public Image Icon 
        { 
            get { return GetIcon(); } 
        }

        public string Caption
        {
            get { return GetCaption(); }
        }

        #endregion
    }
}
