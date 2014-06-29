using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using WSUI.Module.Enums;
using WSUI.Module.Interface;
using WSUI.Module.Service;

namespace WSUI.Control
{
    /// <summary>
    /// Interaction logic for WSSidebarControl.xaml
    /// </summary>
    public partial class WSSidebarControl : UserControl, ISidebarView
    {
        public WSSidebarControl()
        {
            InitializeComponent();
            (this.Resources["DataToPreview"] as Storyboard).Completed += DataToPreviewCompleted;
            (this.Resources["PreviewToData"] as Storyboard).Completed += PreviewToDataCompleted;

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
            //Model.Slide += ModelOnSlide;
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
            if(storyBoard == null)
                return;
            var dataAnimation = storyBoard.Children.FirstOrDefault(c => c.Name == "Data") as DoubleAnimation;
            var previewAnimation = storyBoard.Children.FirstOrDefault(c => c.Name == "Preview") as DoubleAnimation;
            dataAnimation.From = 0;
            dataAnimation.To = -DataGrid.ActualWidth;
            previewAnimation.From = DataGrid.ActualWidth;
            previewAnimation.To = 0;

            DataControl.Width = DataGrid.ActualWidth;
            PreviewControl.Width = DataGrid.ActualWidth;

            PreviewControl.Visibility = Visibility.Visible;
            PreviewControl.BeginStoryboard(storyBoard);

        }

        private void DataToPreview()
        {
            var storyBoard = this.Resources["DataToPreview"] as Storyboard;
            if (storyBoard == null)
                return;

            var dataAnimation = storyBoard.Children.FirstOrDefault(c => c.Name == "Data") as DoubleAnimation;
            var previewAnimation = storyBoard.Children.FirstOrDefault(c => c.Name == "Preview") as DoubleAnimation;
            dataAnimation.From = 0;
            dataAnimation.To = -DataGrid.ActualWidth;
            previewAnimation.From = DataGrid.ActualWidth;
            previewAnimation.To = 0;

            DataControl.Width = DataGrid.ActualWidth;
            PreviewControl.Width = DataGrid.ActualWidth;

            PreviewControl.Visibility = Visibility.Visible;
            PreviewControl.BeginStoryboard(storyBoard);
        }

        private void DataToPreviewCompleted(object sender, EventArgs eventArgs)
        {
            DataControl.Visibility = Visibility.Collapsed;
            PreviewControl.Width = double.NaN;
        }

        private void PreviewToDataCompleted(object sender, EventArgs eventArgs)
        {
            PreviewControl.Visibility = Visibility.Collapsed;
            DataControl.Width = double.NaN;
        }


    }
}