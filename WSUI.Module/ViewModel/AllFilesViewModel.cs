using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using WSUI.Core.Data;
using WSUI.Core.Enums;
using WSUI.Core.Interfaces;
using WSUI.Core.Logger;
using WSUI.Infrastructure.Implements.Systems;
using WSUI.Module.Core;
using WSUI.Module.Interface.Service;
using WSUI.Module.Interface.View;
using WSUI.Module.Service;
using WSUI.Module.Strategy;
using Action = System.Action;
using Application = System.Windows.Application;
using Exception = System.Exception;

namespace WSUI.Module.ViewModel
{
    [KindNameId(KindsConstName.Everything, 0, @"pack://application:,,,/WSUI.Module;Component/Images/View.png", @"M33.597977,10.759002C37.946865,10.759002 41.485962,14.285001 41.485962,18.649 41.485962,23 37.946865,26.535 33.597977,26.535 29.23909,26.535 25.709992,23 25.709992,18.649 25.709992,17.784 25.849955,16.953001 26.109888,16.174002 26.779719,16.881001 27.70948,17.327002 28.759213,17.327002 30.778696,17.327002 32.418278,15.691001 32.418278,13.668001 32.418278,12.663001 32.008381,11.748001 31.348551,11.087002 32.058369,10.876001 32.818176,10.759002 33.597977,10.759002z M33.606682,4.3679962C25.92741,4.3679957 19.698065,10.594956 19.698065,18.27293 19.698065,25.953894 25.92741,32.177862 33.606682,32.177862 41.295838,32.177862 47.515175,25.953894 47.515175,18.27293 47.515175,10.594956 41.295838,4.3679957 33.606682,4.3679962z M34.867642,1.546141E-09C36.890393,2.6508449E-05 58.705193,0.41938579 68.893006,18.299923 68.893006,18.299923 57.1442,36.139837 34.44656,34.768854 34.44656,34.768854 14.428583,36.59984 0,18.299923 0,18.299923 9.0791523,0.4590019 34.716553,0.0010111886 34.716553,0.0010114873 34.768162,-1.4442128E-06 34.867642,1.546141E-09z")]
    public class AllFilesViewModel : KindViewModelBase, IUView<AllFilesViewModel>
    {
        private const double AvaregeTwoRowItemHeight = 40;
        private const double AvaregeOneRowItemHeight = 25;
        private const double ContactValue = 0.2;
        private const double FileValue = 0.15;
        private const double EmailValue = 0.5;


        public AllFilesViewModel(IUnityContainer container, 
            IEventAggregator eventAggregator, 
            ISettingsView<AllFilesViewModel> settingsView,
            IDataView<AllFilesViewModel> dataView)
            : base(container, eventAggregator)
        {
            SettingsView = settingsView;
            SettingsView.Model = this;
            DataView = dataView;
            DataView.Model = this;
            // init
            ID = 0;
            _name = "Everything";
            UIName = _name;
            _prefix = "AllFiles";
            IsOpen = false;
            FlyCommand = new DelegateCommand<object>(o =>
            {
                IsOpen = !IsOpen;
                OnPropertyChanged(() => IsOpen);
            },
                o => true);
            EmailClickCommand = new DelegateCommand<object>(o => EmailClick(o), o => true);
            MoreCommand = new DelegateCommand<object>(MoreCommandExecute, o => true);
            SearchSystem = new AllSearchSystem();
            ContactSource = new ObservableCollection<ISearchObject>();
            FileSource = new ObservableCollection<ISearchObject>();
        }

        public bool IsOpen { get; set; }

        public ICommand FlyCommand { get; private set; }

        public ICommand EmailClickCommand { get; protected set; }

        public ICommand MoreCommand { get; private set; }

        public ObservableCollection<ISearchObject> ContactSource { get; private set; }

        public ObservableCollection<ISearchObject> FileSource { get; private set; }

        public bool IsContactVisible
        {
            get { return ContactSource.Count > 0; }
        }

        public bool IsEmailVisible
        {
            get { return DataSource.Count > 0; }
        }

        public bool IsFileVisible
        {
            get { return FileSource.Count > 0; }
        }

        public double ActualHeight { get; set; }

        public bool IsContactMoreVisible { get; private set; }

        public bool IsEmailMoreVisible { get; private set; }

        public bool IsFileMoreVisible { get; private set; }

        public double ContactHeight { get; private set; }

        public double EmailHeight { get; private set; }

        public double FileHeight { get; private set; }

        public string ContactHeader { get; private set; }

        public string FileHeader { get; private set; }

        public string EmailHeader { get; private set; }

        protected override void OnSearchStringChanged()
        {
            ClearMainDataSource();
            base.OnSearchStringChanged();
        }

        protected override void ClearDataSource()
        {
            base.ClearDataSource();
            FileSource.Clear();
            ContactSource.Clear();
            OnPropertyChanged(() => ContactSource);
            OnPropertyChanged(() => FileSource);
            NotifyItemsVisibilityChanged();
        }

        protected override void ProcessMainResult()
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                IList<ISystemSearchResult> result = SearchSystem.GetResult();
                if (result.All(i => i.Result.Count == 0) && ShowMessageNoMatches)
                {
                    DataSource.Clear();
                    var message = new FileSearchObject
                    {
                        ItemName =
                            string.Format("Search for '{0}' returned no matches. Try different keywords.", SearchString),
                        TypeItem = TypeSearchItem.None
                    };
                    DataSource.Add(message);
                }
                else
                {
                    ProcessContacts(result);
                    ProcessEmails(result);
                    ProcessFiles(result);
                }
                ShowMessageNoMatches = true;
                NotifyItemsVisibilityChanged();
            }), null);
        }

        private void ProcessFiles(IList<ISystemSearchResult> result)
        {
            var files = result.Where(r => r.ObjectType == RuleObjectType.File);
            if (files.Any())
            {
                var fileAll = files.SelectMany(c => c.Result);
                var avaibleHeightAndCount = GetAvaibleHeightAndCount(FileValue, AvaregeOneRowItemHeight);
                IsFileMoreVisible = fileAll.Count() > avaibleHeightAndCount.Item2 - 1;
                FileHeader = IsFileMoreVisible ? string.Format("({0})", fileAll.Count()) : string.Empty;
                CollectionExtensions.AddRange(FileSource, fileAll.Take(avaibleHeightAndCount.Item2 - 1));
                FileHeight = avaibleHeightAndCount.Item1;
            }
        }

        private void ProcessEmails(IList<ISystemSearchResult> result)
        {
            var emails = result.Where(r => r.ObjectType == RuleObjectType.Email);
            if (emails.Any())
            {
                var emailAll = emails.SelectMany(c => c.Result);
                var avaibleHeightAndCount = GetAvaibleHeightAndCount(EmailValue, AvaregeTwoRowItemHeight);
                IsEmailMoreVisible = emailAll.Count() > avaibleHeightAndCount.Item2;
                EmailHeader = IsEmailMoreVisible ? string.Format("({0})", emailAll.Count()) : string.Empty;
                CollectionExtensions.AddRange(DataSource, emailAll.Take(avaibleHeightAndCount.Item2));
                EmailHeight = avaibleHeightAndCount.Item1;
            }
        }

        private void ProcessContacts(IList<ISystemSearchResult> result)
        {
            var contacts = result.Where(r => r.ObjectType == RuleObjectType.Contact);
            if (contacts.Any())
            {
                var contactAll = contacts.SelectMany(c => c.Result);
                var avaibleHeightAndCount = GetAvaibleHeightAndCount(ContactValue, AvaregeOneRowItemHeight);
                IsContactMoreVisible = contactAll.Count() > avaibleHeightAndCount.Item2 - 1;
                ContactHeader = IsContactMoreVisible ? string.Format("({0})", contactAll.Count()) : string.Empty;
                CollectionExtensions.AddRange(ContactSource, contactAll.Take(avaibleHeightAndCount.Item2 - 1));
                ContactHeight = IsContactMoreVisible ? avaibleHeightAndCount.Item1 : contactAll.Count() * AvaregeOneRowItemHeight;
            }
        }

        private Tuple<double, int> GetAvaibleHeightAndCount(double a, double avaregeHeight)
        {
            var avaibleHeight = ActualHeight * a;
            var count = avaibleHeight / avaregeHeight;
            return new Tuple<double, int>(avaibleHeight, (int)count);
        }

        private void EmailClick(object obj)
        {
            try
            {
                CommandElementClick(obj);
            }
            catch (Exception ex)
            {
                WSSqlLogger.Instance.LogError(ex.Message);
            }
        }

        private void MoreCommandExecute(object arg)
        {
            if (arg == null)
                return;
            Parent.SelectKind(arg.ToString());
        }

        private bool CommandElementClick(object obj)
        {
            if (!(obj is CommandSearchObject))
                return false;
            Parent.SelectKind(KindsConstName.People);
            return true;
        }

        private void NotifyItemsVisibilityChanged()
        {
            OnPropertyChanged(() => IsContactVisible);
            OnPropertyChanged(() => IsEmailVisible);
            OnPropertyChanged(() => IsFileVisible);
            OnPropertyChanged(() => IsContactMoreVisible);
            OnPropertyChanged(() => IsEmailMoreVisible);
            OnPropertyChanged(() => IsFileMoreVisible);
            OnPropertyChanged(() => ContactHeight);
            OnPropertyChanged(() => EmailHeight);
            OnPropertyChanged(() => FileHeight);
            OnPropertyChanged(() => ContactHeader);
            OnPropertyChanged(() => EmailHeader);
            OnPropertyChanged(() => FileHeader);
        }

        protected override void OnInit()
        {
            base.OnInit();
            CommandStrategies.Add(TypeSearchItem.Email, CommadStrategyFactory.CreateStrategy(TypeSearchItem.Email, this));
            ICommandStrategy fileAttach = CommadStrategyFactory.CreateStrategy(TypeSearchItem.FileAll, this);
            CommandStrategies.Add(TypeSearchItem.File, fileAttach);
            CommandStrategies.Add(TypeSearchItem.Attachment, fileAttach);
            CommandStrategies.Add(TypeSearchItem.Picture, fileAttach);
            CommandStrategies.Add(TypeSearchItem.FileAll, fileAttach);
            ScrollBehavior = new ScrollBehavior { CountFirstProcess = 300, CountSecondProcess = 100, LimitReaction = 99 };
            ScrollBehavior.SearchGo += OnScrollNeedSearch;
            TopQueryResult = ScrollBehavior.CountFirstProcess;
        }

        #region IUIView

        public ISettingsView<AllFilesViewModel> SettingsView { get; set; }

        public IDataView<AllFilesViewModel> DataView { get; set; }

        #endregion IUIView
    }
}