using System.Drawing;

namespace C4F.DevKit.PreviewHandler.Controls.ImageControl.Interface
{
    public interface IImagePreviewGenerator
    {
        void SetFileName(string filename);
        bool IsSupportFofrmat();
        Image GetImage();
    }
}