using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WSUI.Module.Service
{
    public static class KeyDownBehavior
    {
        public static DependencyProperty KeyDownProperty =
               DependencyProperty.RegisterAttached("KeyDown",
               typeof(ICommand),
               typeof(KeyDownBehavior),
               new UIPropertyMetadata(KeyDownBehavior.KeyDownFired));

        public static void SetKeyDown(DependencyObject target, ICommand value)
        {
            target.SetValue(KeyDownBehavior.KeyDownProperty, value);
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
            ICommand command = (ICommand)control.GetValue(KeyDownBehavior.KeyDownProperty);
            command.Execute(e);
        }
    }
}
