using System.Windows.Forms.VisualStyles;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Practices.Unity;
using OF.Core.Core.MVVM;
using OF.Core.Data;
using OF.Core.Data.UI;
using OF.Core.Extensions;
using OF.Core.Helpers;
using OF.Core.Interfaces;
using OF.Core.Utils.Dialog;
using OF.Infrastructure.Events;
using OF.Infrastructure.Implements.Contact;
using OF.Infrastructure.Interfaces.Search;
using OF.Infrastructure.Payloads;
using OF.Infrastructure.Service;
using OF.Module.Core;
using OF.Module.Enums;
using OF.Module.Interface.Service;
using OF.Module.Interface.View;
using OF.Module.Interface.ViewModel;
using OF.Module.Service;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace OF.Module.ViewModel
{
    public class ContactDetailsViewModel : OFViewModelBase, IContactDetailsViewModel, IScrollableViewExtended
    {
        private const double AvaregeTwoRowItemHeight = 47;
        private const double AvaregeOneRowItemHeight = 25;
        private const double FileValue = 0.2;
        private const double EmailValue = 0.7;
        private const double DefaultHeight = 600;//px
        private const double Delta = 32; // "more" section, mergings and paddings

        private IMainViewModel _mainViewModel;
        private IEventAggregator _eventAggregator;
        private IUnityContainer _unityContainer;

        private IContactSearchSystem _contactEmailSearching;
        private IContactSearchSystem _contactAttachmentSearching;

        private ISearchObject _selectedAttachment;
        private bool _isEmailsInitialized = false;
        private bool _isFilesInitialized = false;
        private IScrollBehavior _scrollBehavior;
        private IScrollBehavior _scrollBehavior2;

        private string _from = string.Empty;
        private string _to = string.Empty;

        private string _searchEmailString = string.Empty;

        private bool _isAttachmentBusy = false;
        private bool _isEmailBusy = false;
        private string _searchAttachmentString;

        private readonly object Lock = new object();


        public ContactDetailsViewModel(IEventAggregator eventAggregator, IUnityContainer unityContainer, IContactDetailsView contactDetailsView, IMainViewModel mainViewModel)
        {
            _eventAggregator = eventAggregator;
            _unityContainer = unityContainer;
            _mainViewModel = mainViewModel;
            View = contactDetailsView;
            contactDetailsView.Model = this;
            InitializeSearchSystem();

            InitCommands();

            _scrollBehavior = new OFScrollBehavior { LimitReaction = 85 };
            _scrollBehavior.SearchGo += ScrollBehaviorOnSearchGo;
            _scrollBehavior2 = new OFScrollBehavior { LimitReaction = 85 };
            _scrollBehavior2.SearchGo += ScrollBehaviorOnSearchGo2;
            EmailsSource = new ObservableCollection<OFEmailSearchObject>();
            ItemsSource = new ObservableCollection<OFAttachmentContentSearchObject>();
            InitContactCommands();
        }

        private void InitCommands()
        {
            CreateEmailCommand = new OFRelayCommand(CreateEmailExecute);
            MoreCommand = new DelegateCommand<object>(MoreCommandExecute, o => true);
            MainEmailSource = new ObservableCollection<OFEmailSearchObject>();
            MainFileSource = new ObservableCollection<OFAttachmentContentSearchObject>();
            ScrollChangeCommand = new DelegateCommand<object>(OnScroll, o => true);
            ScrollChangedCommand2 = new DelegateCommand<object>(OnScroll2, o => true);
            CopyPhoneCommand = new DelegateCommand<object>(CopyPhoneToClipboard, o => true);
            SearchEmailCommand = new DelegateCommand<object>(EmailSearchExecute, o => true);
            SearchAttachmentCommand = new DelegateCommand<object>(AttacmentSearchExecute, o => true);
            EmailContextKeyDownCommand = new DelegateCommand<object>(KeyEmailDown, o => true);
            AttachmentContextKeyDownCommand = new DelegateCommand<object>(KeyAttachmentDown, o => true);
            EmailHeightCalculateCommand = new DelegateCommand<object>(CalculateEmailListBoxHeight, o => true);
        }

        private void EmailSearchExecute(object o)
        {
            RunEmailSearching(_from, _to);
        }

        private void AttacmentSearchExecute(object o)
        {
            RunAttachmentSearching(_from, _to);
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

        public ObservableCollection<OFAttachmentContentSearchObject> ItemsSource { get; private set; }

        public ObservableCollection<OFEmailSearchObject> EmailsSource { get; private set; }

        public ObservableCollection<OFAttachmentContentSearchObject> MainFileSource { get; private set; }

        public ObservableCollection<OFEmailSearchObject> MainEmailSource { get; private set; }

        public void SetDataObject(ISearchObject dataSearchObject)
        {
            _from = string.Empty;
            _to = string.Empty;
            if (dataSearchObject is OFEmailContactSearchObject)
            {
                var temp = dataSearchObject as OFEmailContactSearchObject;
                ApplyEmailContactInfo(temp);
                _from = FirstName;
                _to = temp.EMail.ToLowerInvariant();
            }
            else if (dataSearchObject is OFContactSearchObject)
            {
                var temp = dataSearchObject as OFContactSearchObject;
                ApplyContactInfo(temp);
                _from = string.Format("{0} {1}", temp.FirstName, temp.LastName);
                _to = temp.GetEmail();
            }
            SearchString = _to;

            RunEmailPreviewSearching(_from, _to);
            
            OnPropertyChanged(() => SearchString);
        }

        public bool IsSameData(ISearchObject dataObject)
        {
            if (dataObject is OFEmailContactSearchObject)
            {
                var emailContact = dataObject as OFEmailContactSearchObject;
                var result = FirstName == emailContact.ContactName && ( string.IsNullOrEmpty(emailContact.EMail) || Emails.Any(e => string.Equals(e, emailContact.EMail, StringComparison.InvariantCultureIgnoreCase)));
                return result;
            }
            if (dataObject is OFContactSearchObject)
            {
                var contact = dataObject as OFContactSearchObject;
                var result = FirstName == contact.FirstName && LastName == contact.LastName &&
                       (string.IsNullOrEmpty(contact.EmailAddress1) || Emails.Any(
                           e => string.Equals(e, contact.EmailAddress1, StringComparison.InvariantCultureIgnoreCase)));
                return result;
            }
            return false;
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

        public ISearchObject TrackedElement { get; set; }

        public void ApplyIndexForShowing(int index)
        {
            SelectedIndex = index;
            OnPropertyChanged(() => SelectedIndex);
            ResetSelectedChanged();
        }

        public IEnumerable<OFUIItem> ContactUIItemCollection { get; private set; }

        #endregion [interface]

        #region [commands]

        public ICommand CreateEmailCommand { get; private set; }

        public ICommand MoreCommand { get; private set; }

        public ICommand CopyPhoneCommand { get; private set; }

        public ICommand SearchEmailCommand { get; private set; }

        public ICommand SearchAttachmentCommand { get; private set; }

        public ICommand EmailContextKeyDownCommand { get; private set; }

        public ICommand AttachmentContextKeyDownCommand { get; private set; }

        public ICommand EmailHeightCalculateCommand { get; private set; }

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

        public string SearchCriteria
        {
            get
            {
                return SelectedIndex == 1 ? SearchEmailString
                    : SelectedIndex == 2 ? SearchAttachmentString
                    : string.Empty;
            }
        }

        public IEnumerable<MenuItem> EmailMenuItems { get { return _mainViewModel.EmailsMenuItems; } }

        public IEnumerable<MenuItem> FileMenuItems { get { return _mainViewModel.FileMenuItems; } }

        public string BusinessTelephone { get; private set; }

        public string HomeTelephone { get; private set; }

        public string MobileTelephone { get; private set; }

        public bool BusinessPhoneVisible
        {
            get { return !string.IsNullOrEmpty(BusinessTelephone); }
        }

        public bool HomeTelephoneVisible
        {
            get { return !string.IsNullOrEmpty(HomeTelephone); }
        }

        public bool MobileTelephoneVisible
        {
            get { return !string.IsNullOrEmpty(MobileTelephone); }
        }

        public string SearchEmailString
        {
            get { return _searchEmailString; }
            set
            {
                SearchString = _searchEmailString = value;
                ResetEmailSearch();
            }
        }

        private void ResetEmailSearch()
        {
            if (_contactEmailSearching == null)
            {
                return;
            }
            _contactEmailSearching.ResetMainSystem();
            EmailsSource.Clear();
            OnPropertyChanged(() => EmailsSource);
        }

        public string SearchAttachmentString
        {
            get { return _searchAttachmentString; }
            set
            {
                SearchString = _searchAttachmentString = value;
                ResetAttacmentSearch();
            }
        }

        private void ResetAttacmentSearch()
        {
            if (_contactAttachmentSearching == null)
            {
                return;
            }
            _contactAttachmentSearching.ResetMainSystem();
            ItemsSource.Clear();
            OnPropertyChanged(() => ItemsSource);
        }

        public bool IsEmailBusy { get; private set; }

        public bool IsAttachmentBusy { get; private set; }

        #endregion [property]

        #region [private]

        private IContactDetailsView ContactDetailsView { get { return View as IContactDetailsView; } }

        private void InitializeSearchSystem()
        {
            _contactEmailSearching = new OFContactEmailSearching(Lock);
            _contactEmailSearching.Initialize(_unityContainer);
            _contactEmailSearching.PreviewSearchingFinished += ContactEmailSearchingOnPreviewSearchingFinished;
            _contactEmailSearching.MainSearchingFinished += ContactEmailSearchingOnMainSearchingFinished;

            _contactAttachmentSearching = new OFContactAttachmentSearching(Lock);
            _contactAttachmentSearching.Initialize(_unityContainer);
            _contactAttachmentSearching.PreviewSearchingFinished += ContactAttachmentSearchingOnPreviewSearchingFinished;
            _contactAttachmentSearching.MainSearchingFinished += ContactAttachmentSearchingOnMainSearchingFinished;
        }

        private void ContactAttachmentSearchingOnMainSearchingFinished(object sender, EventArgs eventArgs)
        {
            System.Windows.Application.Current.Dispatcher.BeginInvoke((Action)ProcessMainAttachmentResult);
        }

        private void ContactAttachmentSearchingOnPreviewSearchingFinished(object sender, EventArgs eventArgs)
        {
            System.Windows.Application.Current.Dispatcher.BeginInvoke((Action)ProcessPreviewAttachmentResult);
        }

        private void ContactEmailSearchingOnMainSearchingFinished(object sender, EventArgs eventArgs)
        {
            System.Windows.Application.Current.Dispatcher.BeginInvoke((Action)ProcessMainEmailResult);
        }

        private void ContactEmailSearchingOnPreviewSearchingFinished(object sender, EventArgs eventArgs)
        {
            System.Windows.Application.Current.Dispatcher.BeginInvoke((Action)ProcessPreviewEmailResult);
        }

        private void ProcessMainAttachmentResult()
        {
            IList<ISystemSearchResult> result = _contactAttachmentSearching.GetMainResult();
            if (result == null || !result.Any())
                return;
            foreach (var systemSearchResult in result)
            {
                CollectionExtensions.AddRange(ItemsSource, systemSearchResult.Result.OperationResult.OfType<OFAttachmentContentSearchObject>());
            }
            OnPropertyChanged(() => ItemsSource);
        }

        private void ProcessPreviewAttachmentResult()
        {
            IList<ISystemSearchResult> result = _contactAttachmentSearching.GetPreviewResult();
            if (result == null || !result.Any())
                return;
            List<OFAttachmentContentSearchObject> items = new List<OFAttachmentContentSearchObject>();
            foreach (var systemSearchResult in result)
            {
                items.AddRange(systemSearchResult.Result.OperationResult.OfType<OFAttachmentContentSearchObject>());
            }
            if (!_isFilesInitialized && items.Any())
            {
                var avaibleHeight = GetAvaibleHeightAndCount(FileValue, AvaregeOneRowItemHeight);
                IsFileMoreVisible = items.Count > avaibleHeight.Item2 - 1;
                CollectionExtensions.AddRange(MainFileSource, items.Take(avaibleHeight.Item2 - 1));
                FileHeight = IsFileMoreVisible ? avaibleHeight.Item1 : items.Count * AvaregeOneRowItemHeight;
            }
            NotifyFilesPartChanged();
            RunAttachmentSearching(_from, _to);
        }

        private void ProcessMainEmailResult()
        {
            IList<ISystemSearchResult> result = _contactEmailSearching.GetMainResult();
            if (result == null || !result.Any())
                return;
            foreach (var systemSearchResult in result)
            {
                CollectionExtensions.AddRange(EmailsSource, systemSearchResult.Result.OperationResult.OfType<OFEmailSearchObject>());
            }
            OnPropertyChanged(() => EmailsSource);
        }

        private void ProcessPreviewEmailResult()
        {
            IList<ISystemSearchResult> result = _contactEmailSearching.GetPreviewResult();
            if (result == null || !result.Any())
                return;
            List<OFEmailSearchObject> items = new List<OFEmailSearchObject>();
            foreach (var systemSearchResult in result)
            {
                items.AddRange(systemSearchResult.Result.OperationResult.OfType<OFEmailSearchObject>());
            }
            if (!_isEmailsInitialized && items.Any())
            {
                var avaibleHeight = GetAvaibleHeightAndCount(EmailValue, AvaregeTwoRowItemHeight);
                IsEmailMoreVisible = items.Count > avaibleHeight.Item2;
                CollectionExtensions.AddRange(MainEmailSource, items.Take(avaibleHeight.Item2));
                EmailHeight = IsEmailMoreVisible ? avaibleHeight.Item1 : items.Count * AvaregeTwoRowItemHeight;
                _isEmailsInitialized = true;
            }
            NotifyEmailPartChanged();
            RunEmailSearching(_from, _to);
            RunAttachemntPreviewSearching(_from, _to);
        }

        private void ApplyContactInfo(OFContactSearchObject dataObject)
        {
            FirstName = dataObject.FirstName;
            LastName = dataObject.LastName;
            Emails = (new List<string> { dataObject.EmailAddress1, dataObject.EmailAddress2, dataObject.EmailAddress3 }).Distinct().Where(s => !string.IsNullOrEmpty(s));
            FotoFilepath = OFOutlookHelper.Instance.GetContactFotoTempFileName(dataObject);
            BusinessTelephone = dataObject.BusinessTelephone;
            HomeTelephone = dataObject.HomeTelephone;
            MobileTelephone = dataObject.MobileTelephone;
            RaiseNotification();
        }

        private void ApplyEmailContactInfo(OFEmailContactSearchObject dataObject)
        {
            Emails = new List<string>() { dataObject.EMail };
            string name = string.Empty;
            FirstName = !string.IsNullOrEmpty(dataObject.ContactName) ? dataObject.ContactName : IsAbleGetNameFromContact(dataObject.EMail.ToLower(), ref name) ? name : dataObject.EMail;
            RaiseNotification();
        }

        private bool IsAbleGetNameFromContact(string email, ref string name)
        {
            var contact = OFOutlookHelper.Instance.GetContact(email);
            if (contact == null)
            {
                return false;
            }
            name = string.Format("{0} {1}", contact.FirstName, contact.LastName);
            return true;
        }

        private void RunEmailSearching(string from, string to)
        {
            RunSearching(_contactEmailSearching, from, to, SearchEmailString);
        }

        private void RunAttachmentSearching(string from, string to)
        {
            RunSearching(_contactAttachmentSearching, from, to, SearchAttachmentString);
        }

        private void RunSearching(IContactSearchSystem searchSystem, string from, string to, string searchCriteria)
        {
            if (searchSystem == null)
                return;
            searchSystem.SetSearchCriteria(string.Format("{0};{1};{2}", from, to, searchCriteria));
            searchSystem.StartMainSearch();
        }

        private void RunAttachemntPreviewSearching(string from, string to)
        {
            RunPreviewSearching(_contactAttachmentSearching, from, to, SearchAttachmentString);
        }

        private void RunEmailPreviewSearching(string from, string to)
        {
            RunPreviewSearching(_contactEmailSearching, from, to, SearchEmailString);
        }

        private void RunPreviewSearching(IContactSearchSystem searchSystem, string from, string to,
            string searchCriteria)
        {
            if (searchSystem == null)
                return;
            searchSystem.SetSearchCriteria(string.Format("{0};{1};{2}", from, to, searchCriteria));
            searchSystem.StartPreviewSearch();
        }

        private void RaiseNotification()
        {
            OnPropertyChanged(() => LastName);
            OnPropertyChanged(() => FirstName);
            OnPropertyChanged(() => Emails);
            OnPropertyChanged(() => FotoFilepath);
            OnPropertyChanged(() => BusinessTelephone);
            OnPropertyChanged(() => BusinessPhoneVisible);
            OnPropertyChanged(() => HomeTelephone);
            OnPropertyChanged(() => HomeTelephoneVisible);
            OnPropertyChanged(() => MobileTelephone);
            OnPropertyChanged(() => MobileTelephoneVisible);
        }

        private void RaiseSelectedChanged()
        {
            if (_eventAggregator == null || SelectedElement == null)
                return;
            _eventAggregator.GetEvent<OFSelectedChangedPayloadEvent>().Publish(new OFSearchObjectPayload(SelectedElement));
        }

        private void ResetSelectedChanged()
        {
            if (_eventAggregator == null)
                return;
            _eventAggregator.GetEvent<OFSelectedChangedPayloadEvent>().Publish(new OFSearchObjectPayload(null));
        }

        private void CreateEmailExecute(object o)
        {
            var adr = o as string;
            if (string.IsNullOrEmpty(adr))
                return;

            Outlook.MailItem email = OFOutlookHelper.Instance.CreateNewEmail();
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
            CheckExistingData();
        }

        private void NotifyEmailPartChanged()
        {
            OnPropertyChanged(() => IsEmailVisible);
            OnPropertyChanged(() => IsEmailMoreVisible);
            OnPropertyChanged(() => EmailHeight);
            OnPropertyChanged(() => EmailHeader);
            OnPropertyChanged(() => MainEmailSource);
            CheckExistingData();
        }

        private void MoreCommandExecute(object arg)
        {
            if (arg == null)
                return;
            SelectedIndex = int.Parse(arg.ToString());
            OnPropertyChanged(() => SelectedIndex);
            ResetSelectedChanged();
        }

        private void CheckExistingData()
        {
            if (!_isEmailsInitialized || !_isFilesInitialized)
            {
                IsDataExist = true;
            }
            else
                IsDataExist = IsEmailVisible || IsFileVisible;
            OnPropertyChanged(() => IsDataExist);
        }

        private void InitContactCommands()
        {
            ContactUIItemCollection = new List<OFUIItem>()
            {
                new OFUIItem(){UIName =  "Everething", Data = "M33.597977,10.759002C37.946865,10.759002 41.485962,14.285001 41.485962,18.649 41.485962,23 37.946865,26.535 33.597977,26.535 29.23909,26.535 25.709992,23 25.709992,18.649 25.709992,17.784 25.849955,16.953001 26.109888,16.174002 26.779719,16.881001 27.70948,17.327002 28.759213,17.327002 30.778696,17.327002 32.418278,15.691001 32.418278,13.668001 32.418278,12.663001 32.008381,11.748001 31.348551,11.087002 32.058369,10.876001 32.818176,10.759002 33.597977,10.759002z M33.606682,4.3679962C25.92741,4.3679957 19.698065,10.594956 19.698065,18.27293 19.698065,25.953894 25.92741,32.177862 33.606682,32.177862 41.295838,32.177862 47.515175,25.953894 47.515175,18.27293 47.515175,10.594956 41.295838,4.3679957 33.606682,4.3679962z M34.867642,1.546141E-09C36.890393,2.6508449E-05 58.705193,0.41938579 68.893006,18.299923 68.893006,18.299923 57.1442,36.139837 34.44656,34.768854 34.44656,34.768854 14.428583,36.59984 0,18.299923 0,18.299923 9.0791523,0.4590019 34.716553,0.0010111886 34.716553,0.0010114873 34.768162,-1.4442128E-06 34.867642,1.546141E-09z"},
                new OFUIItem(){UIName =  "Conversations", Data = "M0,4.0800388L0.030031017,4.0800388 12.610706,16.409995 26.621516,30.149985 40.642334,16.409995 53.223011,4.0800388 53.333001,4.0800388 53.333001,39.080039 0,39.080039z M3.1698808,0L26.660885,0 50.161892,0 38.411389,11.791528 26.660885,23.573054 14.920383,11.791528z"},
                new OFUIItem(){UIName =  "File Exchanged", Data = "F1M-1800.12,3181.48C-1800.12,3181.48 -1803.22,3172.42 -1813.67,3175.58 -1813.67,3175.58 -1822.52,3177.87 -1820.93,3188.08L-1807.87,3226.17 -1804.99,3225.18 -1817.71,3188.05C-1817.71,3188.05 -1820.44,3181.75 -1812.98,3178.38 -1812.98,3178.38 -1805.77,3175.6 -1802.56,3183.22L-1786.65,3229.63C-1786.65,3229.63 -1786.14,3234.44 -1790.63,3235.46 -1790.63,3235.46 -1792.78,3236.54 -1796.5,3233.79L-1803.82,3212.42 -1809.85,3194.84C-1809.85,3194.84 -1809.94,3192.33 -1808.27,3191.61 -1807.43,3191.25 -1805.9,3191.14 -1805.05,3193.37L-1794.19,3225.06 -1791.09,3224 -1802.17,3191.67C-1802.17,3191.67 -1803.96,3187.16 -1809.04,3188.84 -1810.81,3189.42 -1813.38,3190.48 -1812.95,3195.18L-1799.38,3234.78C-1799.38,3234.78 -1796.27,3240.08 -1789.53,3238.67 -1789.53,3238.67 -1783.39,3237.06 -1783.69,3229.41L-1800.12,3181.48z"}
            };
        }

        private void CopyPhoneToClipboard(object phoneType)
        {
            var type = (PhoneType)phoneType;
            switch (type)
            {
                case PhoneType.Business:
                    Clipboard.SetText(BusinessTelephone);
                    break;

                case PhoneType.Home:
                    Clipboard.SetText(HomeTelephone);
                    break;

                case PhoneType.Mobile:
                    Clipboard.SetText(MobileTelephone);
                    break;
            }
        }

        protected void KeyEmailDown(object args)
        {
            if (args == null || !(args is KeyEventArgs))
                return;
            var keys = args as KeyEventArgs;
            switch (keys.Key)
            {
                case Key.Enter:
                    RunEmailSearching(_from, _to);
                    break;
            }
        }

        protected void KeyAttachmentDown(object args)
        {
            if (!(args is KeyEventArgs))
                return;
            var keys = args as KeyEventArgs;
            switch (keys.Key)
            {
                case Key.Enter:
                    RunAttachmentSearching(_from, _to);
                    break;
            }
        }

        #endregion [private]

        #region [override]

        protected override void Dispose(bool disposing)
        {
            if (!Disposed && disposing)
            {
                if (_contactAttachmentSearching != null)
                {
                    _contactAttachmentSearching.PreviewSearchingFinished -= ContactAttachmentSearchingOnPreviewSearchingFinished;
                    _contactAttachmentSearching.MainSearchingFinished -= ContactAttachmentSearchingOnMainSearchingFinished;
                }
                if (_contactEmailSearching != null)
                {
                    _contactEmailSearching.PreviewSearchingFinished -= ContactEmailSearchingOnPreviewSearchingFinished;
                    _contactEmailSearching.MainSearchingFinished -= ContactEmailSearchingOnMainSearchingFinished;
                    _contactEmailSearching = null;
                }
            }
            base.Dispose(disposing);
        }

        #endregion [override]

        public ICommand ScrollChangeCommand { get; private set; }

        private void OnScroll(object args)
        {
            InternalOnScroll(args as OFScrollData, _scrollBehavior);
        }

        public ICommand ScrollChangedCommand2
        {
            get;
            private set;
        }

        private void OnScroll2(object args)
        {
            InternalOnScroll(args as OFScrollData, _scrollBehavior2);
        }

        private void InternalOnScroll(OFScrollData args, IScrollBehavior behavior)
        {
            if (args == null || behavior == null)
                return;
            behavior.NeedSearch(args);
        }

        private void CalculateEmailListBoxHeight(object arg)
        {
            var data = arg as OFExpanderData;
            if (data == null)
            {
                return;
            }
            double avaibleHeight = ContactDetailsView.ActualHeight - ContactDetailsView.ActualFileHeight;
            double delta = data.NewSize.Height - data.OldSize.Height;
            double restDelta = (avaibleHeight - Delta) - EmailHeight;
            if ((data.IsScrollBarVisible || data.IsVisibleOne) &&  delta > 0) //
            {
                EmailHeight += restDelta > delta ? delta : restDelta;
                OnPropertyChanged(() => EmailHeight);
            }
        }
    }
}