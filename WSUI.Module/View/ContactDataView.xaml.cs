using System.Windows.Controls;
using System.Windows.Media;
using OF.Core.Interfaces;
using OF.Infrastructure.Service;
using OF.Module.Interface;
using OF.Module.Interface.View;
using OF.Module.ViewModel;

namespace OF.Module.View
{
    /// <summary>
    /// Interaction logic for ContactDataView.xaml
    /// </summary>
    public partial class ContactDataView : IDataView<ContactViewModel>, INavigationView
    {
        public ContactDataView()
        {
            InitializeComponent();
        }

        #region Implementation of IDataView<ContactViewModel>

        public ContactViewModel Model 
        {
            get { return DataContext as ContactViewModel; }
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
