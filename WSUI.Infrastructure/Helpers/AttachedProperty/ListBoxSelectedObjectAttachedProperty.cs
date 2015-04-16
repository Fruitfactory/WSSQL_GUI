using System.Windows;

namespace OF.Infrastructure.Helpers.AttachedProperty
{
    public static class ListBoxSelectedObjectAttachedProperty
    {
        public static readonly DependencyProperty SelectedObjectProperty =
            DependencyProperty.RegisterAttached("SelectedObject", typeof (object), typeof (ListBoxSelectedObjectAttachedProperty), new PropertyMetadata(default(object)));

        public static void SetSelectedObject(UIElement element, object value)
        {
            element.SetValue(SelectedObjectProperty, value);
        }

        public static object GetSelectedObject(UIElement element)
        {
            return (object) element.GetValue(SelectedObjectProperty);
        }
    }
}