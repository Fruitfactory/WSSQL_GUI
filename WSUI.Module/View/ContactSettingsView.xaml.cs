using WSUI.Module.Interface;
using WSUI.Module.ViewModel;

namespace WSUI.Module.View
{
    /// <summary>
    /// Interaction logic for ContactSettingsView.xaml
    /// </summary>
    public partial class ContactSettingsView : ISettingsView<ContactViewModel>
    {
        public ContactSettingsView()
        {
            InitializeComponent();
        }

        #region Implementation of ISettingsView<ContactViewModel>

        public ContactViewModel Model
        {
            get { return DataContext as ContactViewModel; }
            set { DataContext = value; }
        }

        #endregion
    }
}
