using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WSSQLGUI.Controls.ProgressControl
{
    class ProgressManager : IProgressManager
    {
        #region  fields
        private readonly static object _lockObject = new object();

        private static ProgressManager _instance = null;

        private readonly  AutoResetEvent _stopEvent = new AutoResetEvent(false);
        private Task _showTask;
        private Task _waitTask;
        private ProgressForm _progressForm;
        private readonly Stack<ProgressOperation> _stackOperation = new Stack<ProgressOperation>();
        private ProgressOperation _currentOperation;
        private Form _mainForm;


        #endregion

        private ProgressManager()
        {
            _mainForm = Form.FromHandle(Process.GetCurrentProcess().MainWindowHandle) as Form;
        }

        public static ProgressManager Instance
        {
            get
            {
                if(_instance == null)
                {
                    lock(_lockObject)
                        _instance = new ProgressManager();
                }
                return _instance;
            }
        }


        #region main code

        public void StartOperation(ProgressOperation operation)
        {
            if (operation == null)
                return;
            _currentOperation = operation;
            _stackOperation.Push(_currentOperation);
            StartWaiting();
        }

        private void StartWaiting()
        {
            var delayTime = _currentOperation.DelayTime;
            if(delayTime > 0)
            {
                _stopEvent.Reset();
                _waitTask = Task.Factory.StartNew(() =>
                                                      {

                                                          while (true)
                                                          {
                                                              int result = WaitHandle.WaitAny(new WaitHandle[] { _stopEvent }, delayTime);
                                                              if (result == WaitHandle.WaitTimeout)
                                                                  break;
                                                              if (result == 0)
                                                                  return;
                                                          }
                                                          ShowProgressForm();
                                                      });

            }
            else 
                ShowProgressForm();
        }


        private void ShowProgressForm()
        {
            
            _showTask = Task.Factory.StartNew(() =>
                                                  {
                                                      _progressForm = new ProgressForm();
                                                      _progressForm.WindowState = FormWindowState.Normal;
                                                      _progressForm.SetMessage(_currentOperation.Name);
                                                      _progressForm.TopMost = true;
                                                      _progressForm.Location = CalcPosition();
                                                      Application.Run(_progressForm);
                                                  });
            
        }

        private Point CalcPosition()
        {
            return new Point( _mainForm.Location.X + ((_mainForm.Width - _progressForm.Width) / 2),  _mainForm.Location.Y + ((_mainForm.Height - _progressForm.Height) / 2) );
        }

        public void StopOperation()
        {
            _stopEvent.Set();
            _currentOperation = _stackOperation.Pop();
            CloseProgressForm();
        }

        private void CloseProgressForm()
        {
            if (_progressForm == null)
                return;
            _progressForm.CloseExt();
            _progressForm = null;
            _showTask.Wait();
            _showTask = null;
       }

        #endregion
    }
}
