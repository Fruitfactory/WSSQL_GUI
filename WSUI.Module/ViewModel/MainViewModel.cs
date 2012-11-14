using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Text;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;
using WSUI.Infrastructure.Core;
using WSUI.Infrastructure.Service.Helpers;
using WSUI.Infrastructure.Services;
using WSUI.Module.Core;
using WSUI.Module.Interface;
using WSUI.Infrastructure;

namespace WSUI.Module.ViewModel
{
    public class MainViewModel : ViewModelBase, IMainViewModel
    {
        private DelegateCommand<object> _openFile;
        private List<IKindItem> _listItems;
        private ObservableCollection<IKindItem> _colItems;
        private readonly IUnityContainer _container;
        private readonly IRegionManager _regionManager;
        private IKindItem _currentItem = null;
        
        private const string Interface = "WSUI.Module.Interface.IKindItem";

        private BaseSearchData _currentData = null;


        public MainViewModel(IUnityContainer container, IRegionManager region, IKindsView kindView, IPreviewView previewView)
        {
            _container = container;
            _regionManager = region;
            KindsView = kindView;
            kindView.Model = this;
            PreviewView = previewView;
            previewView.Model = this;
        }


        public IKindsView KindsView
        {
            get; protected set;
        }

        public IPreviewView PreviewView
        {
            get; protected set;
        }

        public ICommand OpenCommand
        {
            get { return _openFile ?? (_openFile = new DelegateCommand<object>(o => OpenFile(), o => CanOpenFile())); }
        }

        public ObservableCollection<IKindItem> KindsCollection
        {
            get; protected set;
        }

        public virtual void Init()
        {
            _listItems = new List<IKindItem>();
            KindsCollection = new ObservableCollection<IKindItem>();
            GetAllKinds();
            if (_listItems.Count > 0)
            {
                _listItems.ForEach(k => KindsCollection.Add(k));
                OnChoose(_listItems[0]);
                OnPropertyChanged(() => KindsCollection);

            }
            
        }

        #region private

        private void GetAllKinds()
        {
            var currentAssembly = Assembly.GetExecutingAssembly();
            var list = new List<IKindItem>();
            foreach (var type in currentAssembly.GetTypes())
            {
                if (type.IsClass && !type.IsAbstract && type.GetInterface(Interface, true) != null)
                {
                    var kind = (IKindItem) _container.Resolve(type, null);
                    if (kind == null)
                        continue;
                    if (string.IsNullOrEmpty(kind.Name))
                        continue;
                    kind.Init();
                    kind.Choose += (o, e) => OnChoose(o);
                    _listItems.Add(kind);
                }
            }
            _listItems.Sort((x, y) =>
            {
                if (x.ID < y.ID)
                    return -1;
                else if (x.ID > y.ID)
                    return 1;
                return 0;
            });
        }

        private void OpenFile()
        {
            //var filename = SearchItemHelper.GetFileName(_currentData);
            //if (PreviewView != null)
            //{
            //    PreviewView.SetPreviewFile(filename);
            //}
        }

        private bool CanOpenFile()
        {
            return _currentData != null;
        }

        private void CurrentKindChanged(object kindItem)
        {
            IRegion regionSearch = _regionManager.Regions[RegionNames.SearchRegion];
            if (regionSearch != null)
            {
                var view = GetView(kindItem, "SettingsView");
                if (view != null && !regionSearch.Views.Contains(view))
                {
                    regionSearch.Add(view);
                    regionSearch.Activate(view);
                }
                else if(view != null)
                    regionSearch.Activate(view);
            }
            IRegion regionData = _regionManager.Regions[RegionNames.DataRegion];
            if (regionData != null)
            {
                var view = GetView(kindItem, "DataView");
                if (view != null && !regionData.Views.Contains(view))
                {
                    regionData.Add(view);
                    regionData.Activate(view);
                }
                else if(view != null)
                    regionData.Activate(view);
            }

            if (PreviewView != null)
                PreviewView.ClearPreview();

        }

        private object GetView(object viewModel, string propertyName)
        {
            var prop = viewModel.GetType().GetProperty(propertyName,
                                                       BindingFlags.Public | BindingFlags.NonPublic |
                                                       BindingFlags.Instance);
            if (prop != null)
            {
                return prop.GetValue(viewModel, null);
            }
            return null;
        }

        private void Disconnect()
        {
            if(_currentItem == null)
                return;
            
            _currentItem.Start -= OnStart;
            _currentItem.Complete -= OnComplete;
            _currentItem.Error -= OnError;
            _currentItem.CurrentItemChanged -= OnCurrentItemChanged;
        }

        private void Connect()
        {
            if(_currentItem == null)
                return;
            
            _currentItem.Start += OnStart;
            _currentItem.Complete += OnComplete;
            _currentItem.Error += OnError;
            _currentItem.CurrentItemChanged += OnCurrentItemChanged;
        }

        private void OnCurrentItemChanged(object sender, EventArgs<BaseSearchData> args)
        {
            _currentData = args.Value;
            var filename = SearchItemHelper.GetFileName(_currentData);
            if (PreviewView != null)
            {
                PreviewView.SetPreviewFile(filename);
                PreviewView.SetSearchPattern(_currentItem != null ? _currentItem.SearchString : string.Empty);
            }
        }

        private void OnStart(object sender, EventArgs e)
        {
            EventHandler temp = Start;
            if(temp != null)
                temp(null, null);
        }

        private void OnComplete(object sender, EventArgs<bool> e)
        {
            EventHandler temp = Complete;
            if (temp != null)
                temp(null, null);
        }

        private void OnError(object sender, EventArgs<bool> e)
        {

        }

        private void OnChoose(object sender)
        {
            var sendItem = sender as IKindItem;
            if(sendItem == null)
                return;
            var newItem = _listItems.Find(item => item.Name == sendItem.Name);
            if(newItem == null)
                return;
            newItem.Toggle = true;
            _listItems.ForEach(i => { if (i.Name != newItem.Name) i.Toggle = false; });
            Disconnect();
            _currentItem = newItem;
            Connect();
            CurrentKindChanged(_currentItem);
        }

        #endregion

        #region Implementation of IMainViewModel

        public event EventHandler Start;
        public event EventHandler Complete;

        #endregion
    }
}
