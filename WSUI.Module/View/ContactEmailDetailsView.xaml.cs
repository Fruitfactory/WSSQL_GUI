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
using WSUI.Core.Data;
using WSUI.Infrastructure.Service;
using WSUI.Module.Interface.View;
using WSUI.Module.Interface.ViewModel;

namespace WSUI.Module.View
{
    /// <summary>
    /// Interaction logic for ContactEmailDetailsView.xaml
    /// </summary>
    public partial class ContactEmailDetailsView : IContactKindDetailsView<EmailSearchObject>
    {
        public ContactEmailDetailsView()
        {
            InitializeComponent();
        }

        public IContactKindDetailsViewModel<EmailSearchObject> Model { get; set; }

        private void ListBox_OnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (!(DataContext is IScrollableView))
                return;
            var scrollViewer = VisualTreeHelper.GetChild(listBox, 0) as ScrollViewer;
            if (scrollViewer == null) return;
            var scrollChanged = new ScrollData() { ScrollableHeight = scrollViewer.ScrollableHeight, VerticalOffset = e.VerticalOffset };
            (DataContext as IScrollableView).ScrollChangeCommand.Execute(scrollChanged);
        }
    }
}
