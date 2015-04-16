using System.Windows;

namespace OF.Infrastructure.Helpers.AttachedProperty
{
    public static  class ListBoxShouldSetSelectAttachedProperty
    {
        public static readonly DependencyProperty ShouldSetSelectProperty =
            DependencyProperty.RegisterAttached("ShouldSetSelect", typeof (bool), typeof (ListBoxShouldSetSelectAttachedProperty), new PropertyMetadata(default(bool)));

        public static void SetShouldSetSelect(UIElement element, bool value)
        {
            element.SetValue(ShouldSetSelectProperty, value);
        }

        public static bool GetShouldSetSelect(UIElement element)
        {
            return (bool) element.GetValue(ShouldSetSelectProperty);
        }
    }
}