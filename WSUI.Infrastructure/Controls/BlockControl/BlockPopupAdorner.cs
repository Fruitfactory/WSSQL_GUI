using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using OF.Infrastructure.Controls.Core;

namespace OF.Infrastructure.Controls.BlockControl
{
    public class BlockPopupAdorner : BasePopupAdorner
    {
        #region [fields]

        private const int ShowButtonActivate = 30;
        private BlockControl _blockControl = null;
        private bool _isBloked = false;

        #endregion

        #region [ctor]

        private BlockPopupAdorner()
        {
            _blockControl = new BlockControl();
        }

        #endregion

        #region [instance]

        private static Lazy<BlockPopupAdorner> _instance = new Lazy<BlockPopupAdorner>(() =>
        {
            var inst = new BlockPopupAdorner();
            inst.Init();
            return inst;
        });

        public static BlockPopupAdorner Instance
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