using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Documents;
using System.Windows.Input;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Events;
using WSUI.Core.Interfaces;
using WSUI.Core.Logger;
using WSUI.Infrastructure.Events;
using WSUI.Infrastructure.Payloads;
using WSUI.Module.Interface.Service;
using WSUI.Module.Interface.View;
using WSUI.Module.Interface.ViewModel;
using WSUI.Module.Service;

namespace WSUI.Module.Core
{
    internal abstract class ContactKindDetailsViewModelBase<T> : ViewModelBase , IContactKindDetailsViewModel<T> where T : ISearchObject
    {

        private const double DefaultHeight = 600;//px



        private IMainViewModel _mainViewModel;
        private IEventAggregator _eventAggregator;
        private ISearchSystem _searchSystem;
        private ISearchObject _selectedObject;
        private bool _isInitialized;
        private IScrollBehavior _scrollBehavior;
        private IContactDetailsView _mainContactDetailsView;

        private string _from = string.Empty;
        private string _to = string.Empty;
        private string _searchString;


        #region [ctor]

        protected ContactKindDetailsViewModelBase(IEventAggregator eventAggregator, IMainViewModel mainViewModel, IContactDetailsView mainContactDetailsView, IContactKindDetailsView<T> view)
        {
            _mainViewModel = mainViewModel;
            _eventAggregator = eventAggregator;
            _mainContactDetailsView = mainContactDetailsView;
            View = view;
            view.Model = this;
            ItemSource = new ObservableCollection<T>();
            PreviewSource = new ObservableCollection<T>();
        } 

        #endregion

        #region [properties]

        public object View
        {
            get; private set;
        }

        public string SearchString 
        {
            get { return _searchString; }
            set
            {
                _searchString = value;
                ResetSearch();
            }
        }

        public ISearchObject SelectedObject {
            get { return _selectedObject; }
            set
            {
                _selectedObject = value;
                RaiseSelectedChanged();
            } 
        }
        public ISearchObject TrackedObject { get; set; }

        public ObservableCollection<T> ItemSource { get; private set; }

        public ObservableCollection<T> PreviewSource { get; private set; }

        public ICommand ScrollChangedCommand { get; private set; }

        public ICommand SearchCommand { get; private set; }

        public ICommand KeyDowmCommand { get; private set; }

        public bool IsBusy { get; private set; }

        public bool IsMoreVisible { get; private set; }

        public double Height { get; private set; }

        #endregion

        public virtual void Initialize()
        {
            try
            {
                _searchSystem = CreateSearchSystem();
                _searchSystem.Init();
                _searchSystem.SearchFinished += SearchSystemOnSearchFinished;
                _scrollBehavior = new ScrollBehavior { LimitReaction = 85 };
                _scrollBehavior.SearchGo += ScrollBehaviorOnSearchGo;

            }
            catch (Exception ex)
            {
                WSSqlLogger.Instance.LogError(ex.Message);
            }
        }

        private void ScrollBehaviorOnSearchGo()
        {
            RunSearching();
        }

        private void SearchSystemOnSearchFinished(object o)
        {
            System.Windows.Application.Current.Dispatcher.BeginInvoke((Action)ProcessResult);
        }

        private void ProcessResult()
        {
            IList<ISystemSearchResult> result = _searchSystem.GetResult();
            if (result == null || !result.Any())
                return;
            foreach (var systemSearchResult in result)
            {
                CollectionExtensions.AddRange(ItemSource, systemSearchResult.Result.OfType<T>());
            }
            if (!_isInitialized && ItemSource.Any())
            {
                var avaibleHeight = GetAvaibleHeightAndCount(GetFillValue(), GetAvaregeRowItemHeight());
                IsMoreVisible = ItemSource.Count > avaibleHeight.Item2 - 1;
                CollectionExtensions.AddRange(PreviewSource, ItemSource.Take(avaibleHeight.Item2 - 1));
                Height = IsMoreVisible ? avaibleHeight.Item1 : ItemSource.Count * GetAvaregeRowItemHeight();
                _isInitialized = true;
            }
            IsBusy = false;
            OnPropertyChanged(() => ItemSource);
            OnPropertyChanged(() => IsBusy);
        }

        public void SetSearchContext(string from, string to)
        {
            _from = from;
            _to = to;
        }

        protected abstract ISearchSystem CreateSearchSystem();

        protected abstract double GetFillValue();

        protected abstract double GetAvaregeRowItemHeight();
        
        private void RaiseSelectedChanged()
        {
            if (_eventAggregator == null || SelectedObject == null)
                return;
            _eventAggregator.GetEvent<SelectedChangedPayloadEvent>().Publish(new SearchObjectPayload(SelectedObject));
        }

        private void RunSearching()
        {
            if (_searchSystem == null)
                return;
            _searchSystem.SetSearchCriteria(string.Format("{0};{1};{2}", _from, _to, SearchString));
            _searchSystem.Search();
        }

        private void ResetSearch()
        {
            if (_searchSystem == null)
            {
                return;
            }
            _searchSystem.Reset();
            ItemSource.Clear();
            OnPropertyChanged(() => ItemSource);
        }

        private Tuple<double, int> GetAvaibleHeightAndCount(double a, double avaregeHeight)
        {
            var height = !double.Equals(_mainContactDetailsView.ActualHeight, 0.0) ? _mainContactDetailsView.ActualHeight : DefaultHeight;
            var avaibleHeight = height * a;
            var count = avaibleHeight / avaregeHeight;
            return new Tuple<double, int>(avaibleHeight, (int)count);
        }



    }
}