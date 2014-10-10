using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace WSUI.Infrastructure.Behaviors
{
    public class ListBoxBehavior : Behavior<ListBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PreviewMouseRightButtonDown += AssociatedObjectOnPreviewMouseRightButtonDown;
        }

        private void AssociatedObjectOnPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            mouseButtonEventArgs.Handled = true;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.PreviewMouseRightButtonDown -= AssociatedObjectOnPreviewMouseRightButtonDown;
        }
    }
}