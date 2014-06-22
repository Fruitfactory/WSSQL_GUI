using System.Windows.Controls;
using WSUI.Module.Interface;

namespace WSUI.Control
{
    /// <summary>
    /// Interaction logic for WSSidebarControl.xaml
    /// </summary>
    public partial class WSSidebarControl : UserControl, ISidebarView
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
    }
}