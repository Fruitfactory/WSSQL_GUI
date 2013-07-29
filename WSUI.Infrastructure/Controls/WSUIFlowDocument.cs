using System.Windows.Documents;

namespace WSUI.Infrastructure.Controls
{
    public class WSUIFlowDocument : FlowDocument
    {
        protected override void OnPreviewMouseWheel(System.Windows.Input.MouseWheelEventArgs e)
        {
           //e.Handled = true;
        }
    }
}