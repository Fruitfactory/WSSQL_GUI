using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace WSUI.Infrastructure.Helpers.Extensions
{
    public static class UIExtensions
    {
        public static T GetParent<T>(this DependencyObject obj) where T : class
        {
            try
            {
                var parent = VisualTreeHelper.GetParent(obj);
                if (parent == null) return default(T);
                var result = parent as T;
                return result ?? parent.GetParent<T>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return null;
        }

        public static DependencyObject GetParent(this DependencyObject obj, Type type)
        {
            var parent = VisualTreeHelper.GetParent(obj);
            if (parent == null) return null;
            var result = parent.GetType() == type;
            return result ? parent : parent.GetParent(type);
        }

        public static bool IsDisconnected(this DependencyObject obj)
        {
            var parent = VisualTreeHelper.GetParent(obj);
            if (parent == Application.Current.MainWindow)
                return false;
            if (parent == null)
                return true;
            return parent.IsDisconnected();
        }
    }
}
