using System.Windows;
using OF.Module.Service.Dialogs.Interfaces;

namespace OF.Module.Service.Dialogs.Message
{
    public class MessageModel :  IMessageModel
    {

        public MessageModel()
        {
            Icon = MessageBoxImage.Warning;
            Title = Icon.ToString();
            Button = MessageBoxButton.OK;
        }

        public string Title
        {
            get; set;
        }

        public string Message
        {
            get; set;
        }

        public System.Windows.MessageBoxImage Icon
        {
            get; set;
        }

        public System.Windows.MessageBoxButton Button
        {
            get; set;
        }
    }
}