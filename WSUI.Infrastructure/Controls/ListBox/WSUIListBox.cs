using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using WSUI.Infrastructure.Helpers.Extensions;

namespace WSUI.Infrastructure.Controls.ListBox
{
    public class WSUIListBox : System.Windows.Controls.ListBox
    {
        public WSUIListBox()
        {
            DefaultStyleKey = typeof(System.Windows.Controls.ListBox);
        }

        public override void EndInit()
        {
            base.EndInit();
            var listBoxItem = this.GetParent<ListBoxItem>();
            if (listBoxItem == null)
                return;
            DataTrigger dt = new DataTrigger();
            var binding = new Binding("IsSelected");
            binding.Source = listBoxItem;
            dt.Binding = binding;
            dt.Value = false;
            dt.Setters.Add(new Setter(SelectedIndexProperty, -1));
            this.Triggers.Add(dt);
        }
    }
}