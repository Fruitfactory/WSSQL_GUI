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
using WSUI.Module.Interface.ViewModel;

namespace WSUI.Module.View
{
    /// <summary>
    /// Interaction logic for ElasticSearchView.xaml
    /// </summary>
    public partial class ElasticSearchView : UserControl, IElasticSearchView
    {
        public ElasticSearchView()
        {
            InitializeComponent();
        }

        public IElasticSearchViewModel Model
        {
            get { return DataContext as IElasticSearchViewModel; }
            set { DataContext = value; }
        }
    }
}
