using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Unity;
using OF.Core.Core.MVVM;
using OF.Core.Data;
using OF.Core.Extensions;
using OF.Core.Interfaces;
using OF.Core.Win32;
using OF.Infrastructure.Implements.Systems;
using OF.Module.Interface.View;
using OF.Module.Interface.ViewModel;

namespace OF.Module.ViewModel.Suggest
{
    public class OFEmailSuggestViewModel : OFViewModelBase,IOFEmailSuggestViewModel
    {
        private IEventAggregator _eventAggregator;
        private IUnityContainer _unityContainer;
        private OFContactSearchSystem _contactSearchSystem;
        private IOFEmailSuggestWindow _suggestWindow;
        



        public OFEmailSuggestViewModel(IEventAggregator eventAggregator,IUnityContainer container)
        {
            _eventAggregator = eventAggregator;
            _unityContainer = container;
            _contactSearchSystem = new OFContactSearchSystem();
            _contactSearchSystem.Init(_unityContainer);
            _contactSearchSystem.SearchFinished += ContactSearchSystemOnSearchFinished;
            _suggestWindow = _unityContainer.Resolve<IOFEmailSuggestWindow>();
            _suggestWindow.Model = this;
            Emails = new ObservableCollection<ISearchObject>();
        }

        public void Show(Tuple<IntPtr,string> Data)
        {
            if (_suggestWindow.IsNull())
            {
                return;
            }
            _contactSearchSystem.Reset();
            _contactSearchSystem.SetSearchCriteria(Data.Item2);
            _contactSearchSystem.Search();
            if (!_suggestWindow.IsVisible)
            {
                _suggestWindow.ShowSuggestings(Data.Item1);
            }
        }
        
        public void Hide()
        {
            if (_suggestWindow.IsNull())
            {
                return;
            }
            _suggestWindow.HideSuggestings();
        }

        public ISearchObject SelectedItem
        {
            get { return Get(() => SelectedItem); }
            set { Set(() => SelectedItem, value); }
        }

        public IEnumerable<ISearchObject> Emails
        {
            get
            {
                return Get(() => Emails);
            }
            set
            {
                Set(() =>Emails,  value);
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
                    System.Diagnostics.Debug.WriteLine(string.Format("!!!!! Results: {0}", r.Result.OperationResult.Count));
                    foreach (var systemSearchResult in r.Result.OperationResult)
                    {
                        collection.Add(systemSearchResult as OFBaseSearchObject);
                    }
                });
                Emails = collection;
            }));
        }
    }
}