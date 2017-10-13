using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Practices.Prism.Events;
using OF.Core.Data.ElasticSearch;
using OF.Core.Extensions;
using OF.Core.Win32;
using OF.Infrastructure.Events;
using OF.Infrastructure.Payloads;
using OF.Module.Interface.View;

namespace OF.Module.View.Windows
{
    /// <summary>
    /// Interaction logic for OFEmailSuggestWindow.xaml
    /// </summary>
    public partial class OFEmailSuggestWindow : IOFEmailSuggestWindow
    {
        private readonly IEventAggregator _eventAggregator;


        public OFEmailSuggestWindow(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            InitializeComponent();
            ListBox.KeyDown += ListBox_OnKeyDown;
            ListBox.PreviewKeyDown += ListBox_OnPreviewKeyDown;

        }

        public object Model
        {
            get { return DataContext; }
            set { DataContext = value; }
        }

        public void ShowSuggestings(IntPtr hWnd)
        {
            var windowInterop = new WindowInteropHelper(this)
            {
                Owner = hWnd
            };
            
            WindowsFunction.RECT rect;
            WindowsFunction.GetWindowRect(hWnd, out rect);
            Show();
            Left = rect.Left;
            Top = rect.Bottom;
            WindowsFunction.SetFocus(hWnd);
        }


        public void HideSuggestings()
        {
            Hide();
        }

        public void JumpToEmailList()
        {
            ListBox.Focus();
            ListBox.SelectedIndex = 0;
            var container = ListBox.ItemContainerGenerator.ContainerFromItem(ListBox.SelectedItem) as ListBoxItem;
            if (container.IsNotNull())
            {
                container.Focus();
            }
        }

        public bool IsClosed { get; private set; }

        public new bool IsVisible
        {
            get { return base.IsVisible; }
        }

        protected override void OnClosed(EventArgs e)
        {
            IsClosed = true;
            base.OnClosed(e);
        }
        
        private void ListBox_OnKeyDown(object sender, KeyEventArgs e)
        {
            ProcessKeyDown(e);
        }

        private void ListBox_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            ProcessKeyDown(e);
        }

        private void ProcessKeyDown(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    ListBox.SelectedItem = null;
                    Hide();
                    break;
                case Key.Return:
                    _eventAggregator.GetEvent<OFSelectSuggestEmailEvent>().Publish(true);
                    Hide();
                    break;
            }
        }


        public void Dispose()
        {
            if (ListBox.IsNotNull())
            {
                ListBox.KeyDown -= ListBox_OnKeyDown;
                ListBox.PreviewKeyDown -= ListBox_OnPreviewKeyDown;
            }
            
        }
    }
}
