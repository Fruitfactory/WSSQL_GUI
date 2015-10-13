using System.Windows;

namespace OF.Infrastructure.Helpers.AttachedProperty
{
    public static  class OFListBoxShouldSetSelectAttachedProperty
    {
        public static readonly DependencyProperty ShouldSetSelectProperty =
            DependencyProperty.RegisterAttached("ShouldSetSelect", typeof (bool), typeof (OFListBoxShouldSetSelectAttachedProperty), new PropertyMetadata(default(bool)));

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