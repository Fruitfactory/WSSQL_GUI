using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Nest;
using OF.Core.Extensions;

namespace OF.Core.Core.MVVM
{
    public abstract class OFDataViewModel : OFViewModelBase
    {
        private object _data = null;
        private IEnumerable<PropertyInfo> _properties; 
        protected OFDataViewModel()
        {   
        }

        protected OFDataViewModel(object data)
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
            if (_data.IsNotNull() && _properties.Any(p => String.Equals(p.Name, propertyName,StringComparison.CurrentCultureIgnoreCase)))
            {
                var pi = _properties.FirstOrDefault(p => String.Equals(p.Name, propertyName, StringComparison.CurrentCultureIgnoreCase));
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

        protected override T Get<T>(string propertyName, T defaultValue )
        {
            if (_data.IsNotNull() && _properties.Any(p => String.Equals(p.Name, propertyName, StringComparison.CurrentCultureIgnoreCase)))
            {
                var pi = _properties.FirstOrDefault(p => String.Equals(p.Name, propertyName, StringComparison.CurrentCultureIgnoreCase));
                if (pi.IsNotNull())
                {
                    return pi.GetPropertyValue<T>(_data);
                }
            }
            return base.Get(propertyName, defaultValue);
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