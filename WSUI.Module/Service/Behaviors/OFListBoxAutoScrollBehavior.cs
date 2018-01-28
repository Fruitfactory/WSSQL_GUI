using System;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace OF.Module.Service.Behaviors
{
    public class OFListBoxAutoScrollBehavior : Behavior<ListBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.SelectionChanged += AssociatedObjectOnSelectionChanged;
        }
        
        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.SelectionChanged -= AssociatedObjectOnSelectionChanged;
        }


        private void AssociatedObjectOnSelectionChanged(object sender, SelectionChangedEventArgs selectionChangedEventArgs)
        {
            if (selectionChangedEventArgs.AddedItems != null && selectionChangedEventArgs.AddedItems.Count > 0)
            {
                Dispatcher.BeginInvoke(new Action( () =>
                {
                    AssociatedObject.ScrollIntoView(selectionChangedEventArgs.AddedItems[0]);
                }));
            }
        }
    }
}