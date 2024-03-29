﻿using System;
using System.Diagnostics;
using System.IO;
using OF.Core.Enums;
using OF.Core.Extensions;
using OF.Core.Helpers;
using OF.Core.Interfaces;
using OF.Core.Logger;
using wyDay.TurboActivate;
using Action = System.Action;
using Exception = System.Exception;

namespace OF.Core.Core.LimeLM
{
    public class TurboLimeActivate : IOFTurboLimeActivate
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
        private const int DaysBetweenCheck = 90;
        // we should use 0. In this case TurboActive verify activation with the server every time when we call IsGenuine()

        private const int GraceOfInerErr = 14;
        private const string VersionId = "4d6ed75a527c1957550015.01792667";

        private const string ActivationAppName = "TurboActivate.exe";
        private Action _callback;
        private TurboActivate _turboActivate;


        public TurboLimeActivate()
        {
            _turboActivate = new TurboActivate(VersionId);
        }

        static TurboLimeActivate()
        {
        }

        #region [public]

        public int DaysRemain
        {
            get
            {
                try
                {
                    var daysRemain = (int) _turboActivate.TrialDaysRemaining();
                    return daysRemain;
                }
                catch (Exception ex)
                {
                    OFLogger.Instance.LogError(ex.ToString());                    
                }
                return 0;
            }
        }

        public OFActivationState State
        {
            get
            {
                OFActivationState state = OFActivationState.Error;
                if (IsActivated)
                    return OFActivationState.Activated;
                if (!IsTrialPeriodEnded)
                    return OFActivationState.Trial;
                if (IsTrialPeriodEnded && !IsActivated)
                    return OFActivationState.NonActivated;
                return state;
            }
        }

        public void TryCheckAgain()
        {
            CheckActivationAndTrial();
        }

        public void Activate(Action callback)
        {
            OFLogger.Instance.LogDebug("Trying to activate...");
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
                string sCount = _turboActivate.GetFeatureValue(TimesUsed);
                int iCount;
                int.TryParse(sCount, out iCount);
                string pkeyid = OFRegistryHelper.Instance.GetPKeyId();
                int c = iCount + 1;
                if (!string.IsNullOrEmpty(pkeyid))
                {
                    var result = LimeLMApi.SetDetails(pkeyid, 0, null, new string[] {TimesUsed},
                        new string[] {c.ToString()});
                    OFLogger.Instance.LogDebug("Times used: {0}, Result: {1}", c, result);
                }
            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(ex.ToString());
            }
        }

        #endregion [public]

        #region [private property]

        private bool IsActivated { get; set; }

        private bool IsTrialPeriodEnded { get; set; }

        private bool IsInternetError { get; set; }

        #endregion [private property]

        #region [private]

        public void Init()
        {
            CheckActivationAndTrial();
        }

        private bool CheckActivation()
        {
            try
            {
                IsGenuineResult gr = _turboActivate.IsGenuine(DaysBetweenCheck, GraceOfInerErr, true);
                OFLogger.Instance.LogInfo("GenuineResult: {0}", gr);
                if (gr == IsGenuineResult.Genuine || gr == IsGenuineResult.GenuineFeaturesChanged ||
                    (IsInternetError = gr == IsGenuineResult.InternetError))
                {
                    return true;
                }
                var activated = _turboActivate.IsActivated();
                OFLogger.Instance.LogInfo("TurboActive.IsActivated() result: {0}", activated.ToString());
                if (activated)
                {
                    var result = _turboActivate.IsGenuine();
                    return result == IsGenuineResult.Genuine || result == IsGenuineResult.GenuineFeaturesChanged ||
                           result == IsGenuineResult.InternetError;
                }
                return false;
            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError("Error occured during checking activation: [{0}]", ex.ToString());
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
                string trialExpires = _turboActivate.GetFeatureValue(TrialExpires, null);
                isTrial = int.Parse(_turboActivate.GetFeatureValue(IsTrialKey, null)) > 0;
                OFLogger.Instance.LogDebug("Expires date: {0}", trialExpires);

                if (trialExpires != null)
                {
                    // this is a trial product key
                    // verify the trial hasn't expired
                    bool stillInTrial = _turboActivate.IsDateValid(trialExpires,
                        TA_DateCheckFlags.TA_HAS_NOT_EXPIRED);
                    OFLogger.Instance.LogDebug("Is Still in Trial: {0}", stillInTrial);
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
                OFLogger.Instance.LogError("Error occured during checking new activation: [{0}]", ex.ToString());
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
            bool result = true;
            try
            {
                OFLogger.Instance.LogInfo("UseTrial!!");
                _turboActivate.UseTrial();
                int days = DaysRemain;
                result = days == 0;
                OFLogger.Instance.LogInfo("DaysRemain = {0}", days);
            }
            catch (TrialExpiredException te)
            {
                OFLogger.Instance.LogError(te.ToString());
            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(ex.ToString());                
            }
            return result;
        }

        private void InternalActivate()
        {
            try
            {
                string path = Path.Combine(Path.GetDirectoryName(typeof(TurboLimeActivate).Assembly.Location), ActivationAppName);
                OFLogger.Instance.LogDebug("Path Activate: {0}", path);
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
            catch (Exception ex)
            {
                OFLogger.Instance.LogDebug("----- !!!! <<<<<< ERROR >>>>>>> !!! -----");
                OFLogger.Instance.LogError(ex.ToString());
            }
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
                _turboActivate.Deactivate(deleteKey);
                CheckActivationAndTrial();
                return true;
            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError("{0}", ex.ToString());
            }
            return false;
        }

        #endregion [private]
    }
}