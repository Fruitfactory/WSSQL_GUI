using WSUI.Module.Interface;
using WSUI.Module.ViewModel;

namespace WSUI.Module.View
{
    /// <summary>
    /// Interaction logic for AttachmentSettingsView.xaml
    /// </summary>
    public partial class AttachmentSettingsView : ISettingsView<AttachmentViewModel>
    {
        public AttachmentSettingsView()
        {
            InitializeComponent();
        }

        public AttachmentViewModel Model
        {
            get { return DataContext as AttachmentViewModel; }
            set { DataContext = value; }
        }
    }
}
