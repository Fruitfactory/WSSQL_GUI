using WSUI.Module.Interface;
using WSUI.Module.Interface.View;
using WSUI.Module.ViewModel;

namespace WSUI.Module.View
{
    /// <summary>
    /// Interaction logic for KindsView.xaml
    /// </summary>
    public partial class KindsView : IKindsView
    {
        public KindsView()
        {
            InitializeComponent();
        }

        #region Implementation of IKindsView

        public MainViewModel Model
        {
            get { return DataContext as MainViewModel; }
            set { DataContext = value; }
        }

        #endregion
    }
}
