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
using WSUI.Module.Service.Dialogs.Interfaces;
using WSUI.Infrastructure.Controls.ProgressManager;

namespace WSUI.Module.Service.Dialogs.Message
{
    /// <summary>
    /// Interaction logic for MessageView.xaml
    /// </summary>
    public partial class MessageView
    {

        private IMessageModel _model;


        public MessageView(IMessageModel messageModel)
        {
            _model = messageModel;
            DataContext = _model;
            InitializeComponent();
            InitView();

        }


        private void InitView()
        {
            
            switch (_model.Button)
            {
                case MessageBoxButton.OK:
                    OkVisible();
                    break;
                case MessageBoxButton.OKCancel:
                    OkVisible();
                    CancelVisible();
                    break;
                case MessageBoxButton.YesNo:
                    YesVisible();
                    NoVisible();
                    break;
                case MessageBoxButton.YesNoCancel:
                    YesVisible();
                    NoVisible();
                    CancelVisible();
                    break;
            }
        }


        private void OkVisible()
        {
            btnOkYes.Visibility = Visibility.Visible;
            btnOkYes.Content = MessageBoxResult.OK.ToString();
        }

        private void YesVisible()
        {
            btnOkYes.Visibility = Visibility.Visible;
            btnOkYes.Content = MessageBoxResult.Yes.ToString();
        }
        
        private void CancelVisible()
        {
            btnCancel.Visibility = Visibility.Visible;
            btnCancel.Content = MessageBoxResult.Cancel.ToString();
        }

        private void NoVisible()
        {
            btnNo.Visibility = Visibility.Visible;
            btnNo.Content = MessageBoxResult.No.ToString();
        }

        private void btnOkYes_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void btnNo_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

    }
}
