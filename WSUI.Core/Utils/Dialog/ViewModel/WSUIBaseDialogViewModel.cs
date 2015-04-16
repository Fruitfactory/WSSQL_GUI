using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Input;
using System.Windows.Threading;
using OF.Core.Utils.Dialog.Interfaces;
using Action = System.Action;

namespace OF.Core.Utils.Dialog.ViewModel
{
    public abstract class OFBaseDialogViewModel : IOFViewModel
    {
        protected OFBaseDialogViewModel(IOFView view)
        {
            View = view;
            OKCommand = new OFRelayCommand(OkExecute, CanOkExecute);
            CancelCommand = new OFRelayCommand(CancelExecute, CanCancelExcute);
            Dispatcher.CurrentDispatcher.BeginInvoke((Action)(() => this.View.DataContext = this));
        }

        public string this[string columnName]
        {
            get { return Validate(columnName); }
        }

        public string Error
        {
            get { return ReturnError(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string Title
        {
            get { return FormatTitle(); }
        }

        public IOFView View { get; private set; }

        public ICommand OKCommand { get; private set; }

        public ICommand CancelCommand { get; private set; }

        protected abstract string Validate(string columnName);

        protected virtual string ReturnError()
        {
            return string.Empty;
        }

        protected virtual void OkExecute(object arg)
        {
        }

        protected virtual bool CanOkExecute(object arg)
        {
            return true;
        }

        protected virtual void CancelExecute(object arg)
        {
        }

        protected virtual bool CanCancelExcute(object arg)
        {
            return true;
        }

        protected virtual string FormatTitle()
        {
            return string.Empty;
        }

        ///
        /// <param name="propertyName"></param>
        protected void OnPropertyChanged(string propertyName)
        {
            var temp = PropertyChanged;
            if (temp != null)
            {
                temp(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        ///
        /// <param name="exp"></param>
        protected void OnPropertyChanged<T>(Expression<Func<T>> exp)
        {
            MemberExpression memberExpression = exp.Body as MemberExpression;
            if (memberExpression == null)
                throw new ArgumentException("Expression is empty");
            OnPropertyChanged(memberExpression.Member.Name);
        }

        protected string GetPropertyName<T>(Expression<Func<T>> exp)
        {
            var memExpression = exp.Body as MemberExpression;
            if (memExpression == null)
                return string.Empty;
            return memExpression.Member.Name;
        }
    }
}