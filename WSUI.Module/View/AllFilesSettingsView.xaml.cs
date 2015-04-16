using System.Windows.Input;
using OF.Module.Interface;
using OF.Module.Interface.View;
using OF.Module.ViewModel;

namespace OF.Module.View
{
    /// <summary>
    /// Interaction logic for AllFilesSettingsView.xaml
    /// </summary>
    public partial class AllFilesSettingsView : ISettingsView<AllFilesViewModel>
    {
        public AllFilesSettingsView()
        {
            InitializeComponent();
        }

        #region Implementation of ISettingsView<AllFilesViewModel>

        public AllFilesViewModel Model
        {
            get { return DataContext as AllFilesViewModel; }
            set { DataContext = value; }  
        }

        #endregion

    }
}
