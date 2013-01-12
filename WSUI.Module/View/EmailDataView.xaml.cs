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
using WSUI.Module.Interface;
using WSUI.Module.ViewModel;
using WSUI.Infrastructure.Service;

namespace WSUI.Module.View
{
    /// <summary>
    /// Interaction logic for EmailDataView.xaml
    /// </summary>
    public partial class EmailDataView : IDataView<EmailViewModel>
    {
        public EmailDataView()
        {
            InitializeComponent();
        }

        #region Implementation of IDataView<EmailViewModel>

        public EmailViewModel Model
        {
            get { return DataContext as EmailViewModel; }
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
