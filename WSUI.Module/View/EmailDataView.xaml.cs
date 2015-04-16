using System.Windows.Controls;
using System.Windows.Media;
using OF.Core.Interfaces;
using OF.Core.Extensions;
using OF.Module.Interface;
using OF.Module.Interface.View;
using OF.Module.ViewModel;
using OF.Infrastructure.Service;

namespace OF.Module.View
{
    /// <summary>
    /// Interaction logic for EmailDataView.xaml
    /// </summary>
    public partial class EmailDataView : IDataView<EmailViewModel>, INavigationView
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
