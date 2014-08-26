using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WSUI.Module.Service
{
    public static class DoubleClickBehavior
    {
         
        public static readonly DependencyProperty DoubleClickHandlerProperty =
            DependencyProperty.RegisterAttached("DoubleClickHandler", typeof (ICommand), typeof (DoubleClickBehavior), new UIPropertyMetadata(DoubleClickBehavior.DoubleClickHandle));

        public static void SetDoubleClickHandler(DependencyObject target, ICommand value)
        {
            target.SetValue(DoubleClickBehavior.DoubleClickHandlerProperty,value);
        }

        private static void DoubleClickHandle(DependencyObject target, DependencyPropertyChangedEventArgs args)
        {
            Control fe = target as Control;
            if(fe == null)
                return;
            if (args.NewValue != null && args.OldValue == null)
            {
                fe.MouseDoubleClick += FeOnMouseDoubleClick;
            }
            else if (args.NewValue == null && args.OldValue != null)
            {
                fe.MouseDoubleClick -= FeOnMouseDoubleClick;
            }

        }

        private static void FeOnMouseDoubleClick(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            FrameworkElement element = sender as FrameworkElement;
            ICommand command = (ICommand) element.GetValue(DoubleClickBehavior.DoubleClickHandlerProperty);
            if (command == null)
                return;
            command.Execute(mouseButtonEventArgs);
        }
    }
}