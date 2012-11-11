using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;


namespace WSUI.Infrastructure.Controls.ProgressManager
{
    public class ProgressManager : IProgressManager
    {
        #region  fields
        private readonly static object _lockObject = new object();

        private static ProgressManager _instance = null;

        private readonly  AutoResetEvent _stopEvent = new AutoResetEvent(false);
        private Task _waitTask;
        private IProgressForm _progressForm;
        private readonly Stack<ProgressOperation> _stackOperation = new Stack<ProgressOperation>();
        private ProgressOperation _currentOperation;


        #endregion

        private ProgressManager()
        {
            
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
            _stopEvent.Reset();
            if(delayTime > 0)
            {
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
            Application.Current.Dispatcher.BeginInvoke(new Action(() => 
                                                  {
                                                      _progressForm = new ProgressWindow();
                                                      ((Window)_progressForm).Owner = Application.Current.MainWindow;
                                                      _progressForm.ProcessCommand(ProgressFormCommand.Settings,
                                                                                   _currentOperation);
                                                      ((Window)_progressForm).ShowDialog();
                                                  }));

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
        }

        public void SetProgress(int value)
        {
            if(_progressForm == null)
                return;
            _progressForm.ProcessCommand(ProgressFormCommand.Progress, value);          
        }

        #endregion
    }
}
