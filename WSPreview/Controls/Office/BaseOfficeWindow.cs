using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using C4F.DevKit.PreviewHandler.PInvoke;


namespace C4F.DevKit.PreviewHandler.Controls.Office
{
	public abstract class BaseOfficeWindow
	{
		protected IntPtr _parentHandle;
		protected IntPtr _childHandle;
		protected Dictionary<string,bool> _dictSettings = new Dictionary<string,bool>();
		protected UserControl _parentControl;
        protected int _margin = 1;

		public virtual void CreateApp()
		{
			
		}

		public virtual void UnloadApp()
		{
			
		}

		public virtual void SetParentControl(UserControl parent)
		{
            _parentControl = parent;
            _parentHandle = _parentControl.Handle;
            SetParentWindow();
		}

		protected virtual void SaveOfficeSettings()
		{
			
		}

		public virtual void RestoreOfficeSettings()
		{
			
		}

		public virtual void LoadFile(string filename)
		{
			
		}

		public virtual void ResizeWindow()
		{
            if (_childHandle != null)
            {
                long style = WindowAPI.GetWindowLong(_childHandle, (int)WindowAPI.WindowLongFlags.GWL_STYLE);
                style &= ~(WindowAPI.WS_BORDER | WindowAPI.WS_CAPTION | WindowAPI.WS_SIZEBOX | WindowAPI.WS_SIZEBOX);
                WindowAPI.SetWindowLong(_childHandle, (int)WindowAPI.WindowLongFlags.GWL_STYLE, (int)style);
                WindowAPI.MoveWindow(_childHandle, _margin, _margin,
                    _parentControl.Width - _margin, _parentControl.Height - _margin,
                    true);
                WindowAPI.SetWindowPos(_childHandle, IntPtr.Zero, _margin, _margin,
               _parentControl.Width - _margin, _parentControl.Height - _margin,
               WindowAPI.SetWindowPosFlags.NOZORDER | WindowAPI.SetWindowPosFlags.NOMOVE | WindowAPI.SetWindowPosFlags.DRAWFRAME
               );
            }
		}

		protected virtual void SetParentWindow()
		{
            WindowAPI.SetParent(_childHandle, _parentHandle);

            long lExStyle = WindowAPI.GetWindowLong(_childHandle, (int)WindowAPI.WindowLongFlags.GWL_STYLE);
            lExStyle &= ~(WindowAPI.WS_BORDER | WindowAPI.WS_SIZEBOX | WindowAPI.WS_SIZEBOX | WindowAPI.WS_CAPTION);
            WindowAPI.SetWindowLong(_childHandle, (int)WindowAPI.WindowLongFlags.GWL_STYLE, (int)lExStyle);
            WindowAPI.SetWindowPos(_childHandle, IntPtr.Zero, _margin, _margin, 
                _parentControl.Width - _margin, _parentControl.Height - _margin,
                WindowAPI.SetWindowPosFlags.NOZORDER | WindowAPI.SetWindowPosFlags.NOMOVE | WindowAPI.SetWindowPosFlags.DRAWFRAME
                );
		}
	}
}
