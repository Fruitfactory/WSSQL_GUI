using OF.Module.Interface;
using OF.Module.Interface.View;
using OF.Module.ViewModel;

namespace OF.Module.View
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
