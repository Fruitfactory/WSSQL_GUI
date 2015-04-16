using System;
using System.Windows;
using System.Windows.Threading;
using OF.Core.Utils.Dialog.Interfaces;

namespace OF.Core.Utils.Dialog
{
    /// <summary>
    /// Interaction logic for OFDialogWindow.xaml
    /// </summary>
    public partial class OFDialogWindow : MahApps.Metro.Controls.MetroWindow
    {
        private IOFViewModel _viewModel = null;

        public OFDialogWindow(IOFViewModel viewModel)
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