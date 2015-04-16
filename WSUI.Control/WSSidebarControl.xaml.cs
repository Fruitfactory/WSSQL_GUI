using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using OF.Control.Interfaces;
using OF.Module.Enums;
using OF.Module.Interface;
using OF.Module.Interface.View;
using OF.Module.Interface.ViewModel;
using OF.Module.Service;

namespace OF.Control
{
    /// <summary>
    /// Interaction logic for WSSidebarControl.xaml
    /// </summary>
    public partial class WSSidebarControl : UserControl, ISidebarView,IWSMainControl
    {
        public WSSidebarControl()
        {
            InitializeComponent();
        }

        public IMainViewModel Model
        {
            get
            { return DataContext as IMainViewModel; }
            set
            {
                DataContext = value;
            }
        }

        private void CloseButton_OnClick(object sender, RoutedEventArgs e)
        {
            var temp = Close;
            if (temp != null)
            {
                temp(this, EventArgs.Empty);
            }
        }

        public event EventHandler Close;
    }
}