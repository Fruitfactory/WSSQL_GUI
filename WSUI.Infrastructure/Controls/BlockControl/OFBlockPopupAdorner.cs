using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using OF.Infrastructure.Controls.Core;

namespace OF.Infrastructure.Controls.BlockControl
{
    public class OFBlockPopupAdorner : OFBasePopupAdorner
    {
        #region [fields]

        private const int ShowButtonActivate = 30;
        private OFBlockControl _blockControl = null;
        private bool _isBloked = false;

        #endregion

        #region [ctor]

        private OFBlockPopupAdorner()
        {
            _blockControl = new OFBlockControl();
        }

        #endregion

        #region [instance]

        private static Lazy<OFBlockPopupAdorner> _instance = new Lazy<OFBlockPopupAdorner>(() =>
        {
            var inst = new OFBlockPopupAdorner();
            inst.Init();
            return inst;
        });

        public static OFBlockPopupAdorner Instance
        {
            get { return _instance.Value; }
        }

        #endregion

        public bool IsBlocked
        {
            get { return _isBloked; }
        }

        public bool Block
        {
            get { return IsBlocked; }
            set
            {
                _isBloked = value;
                OnBlockChanged();
            }
        }

        private void Init()
        {
            SetChild(_blockControl);
        }

        private void OnBlockChanged()
        {
            if(IsBlocked)
                Show();
            else
                Hide();
        }

        protected override void ApplyPositionAndSize(Point topLeft, int width, int height)
        {
            _blockControl.SetValue(Canvas.LeftProperty, topLeft.X);
            _blockControl.SetValue(Canvas.TopProperty, topLeft.Y);
            _blockControl.Width = width;
            _blockControl.Height = height - ShowButtonActivate;
            base.ApplyPositionAndSize(topLeft, width, height);
        }
    }
}