using System.Windows.Controls;
using System.Windows.Input;

namespace WSUI.Infrastructure.Controls
{
    public class WSUIFrame : Frame
    {
        protected override void OnPreviewMouseWheel(MouseWheelEventArgs e)
        {
            //e.Handled = true;
        }
    }
}