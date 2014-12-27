using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using WSUI.Core.Enums;

namespace WSUI.Core.Core.MVVM
{
    public class ViewModelBase : INotifyPropertyChanged, IDisposable
    {
        private IDictionary<string, object> _values;
        protected bool Disposed = false;

        public ViewModelBase()
        {
            _values = new Dictionary<string, object>();
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

        protected virtual void Set<T>(Expression<Func<T>> exp, T value)
        {
            var name = GetPropertyName(exp);
            if (_values.ContainsKey(name))
            {
                _values[name] = value;
            }
            else
            {
                _values.Add(name, value);
            }
            OnPropertyChanged(name);
        }

        protected virtual T Get<T>(Expression<Func<T>> exp)
        {
            var name = GetPropertyName(exp);
            if (_values.ContainsKey(name))
            {
                return (T)_values[name];
            }
            return default(T);
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
