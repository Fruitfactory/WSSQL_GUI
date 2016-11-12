using System;
using System.Collections.Generic;
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
using Microsoft.Practices.Unity;
using OF.Core.Extensions;
using OF.Module.Interface.Service;
using OF.Module.Interface.ViewModel;

namespace OF.Module.View.Windows
{
    /// <summary>
    /// Interaction logic for OFMainSettingsWindow.xaml
    /// </summary>
    public partial class OFMainSettingsWindow : IMainSettingsWindow
    {
        private IUnityContainer _unityContainer;

        public OFMainSettingsWindow(IUnityContainer unityContainer)
        {
            InitializeComponent();
            _unityContainer = unityContainer;
            var model = _unityContainer.Resolve<IMainSettingsViewModel>();
            DataContext = model;
            model.Initialize();
            model.Close += ModelOnClose;
        }

        private void ModelOnClose(object sender, EventArgs eventArgs)
        {
            if (DataContext is IMainSettingsViewModel)
            {
                (DataContext as IMainSettingsViewModel).Close -= ModelOnClose;
            }
            Close();
        }

        public void ShowModal()
        {
            var wndInterop = new WindowInteropHelper(this);
            wndInterop.Owner = GetOutlookParent();
            ShowDialog();
        }

        private IntPtr GetOutlookParent()
        {
            var outlook = Process.GetProcesses().Where(p => p.ProcessName.ToUpper().StartsWith("OUTLOOK")).FirstOrDefault();
            return outlook.IsNotNull() ? outlook.MainWindowHandle : IntPtr.Zero;
        }
    }
}
