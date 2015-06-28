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
using Microsoft.Practices.Unity;
using OF.Module.Interface.Service;
using OF.Module.Interface.ViewModel;

namespace OF.Module.View.Windows
{
    /// <summary>
    /// Interaction logic for ElasticSearchRiverSettingsWindow.xaml
    /// </summary>
    public partial class ElasticSearchRiverSettingsWindow : IElasticSearchRiverSettingsWindow
    {
        private IUnityContainer _unityContainer;

        public ElasticSearchRiverSettingsWindow(IUnityContainer unityContainer)
        {
            InitializeComponent();
            _unityContainer = unityContainer;
            var model = _unityContainer.Resolve<IElasticSearchRiverSettingsViewModel>();
            DataContext = model;
            model.Close += ModelOnClose;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (DataContext is IElasticSearchRiverSettingsViewModel)
            {
                (DataContext as IElasticSearchRiverSettingsViewModel).Initialize();
            }
        }

        private void ModelOnClose(object sender, EventArgs eventArgs)
        {
            Close();
        }

        public void ShowModal()
        {
            ShowDialog();
        }
    }
}
