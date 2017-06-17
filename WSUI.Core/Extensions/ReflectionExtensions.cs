using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;


namespace OF.Core.Extensions
{
    public static class ReflectionExtensions
    {
        #region [for types]

        public static IEnumerable<PropertyInfo> GetAllPublicProperties(this Type t)
        {
            var list = t.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
            return list;
        }

        public static T GetPrivateValue<T>(this object source, string propertyName)
        {
            if (source == null || string.IsNullOrEmpty(propertyName))
            {
                return default(T);
            }
            var type = source.GetType();
            var prop = type.GetProperties(BindingFlags.Public |
                                          BindingFlags.NonPublic |
                                          BindingFlags.Instance).FirstOrDefault(p => p.Name.ToUpperInvariant() == propertyName.ToUpperInvariant());

            return prop == null ? default(T) : (T) prop.GetValue(source);
        }

        public static bool HasProperty(this Type t, string name)
        {
            return t.GetAllPublicProperties().Any(pi => String.Equals(pi.Name, name, StringComparison.CurrentCultureIgnoreCase));
        }

        public static bool HasAttribute<T>(this Type t)
        {
            return t.GetCustomAttributes(typeof(T), true).Any();
        }

        public static T GetAttribute<T>(this Type t)
        {
            return t.GetCustomAttributes(typeof(T), true).OfType<T>().ElementAt(0);
        }

        #endregion [for types]

        #region [property info]

        public static T GetPropertyValue<T>(this PropertyInfo pi, object context)
        {
            return (T)pi.GetValue(context, null);
        }

        public static void SetPropertyValue<T>(this PropertyInfo pi, object context, T value)
        {

            if (pi.IsNullable())
            {
                if (pi.PropertyType == typeof(Guid?))
                {
                    pi.SetValue(context, (value as Guid?).Value == Guid.Empty ? (object)null : (value as Guid?).Value,
                        null);
                }
            }
            else
                pi.SetValue(context, value, null);
        }

        public static bool HasAttribute<T>(this PropertyInfo pi)
        {
            object[] arr = pi.GetCustomAttributes(typeof(T), true);
            return arr.Length > 0;
        }

        public static T GetAttribute<T>(this PropertyInfo pi)
        {
            object[] arr = pi.GetCustomAttributes(typeof(T), true);
            return (T)arr[0];
        }

        public static bool IsNullable(this PropertyInfo pi)
        {
            return Nullable.GetUnderlyingType(pi.PropertyType) != null;
        }

        #endregion [property info]

        #region [static]

        public static string GetPropertyName<T>(Expression<Func<T>> exp)
        {
            var me = exp.Body as MemberExpression;
            if (me == null)
                return string.Empty;
            return me.Member.Name;
        }

        #endregion [static]
    }
}