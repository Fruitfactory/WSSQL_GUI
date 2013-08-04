using System.Windows.Controls;

namespace WSUI.Infrastructure.Controls
{
    public class WSUIFlowDocumentViewer : FlowDocumentScrollViewer
    {
     
        protected override void OnPreviewMouseWheel(System.Windows.Input.MouseWheelEventArgs e)
        {
            //e.Handled = true;
        }

    }
}