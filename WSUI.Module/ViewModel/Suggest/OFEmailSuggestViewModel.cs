using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Unity;
using Newtonsoft.Json;
using OF.Core.Core.MVVM;
using OF.Core.Data;
using OF.Core.Enums;
using OF.Core.Extensions;
using OF.Core.Interfaces;
using OF.Core.Utils.Dialog;
using OF.Core.Win32;
using OF.Infrastructure.Events;
using OF.Infrastructure.Implements.Systems;
using OF.Infrastructure.Payloads;
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
        private OFContactSearchSystem _contactSearchSystem;
        private IOFEmailSuggestWindow _suggestWindow;
        private IntPtr _hWnd;



        public OFEmailSuggestViewModel(IEventAggregator eventAggregator, IUnityContainer container)
        {
            _eventAggregator = eventAggregator;
            _unityContainer = container;
            _contactSearchSystem = new OFContactSearchSystem();
            _contactSearchSystem.Init(_unityContainer);
            _contactSearchSystem.SearchFinished += ContactSearchSystemOnSearchFinished;
            Emails = new ObservableCollection<ISearchObject>();
            _suggestWindow = _unityContainer.Resolve<IOFEmailSuggestWindow>();
            _suggestWindow.Model = this;
        }

        public void Show(Tuple<IntPtr, string> Data)
        {
            if (_suggestWindow.IsNull())
            {
                return;
            }
            _contactSearchSystem.Reset();
            _contactSearchSystem.SetSearchCriteria(Data.Item2);
            _contactSearchSystem.Search();
            _hWnd = Data.Item1;
        }

        public void Hide()
        {
            if (_suggestWindow.IsNull())
            {
                return;
            }
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
                    var contact = SelectedItem as OFContactSearchObject;
                    if (contact.IsNotNull())
                    {
                        _eventAggregator.GetEvent<OFSuggestedEmailEvent>().Publish(new OFSuggestedEmailPayload(new Tuple<IntPtr, string>(_hWnd, contact.EmailAddress1)));
                        Hide();
                    }
                    break;
            }
        }

        public ISearchObject SelectedItem
        {
            get { return Get(() => SelectedItem); }
            set { Set(() => SelectedItem, value); }
        }

        public IList<ISearchObject> Emails
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

        private void ContactSearchSystemOnSearchFinished(object o)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                Emails = null;
                var collection = new ObservableCollection<ISearchObject>();

                var results = _contactSearchSystem.GetResult();

                results.OrderBy(r => r.Priority).ForEach(r =>
                {
                    foreach (var systemSearchResult in r.Result.OperationResult)
                    {
                        collection.Add(systemSearchResult as OFBaseSearchObject);
                    }
                });
                Emails = collection.Where(so  => so is OFContactSearchObject ? !string.IsNullOrEmpty((so as OFContactSearchObject).EmailAddress1) : so is OFEmailContactSearchObject ? !string.IsNullOrEmpty((so as OFEmailContactSearchObject).EMail) : false).ToList();
                if (!_suggestWindow.IsVisible)
                {
                    _suggestWindow.ShowSuggestings(_hWnd);
                }
            }));
        }

    }
}