using System.Windows.Controls;
using System.Windows.Media;
using OF.Core.Interfaces;
using OF.Infrastructure.Service;
using OF.Module.Interface.View;
using OF.Module.ViewModel;

namespace OF.Module.View
{
    /// <summary>
    ///     Interaction logic for AdvancesSearchDataView.xaml
    /// </summary>
    public partial class AdvancedSearchDataView : IDataView<AdvancedSearchViewModel>, INavigationView
    {
        public AdvancedSearchDataView()
        {
            InitializeComponent();
        }

        public AdvancedSearchViewModel Model
        {
            get { return DataContext as AdvancedSearchViewModel; }
            set { DataContext = value; }
        }

        private void ListBox_OnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (DataContext == null ||
                !(DataContext is IScrollableView))
                return;
            int count = VisualTreeHelper.GetChildrenCount(listBox);
            var scrollViewer = VisualTreeHelper.GetChild(listBox, 0) as ScrollViewer;
            if (scrollViewer == null) return;

            var scrollChanged = new OFScrollData
            {
                ScrollableHeight = scrollViewer.ScrollableHeight,
                VerticalOffset = e.VerticalOffset
            };
            (DataContext as IScrollableView).ScrollChangeCommand.Execute(scrollChanged);
        }
    }
}