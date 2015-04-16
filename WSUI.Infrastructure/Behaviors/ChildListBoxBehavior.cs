using System;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using OF.Infrastructure.Controls.ListBox;
using OF.Infrastructure.Helpers.AttachedProperty;
using OF.Core.Extensions;

namespace OF.Infrastructure.Behaviors
{
    public class ChildListBoxBehavior : Behavior<ListBox>
    {
        private int _parentIndex = -1;
        private OFListBox Parent { get; set; }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.SelectionChanged += AssociatedObjectOnSelectionChanged;
            var listBoxItemParent = AssociatedObject.GetParentCore<ListBoxItem>();
            if (listBoxItemParent == null)
                return;
            Parent = listBoxItemParent.GetParent<OFListBox>();
            if (Parent == null)
                return;
            Parent.ResetSelection += ParentOnSelectionChanged;
            _parentIndex = Parent.Items.IndexOf(listBoxItemParent.DataContext);
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.SelectionChanged -= AssociatedObjectOnSelectionChanged;
            if (Parent != null)
            {
                Parent.SelectionChanged -= ParentOnSelectionChanged;
            }
        }

        private void AssociatedObjectOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Debug.WriteLine("CHILD");
            if (e.AddedItems != null && e.AddedItems.Count > 0)
            {
                ListBoxShouldSetSelectAttachedProperty.SetShouldSetSelect(Parent, false);
                Parent.SelectedIndex = _parentIndex;
                ListBoxSelectedObjectAttachedProperty.SetSelectedObject(Parent, AssociatedObject.SelectedItem);
                ListBoxShouldSetSelectAttachedProperty.SetShouldSetSelect(Parent, true);
            }
        }

        private void ParentOnSelectionChanged(object sender, EventArgs args)
        {
            if (Parent == null || Parent.SelectedIndex < 0 || Parent.SelectedIndex == _parentIndex)
                return;
            Parent.IsChildUnselectAll = true;
            AssociatedObject.UnselectAll();
            Parent.IsChildUnselectAll = false;
        }

    }
}