using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using WSUI.Core.Data;
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
    public class ContactDetailsViewModel : ViewModelBase, IContactDetailsViewModel, IScrollableView
    {

        private const double AvaregeTwoRowItemHeight = 45;
        private const double AvaregeOneRowItemHeight = 25;
        private const double FileValue = 0.2;
        private const double EmailValue = 0.7;


        private IEventAggregator _eventAggregator;
        private ISearchSystem _attachmentSearchSystem;
        private ISearchSystem _emailSearchSystem;
        private ISearchObject _selectedAttachment;
        private bool _isEmailsInitialized = false;
        private IScrollBehavior _scrollBehavior;

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
            _scrollBehavior = new ScrollBehavior {LimitReaction = 85};
            _scrollBehavior.SearchGo += ScrollBehaviorOnSearchGo;
            EmailsSource = new ObservableCollection<EmailSearchObject>();
            IsDataExist = true;
        }

        private void ScrollBehaviorOnSearchGo()
        {
            RunEmailSearching(_from,_to);                    
        }

        #region [interface]

        public object View { get; private set; }

        public ObservableCollection<AttachmentSearchObject> ItemsSource { get; private set; }

        public ObservableCollection<EmailSearchObject> EmailsSource { get; private set; }

        public ObservableCollection<AttachmentSearchObject> MainFileSource { get; private set; }

        public ObservableCollection<EmailSearchObject> MainEmailSource { get; private set; }

        public void SetDataObject(ISearchObject dataSearchObject)
        {
            _from = string.Empty;// OutlookHelper.Instance.GetCurrentyUserEmail().ToLowerInvariant();
            _to = string.Empty;
            if (dataSearchObject is EmailContactSearchObject)
            {
                var temp = dataSearchObject as EmailContactSearchObject;
                ApplyEmailContactInfo(temp);
                _from = temp.EMail.ToLowerInvariant();
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
            var listResult = new ObservableCollection<AttachmentSearchObject>();
            foreach (var systemSearchResult in result)
            {
                CollectionExtensions.AddRange(listResult, systemSearchResult.Result.OfType<AttachmentSearchObject>());
            }
            if (listResult.Any())
            {
                var avaibleHeight = GetAvaibleHeightAndCount(FileValue, AvaregeOneRowItemHeight);
                IsFileMoreVisible = listResult.Count > avaibleHeight.Item2 - 1;
                CollectionExtensions.AddRange(MainFileSource, listResult.Take(avaibleHeight.Item2 - 1));
                FileHeight = IsFileMoreVisible ? avaibleHeight.Item1 : listResult.Count * AvaregeOneRowItemHeight;
            }
            ItemsSource = listResult;
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
            if (EmailsSource.Any() && !_isEmailsInitialized)
            {
                var avaibleHeight = GetAvaibleHeightAndCount(EmailValue, AvaregeTwoRowItemHeight);
                IsEmailMoreVisible = EmailsSource.Count > avaibleHeight.Item2 - 1;
                CollectionExtensions.AddRange(MainEmailSource, EmailsSource.Take(avaibleHeight.Item2 - 1));
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
            FirstName = IsAbleGetNameFromContact(dataObject.EMail, ref name) ? name : dataObject.EMail;
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
            var avaibleHeight = ContactDetailsView.ActualHeight * a;
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
            var scrollArgs = args as ScrollData;
            if (scrollArgs != null && _scrollBehavior != null)
            {
                _scrollBehavior.NeedSearch(scrollArgs);
            }
        }

    }
}