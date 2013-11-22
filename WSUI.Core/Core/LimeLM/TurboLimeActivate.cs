using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using WSUI.Core.Enums;
using WSUI.Core.Logger;

namespace WSUI.Core.Core.LimeLM
{
    public class TurboLimeActivate
    {
        // O_o
        private const int DaysBetweenCheck = 0; // we should use 0. In this case TurboActive verify activation with the server every time when we call IsGenuine()


        private const int GraceOfInerErr = 14;
        private const string VersionId = "4d6ed75a527c1957550015.01792667";

        private const string ActivationAppName = "TurboActivate.exe";
        private Action _callback;

        #region [static]

        private static Lazy<TurboLimeActivate> _Instance = new Lazy<TurboLimeActivate>(() =>
        {
            var inst = new TurboLimeActivate();
            inst.Init();
            return inst;
        });

        public static TurboLimeActivate Instance
        {
            get { return _Instance.Value; }
        }

        #endregion

        #region [public]

        public int DaysRemain
        {
            get;
            private set;
        }

        public ActivationState State
        {
            get
            {
                ActivationState state = ActivationState.Error;
                if (IsInternetError)
                    return ActivationState.Error;
                if(IsActivated)
                    return ActivationState.Activated;
                if(!IsTrialPeriodEnded)
                    return ActivationState.Trial;
                if (IsTrialPeriodEnded && !IsActivated)
                    return ActivationState.NonActivated;
                return state;
            }
        }

        public void TryCheckAgain()
        {
            CheckActivationAndTrial();
        }

        public void Activate(Action callback)
        {
            _callback = callback;
            InternalActivate();
        }

        public bool Deactivate(bool deleteKey = false)
        {
            return InternalDeactivate(deleteKey);
        }

        #endregion

        #region [private property]

        private bool IsActivated
        {
            get;
            set;
        }

        private bool IsTrialPeriodEnded
        {
            get;
            set;
        }

        private bool IsInternetError
        {
            get;
            set;
        }


        #endregion

        #region [private]

        private void Init()
        {
            TurboActivate.VersionGUID = VersionId;
            CheckActivationAndTrial();
        }

        private bool CheckActivation()
        {
            IsGenuineResult gr = TurboActivate.IsGenuine(DaysBetweenCheck, GraceOfInerErr, true);
            IsInternetError = gr == IsGenuineResult.InternetError;
            return gr == IsGenuineResult.Genuine || gr == IsGenuineResult.GenuineFeaturesChanged ||
                   IsInternetError;
        }

        private void CheckActivationAndTrial()
        {
            IsActivated = CheckActivation();
            if (!IsActivated)
            {
                IsTrialPeriodEnded = CheckTrialPeriod();
            }
        }

        
        private bool CheckTrialPeriod()
        {
            WSSqlLogger.Instance.LogInfo("UseTrial!!");
            TurboActivate.UseTrial();
            DaysRemain = TurboActivate.TrialDaysRemaining();
            WSSqlLogger.Instance.LogInfo("DaysRemain = {0}",DaysRemain);
            return DaysRemain == 0;
        }

        private void InternalActivate()
        {
            string path = Path.Combine(Path.GetDirectoryName(typeof(TurboLimeActivate).Assembly.Location), ActivationAppName);
            WSSqlLogger.Instance.LogInfo("Path Activate: {0}",path);
            Process activationProcess = new Process()
            {
                StartInfo =
                {
                    FileName = path
                },
                EnableRaisingEvents = true
            };            
            activationProcess.Exited += ActivationProcessOnExited;
            activationProcess.Start();
        }

        private void ActivationProcessOnExited(object sender, EventArgs eventArgs)
        {
            ((Process) sender).Exited -= ActivationProcessOnExited;
            CheckActivationAndTrial();
            if (_callback != null)
            {
                _callback();
                _callback = null;
            }
        }

        private bool InternalDeactivate(bool deleteKey)
        {
            try
            {
                TurboActivate.Deactivate(deleteKey);
                CheckActivationAndTrial();
                return true;
            }
            catch (Exception ex)
            {
                WSSqlLogger.Instance.LogError("{0}",ex.Message);
            }
            return false;
        }


        #endregion

    }
}