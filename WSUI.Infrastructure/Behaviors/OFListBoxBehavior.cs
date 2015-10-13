using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace OF.Infrastructure.Behaviors
{
    public class OFListBoxBehavior : Behavior<ListBox>
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