using System;
using System.Windows;
using System.Windows.Controls;
using OF.Infrastructure.Controls.Core;

namespace OF.Infrastructure.Controls.BusyControl
{
    public class BusyPopupAdorner : BasePopupAdorner, IBusyPopupAdorner
    {
        #region [needs]

        private string _message = string.Empty;
        private bool _isBusy = false;
        private BusyIndicator _indicator = null;

        #endregion

        #region [ctor]

        private BusyPopupAdorner()
        {
            _indicator = new BusyIndicator();
        }

        #endregion

        #region [instance]

        private static readonly Lazy<IBusyPopupAdorner> _instance = new Lazy<IBusyPopupAdorner>(() =>
        {
            var inst = new BusyPopupAdorner();
            inst.Init();
            return inst;
        });

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

        private void Init()
        {
            SetChild(_indicator);
        }

        private void OnBusyChanged()
        {
            if(IsBusy)
                Show();
            else
                Hide();
        }

        private void OnMessageChanged()
        {
            _indicator.Message = Message;
        }

        protected override void ApplyPositionAndSize(Point pt, int width, int height)
        {
            _indicator.SetValue(Canvas.LeftProperty, pt.X);
            _indicator.SetValue(Canvas.TopProperty, pt.Y);
            _indicator.Width = width;
            _indicator.Height = height;
            base.ApplyPositionAndSize(pt,width,height);
        }


    }
}