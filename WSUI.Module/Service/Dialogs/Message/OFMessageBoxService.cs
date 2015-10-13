using System;
using System.Windows;
using OF.Module.Service.Dialogs.Interfaces;

namespace OF.Module.Service.Dialogs.Message
{
    public class OFMessageBoxService
    {

        private static Lazy<OFMessageBoxService> _service = new Lazy<OFMessageBoxService>(() =>
                                                                                          {
                                                                                              return
                                                                                                  new OFMessageBoxService();
                                                                                          });

        public static OFMessageBoxService Instance
        {
            get { return _service.Value; }
        }


        public bool? Show(string title,string message)
        {
            IMessageModel model = new OFMessageModel()
                                      {
                                          Title = title,
                                          Message = message,
                                          Icon = MessageBoxImage.Information,
                                          Button = MessageBoxButton.OK
                                      };
            return new OFMessageView(model).ShowDialog();
        }

        public bool? Show(string title,string message,MessageBoxButton button)
        {
            IMessageModel model = new OFMessageModel()
                                      {
                                          Title = title,
                                          Message = message,
                                          Icon = MessageBoxImage.Information,
                                          Button = button
                                      };
            return new OFMessageView(model).ShowDialog();
        }

        public bool? Show(string title,string message,MessageBoxButton button,MessageBoxImage image)
        {
            IMessageModel model = new OFMessageModel()
                                      {
                                          Title = title,
                                          Message = message,
                                          Icon = image,
                                          Button = button
                                      };
            return new OFMessageView(model).ShowDialog();
        }

    }
}
