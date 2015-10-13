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
    public class OFSelectedItemTrackingBehavior : Behavior<ListBox>
    {

        #region [dependency property]

        public static readonly DependencyProperty TrackedItemProperty = DependencyProperty.Register(
            "TrackedItem", typeof (OFBaseSearchObject), typeof (OFSelectedItemTrackingBehavior), new PropertyMetadata(default(OFBaseSearchObject)));

        public OFBaseSearchObject TrackedItem
        {
            get { return (OFBaseSearchObject) GetValue(TrackedItemProperty); }
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
                    TrackedItem = item.DataContext as OFBaseSearchObject;
                }
            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(ex.Message);
            }
            
        }

        #endregion



    }
}