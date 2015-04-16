using OF.Module.Interface;
using OF.Module.Interface.View;
using OF.Module.ViewModel;

namespace OF.Module.View
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
