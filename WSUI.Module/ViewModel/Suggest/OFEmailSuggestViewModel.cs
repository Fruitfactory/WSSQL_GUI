using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Unity;
using Newtonsoft.Json;
using OF.Core.Core.MVVM;
using OF.Core.Data;
using OF.Core.Data.ElasticSearch;
using OF.Core.Enums;
using OF.Core.Extensions;
using OF.Core.Helpers;
using OF.Core.Interfaces;
using OF.Core.Utils.Dialog;
using OF.Core.Win32;
using OF.Infrastructure.Events;
using OF.Infrastructure.Implements.Systems;
using OF.Infrastructure.Payloads;
using OF.Module.Data;
using OF.Module.Events;
using OF.Module.Interface.View;
using OF.Module.Interface.ViewModel;
using Application = System.Windows.Application;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace OF.Module.ViewModel.Suggest
{
    public class OFEmailSuggestViewModel : OFViewModelBase, IOFEmailSuggestViewModel
    {
        private IEventAggregator _eventAggregator;
        private IUnityContainer _unityContainer;
        private IOFEmailSuggestWindow _suggestWindow;
        private IntPtr _hWnd;
        private IOFElasticsearchShortContactClient _contactClient;
        private List<OFShortContact> _contacts;

        private SubscriptionToken _tokenSuggestData;




        public OFEmailSuggestViewModel(IEventAggregator eventAggregator, IUnityContainer container, IOFElasticsearchShortContactClient contactClient)
        {
            _eventAggregator = eventAggregator;
            _unityContainer = container;
            _contactClient = contactClient;
            Emails = null;
            _suggestWindow = _unityContainer.Resolve<IOFEmailSuggestWindow>();
            _suggestWindow.Model = this;
            LoadContacts();
            _tokenSuggestData = _eventAggregator.GetEvent<OFSuggestWindowVisible>().Subscribe(OnSuggestWindowVisible);
        }

        private void OnSuggestWindowVisible(OFSuggestWindowData ofSuggestWindowData)
        {
            ofSuggestWindowData.IsVisible = _suggestWindow.IsVisible;
        }


        public void Show(Tuple<IntPtr, string> Data)
        {
            if (_suggestWindow.IsNull() || _contacts.IsNull() || !_contacts.Any())
            {
                return;
            }
            var criteria = Data.Item2.ToLowerInvariant();
            Emails = new ObservableCollection<OFShortContact>(_contacts.Where(c => c.Email.Contains(Data.Item2) || c.Name.ToLowerInvariant().Contains(criteria)));
            

            _hWnd = Data.Item1;
            
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (!_suggestWindow.IsVisible)
                {
                    _suggestWindow.ShowSuggestings(_hWnd);
                }
            }));
        }

        public void Hide()
        {
            if (_suggestWindow.IsNull())
            {
                return;
            }
            SelectedItem = null;
            _suggestWindow.HideSuggestings();
        }

        public void ProcessSelection(OFActionType type)
        {
            int index = -1;
            switch (type)
            {
                case OFActionType.UpSuggestEmail:
                    index = Emails.IndexOf(SelectedItem);
                    index--;
                    if (index >= 0)
                    {
                        SelectedItem = Emails[index];
                    }
                    break;
                case OFActionType.DownSuggestEmail:
                    index = Emails.IndexOf(SelectedItem);
                    index++;
                    if (index < Emails.Count)
                    {
                        SelectedItem = Emails[index];
                    }
                    break;
                case OFActionType.SelectSuggestEmail:
                    if (SelectedItem.IsNotNull())
                    {
                        _eventAggregator.GetEvent<OFSuggestedEmailEvent>().Publish(new OFSuggestedEmailPayload(new Tuple<IntPtr, string>(_hWnd, SelectedItem.Email)));
                        Hide();
                    }
                    break;
            }
        }

        public void UpdateSuggectingList()
        {
            LoadContacts();
        }

        public OFShortContact SelectedItem
        {
            get { return Get(() => SelectedItem); }
            set { Set(() => SelectedItem, value); }
        }

        public ObservableCollection<OFShortContact> Emails
        {
            get
            {
                return Get(() => Emails);
            }
            set
            {
                Set(() => Emails, value);
            }
        }

        private void LoadContacts()
        {
            if (!OFRegistryHelper.Instance.CheckAutoCompleateState())
            {
                return;
            }
            Task.Factory.StartNew(() =>
            {
                _contacts = new List<OFShortContact>(_contactClient.GetAllSuggestionContacts());
            });
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (_tokenSuggestData.IsNotNull())
            {
                _eventAggregator.GetEvent<OFSuggestWindowVisible>().Unsubscribe(_tokenSuggestData);
            }
        }
    }
}