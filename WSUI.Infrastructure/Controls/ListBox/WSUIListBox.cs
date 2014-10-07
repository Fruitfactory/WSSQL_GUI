﻿using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using WSUI.Infrastructure.Helpers.AttachedProperty;
using WSUI.Core.Extensions;

namespace WSUI.Infrastructure.Controls.ListBox
{
    public class WSUIListBox : System.Windows.Controls.ListBox
    {

        public EventHandler ResetSelection;


        public WSUIListBox()
        {
            DefaultStyleKey = typeof(System.Windows.Controls.ListBox);
            IsChildUnselectAll = false;
        }

        public bool IsChildUnselectAll { get; set; }

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            if(IsChildUnselectAll)
                return;
            Debug.WriteLine("MAIN");
            if(e.AddedItems != null && e.AddedItems.Count > 0)
                base.OnSelectionChanged(e);
            RaiseResetSelection();
            if (ReferenceEquals(this, e.Source) && ListBoxShouldSetSelectAttachedProperty.GetShouldSetSelect(this) && SelectedItem != null)
            {
                ListBoxSelectedObjectAttachedProperty.SetSelectedObject(this, SelectedItem);
            }
        }

        private void RaiseResetSelection()
        {
            EventHandler temp = this.ResetSelection;
            if (temp != null)
                temp(this, EventArgs.Empty);
        }
    }
}