using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using OF.Core.Extensions;

namespace OF.Infrastructure.Behaviors
{
  public class GridListItemBehavior : Behavior<Grid>
  {
    protected override void OnAttached()
    {
      base.OnAttached();
      AssociatedObject.PreviewMouseLeftButtonDown += AssociatedObjectOnPreviewMouseLeftButtonDown;
    }

    protected override void OnDetaching()
    {
      AssociatedObject.PreviewMouseLeftButtonDown -= AssociatedObjectOnPreviewMouseLeftButtonDown;
      base.OnDetaching();
    }

    private void AssociatedObjectOnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs mouseButtonEventArgs)
    {
      var item = AssociatedObject.GetParentCore<ListBoxItem>();
      if (item == null || item.IsSelected)
        return;
      item.IsSelected = true;
    }

  }
}
