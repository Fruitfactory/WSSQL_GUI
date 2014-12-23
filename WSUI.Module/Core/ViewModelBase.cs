using System;
using System.ComponentModel;
using System.Linq.Expressions;
using WSUI.Core.Enums;

namespace WSUI.Module.Core
{
    public class ViewModelBase : INotifyPropertyChanged, IDisposable
    {
        protected bool Disposed = false;

        public ViewModelBase()
        {
            Host = HostType.Unknown;
        }

        protected virtual void OnPropertyChanged(string property)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(property);
                handler(this, e);
            }
        }

        protected void OnPropertyChanged<T>(Expression<Func<T>> action)
        {
            var propertyName = GetPropertyName(action);
            OnPropertyChanged(propertyName);
        }

        protected static string GetPropertyName<T>(Expression<Func<T>> action)
        {
            var expression = (MemberExpression)action.Body;
            var propertyName = expression.Member.Name;
            return propertyName;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public HostType Host { get; protected set; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);            
        }

        protected virtual void Dispose(bool disposing)
        {
            Disposed = true;
        }

    }
}
