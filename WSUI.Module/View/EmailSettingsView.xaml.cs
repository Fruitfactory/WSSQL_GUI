using OF.Module.Interface;
using OF.Module.Interface.View;
using OF.Module.ViewModel;

namespace OF.Module.View
{
    /// <summary>
    /// Interaction logic for EmailSettingsView.xaml
    /// </summary>
    public partial class EmailSettingsView : ISettingsView<EmailViewModel>
    {
        public EmailSettingsView()
        {
            InitializeComponent();
        }

        #region Implementation of ISettingsView<EmailViewModel>

        public EmailViewModel Model
        {
            get { return DataContext as EmailViewModel; }
            set { DataContext = value; }
        }

        #endregion
    }
}
