using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using OF.Core.Win32;
using OF.Infrastructure.Controls.Application;

namespace OF.Infrastructure.Controls.Core
{
    public abstract class OFBasePopupAdorner : IDisposable
    {
        #region [fields]

        private Popup _popup = null;

        #endregion

        #region [ctor]

        protected OFBasePopupAdorner()
        {
            _popup = new Popup{IsOpen = false,AllowsTransparency = false};
            _popup.LayoutUpdated += PopupOnLayoutUpdated;

        }
        #endregion

        #region [protected]

        protected bool SetChild(UIElement child)
        {
            if (_popup == null)
                return false;
            _popup.Child = child;
            return true;
        }

        protected void Show()
        {
            if (_popup == null)
                return;
            _popup.IsOpen = true;
        }

        protected void Hide()
        {
            if(_popup == null)
                return;
            _popup.IsOpen = false;
        }

        protected virtual void ApplyPositionAndSize(Point topLeft, int width, int height)
        {
            var horizontalOffset = topLeft.X;
            var verticalOffset = topLeft.Y;
            if (_popup.HorizontalOffset != horizontalOffset) _popup.HorizontalOffset = horizontalOffset;
            if (_popup.VerticalOffset != verticalOffset) _popup.VerticalOffset = verticalOffset;
        }
        
        #endregion

        #region [private]

        private void PopupOnLayoutUpdated(object sender, EventArgs eventArgs)
        {
            LayoutUpdated();
        }

        private void LayoutUpdated()
        {
            try
            {
                Point topLeft;
                int width, height;

                if ((System.Windows.Application.Current as OFAppEmpty) != null)
                {
                    var app = System.Windows.Application.Current as OFAppEmpty;
                    FrameworkElement element = app.MainControl as FrameworkElement;
                    topLeft = GetScreenLocation((UserControl)element, new System.Windows.Point(0, 0));
                    width =(int)app.MainControl.ActualWidth;
                    height =(int)app.MainControl.ActualHeight;
                }
                else
                {
                    var app = System.Windows.Application.Current;
                    var hwnd = WindowsFunction.GetForegroundWindow();
                    WindowsFunction.RECT rect;
                    WindowsFunction.GetWindowRect(hwnd, out rect);
                    topLeft = new System.Windows.Point(rect.Left, rect.Top);
                    width =(int)app.MainWindow.ActualWidth;
                    height =(int)app.MainWindow.ActualHeight;
                }
                ApplyPositionAndSize(topLeft,width,height);
            }
            catch (Exception)
            {

            }
        }

        private Point GetScreenLocation(UserControl ctrl, Point pt)
        {
            return WindowsFunction.TransformToScreen(pt, ctrl);
        }

        #endregion

        public void Dispose()
        {
            if (_popup != null)
            {
                _popup.IsOpen = false;
                _popup.LayoutUpdated -= PopupOnLayoutUpdated;
            }
        }

    }
}