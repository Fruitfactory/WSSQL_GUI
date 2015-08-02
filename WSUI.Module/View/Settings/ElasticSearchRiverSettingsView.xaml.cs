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
using OF.Module.Interface.View;
using OF.Module.Interface.ViewModel;

namespace OF.Module.View.Settings
{
    /// <summary>
    /// Interaction logic for ElasticSearchRiverSettingsView.xaml
    /// </summary>
    public partial class ElasticSearchRiverSettingsView : UserControl, IElasticSearchRiverSettingsView
    {
        public ElasticSearchRiverSettingsView()
        {
            InitializeComponent();
        }

        public IElasticSearchRiverSettingsViewModel Model
        {
            get 
            { 
                return DataContext as IElasticSearchRiverSettingsViewModel;
            }
            set { DataContext = value; }
        }
    }
}
