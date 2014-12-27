using System.Windows.Controls;
using System.Windows.Media;
using WSUI.Core.Interfaces;
using WSUI.Infrastructure.Service;
using WSUI.Module.Interface.View;
using WSUI.Module.ViewModel;

namespace WSUI.Module.View
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

            var scrollChanged = new ScrollData
            {
                ScrollableHeight = scrollViewer.ScrollableHeight,
                VerticalOffset = e.VerticalOffset
            };
            (DataContext as IScrollableView).ScrollChangeCommand.Execute(scrollChanged);
        }
    }
}