using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Instrumentation;
using Microsoft.Practices.Prism;
using Outlook = Microsoft.Office.Interop.Outlook;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using System.Collections.Generic;
using System.Windows.Input;
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

namespace WSUI.Module.ViewModel
{
    public class ContactDetailsViewModel : ViewModelBase, IContactDetailsViewModel
    {
        private IEventAggregator _eventAggregator;
        private ISearchSystem _searchSystem;
        private ISearchObject _selectedAttachment;
        

        public ContactDetailsViewModel(IRegionManager regionManager, IEventAggregator eventAggregator, IContactDetailsView contactDetailsView)
        {
            _eventAggregator = eventAggregator;
            View = contactDetailsView;
            contactDetailsView.Model = this;
            CreateEmailCommand = new WSUIRelayCommand(CreateEmailExecute);
            InitializeSearchSystem();
        }

        #region [interface]

        public object View { get; private set; }

        public ObservableCollection<AttachmentSearchObject> ItemsSource { get; private set; }

        public void SetDataObject(ISearchObject dataSearchObject)
        {
            var from = OutlookHelper.Instance.GetCurrentyUserEmail().ToLowerInvariant();
            var to = string.Empty;
            if (dataSearchObject is EmailContactSearchObject)
            {
                var temp = dataSearchObject as EmailContactSearchObject;
                ApplyEmailContactInfo(temp);
                to = temp.EMail.ToLowerInvariant();
            }
            else if (dataSearchObject is ContactSearchObject)
            {
                var temp = dataSearchObject as ContactSearchObject;
                ApplyContactInfo(temp);
                to = temp.GetEmail();
            }
            RunAttachmentSearching(from, to);
        }

        public ISearchObject SelectedAttachement 
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

        #endregion [commands]

        #region [property]

        public IEnumerable<string> Emails { get; private set; }

        public string FirstName { get; private set; }

        public string LastName { get; private set; }

        public string FotoFilepath { get; private set; }

        #endregion [property]

        #region [private]

        private void InitializeSearchSystem()
        {
            _searchSystem = new ContactAttachmentSearchSystem();
            _searchSystem.Init();
            _searchSystem.SearchStarted += SearchStarted;
            _searchSystem.SearchStoped += SearchStoped;
            _searchSystem.SearchFinished += SearchFinished;
        }

        private void SearchStoped(object o)
        {
        }

        private void SearchStarted(object o)
        {
        }

        private void SearchFinished(object o)
        {
            System.Windows.Application.Current.Dispatcher.BeginInvoke((Action)ProcessResult);
        }

        private void ProcessResult()
        {
            IList<ISystemSearchResult> result = _searchSystem.GetResult();
            if (result == null || !result.Any())
                return;
            var listResult = new ObservableCollection<AttachmentSearchObject>();
            foreach (var systemSearchResult in result)
            {
                CollectionExtensions.AddRange(listResult, systemSearchResult.Result.OfType<AttachmentSearchObject>());
            }
            ItemsSource = listResult;
            OnPropertyChanged(() => ItemsSource);
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
            RaiseNotification();
        }

        private void RunAttachmentSearching(string from, string to)
        {
            if (_searchSystem == null)
                return;
            _searchSystem.SetSearchCriteria(string.Format("{0};{1}", from, to));
            _searchSystem.Search();
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
            if(_eventAggregator == null)
                return;
            _eventAggregator.GetEvent<SelectedChangedPayloadEvent>().Publish(new SearchObjectPayload(SelectedAttachement));
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

        #endregion [private]

        #region [override]

        protected override void Dispose(bool disposing)
        {
            if (!Disposed && disposing)
            {
                if (_searchSystem != null)
                {
                    _searchSystem.SearchFinished -= SearchFinished;
                    _searchSystem.SearchStarted -= SearchStarted;
                    _searchSystem.SearchFinished -= SearchFinished;
                    _searchSystem = null;
                }
            }
            base.Dispose(disposing);
        }

        #endregion [override]
    }
}