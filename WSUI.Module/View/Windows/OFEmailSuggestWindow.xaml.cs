using System;
using System.Collections.Generic;
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
using OF.Core.Win32;
using OF.Module.Interface.View;

namespace OF.Module.View.Windows
{
    /// <summary>
    /// Interaction logic for OFEmailSuggestWindow.xaml
    /// </summary>
    public partial class OFEmailSuggestWindow : IOFEmailSuggestWindow
    {
        public OFEmailSuggestWindow()
        {
            InitializeComponent();
            this.PreviewKeyDown += OFEmailSuggestWindow_PreviewKeyDown;
        }

        private void OFEmailSuggestWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    Hide();
                    break;
            }
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
            Focus();
            Show();
            Left = rect.Left;
            Top = rect.Bottom;
        }


        public void HideSuggestings()
        {
            Hide();
        }

        public new bool IsVisible
        {
            get { return base.IsVisible; }
        }
    }
}
