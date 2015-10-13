using System;
using System.Windows;
using System.Windows.Controls;
using OF.Infrastructure.Controls.Core;

namespace OF.Infrastructure.Controls.BusyControl
{
    public class OFBusyPopupAdorner : OFBasePopupAdorner, IBusyPopupAdorner
    {
        #region [needs]

        private string _message = string.Empty;
        private bool _isBusy = false;
        private OFBusyIndicator _indicator = null;

        #endregion

        #region [ctor]

        private OFBusyPopupAdorner()
        {
            _indicator = new OFBusyIndicator();
        }

        #endregion

        #region [instance]

        private static readonly Lazy<IBusyPopupAdorner> _instance = new Lazy<IBusyPopupAdorner>(() =>
        {
            var inst = new OFBusyPopupAdorner();
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