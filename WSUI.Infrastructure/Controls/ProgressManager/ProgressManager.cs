﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        private Thread _uiThread;

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
                                                              System.Diagnostics.Debug.WriteLine(
                                                                  "Show progress delay > 0 ----------------------------------");
                                                          });

                }
                else
                {
                    
                    ShowProgressForm();
                    System.Diagnostics.Debug.WriteLine("Show progress delay == 0 ----------------------------------");
                }
            }
        }


        private void ShowProgressForm()
        {
            
            _uiThread = new Thread(ShowSplash);
            _uiThread.Name = "ShowSplash";
            _uiThread.SetApartmentState(ApartmentState.STA);
            _uiThread.Start();
        }

        private void ShowSplash()
        {
            lock(_lockObject)
               if(_currentOperation == null)
                    return;
            
            _progressForm = new ProgressWindow();
            ((Window) _progressForm).Topmost = true;
            
            _progressForm.ProcessCommand(ProgressFormCommand.Settings, _currentOperation);
            ((Window) _progressForm).Closed += (o, e) =>
                                                   {
                                                       ((Window) _progressForm).Dispatcher.InvokeShutdown();
                                                       System.Diagnostics.Debug.WriteLine(
                                                           "Close dialog ----------------------------------");
                                                   };
            System.Diagnostics.Debug.WriteLine("Show dialog ----------------------------------");
            ((Window)_progressForm).Show();
            System.Windows.Threading.Dispatcher.Run();
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
            System.Diagnostics.Debug.WriteLine("Stop progress ----------------------------------");
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
    }
}