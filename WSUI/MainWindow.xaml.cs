using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro;
using MahApps.Metro.Controls;
using WSUI.Module.Interface;
using WSUI.Infrastructure.Controls.ProgressAdorner;

namespace WSUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : IMainView//: Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        #region Implementation of IMainView

        public IMainViewModel Model
        {
            get { return DataContext as IMainViewModel; } 
            set
            {
                if(value != null)
                {
                    DataContext = value;
                    (DataContext as IMainViewModel).Start += OnStart;
                    (DataContext as IMainViewModel).Complete += OnComplete;
                }
            }
        }

        #endregion


        private void OnStart(object sender,  EventArgs e)
        {
            this.Cursor = Cursors.AppStarting;
        }

        private void OnComplete(object sender, EventArgs e)
        {

            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                                                                      {
                                                                          Cursor = Cursors.Arrow;
                                                                      }));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ThemeManager.ChangeTheme(this, ThemeManager.DefaultAccents.First(a => a.Name == "Blue"), Theme.Light);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ThemeManager.ChangeTheme(this, ThemeManager.DefaultAccents.First(a => a.Name == "Blue"), Theme.Dark);
        }

        public override void EndInit()
        {
            base.EndInit();
            this.InvalidateVisual();
        }

        protected override void OnClosed(EventArgs e)
        {
            (DataContext as IMainViewModel).Clear();
            base.OnClosed(e);
        }

    }
}
