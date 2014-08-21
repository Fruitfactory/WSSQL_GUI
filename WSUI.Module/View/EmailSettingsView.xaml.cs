using WSUI.Module.Interface;
using WSUI.Module.Interface.View;
using WSUI.Module.ViewModel;

namespace WSUI.Module.View
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
