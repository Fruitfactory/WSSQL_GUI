﻿using System.Windows.Controls;
using System.Windows.Media;
using WSUI.Infrastructure.Service;
using WSUI.Module.Interface;
using WSUI.Module.Interface.View;
using WSUI.Module.ViewModel;
using WSUI.Core.Interfaces;

namespace WSUI.Module.View
{
    /// <summary>
    /// Interaction logic for AllFilesDataView.xaml
    /// </summary>
    public partial class AllFilesDataView : IDataView<AllFilesViewModel>,INavigationView
    {
        public AllFilesDataView()
        {
            InitializeComponent();
            listBox.SelectionChanged += ListBoxOnSelectionChanged;
        }

        private void ListBoxOnSelectionChanged(object sender, SelectionChangedEventArgs selectionChangedEventArgs)
        {
            //listBox.OnSelectedChanged(ref _oldIndex);
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
