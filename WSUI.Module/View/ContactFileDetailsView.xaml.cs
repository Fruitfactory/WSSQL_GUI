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
using OF.Core.Data;
using OF.Core.Extensions;
using OF.Infrastructure.Service;
using OF.Module.Interface.View;
using OF.Module.Interface.ViewModel;

namespace OF.Module.View
{
    /// <summary>
    /// Interaction logic for ContactFileDetailasView.xaml
    /// </summary>
    public partial class ContactFileDetailsView : IContactKindDetailsView<AttachmentSearchObject>
    {
        public ContactFileDetailsView()
        {
            InitializeComponent();
        }

        public IContactKindDetailsViewModel<AttachmentSearchObject> Model { get; set; }

        private void ListBox_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (!(DataContext is IScrollableViewExtended))
                return;
            var scrollViewer = FileListBox.GetListBoxScrollViewer();
            if (scrollViewer == null) return;
            var scrollChanged = new ScrollData() { ScrollableHeight = scrollViewer.ScrollableHeight, VerticalOffset = e.VerticalOffset };
            (DataContext as IScrollableViewExtended).ScrollChangedCommand2.Execute(scrollChanged);
        }
    }
}
