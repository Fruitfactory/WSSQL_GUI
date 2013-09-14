using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;
using WSUI.Core.Win32;
using WSUI.Infrastructure.Controls.Application;
using System.Windows.Media;

namespace WSUI.Infrastructure.Controls.BusyControl
{
    public class BusyPopupAdorner : IBusyPopupAdorner
    {
        #region [needs]

        private string _message = string.Empty;
        private bool _isBusy = false;
        private Popup _popup = null;
        private BusyIndicator _indicator = null;

        #endregion

        #region [ctor]

        private BusyPopupAdorner()
        {
            _indicator = new BusyIndicator();
            _popup = new Popup(){IsOpen =  false, Child = _indicator,AllowsTransparency = true};
            _popup.LayoutUpdated += PopupOnLayoutUpdated;
        }

        #endregion

        #region [instance]

        private static readonly Lazy<IBusyPopupAdorner> _instance = new Lazy<IBusyPopupAdorner>(() => new BusyPopupAdorner());
        public static IBusyPopupAdorner Instance
        {
            get { return _instance.Value; }
        }

        #endregion

        public bool IsBusy
        {
            get { return _isBusy; } 
            set { _isBusy = value; OnBusyChanged();}
        }

        public string Message
        {
            get { return _message; }
            set { _message = value; OnMessageChanged();}
        }

        private void OnBusyChanged()
        {
            _popup.IsOpen = IsBusy;
        }

        private void OnMessageChanged()
        {
            _indicator.Message = Message;
        }

        private void PopupOnLayoutUpdated(object sender, EventArgs eventArgs)
        {
            try
            {
                if ((System.Windows.Application.Current as AppEmpty) != null)
                {
                    var app = System.Windows.Application.Current as AppEmpty;
                    FrameworkElement element = app.MainControl as FrameworkElement;
                    var topleft = GetScreenLocation((UserControl)element, new Point(0, 0));
                    ApplyPositionAndSize(topleft, (int)app.MainControl.ActualWidth,(int)app.MainControl.ActualHeight);
                   
                }
                else
                {
                    var app = System.Windows.Application.Current;
                    var hwnd = WindowsFunction.GetForegroundWindow();
                    WindowsFunction.RECT rect;
                    WindowsFunction.GetWindowRect(hwnd, out rect);
                    var topleft = new Point(rect.Left,rect.Top);
                    ApplyPositionAndSize(topleft, (int)app.MainWindow.ActualWidth, (int)app.MainWindow.ActualHeight);
                }
            }
            catch (Exception)
            {
                
            }
        }

        private Point GetScreenLocation(UserControl ctrl, Point pt)
        {
            return WindowsFunction.TransformToScreen(pt, ctrl);
        }

        private void ApplyPositionAndSize(Point pt, int width, int height)
        {
            _indicator.SetValue(Canvas.LeftProperty, pt.X);
            _indicator.SetValue(Canvas.TopProperty, pt.Y);
            _indicator.Width = width;
            _indicator.Height = height;

            var horizontalOffset = pt.X;
            var verticalOffset = pt.Y;
            if (_popup.HorizontalOffset != horizontalOffset) _popup.HorizontalOffset = horizontalOffset;
            if (_popup.VerticalOffset != verticalOffset) _popup.VerticalOffset = verticalOffset;

        }


    }
}