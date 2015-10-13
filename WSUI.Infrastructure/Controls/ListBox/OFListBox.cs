using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using OF.Core.Data;
using OF.Infrastructure.Helpers.AttachedProperty;
using OF.Core.Extensions;

namespace OF.Infrastructure.Controls.ListBox
{
    public class OFListBox : System.Windows.Controls.ListBox
    {

        private ScrollViewer _scrollHost;

        public EventHandler ResetSelection;



        #region [dependency property]

        public static readonly DependencyProperty CalculateActualHeightCommandProperty = DependencyProperty.Register(
            "CalculateActualHeightCommand", typeof (ICommand), typeof (OFListBox), new PropertyMetadata(default(ICommand)));

        public ICommand CalculateActualHeightCommand
        {
            get { return (ICommand) GetValue(CalculateActualHeightCommandProperty); }
            set { SetValue(CalculateActualHeightCommandProperty, value); }
        } 

        #endregion


        public OFListBox()
        {
            DefaultStyleKey = typeof(System.Windows.Controls.ListBox);
            IsChildUnselectAll = false;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _scrollHost =  VisualTreeHelper.GetChild(this, 0) as ScrollViewer;
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
            if (ReferenceEquals(this, e.Source) && OFListBoxShouldSetSelectAttachedProperty.GetShouldSetSelect(this) && SelectedItem != null)
            {
                OFListBoxSelectedObjectAttachedProperty.SetSelectedObject(this, SelectedItem);
            }
        }

        private void RaiseResetSelection()
        {
            EventHandler temp = this.ResetSelection;
            if (temp != null)
                temp(this, EventArgs.Empty);
        }

        protected override void OnPreviewMouseRightButtonDown(MouseButtonEventArgs e)
        {
            e.Handled = true;
            base.OnPreviewMouseRightButtonDown(e);
        }

        internal void RaiseCalculationHeight(OFExpanderData data)
        {
            this.InvalidateMeasure();
            if (CalculateActualHeightCommand == null)
                return;
            if (CalculateActualHeightCommand.CanExecute(data))
            {
                data.IsScrollBarVisible = _scrollHost.ComputedVerticalScrollBarVisibility == Visibility.Visible;
                data.IsVisibleOne = _scrollHost.ViewportHeight == 1;
                CalculateActualHeightCommand.Execute(data);
            }
        }

    }
}