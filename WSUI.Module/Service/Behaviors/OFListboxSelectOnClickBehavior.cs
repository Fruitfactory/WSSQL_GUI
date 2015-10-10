using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using OF.Core.Interfaces;
using OF.Infrastructure.Implements.Systems;

namespace OF.Module.Service.Behaviors
{
    public class OFListboxSelectOnClickBehavior : Behavior<ListBox>
    {
        private bool _suppressChanging = false;
        private object _lastSelected = null;

        public static readonly DependencyProperty SelectedObjectProperty = DependencyProperty.RegisterAttached(
            "SelectedObject", typeof(object), typeof(OFListboxSelectOnClickBehavior), new PropertyMetadata(default(object)));

        public static void SetSelectedObject(DependencyObject element, object value)
        {
            element.SetValue(SelectedObjectProperty, value);
        }

        public static object GetSelectedObject(DependencyObject element)
        {
            return (object)element.GetValue(SelectedObjectProperty);
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.SelectionChanged += AssociatedObjectOnSelectionChanged;
            AssociatedObject.AddHandler(FrameworkElement.MouseDownEvent, new MouseButtonEventHandler(ListBoxMouseDown), true);
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.SelectionChanged -= AssociatedObjectOnSelectionChanged;
            AssociatedObject.RemoveHandler(FrameworkElement.MouseDownEvent, new MouseButtonEventHandler(ListBoxMouseDown));
        }


        private void ListBoxMouseDown(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            if (_suppressChanging)
            {
                _suppressChanging = false;
                return;
            }
            if (AssociatedObject.SelectedItem == null)
                return;
            if (ReferenceEquals(_lastSelected, AssociatedObject.SelectedItem))
            {
                AssociatedObject.SetValue(OFListboxSelectOnClickBehavior.SelectedObjectProperty, AssociatedObject.SelectedItem);
            }
        }

        private void AssociatedObjectOnSelectionChanged(object sender, SelectionChangedEventArgs selectionChangedEventArgs)
        {
            _suppressChanging = true;
            AssociatedObject.SetValue(OFListboxSelectOnClickBehavior.SelectedObjectProperty, AssociatedObject.SelectedItem);
            _lastSelected = AssociatedObject.SelectedItem;
        }

    }
}