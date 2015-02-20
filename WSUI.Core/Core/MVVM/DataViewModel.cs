using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using WSUI.Core.Extensions;

namespace WSUI.Core.Core.MVVM
{
    public class DataViewModel : ViewModelBase
    {
        private object _data = null;
        private IEnumerable<PropertyInfo> _properties; 
        public DataViewModel()
        {   
        }

        public DataViewModel(object data)
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
            if (_data.IsNotNull())
            {
                _properties = _data.GetType().GetAllPublicProperties();
            }
        }

        protected override void Set<T>(string propertyName, T value)
        {

            if (_data.IsNotNull() && _data.GetType().HasProperty(propertyName))
            {
                var pi = _properties.FirstOrDefault(p => p.Name.ToLowerInvariant() == propertyName);
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
            if (_data.IsNotNull() && _data.GetType().HasProperty(name))
            {
                var pi = _properties.FirstOrDefault(p => p.Name.ToLowerInvariant() == name);
                if (pi.IsNotNull())
                {
                    return pi.GetPropertyValue<T>(_data);
                }
            }
            return base.Get(name, defaultValue);
        }
    }
}