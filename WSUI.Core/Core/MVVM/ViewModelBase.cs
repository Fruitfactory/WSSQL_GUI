using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using OF.Core.Data.ElasticSearch.Response;
using OF.Core.Enums;

namespace OF.Core.Core.MVVM
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

        protected void Set<T>(Expression<Func<T>> exp, T value)
        {
            var name = GetPropertyName(exp);
            Set<T>(name,value);
        }

        protected virtual void Set<T>(string propertyName, T value)
        {
            string property = propertyName.ToLowerInvariant();
            if (_values.ContainsKey(property))
            {
                _values[property] = value;
            }
            else
            {
                _values.Add(property, value);
            }
            OnPropertyChanged(propertyName);
        }


        protected T Get<T>(Expression<Func<T>> exp)
        {
            var name = GetPropertyName(exp);
            return Get<T>(name,default(T));
        }

        protected T Get<T>(Expression<Func<T>> exp, T defaultValue)
        {
            var name = GetPropertyName(exp);
            return Get<T>(name, defaultValue);
        }

        protected virtual T Get<T>(string name, T defaultValue)
        {
            string property = name.ToLowerInvariant();
            if (_values.ContainsKey(property))
            {
                return (T)_values[property];
            }
            return defaultValue;
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
