﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;
using OF.Core.Logger;


namespace OF.Infrastructure.Controls.ProgressManager
{
    public class OFProgressManager : IProgressManager
    {
        #region  fields
        private readonly static object _lockObject = new object();

        private static OFProgressManager _instance = null;

        private readonly  AutoResetEvent _stopEvent = new AutoResetEvent(false);
        private Task _waitTask;
        private IProgressForm _progressForm;
        private readonly Stack<OFProgressOperation> _stackOperation = new Stack<OFProgressOperation>();
        private OFProgressOperation _currentOperation;
        private Thread _uiThread;

        #endregion

        private OFProgressManager()
        {
        }

        public static OFProgressManager Instance
        {
            get
            {
                if(_instance == null)
                {
                    lock(_lockObject)
                        _instance = new OFProgressManager();
                }
                return _instance;
            }
        }


        #region main code

        public void StartOperation(OFProgressOperation operation)
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

            lock (_lockObject)
            {
                _stopEvent.Reset();

                if (delayTime > 0)
                {
                    _waitTask = Task.Factory.StartNew(() =>
                                                          {

                                                              while (true)
                                                              {
                                                                  int result =
                                                                      WaitHandle.WaitAny(new WaitHandle[] {_stopEvent},
                                                                                         delayTime);
                                                                  if (result == WaitHandle.WaitTimeout)
                                                                      break;
                                                                  if (result == 0)
                                                                      return;
                                                              }
                                                              
                                                              ShowProgressForm();
                                                              //OFLogger.Instance.LogInfo("Show progress delay > 0 ----------------------------------");
                                                          });

                }
                else
                {
                    
                    ShowProgressForm();
                    //OFLogger.Instance.LogInfo(">>>Show progress delay == 0");
                }
            }
        }


        private void ShowProgressForm()
        {
            
            _uiThread = new Thread(ShowSplash);
            _uiThread.IsBackground = true;
            _uiThread.Name = "ShowSplash";
            _uiThread.SetApartmentState(ApartmentState.STA);
            _uiThread.Start();
        }

        private void ShowSplash()
        {
            lock (_lockObject)
            {
                if (_currentOperation == null)
                    return;

                SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));

                _progressForm = new OFProgressWindow();
                var wih = new WindowInteropHelper((Window)_progressForm);
                wih.Owner = _currentOperation.MainHandle;
                try
                {
                    _progressForm.ProcessCommand(ProgressFormCommand.Settings, _currentOperation);
                    ((Window) _progressForm).Closed += OnClosedForm;
                    //OFLogger.Instance.LogInfo("Show dialog");
                }
                catch (Exception ex)
                {
                    OFLogger.Instance.LogError(string.Format("{0} - {1}","ShowSplash",ex.ToString()));
                }
            }
            ((Window)_progressForm).ShowDialog();
            System.Windows.Threading.Dispatcher.Run();
        }


        private void OnClosedForm(object sender,EventArgs args)
        {
            ((Window) _progressForm).Dispatcher.BeginInvokeShutdown(DispatcherPriority.Background);
            OFLogger.Instance.LogDebug(
                "Close dialog");
        }

        public void StopOperation()
        {
            lock (_lockObject)
            {
                _stopEvent.Set();
                
                if (_stackOperation.Count > 0)
                {
                    _currentOperation = null;
                    _stackOperation.Pop();
                }
            }
            OFLogger.Instance.LogDebug("Stop progress");
            CloseProgressForm();
        }

        private void CloseProgressForm()
        {
            if (_progressForm == null)
                return;
            _progressForm.CloseExt();
        }

        public void SetProgress(int value)
        {
            if(_progressForm == null)
                return;
            _progressForm.ProcessCommand(ProgressFormCommand.Progress, value);          
        }

        #endregion


        public bool InProgress
        {
            get { return _currentOperation != null; }
        }
    }
}
