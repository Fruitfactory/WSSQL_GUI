using System.Diagnostics;
using System.Linq;
using System.Windows;
using OF.Core.Extensions;
using OF.Module.Service.Dialogs.Interfaces;

namespace OF.Module.Service.Dialogs.Message
{
    /// <summary>
    /// Interaction logic for MessageView.xaml
    /// </summary>
    public partial class OFMessageView
    {

        private IMessageModel _model;


        public OFMessageView(IMessageModel messageModel)
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
