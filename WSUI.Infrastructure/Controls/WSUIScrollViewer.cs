using System.Windows.Controls;
namespace WSUI.Infrastructure.Controls
{
    public class WSUIScrollViewer : ScrollViewer
    {
        protected override void OnPreviewMouseWheel(System.Windows.Input.MouseWheelEventArgs e)
        {
            //e.Handled = true;
        }
    }
}