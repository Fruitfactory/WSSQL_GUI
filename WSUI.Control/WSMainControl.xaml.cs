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
using WSUI.Control.Interfaces;
using WSUI.Module.Interface;

namespace WSUI.Control
{
    /// <summary>
    /// Interaction logic for WSMainControl.xaml
    /// </summary>
    public partial class WSMainControl : UserControl, IMainView, IWSMainControl
    {
        public WSMainControl()
        {
            InitializeComponent();
        }

        #region Implementation of IMainView

        public IMainViewModel Model
        {
            get { return DataContext as IMainViewModel; }
            set
            {
                if (value != null)
                {
                    DataContext = value;
                    (DataContext as IMainViewModel).Start += OnStart;
                    (DataContext as IMainViewModel).Complete += OnComplete;
                }
            }
        }

        #endregion

        private void OnStart(object sender, EventArgs e)
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

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
        }

        public event EventHandler Close;

        private void CloseButton_OnClick(object sender, RoutedEventArgs e)
        {
            EventHandler temp = Close;
            if (temp != null)
            {
                temp(this, EventArgs.Empty);
            }
        }

    }
}
