using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Microsoft.Office.Interop.Outlook;
using WSUI.Core.Enums;
using WSUI.Core.Logger;
using WSUI.Core.Utils.Dialog;
using WSUI.Core.Utils.Dialog.Interfaces;
using WSUI.Core.Utils.Dialog.View;
using WSUI.Core.Utils.Dialog.ViewModel;
using Action = System.Action;
using Exception = System.Exception;

namespace WSUI.Core.Core.LimeLM
{
    public class TurboLimeActivate
    {

#region [internal class for new licensing]

        class CheckActivationResult
        {
            public CheckActivationResult(bool isActivated, bool isTrial, bool checkInOldWay)
            {
                IsActivated = isActivated;
                IsTrial = isTrial;
                CheckInOldWay = checkInOldWay;
            }

            public bool IsActivated { get; private set; }
            public bool IsTrial { get; private set; }
            public bool CheckInOldWay { get; private set; }
        }

#endregion

        #region [fields const]

        private const string TrialExpires = "trial_expires";
        private const string TimesUsed = "times_used";
        private const string UserEmail = "user_email";
        private const string IsTrialKey = "is_trial_key";

        #endregion


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
            get { return TurboActivate.TrialDaysRemaining(); }
        }

        public ActivationState State
        {
            get
            {
                ActivationState state = ActivationState.Error;
                if (IsInternetError)
                {
                    WSSqlLogger.Instance.LogWarning("Check - Internet connection is available or Lime services (servers) are available.");
                    return ActivationState.Error;
                }
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

        private CheckActivationResult CheckActivationNew()
        {
            //TODO: First check if the user is activated, if not then don't continue
            //      with any of the following code. It won't work.

            // Use the IsGenuineEx() function to check if they're activated.
            // For more info see the "Using TurboActivate" article
            // for your particular language.

            string trialExpires = TurboActivate.GetFeatureValue(TrialExpires, null);
            bool isTrial = int.Parse(TurboActivate.GetFeatureValue(IsTrialKey, null)) > 0;
            bool isActivated = false;
            bool checkInOldWay = true;
            WSSqlLogger.Instance.LogInfo("Expires date: {0}",trialExpires);

            if (trialExpires != null)
            {
                // this is a trial product key
                // verify the trial hasn't expired
                bool stillInTrial = TurboActivate.IsDateValid(trialExpires,
                    TurboActivate.TA_DateCheckFlags.TA_HAS_NOT_EXPIRED);
                isActivated = stillInTrial;
                checkInOldWay = false;
            }
            else
            {
                isActivated = false;
                checkInOldWay = true;
            }

            return new CheckActivationResult(isActivated,isTrial,checkInOldWay);
        }

        private void CheckActivationAndTrial()
        {
            IsActivated = CheckActivation();
            if (IsActivated)
            {
                var checkResult = CheckActivationNew();
                IsActivated = checkResult.IsActivated; // according "trial_expires" value
                IsTrialPeriodEnded = checkResult.IsTrial && !checkResult.IsActivated;
            }
            else
            {
                IsTrialPeriodEnded = CheckTrialPeriod();
            }
        }


        private bool CheckTrialPeriod()
        {
            WSSqlLogger.Instance.LogInfo("UseTrial!!");
            TurboActivate.UseTrial();
            int days = DaysRemain;
            WSSqlLogger.Instance.LogInfo("DaysRemain = {0}",days);
            return days == 0;
        }

        private void InternalActivate()
        {
            // TODO: ask for email. generate key, activate them. and then repeat checking.
            if (!IsTrialPeriodEnded)
            {
                WSSqlLogger.Instance.LogInfo("Old way for licensing!!");
                IWSUIEmailViewModel viewModel = new WSUIEmailViewModel(new WSUIEmailView());
                var dlg = new WSUIDialogWindow(viewModel){Width = 400, Height = 250};
                var result = dlg.ShowDialog();
                if (result.HasValue && result.Value && !string.IsNullOrEmpty(viewModel.Email1))
                {
                    try
                    {
                        var key = LimeLMApi.GenerateAndReturnKey(viewModel.Email1);
                        if (string.IsNullOrEmpty(key))
                        {
                            WSSqlLogger.Instance.LogWarning("The key hasn't been generated.");
                        }
                        else
                        {
                            TurboActivate.CheckAndSavePKey(key.Trim(), TurboActivate.TA_Flags.TA_USER);
                            TurboActivate.Activate();
                            ActivationProcess();
                            WSSqlLogger.Instance.LogInfo("Activated");
                        }
                    }
                    catch (Exception ex)
                    {
                        WSSqlLogger.Instance.LogError(ex.Message);
                    }
                }
            }
        }

        private void ActivationProcess()
        {
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