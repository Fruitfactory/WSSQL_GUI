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
using OF.Infrastructure.Service;
using OF.Module.Interface.View;
using OF.Module.Interface.ViewModel;

namespace OF.Module.View
{
    /// <summary>
    /// Interaction logic for ContactEmailDetailsView.xaml
    /// </summary>
    public partial class ContactEmailDetailsView : IContactKindDetailsView<OFEmailSearchObject>
    {
        public ContactEmailDetailsView()
        {
            InitializeComponent();
        }

        public IContactKindDetailsViewModel<OFEmailSearchObject> Model { get; set; }

        private void ListBox_OnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (!(DataContext is IScrollableView))
                return;
            var scrollViewer = VisualTreeHelper.GetChild(listBox, 0) as ScrollViewer;
            if (scrollViewer == null) return;
            var scrollChanged = new OFScrollData() { ScrollableHeight = scrollViewer.ScrollableHeight, VerticalOffset = e.VerticalOffset };
            (DataContext as IScrollableView).ScrollChangeCommand.Execute(scrollChanged);
        }
    }
}
