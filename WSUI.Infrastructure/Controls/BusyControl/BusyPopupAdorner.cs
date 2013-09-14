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
                    var topleft = GetScreenLocation((UserControl)element,new Point(0,0));

                    _indicator.SetValue(Canvas.LeftProperty, topleft.X);
                    _indicator.SetValue(Canvas.TopProperty, topleft.Y);
                    _indicator.Width = app.MainControl.ActualWidth;
                    _indicator.Height = app.MainControl.ActualHeight;

                    System.Diagnostics.Debug.WriteLine("X={0},Y={1},W={2},H={3}",topleft.X,topleft.Y,_indicator.Width,_indicator.Height);

                    var horizontalOffset = topleft.X;
                    var verticalOffset = topleft.Y;
                    if (_popup.HorizontalOffset != horizontalOffset) _popup.HorizontalOffset = horizontalOffset;
                    if (_popup.VerticalOffset != verticalOffset) _popup.VerticalOffset = verticalOffset;
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

    }
}