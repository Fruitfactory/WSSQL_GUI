using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WSUI.Core.Logger;

namespace WSUI.Core.Extensions
{
    public static class UiExtensions
    {

        private const string ParentCoreFieldName = "_parent";
        private const BindingFlags PrivateFlags = BindingFlags.Instance | BindingFlags.NonPublic;

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

        public static T GetParentCore<T>(this FrameworkElement obj) where T : FrameworkElement 
        {
            try
            {
                var fi = GetField(ParentCoreFieldName, typeof (FrameworkElement), PrivateFlags);
                if(fi ==  null)
                    return default(T);
                var result = obj.InternalGetParentCore<T>(fi);
                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return default(T);
        }

        private static FieldInfo GetField(string name, Type type, BindingFlags flags)
        {
            var fi = type.GetField(name, flags);
            return fi;
        }

        private static T InternalGetParentCore<T>(this FrameworkElement obj, FieldInfo fi) where  T: FrameworkElement
        {
            var parent = VisualTreeHelper.GetParent(obj) ?? GetFieldValue(obj, fi);
            if (parent == null) return default(T);
            var result = parent as T;
            return result ?? ((FrameworkElement) parent).InternalGetParentCore<T>(fi);
        }

        private static object GetFieldValue(object obj, FieldInfo fi)
        {
            return fi.GetValue(obj);
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

        public static IEnumerable<T> GetChildren<T>(this DependencyObject obj) where T : class
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
            var children = ((UIElement)element).GetChildrenProperty();
            if (children != null)
            {
                foreach (var uiElement in children)
                {
                    if (uiElement is T)
                    {
                        list.Add(uiElement as T);
                    }
                    FindChildOfType<T>((DependencyObject)uiElement, list);
                }
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

        private static IEnumerable GetChildrenProperty(this UIElement uiElement)
        {
            var property = uiElement.GetType().GetProperty("Children");

            if (property != null)
                return (UIElementCollection)property.GetValue(uiElement, null);
            return null;
        }

        public static IEnumerable<DependencyObject> GetVisualChildren(this DependencyObject parent)
        {
            int childCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int counter = 0; counter < childCount; ++counter)
                yield return VisualTreeHelper.GetChild(parent, counter);
        }

        public static IEnumerable<DependencyObject> GetAllVisualChildren(this DependencyObject parent)
        {
            List<DependencyObject> list1 = new List<DependencyObject>();
            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int childIndex = 0; childIndex < childrenCount; ++childIndex)
                list1.Add(VisualTreeHelper.GetChild(parent, childIndex));
            List<DependencyObject> list2 = new List<DependencyObject>();
            foreach (DependencyObject parent1 in list1)
            {
                list2.Add(parent1);
                list2.AddRange(GetAllVisualChildren(parent1));
            }
            return (IEnumerable<DependencyObject>)list2;
        }

        public static IEnumerable<FrameworkElement> GetLogicalChildren(this FrameworkElement parent)
        {
            if (parent is Control)
                (parent as Control).ApplyTemplate();
            //EnsureName(parent);
            string parentName = parent.Name;
            Stack<FrameworkElement> stack = new Stack<FrameworkElement>(Enumerable.OfType<FrameworkElement>((IEnumerable)GetVisualChildren((DependencyObject)parent)));
            while (stack.Count > 0)
            {
                FrameworkElement element = stack.Pop();
                if (element.FindName(parentName) == parent)
                {
                    yield return element;
                }
                else
                {
                    foreach (FrameworkElement frameworkElement in Enumerable.OfType<FrameworkElement>((IEnumerable)GetVisualChildren((DependencyObject)element)))
                        stack.Push(frameworkElement);
                }
            }
        }

        public static IEnumerable<FrameworkElement> GetAllLogicalChildren(this FrameworkElement parent)
        {
            List<FrameworkElement> list1 = new List<FrameworkElement>();
            List<FrameworkElement> list2 = Enumerable.ToList<FrameworkElement>(GetLogicalChildren(parent));
            list1.Add(parent);
            foreach (FrameworkElement parent1 in list2)
                list1.AddRange(GetAllLogicalChildren(parent1));
            return (IEnumerable<FrameworkElement>)list1;
        }

        private static void EnsureName(FrameworkElement parent)
        {
            if (!(parent.Name == string.Empty))
                return;
            parent.SetValue(FrameworkElement.NameProperty, Guid.NewGuid().ToString());
        }

        public static void OnSelectedChanged(this ListBox listBox, ref int oldIndex)
        {
            try
            {
                if (oldIndex == -1)
                {
                    oldIndex = listBox.SelectedIndex;
                    return;
                }
                var currentIndex = listBox.SelectedIndex;
                var listBoxItem = listBox.ItemContainerGenerator.ContainerFromIndex(oldIndex) as ListBoxItem;
                if (oldIndex != currentIndex && listBoxItem != null)
                {
                    var childrens = listBoxItem.GetAllVisualChildren().SelectMany(c => c.GetChildren<ListBox>()).OfType<ListBox>();
                    if (childrens.Any())
                    {
                        childrens.ElementAt(0).SelectedIndex = -1;
                    }
                }
                oldIndex = currentIndex;
            }
            catch (Exception ex)
            {
                WSSqlLogger.Instance.LogError(ex.Message);
                oldIndex = -1;
            }
        }

        public static UIElement GetParentProperty(this UIElement uiElement)
        {
            var property = uiElement.GetType().GetProperty("Parent");
            if (property != null)
                return (UIElement)property.GetValue(uiElement, null);

            return null;
        }

        public static ScrollViewer GetListBoxScrollViewer(this ListBox listBox)
        {
            if (listBox == null)
                return null;
            var border = VisualTreeHelper.GetChild(listBox, 0);
            return VisualTreeHelper.GetChild(border, 0) as ScrollViewer;
        }

        public static T GetControlAtPoint<T>(this Visual obj, Point pt) where T : Visual
        {
            if (obj == null)
            {
                return default(T);
            }
            var hitResult = VisualTreeHelper.HitTest(obj, pt);
            if (hitResult == null)
            {
                return default(T);
            }
            var item = FindParent<ListBoxItem>(hitResult.VisualHit);
            return item as T;
        }

        public static T FindParent<T>(DependencyObject from) where T : class
        {
            T result = null;
            DependencyObject parent = VisualTreeHelper.GetParent(from);

            if (parent is T)
                result = parent as T;
            else if (parent != null)
                result = FindParent<T>(parent);

            return result;
        }

    }
}
