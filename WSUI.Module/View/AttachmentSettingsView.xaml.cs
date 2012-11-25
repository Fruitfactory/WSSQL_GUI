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
using WSUI.Module.Interface;
using WSUI.Module.ViewModel;

namespace WSUI.Module.View
{
    /// <summary>
    /// Interaction logic for AttachmentSettingsView.xaml
    /// </summary>
    public partial class AttachmentSettingsView : ISettingsView<AttachmentViewModel>
    {
        public AttachmentSettingsView()
        {
            InitializeComponent();
        }

        public AttachmentViewModel Model
        {
            get { return DataContext as AttachmentViewModel; }
            set { DataContext = value; }
        }
    }
}
