using System.Windows;
using System.Windows.Input;

namespace OF.Module.Service
{
    public static class OFKeyDownBehavior
    {
        public static DependencyProperty KeyDownProperty =
               DependencyProperty.RegisterAttached("KeyDown",
               typeof(ICommand),
               typeof(OFKeyDownBehavior),
               new UIPropertyMetadata(OFKeyDownBehavior.KeyDownFired));

        public static void SetKeyDown(DependencyObject target, ICommand value)
        {
            target.SetValue(OFKeyDownBehavior.KeyDownProperty, value);
        }

        private static void KeyDownFired(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement control = target as FrameworkElement;
            if (control != null)
            {
                if ((e.NewValue != null) && (e.OldValue == null))
                {
                    control.KeyDown += KeyDown;
                }
                else if ((e.NewValue == null) && (e.OldValue != null))
                {
                    control.KeyDown -= KeyDown;
                }
            }
        }

        private static void KeyDown(object sender, KeyEventArgs e)
        {
            FrameworkElement control = sender as FrameworkElement;
            ICommand command = (ICommand)control.GetValue(OFKeyDownBehavior.KeyDownProperty);
            command.Execute(e);
        }
    }
}
