using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using WSUI.Infrastructure.Helpers.Extensions;

namespace WSUI.Infrastructure.Behaviors
{
    public class ExpanderEmailBehavior : Behavior<Expander>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            
            AssociatedObject.PreviewMouseLeftButtonDown += AssociatedObjectOnPreviewMouseLeftButtonDown;
        }

        private void AssociatedObjectOnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            if (mouseButtonEventArgs.Source == null || sender == null)
                return;
            if (mouseButtonEventArgs.Source != sender)
                return;

            var item = AssociatedObject.GetParentCore<ListBoxItem>();
            if (item != null)
            {
                item.IsSelected = false;
                item.IsSelected = true;
            }
            var childListBox = AssociatedObject.GetChildren<ListBox>();
            if (childListBox != null && childListBox.Any())
            {
                childListBox.First().SelectedIndex = -1;
            }
        }

        protected override void OnDetaching()
        {
            AssociatedObject.PreviewMouseLeftButtonDown -= AssociatedObjectOnPreviewMouseLeftButtonDown;
            base.OnDetaching();
        }

    }

}