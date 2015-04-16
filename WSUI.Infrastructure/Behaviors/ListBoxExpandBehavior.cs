using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using OF.Core.Data;
using OF.Core.Extensions;
using OF.Infrastructure.Controls.ListBox;
using Size = System.Drawing.Size;

namespace OF.Infrastructure.Behaviors
{
    public class ListBoxExpandBehavior : Behavior<Expander>
    {

        private bool IsExpanded { get; set; }
        private bool IsCollapsed { get; set; }
        private OFListBox ParentListBox { get; set; }
        protected override void OnAttached()
        {
            base.OnAttached();
            ParentListBox = AssociatedObject.GetParent<OFListBox>();
            AssociatedObject.Expanded += AssociatedObjectOnExpanded;
            AssociatedObject.Collapsed += AssociatedObjectOnCollapsed;
            AssociatedObject.SizeChanged += AssociatedObjectOnSizeChanged;
            IsExpanded = false;
            IsCollapsed = false;
        }

        private void AssociatedObjectOnSizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
        {
            var point = AssociatedObject.TranslatePoint(new Point(0, 0), ParentListBox);
            if (IsExpanded && ParentListBox  != null)
            {
                ParentListBox.RaiseCalculationHeight(new OFExpanderData(){ExpandDirection =  AssociatedObject.ExpandDirection,NewSize = sizeChangedEventArgs.NewSize, OldSize = sizeChangedEventArgs.PreviousSize, Location = point});
                IsExpanded = false;
            }
            if (IsCollapsed && ParentListBox != null)
            {
                ParentListBox.RaiseCalculationHeight(new OFExpanderData() { ExpandDirection = AssociatedObject.ExpandDirection, NewSize = sizeChangedEventArgs.NewSize, OldSize = sizeChangedEventArgs.PreviousSize, Location = point });
                IsCollapsed = false;
            }
            System.Diagnostics.Debug.WriteLine(AssociatedObject.ExpandDirection.ToString());
        }


        protected override void OnDetaching()
        {
            base.OnDetaching();
            ParentListBox = null;
            AssociatedObject.Expanded -= AssociatedObjectOnExpanded;
            AssociatedObject.Collapsed -= AssociatedObjectOnCollapsed;
            AssociatedObject.SizeChanged -= AssociatedObjectOnSizeChanged; 
        }

        private void AssociatedObjectOnExpanded(object sender, RoutedEventArgs routedEventArgs)
        {
            IsExpanded = true;
        }

        private void AssociatedObjectOnCollapsed(object sender, RoutedEventArgs routedEventArgs)
        {
            IsCollapsed = true;
        }


    }
}