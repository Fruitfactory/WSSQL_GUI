using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using WSUI.Core.Data;
using WSUI.Core.Helpers;
using WSUI.Core.Interfaces;
using WSUI.Core.Utils.Dialog;
using WSUI.Infrastructure.Events;
using WSUI.Infrastructure.Implements.Systems;
using WSUI.Infrastructure.Payloads;
using WSUI.Module.Core;
using WSUI.Module.Interface.View;
using WSUI.Module.Interface.ViewModel;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace WSUI.Module.ViewModel
{
    public class ContactDetailsViewModel : ViewModelBase, IContactDetailsViewModel
    {

        private const double AvaregeTwoRowItemHeight = 40;
        private const double AvaregeOneRowItemHeight = 25;
        private const double FileValue = 0.2;
        private const double EmailValue = 0.6;


        private IEventAggregator _eventAggregator;
        private ISearchSystem _attachmentSearchSystem;
        private ISearchSystem _emailSearchSystem;
        private ISearchObject _selectedAttachment;

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
        }

        #region [interface]

        public object View { get; private set; }

        public ObservableCollection<AttachmentSearchObject> ItemsSource { get; private set; }

        public ObservableCollection<EmailSearchObject> EmailsSource { get; private set; }

        public ObservableCollection<AttachmentSearchObject> MainFileSource { get; private set; }

        public ObservableCollection<EmailSearchObject> MainEmailSource { get; private set; }

        public void SetDataObject(ISearchObject dataSearchObject)
        {
            var from = string.Empty;// OutlookHelper.Instance.GetCurrentyUserEmail().ToLowerInvariant();
            var to = string.Empty;
            if (dataSearchObject is EmailContactSearchObject)
            {
                var temp = dataSearchObject as EmailContactSearchObject;
                ApplyEmailContactInfo(temp);
                from = temp.EMail.ToLowerInvariant();
                to = temp.EMail.ToLowerInvariant();
            }
            else if (dataSearchObject is ContactSearchObject)
            {
                var temp = dataSearchObject as ContactSearchObject;
                ApplyContactInfo(temp);
                from = string.Format("{0} {1}", temp.FirstName, temp.LastName);
                to = temp.GetEmail();
            }
            SearchString = to;
            RunAttachmentSearching(from, to);
            RunEmailSearching(from, to);
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
            get { return _isAttachmentBusy || _isEmailBusy; }
        }
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

        private void InitializeSearchSystem()
        {
            _attachmentSearchSystem = new ContactAttachmentSearchSystem();
            _attachmentSearchSystem.Init();
            _attachmentSearchSystem.SearchStarted += AttachmentSearchStarted;
            _attachmentSearchSystem.SearchStoped += AttachmentSearchStoped;
            _attachmentSearchSystem.SearchFinished += AttachmentSearchFinished;

            _emailSearchSystem = new ContactEmailSearchSystem();
            _emailSearchSystem.Init();
            _emailSearchSystem.SearchFinished += EmailSearchFinished;
        }

        private void EmailSearchFinished(object o)
        {
            System.Windows.Application.Current.Dispatcher.BeginInvoke((Action)ProcessEmailResult);
        }

        private void AttachmentSearchStoped(object o)
        {
        }

        private void AttachmentSearchStarted(object o)
        {
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
                FileHeader = IsFileMoreVisible ? string.Format("({0})", listResult.Count) : string.Empty;
                CollectionExtensions.AddRange(MainFileSource, listResult.Take(avaibleHeight.Item2 - 1));
                FileHeight = avaibleHeight.Item1;
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
            var listResult = new ObservableCollection<EmailSearchObject>();
            foreach (var systemSearchResult in result)
            {
                CollectionExtensions.AddRange(listResult, systemSearchResult.Result.OfType<EmailSearchObject>());
            }
            if (listResult.Any())
            {
                var avaibleHeight = GetAvaibleHeightAndCount(EmailValue, AvaregeTwoRowItemHeight);
                IsEmailMoreVisible = listResult.Count > avaibleHeight.Item2 - 2;
                EmailHeader = IsEmailMoreVisible ? string.Format("({0})", listResult.Count) : string.Empty;
                CollectionExtensions.AddRange(MainEmailSource, listResult.Take(avaibleHeight.Item2 - 2));
                EmailHeight = avaibleHeight.Item1;
            }

            EmailsSource = listResult;
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
            FirstName = dataObject.EMail;
            RaiseNotification();
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
            var avaibleHeight = ActualHeight * a;
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

        #endregion [private]

        #region [override]

        protected override void Dispose(bool disposing)
        {
            if (!Disposed && disposing)
            {
                if (_attachmentSearchSystem != null)
                {
                    _attachmentSearchSystem.SearchFinished -= AttachmentSearchFinished;
                    _attachmentSearchSystem.SearchStarted -= AttachmentSearchStarted;
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
    }
}