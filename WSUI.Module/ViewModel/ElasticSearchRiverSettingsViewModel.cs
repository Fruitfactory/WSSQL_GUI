using System.Collections.ObjectModel;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Unity;
using OF.Core.Core.MVVM;
using OF.Module.Interface.View;
using OF.Module.Interface.ViewModel;

namespace OF.Module.ViewModel
{
    public class ElasticSearchRiverSettingsViewModel : ViewModelBase, IElasticSearchRiverSettingsViewModel
    {
        private IElasticSearchRiverSettingsView _view = null;
        private readonly IEventAggregator _eventAggregator;
        private readonly IUnityContainer _unityContainer;

        public ElasticSearchRiverSettingsViewModel(IEventAggregator eventAggregator, IUnityContainer unityContainer)
        {
            _eventAggregator = eventAggregator;
            _unityContainer = unityContainer;
        }


        #region [properties]

        public bool EveryNightOrIdle
        {
            get { return Get(() => EveryNightOrIdle); }
            set { Set(() => EveryNightOrIdle, value); }
        }

        public bool OnlyAt
        {
            get { return Get(() => OnlyAt); }
            set { Set(() => OnlyAt, value); }
        }

        public bool EveryHours
        {
            get { return Get(() => EveryHours); }
            set { Set(() => EveryHours, value); }
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

        public int RepeatHours
        {
            get { return Get(() => RepeatHours); }
            set { Set(() => RepeatHours, value); }
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


        #endregion



    }
}