using OF.Module.Interface;
using OF.Module.Interface.View;
using OF.Module.ViewModel;

namespace OF.Module.View
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
