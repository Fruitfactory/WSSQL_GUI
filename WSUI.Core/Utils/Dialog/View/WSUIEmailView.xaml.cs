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
using WSUI.Core.Utils.Dialog.Interfaces;

namespace WSUI.Core.Utils.Dialog.View
{
    /// <summary>
    /// Interaction logic for WSUIEmailView.xaml
    /// </summary>
    public partial class WSUIEmailView : UserControl,IWSUIView
    {
        public WSUIEmailView()
        {
            InitializeComponent();
        }
    }
}
