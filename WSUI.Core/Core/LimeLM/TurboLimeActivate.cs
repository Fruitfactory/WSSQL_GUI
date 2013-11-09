using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using WSUI.Core.Enums;

namespace WSUI.Core.Core.LimeLM
{
    public class TurboLimeActivate
    {
        private const int DaysBetweenCheck = 1;
        private const int GraceOfInerErr = 14;
        private const string VersionId = "4d6ed75a527c1957550015.01792667";

        private const string ActivationAppName = "TurboActivate.exe";


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
                ActivationState state = ActivationState.None;
                if (IsInternetError)
                    return ActivationState.Error;
                if(IsActivated)
                    return ActivationState.Activated;
                if(!IsActivated || IsTrialPeriodEnded)
                    return ActivationState.NonActivated;
                return state;
            }
        }

        public void TryCheckAgain()
        {
            IsActivated = CheckActivation();
        }

        public void Activate()
        {
            InternalActivate();
        }

        public void Deactivate()
        {
            InternalActivate();
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
            IsActivated = CheckActivation();
            if (!IsActivated)
            {
                IsTrialPeriodEnded = CheckTrialPeriod();
            }
        }

        private bool CheckActivation()
        {
            IsGenuineResult gr = TurboActivate.IsGenuine(DaysBetweenCheck, GraceOfInerErr, true);
            IsInternetError = gr == IsGenuineResult.InternetError;
            return gr == IsGenuineResult.Genuine || gr == IsGenuineResult.GenuineFeaturesChanged ||
                   IsInternetError;
        }
        
        private bool CheckTrialPeriod()
        {
            TurboActivate.UseTrial();
            DaysRemain = TurboActivate.TrialDaysRemaining();
            return DaysRemain == 0;
        }

        private void InternalActivate()
        {
            Process activationProcess = new Process()
            {
                StartInfo =
                {
                    FileName = Path.Combine(Path.GetDirectoryName(Assembly.GetCallingAssembly().Location),ActivationAppName)
                },
                EnableRaisingEvents = true
            };            
            activationProcess.Exited += ActivationProcessOnExited;
            activationProcess.Start();
        }

        private void ActivationProcessOnExited(object sender, EventArgs eventArgs)
        {
            ((Process) sender).Exited -= ActivationProcessOnExited;
            IsActivated = TurboActivate.IsActivated();
        }

        private void InternalDeactivate()
        {
            TurboActivate.Deactivate();
            IsActivated = false;
        }


        #endregion

    }
}