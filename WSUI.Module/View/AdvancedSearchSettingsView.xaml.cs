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
using WSUI.Module.Interface.View;
using WSUI.Module.ViewModel;

namespace WSUI.Module.View
{
    /// <summary>
    /// Interaction logic for AdvancedSearchSettingsView.xaml
    /// </summary>
    public partial class AdvancedSearchSettingsView : ISettingsView<AdvancedSearchViewModel>
    {
        public AdvancedSearchSettingsView()
        {
            InitializeComponent();
        }

        public AdvancedSearchViewModel Model
        {
            get { return DataContext as AdvancedSearchViewModel; }
            set { DataContext = value; }
        }
    }
}
