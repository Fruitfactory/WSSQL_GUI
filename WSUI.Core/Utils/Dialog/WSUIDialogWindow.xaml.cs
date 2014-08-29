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
using System.Windows.Threading;
using MahApps.Metro.Controls;
using WSUI.Core.Utils.Dialog.Interfaces;

namespace WSUI.Core.Utils.Dialog
{
    /// <summary>
    /// Interaction logic for WSUIDialogWindow.xaml
    /// </summary>
    public partial class WSUIDialogWindow : MahApps.Metro.Controls.MetroWindow
    {
        private IWSUIViewModel _viewModel = null;

        public WSUIDialogWindow(IWSUIViewModel viewModel)
        {
            _viewModel = viewModel;
            InitializeComponent();
            Dispatcher.BeginInvoke((Action)(() => this.DataContext = _viewModel));
        }

        private void BtnOk_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void BtnCancel_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
