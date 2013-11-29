using System;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using WSUI.Infrastructure.Helpers.AttachedProperty;

namespace WSUI.Infrastructure.Behaviors
{
    //public class MainListBoxBehavior : Behavior<ListBox>
    //{
    //    protected override void OnAttached()
    //    {
    //        base.OnAttached();
    //        AssociatedObject.SelectionChanged += AssociatedObjectOnSelectionChanged;
    //    }
    //    protected override void OnDetaching()
    //    {
    //        base.OnDetaching();
    //        AssociatedObject.SelectionChanged -= AssociatedObjectOnSelectionChanged;
    //    }

    //    private void AssociatedObjectOnSelectionChanged(object sender, SelectionChangedEventArgs selectionChangedEventArgs)
    //    {
    //        if (ReferenceEquals(sender, selectionChangedEventArgs.OriginalSource) &&
    //            ListBoxShouldSetSelectAttachedProperty.GetShouldSetSelect(AssociatedObject) && AssociatedObject.SelectedItem != null)
    //        {
    //            Debug.WriteLine("Set Main ListBox");
    //            ListBoxSelectedObjectAttachedProperty.SetSelectedObject(AssociatedObject, AssociatedObject.SelectedItem);
    //        }
    //    }

    //}
}