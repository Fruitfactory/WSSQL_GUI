using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using C4F.DevKit.PreviewHandler.PreviewHandlerHost;
using WSUI.Module.Interface;
using WSUI.Module.Service;
using WSUI.Module.ViewModel;

namespace WSUI.Module.View
{
    /// <summary>
    /// Interaction logic for PreviewView.xaml
    /// </summary>
    public partial class PreviewView : IPreviewView
    {
        //private PreviewHandlerHostControl _previewControl;
        
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

        #endregion


        public void Init()
        {
            //OverlayStyle style = OverlayStyle.WPF;
            //if (style == OverlayStyle.WPF)
            //{
            //    PreviewWindow wbo = new PreviewWindow(_previewTarget);
            //    //_previewControl = wbo.Preview;

            //}
            //else if (style == OverlayStyle.WinForms)
            //{
            //    PreviewOverlayWF wbo = new PreviewOverlayWF(_previewTarget);
            //    _previewControl = wbo.Preview;
            //}
        }

        private void resetPopup()
        {
            var offset = popup.HorizontalOffset;
            popup.HorizontalOffset = offset + 1;
            popup.HorizontalOffset = offset;

            // Resizing
            popup.Width = grid.ActualWidth;
            popup.Height = grid.ActualHeight;
            //popup.PlacementRectangle = new Rect(0,0, _previewControl.Width,_previewControl.Height);
            popup.Placement = PlacementMode.Relative;
        }

        private void MetroContentControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            resetPopup();
        }

    }
}
