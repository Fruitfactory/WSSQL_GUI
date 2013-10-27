using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using MahApps.Metro.Controls;
using WSUI.Infrastructure.Helpers.Extensions;
using WSUI.Infrastructure.Service;
using WSUI.Module.Interface;
using WSUI.Module.ViewModel;

namespace WSUI.Module.View
{
    /// <summary>
    /// Interaction logic for AllFilesDataView.xaml
    /// </summary>
    public partial class AllFilesDataView : IDataView<AllFilesViewModel>
    {
        private int _oldIndex = -1;

        public AllFilesDataView()
        {
            InitializeComponent();
            listBox.SelectionChanged += ListBoxOnSelectionChanged;
        }

        private void ListBoxOnSelectionChanged(object sender, SelectionChangedEventArgs selectionChangedEventArgs)
        {
            listBox.OnSelectedChanged(ref _oldIndex);
        }

        #region Implementation of IDataView<AllFilesViewModel>

        public AllFilesViewModel Model 
        {
            get { return DataContext as AllFilesViewModel; }
            set { DataContext = value; }
        }

        #endregion

        private void ListBox_ScrollChanged_1(object sender, ScrollChangedEventArgs e)
        {
            if (DataContext == null ||
                !(DataContext is IScrollableView))
                return;
            int count = VisualTreeHelper.GetChildrenCount(listBox);
            var scrollViewer = VisualTreeHelper.GetChild(listBox, 0) as ScrollViewer;
            if (scrollViewer == null) return;

            var scrollChanged = new ScrollData() { ScrollableHeight = scrollViewer.ScrollableHeight, VerticalOffset = e.VerticalOffset };
            (DataContext as IScrollableView).ScrollChangeCommand.Execute(scrollChanged);
        }
    }
}
