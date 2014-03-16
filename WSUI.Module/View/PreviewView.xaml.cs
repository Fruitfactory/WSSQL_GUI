using System.Windows;
using System.Windows.Interop;
using WSPreview.PreviewHandler.PreviewHandlerHost;
using WSUI.Core.Interfaces;
using WSUI.Module.Interface;
using WSUI.Module.ViewModel;
using WSUI.Core.Win32;

namespace WSUI.Module.View
{
    /// <summary>
    /// Interaction logic for PreviewView.xaml
    /// </summary>
    public partial class PreviewView : IPreviewView
    {
        public PreviewView()
        {
            InitializeComponent();
            
        }

        #region Implementation of IPreviewView

        public MainViewModel Model
        {
            get { return DataContext as MainViewModel; }
            set { DataContext = value; }
        }
        public bool SetPreviewFile(string filename)
        {
            if (_previewControl == null)
                return false;
            _previewControl.FilePath = filename;
            return true;
        }

        public void SetSearchPattern(string pattern)
        {
            if (!string.IsNullOrEmpty(pattern))
                _previewControl.SearchCriteria = pattern;
        }

        public void ClearPreview()
        {
            _previewControl.UnloadPreview();
        }

        public void PassActionForPreview(IWSAction action)
        {
            _previewControl.PassAction(action);
        }

        #endregion


        public void Init()
        {
           
        }

        private void resetPopup()
        {
            //var offset = popup.HorizontalOffset;
            //popup.HorizontalOffset = offset + 1;
            //popup.HorizontalOffset = offset;

            //// Resizing
            //popup.Width = grid.ActualWidth;
            //popup.Height = grid.ActualHeight;
            ////popup.PlacementRectangle = new Rect(0, 0, grid.Width, grid.Height);
            //popup.Placement = PlacementMode.Center;
        }

        private void MetroContentControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            resetPopup();
        }



        
    }
}
