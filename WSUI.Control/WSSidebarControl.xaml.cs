using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using WSUI.Control.Interfaces;
using WSUI.Module.Enums;
using WSUI.Module.Interface;
using WSUI.Module.Service;

namespace WSUI.Control
{
    /// <summary>
    /// Interaction logic for WSSidebarControl.xaml
    /// </summary>
    public partial class WSSidebarControl : UserControl, ISidebarView,IWSMainControl
    {
        public WSSidebarControl()
        {
            InitializeComponent();
        }

        public IMainViewModel Model
        {
            get
            { return DataContext as IMainViewModel; }
            set
            {
                DataContext = value;
                ModelChanged();
            }
        }

        private void ModelChanged()
        {
            Model.Slide += ModelOnSlide;
        }

        private void ModelOnSlide(object sender, SlideDirectionEventArgs slideDirectionEventArgs)
        {
            if (slideDirectionEventArgs == null)
                return;
            switch (slideDirectionEventArgs.Direction)
            {
                case UiSlideDirection.DataToPreview:
                    DataToPreview();
                    break;

                case UiSlideDirection.PreviewToData:
                    PreviewToData();
                    break;
            }
        }

        private void PreviewToData()
        {
            var storyBoard = this.Resources["PreviewToData"] as Storyboard;
            if (storyBoard == null)
                return;

            DataGrid.BeginStoryboard(storyBoard);
        }

        private void DataToPreview()
        {
            var storyBoard = this.Resources["DataToPreview"] as Storyboard;
            if (storyBoard == null)
                return;

            DataGrid.BeginStoryboard(storyBoard);
        }

        private void CloseButton_OnClick(object sender, RoutedEventArgs e)
        {
            var temp = Close;
            if (temp != null)
            {
                temp(this, EventArgs.Empty);
            }
        }

        public event EventHandler Close;
    }
}