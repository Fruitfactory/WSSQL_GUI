using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using WSUI.Control.Interfaces;
using WSUI.Module.Enums;
using WSUI.Module.Interface;
using WSUI.Module.Interface.View;
using WSUI.Module.Interface.ViewModel;
using WSUI.Module.Service;

namespace WSUI.Control
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