using System.Windows.Documents;

namespace WSUI.Infrastructure.Controls
{
    public class WSUIRun : Run
    {
        public WSUIRun(string text)
            :base(text)
        {}

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