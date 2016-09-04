using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Windows.Input;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Unity;
using Newtonsoft.Json;
using OF.Core;
using OF.Core.Core.ElasticSearch;
using OF.Core.Core.MVVM;
using OF.Core.Data.ElasticSearch;
using OF.Core.Data.ElasticSearch.Response;
using OF.Core.Enums;
using OF.Core.Extensions;
using OF.Core.Helpers;
using OF.Core.Interfaces;
using OF.Core.Logger;
using OF.Core.Utils.Dialog;
using OF.Module.Interface.View;
using OF.Module.Interface.ViewModel;

namespace OF.Module.ViewModel.Settings
{
    public class OFElasticSearchRiverSettingsViewModel : OFViewModelBase, IElasticSearchRiverSettingsViewModel
    {
        private IElasticSearchRiverSettingsView _view = null;
        private readonly IEventAggregator _eventAggregator;
        private readonly IUnityContainer _unityContainer;
        private OFRiverMeta _settingsMeta;
        private bool _canForce = true;

        #region [local classes]

        class OnlyAtSettings
        {
            [JsonProperty("hour_only_at")]
            public int HourOnlyAt { get; set; }

            [JsonProperty("hour_type")]
            public string HourType { get; set; }
        }

        class EveryHourPeriodSettings
        {
            [JsonProperty("hour_period")]
            public int HourPeriod { get; set; }
        }


        class NightIdleSettings
        {
            [JsonProperty("idle_time")]
            public int IdleTime { get; set; }
        }


        #endregion


        public OFElasticSearchRiverSettingsViewModel(IEventAggregator eventAggregator, IUnityContainer unityContainer)
        {
            _eventAggregator = eventAggregator;
            _unityContainer = unityContainer;
        }


        #region [properties]

        public bool EveryNightOrIdle
        {
            get { return Get(() => EveryNightOrIdle); }
            set
            {
                Set(() => EveryNightOrIdle, value);
                OnEveryNightOrIdleChanged(value);
            }
        }

        

        public bool OnlyAt
        {
            get { return Get(() => OnlyAt); }
            set
            {
                Set(() => OnlyAt, value);
                OnOnlyAtChanged(value);
            }
        }

        

        public bool EveryHours
        {
            get { return Get(() => EveryHours); }
            set
            {
                Set(() => EveryHours, value);
                OnEveryHoursChanged(value);
            }
        }

        public bool Never
        {
            get { return Get(() => Never); }
            set { Set(() => Never, value); }
        }

        public ObservableCollection<int> HoursSource
        {
            get { return Get(() => HoursSource); }
            set { Set(() => HoursSource, value); }
        }

        public int HourOnlyAt
        {
            get { return Get(() => HourOnlyAt); }
            set { Set(() => HourOnlyAt, value); }
        }

        public string HourType
        {
            get { return Get(() => HourType); }
            set { Set(() => HourType, value); }
        }

        public ObservableCollection<string> HourTypes
        {
            get { return Get(() => HourTypes); }
            set { Set(() => HourTypes, value); }
        }

        public int RepeatHours
        {
            get { return Get(() => RepeatHours); }
            set { Set(() => RepeatHours, value); }
        }


        public int IdleTime
        {
            get { return Get(() => IdleTime); }
            set { Set(() => IdleTime, value); }
        }

        public ICommand ForceCommand
        {
            get { return Get(() => ForceCommand); }
            set { Set(() => ForceCommand, value); }
        }


        public void ApplySettings()
        {
            Save();
        }

        public object View
        {
            get
            {
                if (_view == null)
                {
                    _view = _unityContainer.Resolve<IElasticSearchRiverSettingsView>();
                    _view.Model = this;
                }
                return _view;
            }
        }

        public void Initialize()
        {
            ForceCommand = new OFRelayCommand(ForceCommandExecute,CanForceCommandExecute);
            HoursSource = new ObservableCollection<int>()
            {
                1,2,3,4,5,6,7,8,9,10,11,12
            };
            HourTypes = new ObservableCollection<string>(){"AM","PM"};
            ReadSettings();
            HasChanges = false;
        }

        public bool HasDetailsChanges { get { return HasChanges; } }

        #endregion

        #region [private]

        private void OnOnlyAtChanged(bool newValue)
        {
            if (HourOnlyAt == default(int))
            {
                HourOnlyAt = 1;
            }
            if (HourType.IsEmpty())
            {
                HourType = "AM";
            }
        }

        private void OnEveryHoursChanged(bool value)
        {
            if (RepeatHours == default(int))
            {
                RepeatHours = 1;
            }
        }
        
        private void OnEveryNightOrIdleChanged(bool value)
        {
            if(IdleTime == default(int))
            {
                IdleTime = 1;
            }
        }


        private void ReadSettings()
        {
            try
            {
                _settingsMeta =
                    OFObjectJsonSaveReadHelper.Instance.Read<OFRiverMeta>(GlobalConst.SettingsRiverFile);
                if (_settingsMeta.IsNull())
                {
                    _settingsMeta = new OFRiverMeta(OFElasticSearchClientBase.DefaultInfrastructureName);
                }
                if (_settingsMeta.IsNotNull())
                {
                    switch (_settingsMeta.Pst.Schedule.ScheduleType)
                    {
                        case OFRiverSchedule.EveryNightOrIdle:
                            EveryNightOrIdle = true;
                            var nightIdleTimeSettings =
                                JsonConvert.DeserializeObject(_settingsMeta.Pst.Schedule.Settings,
                                    typeof (NightIdleSettings)) as NightIdleSettings;
                            if (nightIdleTimeSettings.IsNotNull())
                            {
                                IdleTime = nightIdleTimeSettings.IdleTime  / 60;
                            }
                            break;
                        case OFRiverSchedule.EveryHours:
                            EveryHours = true;
                            var periodSettings = JsonConvert.DeserializeObject(_settingsMeta.Pst.Schedule.Settings, typeof(EveryHourPeriodSettings)) as EveryHourPeriodSettings;
                            if (periodSettings.IsNotNull())
                            {
                                RepeatHours = periodSettings.HourPeriod;
                            }
                            break;
                        case OFRiverSchedule.OnlyAt:
                            OnlyAt = true;
                            var onlyAtSettings =
                                JsonConvert.DeserializeObject(_settingsMeta.Pst.Schedule.Settings, typeof(OnlyAtSettings)) as OnlyAtSettings;
                            if (onlyAtSettings.IsNotNull())
                            {
                                HourOnlyAt = onlyAtSettings.HourOnlyAt;
                                HourType = onlyAtSettings.HourType;
                            }
                            break;
                        case OFRiverSchedule.Never:
                            Never = true;
                            break;
                        default:
                            break;
                    }
                }

            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(ex.Message);                
            }
        }

        private void Save()
        {
            if (_settingsMeta.IsNull())
            {
                return;
            }

            if (EveryNightOrIdle)
            {
                _settingsMeta.Pst.Schedule.ScheduleType = OFRiverSchedule.EveryNightOrIdle; 
                _settingsMeta.Pst.Schedule.Settings = JsonConvert.SerializeObject(new NightIdleSettings(){IdleTime  = (IdleTime * 60)});
            }
            if (OnlyAt)
            {
                _settingsMeta.Pst.Schedule.ScheduleType = OFRiverSchedule.OnlyAt;
                _settingsMeta.Pst.Schedule.Settings = JsonConvert.SerializeObject(new OnlyAtSettings(){HourOnlyAt = HourOnlyAt,HourType = HourType});
            }
            if (EveryHours)
            {
                _settingsMeta.Pst.Schedule.ScheduleType = OFRiverSchedule.EveryHours;
                _settingsMeta.Pst.Schedule.Settings = JsonConvert.SerializeObject(new EveryHourPeriodSettings(){HourPeriod = RepeatHours});
            }
            if (Never)
            {
                _settingsMeta.Pst.Schedule.ScheduleType = OFRiverSchedule.Never;
                _settingsMeta.Pst.Schedule.Settings = String.Empty;
            }
            var updateClient = _unityContainer.Resolve<IElasticUpdateSettingsClient>();
            if (updateClient.IsNotNull())
            {
                updateClient.UpdateSettings(_settingsMeta);
            }
        }


        private bool CanForceCommandExecute(object o)
        {

            try
            {
                var restElasticSearchClient = _unityContainer.Resolve<IElasticSearchRiverStatus>();
                if (restElasticSearchClient == null)
                {
                    return false;
                }

                var status = restElasticSearchClient.GetRiverStatus();
                return _canForce && status != null && status.Response != null &&
                       status.Response.Status == OFRiverStatus.StandBy;
            }
            catch (WebException we)
            {
                OFLogger.Instance.LogError(we.ToString());
            }
            return false;
        }

        private void ForceCommandExecute(object o)
        {
            var force = _unityContainer.Resolve<IElasticSearchForceClient>();
            if (force == null)
            {
                return;
            }
            force.Force();
            _canForce = false;
        }

        #endregion


    }
}