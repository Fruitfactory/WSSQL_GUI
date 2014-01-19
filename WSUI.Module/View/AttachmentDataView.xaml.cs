﻿using System.Windows.Controls;
using System.Windows.Media;
using WSUI.Infrastructure.Service;
using WSUI.Module.Interface;
using WSUI.Module.ViewModel;

namespace WSUI.Module.View
{
    /// <summary>
    /// Interaction logic for AttachmentDataView.xaml
    /// </summary>
    public partial class AttachmentDataView : IDataView<AttachmentViewModel>
    {
        public AttachmentDataView()
        {
            InitializeComponent();
        }

        public AttachmentViewModel Model
        {
            get { return DataContext as AttachmentViewModel; }
            set { DataContext = value; }
        }

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
