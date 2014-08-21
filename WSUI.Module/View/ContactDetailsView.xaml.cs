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
    /// Interaction logic for ContactDetailsView.xaml
    /// </summary>
    public partial class ContactDetailsView : IContactDetailsView
    {
        public ContactDetailsView()
        {
            InitializeComponent();
        }

        public IContactDetailsViewModel Model
        {
            get { return DataContext as IContactDetailsViewModel; }
            set { DataContext = value; }
        }
    }
}
