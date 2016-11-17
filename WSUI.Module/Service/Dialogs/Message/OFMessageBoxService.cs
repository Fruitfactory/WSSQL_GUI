using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Interop;
using OF.Core.Extensions;
using OF.Core.Win32;
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
            return InternalShowMessage(model);
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
            return InternalShowMessage(model);
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
            return InternalShowMessage(model);
        }


        private bool? InternalShowMessage(IMessageModel model)
        {
            var dialog = new OFMessageView(model);
            var wndInterop = new WindowInteropHelper(dialog);
            wndInterop.Owner = GetOutlookParent();
            return dialog.ShowDialog();
        }

        private IntPtr GetOutlookParent()
        {
            var outlook = Process.GetProcesses().Where(p => p.ProcessName.ToUpper().StartsWith("OUTLOOK")).FirstOrDefault();
            return outlook.IsNotNull() ? outlook.MainWindowHandle : IntPtr.Zero;
        }

    }
}
