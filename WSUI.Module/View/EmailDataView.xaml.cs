using System.Windows.Controls;
using System.Windows.Media;
using WSUI.Infrastructure.Helpers.Extensions;
using WSUI.Module.Interface;
using WSUI.Module.Interface.View;
using WSUI.Module.ViewModel;
using WSUI.Infrastructure.Service;

namespace WSUI.Module.View
{
    /// <summary>
    /// Interaction logic for EmailDataView.xaml
    /// </summary>
    public partial class EmailDataView : IDataView<EmailViewModel>
    {
        private int _oldIndex = -1;

        public EmailDataView()
        {
            InitializeComponent();
            listBox.SelectionChanged += ListBoxOnSelectionChanged;
        }

        private void ListBoxOnSelectionChanged(object sender, SelectionChangedEventArgs selectionChangedEventArgs)
        {
            listBox.OnSelectedChanged(ref _oldIndex);
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
