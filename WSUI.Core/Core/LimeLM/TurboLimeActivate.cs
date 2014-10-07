using System;
using System.Diagnostics;
using System.IO;
using WSUI.Core.Enums;
using WSUI.Core.Helpers;
using WSUI.Core.Logger;
using Action = System.Action;
using Exception = System.Exception;

namespace WSUI.Core.Core.LimeLM
{
    public class TurboLimeActivate
    {
        #region [internal class for new licensing]

        private class CheckActivationResult
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

        #endregion [internal class for new licensing]

        #region [fields const]

        private const string TrialExpires = "trial_expires";
        private const string TimesUsed = "times_used";
        private const string UserEmail = "user_email";
        private const string IsTrialKey = "is_trial_key";

        private const int TimeUsedId = 2568; // can get from https://wyday.com/limelm/version/2568/edit-feature/

        #endregion [fields const]

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

        #endregion [static]

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
                    return ActivationState.Error; // TODO: what I should return Error means that OF won't work, Trial - OF wil'l work but in this case we will show "Buy"/"Activate" buttons.
                }
                if (IsActivated)
                    return ActivationState.Activated;
                if (!IsTrialPeriodEnded)
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

        public void IncreaseTimeUsedFlag()
        {
            try
            {
                string sCount = TurboActivate.GetFeatureValue(TimesUsed);
                int iCount;
                int.TryParse(sCount, out iCount);
                string pkeyid = RegistryHelper.Instance.GetPKeyId();
                int c = iCount + 1;
                if (!string.IsNullOrEmpty(pkeyid))
                {
                    var result = LimeLMApi.SetDetails(pkeyid, 0, null, new string[] { TimesUsed }, new string[] { c.ToString() });
                    WSSqlLogger.Instance.LogInfo("Times used: {0}, Result: {1}", c, result);
                }
            }
            catch (Exception ex)
            {
                WSSqlLogger.Instance.LogError(ex.Message);
            }
        }

        #endregion [public]

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

        #endregion [private property]

        #region [private]

        private void Init()
        {
            TurboActivate.VersionGUID = VersionId;
            CheckActivationAndTrial();
        }

        private bool CheckActivation()
        {
            try
            {
                IsGenuineResult gr = TurboActivate.IsGenuine(DaysBetweenCheck, GraceOfInerErr, true);
                IsInternetError = gr == IsGenuineResult.InternetError;
                WSSqlLogger.Instance.LogInfo("GenuineResult: {0}", gr);
                return gr == IsGenuineResult.Genuine || gr == IsGenuineResult.GenuineFeaturesChanged ||
                       IsInternetError;
            }
            catch (Exception ex)
            {
                WSSqlLogger.Instance.LogError("Error occured during checking activation: [{0}]", ex.Message);
                return false;
            }
        }

        private CheckActivationResult CheckActivationNew()
        {
            //TODO: First check if the user is activated, if not then don't continue
            //      with any of the following code. It won't work.

            // Use the IsGenuineEx() function to check if they're activated.
            // For more info see the "Using TurboActivate" article
            // for your particular language.
            bool isTrial = false;
            bool isActivated = false;
            bool checkInOldWay = true;
            try
            {
                string trialExpires = TurboActivate.GetFeatureValue(TrialExpires, null);
                isTrial = int.Parse(TurboActivate.GetFeatureValue(IsTrialKey, null)) > 0;
                WSSqlLogger.Instance.LogInfo("Expires date: {0}", trialExpires);

                if (trialExpires != null)
                {
                    // this is a trial product key
                    // verify the trial hasn't expired
                    bool stillInTrial = TurboActivate.IsDateValid(trialExpires,
                        TurboActivate.TA_DateCheckFlags.TA_HAS_NOT_EXPIRED);
                    WSSqlLogger.Instance.LogInfo("Is Still in Trial: {0}", stillInTrial);
                    //DaysRemain = (DateTime.Parse(trialExpires).Date - DateTime.Now.Date).Days;
                    //DaysRemain = DaysRemain <= 0 ? 0 : DaysRemain;
                    isActivated = stillInTrial;
                    checkInOldWay = false;
                }
                else
                {
                    isActivated = false;
                    checkInOldWay = true;
                }
            }
            catch (Exception ex)
            {
                WSSqlLogger.Instance.LogError("Error occured during checking new activation: [{0}]", ex.Message);
            }

            return new CheckActivationResult(isActivated, isTrial, checkInOldWay);
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
            int days = DaysRemain;
            WSSqlLogger.Instance.LogInfo("DaysRemain = {0}", days);
            return days == 0;
        }

        private void InternalActivate()
        {
            string path = Path.Combine(Path.GetDirectoryName(typeof(TurboLimeActivate).Assembly.Location), ActivationAppName);
            WSSqlLogger.Instance.LogInfo("Path Activate: {0}", path);
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
            ((Process)sender).Exited -= ActivationProcessOnExited;
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
                WSSqlLogger.Instance.LogError("{0}", ex.Message);
            }
            return false;
        }

        #endregion [private]
    }
}