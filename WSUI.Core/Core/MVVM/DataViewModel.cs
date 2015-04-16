using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Nest;
using OF.Core.Extensions;

namespace OF.Core.Core.MVVM
{
    public abstract class DataViewModel : ViewModelBase
    {
        private object _data = null;
        private IEnumerable<PropertyInfo> _properties; 
        protected DataViewModel()
        {   
        }

        protected DataViewModel(object data)
        {
            SetDataObject(data);
        }

        public T GetDataObject<T>()
        {
            return (T) _data;
        }

        protected void SetDataObject<T>(T data)
        {
            _data = data;
            if (_data.IsNotNull() && _properties.IsNull())
            {
                _properties = _data.GetType().GetAllPublicProperties();
            }
            OnPropertyChanged(null);
        }

        protected override void Set<T>(string propertyName, T value)
        {
            string property = propertyName.ToLowerInvariant();
            if (_data.IsNotNull() && _properties.Any(p => p.Name.ToLowerInvariant() == property))
            {
                var pi = _properties.FirstOrDefault(p => p.Name.ToLowerInvariant() == property);
                if (pi.IsNotNull())
                {
                    pi.SetPropertyValue(_data, value);
                }
            }
            else
            {
                base.Set(propertyName, value);    
            }
        }

        protected override T Get<T>(string name, T defaultValue)
        {
            string property = name.ToLowerInvariant();
            if (_data.IsNotNull() && _properties.Any(p => p.Name.ToLowerInvariant() == property))
            {
                var pi = _properties.FirstOrDefault(p => p.Name.ToLowerInvariant() == property);
                if (pi.IsNotNull())
                {
                    return pi.GetPropertyValue<T>(_data);
                }
            }
            return base.Get(name, defaultValue);
        }

        protected override void OnPropertyChanged(string property)
        {
            if (property.IsStringEmptyOrNull())
            {
                foreach (var propertyInfo in _properties)
                {
                    base.OnPropertyChanged(propertyInfo.Name);
                }
                return;
            }
            base.OnPropertyChanged(property);
        }

        public virtual void Update(object data)
        {
            SetDataObject(data);
        }
    }
}