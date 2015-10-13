using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using OF.Core.Extensions;

namespace OF.Infrastructure.Behaviors
{
    public class OFExpanderEmailBehavior : Behavior<Expander>
    {
        private const string TemplatePartExpanderImageName = "ExpanderImage";

        protected override void OnAttached()
        {
            base.OnAttached();
            
            AssociatedObject.PreviewMouseLeftButtonDown += AssociatedObjectOnPreviewMouseLeftButtonDown;
        }

        private void AssociatedObjectOnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            var fe = mouseButtonEventArgs.OriginalSource as FrameworkElement;
            if (mouseButtonEventArgs.Source == null 
                || sender == null 
                || mouseButtonEventArgs.Source != sender 
                || ( fe != null && fe.Name == TemplatePartExpanderImageName))
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