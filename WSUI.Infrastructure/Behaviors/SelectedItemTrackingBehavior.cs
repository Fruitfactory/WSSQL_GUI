using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;
using OF.Core.Data;
using OF.Core.Extensions;
using OF.Core.Logger;

namespace OF.Infrastructure.Behaviors
{
    public class SelectedItemTrackingBehavior : Behavior<ListBox>
    {

        #region [dependency property]

        public static readonly DependencyProperty TrackedItemProperty = DependencyProperty.Register(
            "TrackedItem", typeof (BaseSearchObject), typeof (SelectedItemTrackingBehavior), new PropertyMetadata(default(BaseSearchObject)));

        public BaseSearchObject TrackedItem
        {
            get { return (BaseSearchObject) GetValue(TrackedItemProperty); }
            set { SetValue(TrackedItemProperty, value); }
        }

        #endregion

        
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.MouseMove += AssociatedObjectOnMouseMove;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.MouseMove -= AssociatedObjectOnMouseMove;
        }

        #region [private]

        private void AssociatedObjectOnMouseMove(object sender, MouseEventArgs mouseEventArgs)
        {
            if (AssociatedObject.ContextMenu != null && AssociatedObject.ContextMenu.IsVisible)
            {
                return;
            }
            try
            {
                var pt = mouseEventArgs.GetPosition(AssociatedObject);
                var item = AssociatedObject.GetControlAtPoint<ListBoxItem>(pt);
                if (item != null)
                {
                    TrackedItem = item.DataContext as BaseSearchObject;
                }
            }
            catch (Exception ex)
            {
                WSSqlLogger.Instance.LogError(ex.Message);
            }
            
        }

        #endregion



    }
}