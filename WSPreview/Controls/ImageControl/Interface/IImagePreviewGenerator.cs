using System.Drawing;

namespace WSPreview.PreviewHandler.Controls.ImageControl.Interface
{
    public interface IImagePreviewGenerator
    {
        void SetFileName(string filename);
        bool IsSupportFormat();
        Image GetImage();
    }
}