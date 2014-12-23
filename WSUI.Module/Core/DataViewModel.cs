using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace WSUI.Module.Core
{
    public class DataViewModel : ViewModelBase
    {
        protected readonly Dictionary<string, object> _values = new Dictionary<string, object>();

        protected T Get<T>(Expression<Func<T>> expression)
        {
            return Get<T>(GetPropertyName(expression), default(T));
        }

        protected T Get<T>(Expression<Func<T>> expression, T defaultValue)
        {
            return Get<T>(GetPropertyName(expression), defaultValue);
        }

        protected T Get<T>(string name)
        {
            return Get<T>(name, default(T));
        }

        protected virtual T Get<T>(string name, T defaultValue)
        {
            if (_values.ContainsKey(name))
            {
                return (T)_values[name];
            }
            return defaultValue;
        }

        protected void Set<T>(Expression<Func<T>> expression, T val)
        {
            var name = GetPropertyName(expression);
            Set<T>(name, val);
        }

        protected virtual void Set<T>(string name, T val)
        {
            _values[name] = val;
            OnPropertyChanged(name);
        }
    }
}