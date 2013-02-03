using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using WSUI.Module.Service.Dialogs.Interfaces;

namespace WSUI.Module.Service.Dialogs.Message
{
    public class MessageBoxService
    {

        private static Lazy<MessageBoxService> _service = new Lazy<MessageBoxService>(() =>
                                                                                          {
                                                                                              return
                                                                                                  new MessageBoxService();
                                                                                          });

        public static MessageBoxService Instance
        {
            get { return _service.Value; }
        }


        public bool? Show(string title,string message)
        {
            IMessageModel model = new MessageModel()
                                      {
                                          Title = title,
                                          Message = message,
                                          Icon = MessageBoxImage.Information,
                                          Button = MessageBoxButton.OK
                                      };
            return new MessageView(model).ShowDialog();
        }

        public bool? Show(string title,string message,MessageBoxButton button)
        {
            IMessageModel model = new MessageModel()
                                      {
                                          Title = title,
                                          Message = message,
                                          Icon = MessageBoxImage.Information,
                                          Button = button
                                      };
            return new MessageView(model).ShowDialog();
        }

        public bool? Show(string title,string message,MessageBoxButton button,MessageBoxImage image)
        {
            IMessageModel model = new MessageModel()
                                      {
                                          Title = title,
                                          Message = message,
                                          Icon = image,
                                          Button = button
                                      };
            return new MessageView(model).ShowDialog();
        }

    }
}
