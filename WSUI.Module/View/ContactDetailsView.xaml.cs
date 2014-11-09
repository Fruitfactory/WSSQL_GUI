using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WSUI.Core.Extensions;
using WSUI.Core.Interfaces;
using WSUI.Infrastructure.Service;
using WSUI.Module.Interface.View;
using WSUI.Module.Interface.ViewModel;

namespace WSUI.Module.View
{
    /// <summary>
    /// Interaction logic for ContactDetailsView.xaml
    /// </summary>
    public partial class ContactDetailsView : IContactDetailsView, INavigationView
    {
        public ContactDetailsView()
        {
            InitializeComponent();
            WsuiListBox.SelectionChanged += WsuiListBoxOnSelectionChanged;
            ListBox.SelectionChanged += ListBoxOnSelectionChanged;
        }

        private void ListBoxOnSelectionChanged(object sender, SelectionChangedEventArgs selectionChangedEventArgs)
        {
            WsuiListBox.SelectedIndex = -1;
        }

        private void WsuiListBoxOnSelectionChanged(object sender, SelectionChangedEventArgs selectionChangedEventArgs)
        {
            ListBox.SelectedIndex = -1;
        }

        public IContactDetailsViewModel Model
        {
            get { return DataContext as IContactDetailsViewModel; }
            set { DataContext = value; }
        }

        double IContactDetailsView.ActualHeight
        {
            get 
            {
                this.InvalidateMeasure();
                return MainTabGrid.RenderSize.Height; 
            }
        }

        double IContactDetailsView.ActualFileHeight
        {
            get { return FileHeaderControl.RenderSize.Height; }
        }


        private void ListBox_OnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (!(DataContext is IScrollableView))
                return;
            var scrollViewer = VisualTreeHelper.GetChild(listBox, 0) as ScrollViewer;
            if (scrollViewer == null) return;
            var scrollChanged = new ScrollData() { ScrollableHeight = scrollViewer.ScrollableHeight, VerticalOffset = e.VerticalOffset };
            (DataContext as IScrollableView).ScrollChangeCommand.Execute(scrollChanged);
        }

        private void ListBox_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (!(DataContext is IScrollableViewExtended))
                return;
            var scrollViewer = FileListBox.GetListBoxScrollViewer();
            if (scrollViewer == null) return;
            var scrollChanged = new ScrollData() { ScrollableHeight = scrollViewer.ScrollableHeight, VerticalOffset = e.VerticalOffset };
            (DataContext as IScrollableViewExtended).ScrollChangedCommand2.Execute(scrollChanged);
        }
    }
}
