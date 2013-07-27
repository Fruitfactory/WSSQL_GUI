using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WSUI.Infrastructure.Service.Helpers
{
    public static class Extensions
    {
        internal static IEnumerable<T> GetChildren<T>(this DependencyObject obj) where T : class
        {
            if (obj == null)
                return default(IEnumerable<T>);
            var list = new List<T>();
            FindChildOfType<T>(obj, list);
            return list;
        }

        private static void FindChildOfType<T>(DependencyObject obj, List<T> list) where T : class
        {
            var element = GetUIElement(obj);
            if (element == null)
                return;
            if (!(element is TextBox))
            {
                var children = ((UIElement)element).GetChildrenProperty();
                if (children != null)
                {
                    foreach (var uiElement in children)
                    {
                        if (uiElement is T)
                        {
                            list.Add(uiElement as T);
                            continue;
                        }
                        FindChildOfType<T>((DependencyObject)uiElement, list);
                    }
                }
            }
            else if (VisualTreeHelper.GetChildrenCount(element) >  0)
            {
                var children = VisualTreeHelper.GetChild(element, 0) as T;
                if(children  != null)
                    list.Add(children);
            }
            
        }

        private static UIElement GetUIElement(DependencyObject obj)
        {
            if (obj is ContentControl)
            {
                return ((ContentControl)obj).Content as UIElement;
            }
            return (UIElement)obj;
        }

        private static UIElementCollection GetChildrenProperty(this UIElement uiElement)
        {
            var property = uiElement.GetType().GetProperty("Children");
            if (property != null)
                return ((UIElementCollection)property.GetValue(uiElement, null));
            return null;
        } 
    }
}