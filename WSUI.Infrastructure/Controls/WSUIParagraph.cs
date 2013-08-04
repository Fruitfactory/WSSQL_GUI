using System.Windows.Documents;

namespace WSUI.Infrastructure.Controls
{
    public class WSUIParagraph : Paragraph
    {
        protected override void OnMouseWheel(System.Windows.Input.MouseWheelEventArgs e)
        {
            e.Handled = true;
        }

        protected override void OnPreviewMouseWheel(System.Windows.Input.MouseWheelEventArgs e)
        {
            //e.Handled = true;
        }
    }
}