using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using OF.Core.Interfaces;
using OF.Core.Extensions;
using OF.Infrastructure.Service;
using OF.Module.Interface;
using OF.Module.Interface.View;
using OF.Module.ViewModel;

namespace OF.Module.View
{
    /// <summary>
    /// Interaction logic for AttachmentDataView.xaml
    /// </summary>
    public partial class AttachmentDataView : IDataView<AttachmentViewModel>, INavigationView
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
            if (!(DataContext is IScrollableView))
                return;
            var scrollViewer = listBox.GetListBoxScrollViewer();
            if (scrollViewer == null) return;
            var scrollChanged = new OFScrollData() { ScrollableHeight = scrollViewer.ScrollableHeight, VerticalOffset = e.VerticalOffset };
            (DataContext as IScrollableView).ScrollChangeCommand.Execute(scrollChanged);
        }
    }
}
