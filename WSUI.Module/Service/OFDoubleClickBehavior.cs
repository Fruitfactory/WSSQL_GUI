using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace OF.Module.Service
{
    public static class OFDoubleClickBehavior
    {
         
        public static readonly DependencyProperty DoubleClickHandlerProperty =
            DependencyProperty.RegisterAttached("DoubleClickHandler", typeof (ICommand), typeof (OFDoubleClickBehavior), new UIPropertyMetadata(OFDoubleClickBehavior.DoubleClickHandle));

        public static void SetDoubleClickHandler(DependencyObject target, ICommand value)
        {
            target.SetValue(OFDoubleClickBehavior.DoubleClickHandlerProperty,value);
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
            ICommand command = (ICommand) element.GetValue(OFDoubleClickBehavior.DoubleClickHandlerProperty);
            if (command == null)
                return;
            command.Execute(mouseButtonEventArgs);
        }
    }
}