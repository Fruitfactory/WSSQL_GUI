using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Input;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using WSUI.Core.Data;
using WSUI.Core.Data.UI;
using WSUI.Core.Extensions;
using WSUI.Core.Helpers;
using WSUI.Core.Interfaces;
using WSUI.Core.Utils.Dialog;
using WSUI.Infrastructure.Events;
using WSUI.Infrastructure.Implements.Systems;
using WSUI.Infrastructure.Payloads;
using WSUI.Infrastructure.Service;
using WSUI.Module.Core;
using WSUI.Module.Interface.Service;
using WSUI.Module.Interface.View;
using WSUI.Module.Interface.ViewModel;
using WSUI.Module.Service;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace WSUI.Module.ViewModel
{
    public class ContactDetailsViewModel : ViewModelBase, IContactDetailsViewModel, IScrollableViewExtended
    {

        private const double AvaregeTwoRowItemHeight = 47;
        private const double AvaregeOneRowItemHeight = 25;
        private const double FileValue = 0.2;
        private const double EmailValue = 0.7;
        private const double DefaultHeight = 600;//px


        private IEventAggregator _eventAggregator;
        private ISearchSystem _attachmentSearchSystem;
        private ISearchSystem _emailSearchSystem;
        private ISearchObject _selectedAttachment;
        private bool _isEmailsInitialized = false;
        private bool _isFilesInitialized = false;
        private IScrollBehavior _scrollBehavior;
        private IScrollBehavior _scrollBehavior2;

        private string _from = string.Empty;
        private string _to = string.Empty;


        private bool _isAttachmentBusy = true;
        private bool _isEmailBusy = true;

        public ContactDetailsViewModel(IEventAggregator eventAggregator, IContactDetailsView contactDetailsView)
        {
            _eventAggregator = eventAggregator;
            View = contactDetailsView;
            contactDetailsView.Model = this;
            CreateEmailCommand = new WSUIRelayCommand(CreateEmailExecute);
            MoreCommand = new DelegateCommand<object>(MoreCommandExecute, o => true);
            InitializeSearchSystem();
            MainEmailSource = new ObservableCollection<EmailSearchObject>();
            MainFileSource = new ObservableCollection<AttachmentSearchObject>();
            ScrollChangeCommand = new DelegateCommand<object>(OnScroll, o => true);
            ScrollChangedCommand2 = new DelegateCommand<object>(OnScroll2, o => true);
            _scrollBehavior = new ScrollBehavior { LimitReaction = 85 };
            _scrollBehavior.SearchGo += ScrollBehaviorOnSearchGo;
            _scrollBehavior2 = new ScrollBehavior { LimitReaction = 85 };
            _scrollBehavior2.SearchGo += ScrollBehaviorOnSearchGo2;
            EmailsSource = new ObservableCollection<EmailSearchObject>();
            ItemsSource = new ObservableCollection<AttachmentSearchObject>();
            IsDataExist = true;
            InitContactCommands();
        }



        private void ScrollBehaviorOnSearchGo()
        {
            RunEmailSearching(_from, _to);
        }

        private void ScrollBehaviorOnSearchGo2()
        {
            RunAttachmentSearching(_from, _to);
        }


        #region [interface]

        public object View { get; private set; }

        public ObservableCollection<AttachmentSearchObject> ItemsSource { get; private set; }

        public ObservableCollection<EmailSearchObject> EmailsSource { get; private set; }

        public ObservableCollection<AttachmentSearchObject> MainFileSource { get; private set; }

        public ObservableCollection<EmailSearchObject> MainEmailSource { get; private set; }

        public void SetDataObject(ISearchObject dataSearchObject)
        {
            _from = string.Empty;
            _to = string.Empty;
            if (dataSearchObject is EmailContactSearchObject)
            {
                var temp = dataSearchObject as EmailContactSearchObject;
                ApplyEmailContactInfo(temp);
                _from = FirstName;
                _to = temp.EMail.ToLowerInvariant();
            }
            else if (dataSearchObject is ContactSearchObject)
            {
                var temp = dataSearchObject as ContactSearchObject;
                ApplyContactInfo(temp);
                _from = string.Format("{0} {1}", temp.FirstName, temp.LastName);
                _to = temp.GetEmail();
            }
            SearchString = _to;
            RunAttachmentSearching(_from, _to);
            RunEmailSearching(_from, _to);
            OnPropertyChanged(() => SearchString);
        }

        public ISearchObject SelectedElement
        {
            get { return _selectedAttachment; }
            set
            {
                _selectedAttachment = value;
                RaiseSelectedChanged();
            }
        }

        public void ApplyIndexForShowing(int index)
        {
            SelectedIndex = index;
            OnPropertyChanged(() => SelectedIndex);
        }

        public IEnumerable<UIItem> ContactUIItemCollection { get; private set; }

        #endregion [interface]

        #region [commands]

        public ICommand CreateEmailCommand { get; private set; }

        public ICommand MoreCommand { get; private set; }

        #endregion [commands]

        #region [property]

        public bool IsBusy
        {
            get
            {
                var result = _isAttachmentBusy || _isEmailBusy;
                if (!result)
                {
                    CheckExistingData();
                }
                return result;
            }
        }

        public bool IsDataExist { get; private set; }

        public IEnumerable<string> Emails { get; private set; }

        public string FirstName { get; private set; }

        public string LastName { get; private set; }

        public string FotoFilepath { get; private set; }

        public string SearchString { get; private set; }

        public bool IsEmailVisible
        {
            get { return MainEmailSource.Count > 0; }
        }

        public bool IsFileVisible
        {
            get { return MainFileSource.Count > 0; }
        }

        public double ActualHeight { get; set; }

        public bool IsEmailMoreVisible { get; private set; }

        public bool IsFileMoreVisible { get; private set; }

        public double EmailHeight { get; private set; }

        public double FileHeight { get; private set; }

        public string FileHeader { get; private set; }

        public string EmailHeader { get; private set; }

        public int SelectedIndex { get; private set; }

        #endregion [property]

        #region [private]

        private IContactDetailsView ContactDetailsView { get { return View as IContactDetailsView; } }

        private void InitializeSearchSystem()
        {
            _attachmentSearchSystem = new ContactAttachmentSearchSystem();
            _attachmentSearchSystem.Init();
            _attachmentSearchSystem.SearchFinished += AttachmentSearchFinished;

            _emailSearchSystem = new ContactEmailSearchSystem();
            _emailSearchSystem.Init();
            _emailSearchSystem.SearchFinished += EmailSearchFinished;
        }

        private void EmailSearchFinished(object o)
        {
            System.Windows.Application.Current.Dispatcher.BeginInvoke((Action)ProcessEmailResult);
        }

        private void AttachmentSearchFinished(object o)
        {
            System.Windows.Application.Current.Dispatcher.BeginInvoke((Action)ProcessResult);
        }

        private void ProcessResult()
        {
            IList<ISystemSearchResult> result = _attachmentSearchSystem.GetResult();
            if (result == null || !result.Any())
                return;
            foreach (var systemSearchResult in result)
            {
                CollectionExtensions.AddRange(ItemsSource, systemSearchResult.Result.OfType<AttachmentSearchObject>());
            }
            if (!_isFilesInitialized && ItemsSource.Any())
            {
                var avaibleHeight = GetAvaibleHeightAndCount(FileValue, AvaregeOneRowItemHeight);
                IsFileMoreVisible = ItemsSource.Count > avaibleHeight.Item2 - 1;
                CollectionExtensions.AddRange(MainFileSource, ItemsSource.Take(avaibleHeight.Item2 - 1));
                FileHeight = IsFileMoreVisible ? avaibleHeight.Item1 : ItemsSource.Count * AvaregeOneRowItemHeight;
            }
            _isAttachmentBusy = false;
            OnPropertyChanged(() => ItemsSource);
            OnPropertyChanged(() => IsBusy);
            NotifyFilesPartChanged();

        }

        private void ProcessEmailResult()
        {
            IList<ISystemSearchResult> result = _emailSearchSystem.GetResult();
            if (result == null || !result.Any())
                return;
            foreach (var systemSearchResult in result)
            {
                CollectionExtensions.AddRange(EmailsSource, systemSearchResult.Result.OfType<EmailSearchObject>());
            }
            if (!_isEmailsInitialized && EmailsSource.Any())
            {
                var avaibleHeight = GetAvaibleHeightAndCount(EmailValue, AvaregeTwoRowItemHeight);
                IsEmailMoreVisible = EmailsSource.Count > avaibleHeight.Item2;
                CollectionExtensions.AddRange(MainEmailSource, EmailsSource.Take(avaibleHeight.Item2));
                EmailHeight = IsEmailMoreVisible ? avaibleHeight.Item1 : EmailsSource.Count * AvaregeTwoRowItemHeight;
                _isEmailsInitialized = true;
            }
            _isEmailBusy = false;
            OnPropertyChanged(() => EmailsSource);
            OnPropertyChanged(() => IsBusy);
            NotifyEmailPartChanged();
        }

        private void ApplyContactInfo(ContactSearchObject dataObject)
        {
            FirstName = dataObject.FirstName;
            LastName = dataObject.LastName;
            Emails = new List<string> { dataObject.EmailAddress, dataObject.EmailAddress2, dataObject.EmailAddress3 };
            FotoFilepath = OutlookHelper.Instance.GetContactFotoTempFileName(dataObject);
            RaiseNotification();
        }

        private void ApplyEmailContactInfo(EmailContactSearchObject dataObject)
        {
            Emails = new List<string>() { dataObject.EMail };
            string name = string.Empty;
            FirstName = IsAbleGetNameFromContact(dataObject.EMail.ToLower(), ref name) ? name : dataObject.EMail;
            RaiseNotification();
        }

        private bool IsAbleGetNameFromContact(string email, ref string name)
        {
            var contact = OutlookHelper.Instance.GetContact(email);
            if (contact == null)
            {
                return false;
            }
            name = string.Format("{0} {1}", contact.FirstName, contact.LastName);
            return true;
        }

        private void RunEmailSearching(string from, string to)
        {
            RunSearching(_emailSearchSystem, from, to);
        }

        private void RunAttachmentSearching(string from, string to)
        {
            RunSearching(_attachmentSearchSystem, from, to);
        }

        private void RunSearching(ISearchSystem searchSystem, string from, string to)
        {
            if (searchSystem == null)
                return;
            searchSystem.SetSearchCriteria(string.Format("{0};{1}", from, to));
            searchSystem.Search();
        }

        private void RaiseNotification()
        {
            OnPropertyChanged(() => LastName);
            OnPropertyChanged(() => FirstName);
            OnPropertyChanged(() => Emails);
            OnPropertyChanged(() => FotoFilepath);
        }

        private void RaiseSelectedChanged()
        {
            if (_eventAggregator == null)
                return;
            _eventAggregator.GetEvent<SelectedChangedPayloadEvent>().Publish(new SearchObjectPayload(SelectedElement));
        }

        private void CreateEmailExecute(object o)
        {
            var adr = o as string;
            if (string.IsNullOrEmpty(adr))
                return;

            Outlook.MailItem email = OutlookHelper.Instance.CreateNewEmail();
            email.To = adr;
            email.BodyFormat = Outlook.OlBodyFormat.olFormatHTML;
            email.Display(false);
        }

        private Tuple<double, int> GetAvaibleHeightAndCount(double a, double avaregeHeight)
        {
            var height = !double.Equals(ContactDetailsView.ActualHeight, 0.0) ? ContactDetailsView.ActualHeight : DefaultHeight;
            var avaibleHeight = height * a;
            var count = avaibleHeight / avaregeHeight;
            return new Tuple<double, int>(avaibleHeight, (int)count);
        }

        private void NotifyFilesPartChanged()
        {
            OnPropertyChanged(() => IsFileVisible);
            OnPropertyChanged(() => IsFileMoreVisible);
            OnPropertyChanged(() => FileHeight);
            OnPropertyChanged(() => FileHeader);
            OnPropertyChanged(() => MainFileSource);
        }

        private void NotifyEmailPartChanged()
        {
            OnPropertyChanged(() => IsEmailVisible);
            OnPropertyChanged(() => IsEmailMoreVisible);
            OnPropertyChanged(() => EmailHeight);
            OnPropertyChanged(() => EmailHeader);
            OnPropertyChanged(() => MainEmailSource);
        }

        private void MoreCommandExecute(object arg)
        {
            if (arg == null)
                return;
            SelectedIndex = int.Parse(arg.ToString());
            OnPropertyChanged(() => SelectedIndex);
        }


        private void CheckExistingData()
        {
            IsDataExist = IsEmailVisible || IsFileVisible;
            OnPropertyChanged(() => IsDataExist);
        }

        private void InitContactCommands()
        {
            ContactUIItemCollection = new List<UIItem>()
            {
                new UIItem(){UIName =  "Everething", Data = "M33.597977,10.759002C37.946865,10.759002 41.485962,14.285001 41.485962,18.649 41.485962,23 37.946865,26.535 33.597977,26.535 29.23909,26.535 25.709992,23 25.709992,18.649 25.709992,17.784 25.849955,16.953001 26.109888,16.174002 26.779719,16.881001 27.70948,17.327002 28.759213,17.327002 30.778696,17.327002 32.418278,15.691001 32.418278,13.668001 32.418278,12.663001 32.008381,11.748001 31.348551,11.087002 32.058369,10.876001 32.818176,10.759002 33.597977,10.759002z M33.606682,4.3679962C25.92741,4.3679957 19.698065,10.594956 19.698065,18.27293 19.698065,25.953894 25.92741,32.177862 33.606682,32.177862 41.295838,32.177862 47.515175,25.953894 47.515175,18.27293 47.515175,10.594956 41.295838,4.3679957 33.606682,4.3679962z M34.867642,1.546141E-09C36.890393,2.6508449E-05 58.705193,0.41938579 68.893006,18.299923 68.893006,18.299923 57.1442,36.139837 34.44656,34.768854 34.44656,34.768854 14.428583,36.59984 0,18.299923 0,18.299923 9.0791523,0.4590019 34.716553,0.0010111886 34.716553,0.0010114873 34.768162,-1.4442128E-06 34.867642,1.546141E-09z"},
                new UIItem(){UIName =  "Conversations", Data = "M0,4.0800388L0.030031017,4.0800388 12.610706,16.409995 26.621516,30.149985 40.642334,16.409995 53.223011,4.0800388 53.333001,4.0800388 53.333001,39.080039 0,39.080039z M3.1698808,0L26.660885,0 50.161892,0 38.411389,11.791528 26.660885,23.573054 14.920383,11.791528z"},
                new UIItem(){UIName =  "File Exchanged", Data = "F1M-1800.12,3181.48C-1800.12,3181.48 -1803.22,3172.42 -1813.67,3175.58 -1813.67,3175.58 -1822.52,3177.87 -1820.93,3188.08L-1807.87,3226.17 -1804.99,3225.18 -1817.71,3188.05C-1817.71,3188.05 -1820.44,3181.75 -1812.98,3178.38 -1812.98,3178.38 -1805.77,3175.6 -1802.56,3183.22L-1786.65,3229.63C-1786.65,3229.63 -1786.14,3234.44 -1790.63,3235.46 -1790.63,3235.46 -1792.78,3236.54 -1796.5,3233.79L-1803.82,3212.42 -1809.85,3194.84C-1809.85,3194.84 -1809.94,3192.33 -1808.27,3191.61 -1807.43,3191.25 -1805.9,3191.14 -1805.05,3193.37L-1794.19,3225.06 -1791.09,3224 -1802.17,3191.67C-1802.17,3191.67 -1803.96,3187.16 -1809.04,3188.84 -1810.81,3189.42 -1813.38,3190.48 -1812.95,3195.18L-1799.38,3234.78C-1799.38,3234.78 -1796.27,3240.08 -1789.53,3238.67 -1789.53,3238.67 -1783.39,3237.06 -1783.69,3229.41L-1800.12,3181.48z"}
            };

        }

        #endregion [private]

        #region [override]

        protected override void Dispose(bool disposing)
        {
            if (!Disposed && disposing)
            {
                if (_attachmentSearchSystem != null)
                {
                    _attachmentSearchSystem.SearchFinished -= AttachmentSearchFinished;
                    _attachmentSearchSystem = null;
                }
                if (_emailSearchSystem != null)
                {
                    _emailSearchSystem.SearchFinished -= EmailSearchFinished;
                    _emailSearchSystem = null;
                }
            }
            base.Dispose(disposing);
        }

        #endregion [override]

        public ICommand ScrollChangeCommand { get; private set; }

        private void OnScroll(object args)
        {
            InternalOnScroll(args as ScrollData, _scrollBehavior);
        }

        public ICommand ScrollChangedCommand2
        {
            get;
            private set;
        }


        private void OnScroll2(object args)
        {
            InternalOnScroll(args as ScrollData, _scrollBehavior2);
        }

        private void InternalOnScroll(ScrollData args, IScrollBehavior behavior)
        {
            if (args == null || behavior == null)
                return;
            behavior.NeedSearch(args);
        }

    }
}