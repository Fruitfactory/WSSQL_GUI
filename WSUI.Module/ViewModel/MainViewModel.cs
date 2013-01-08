using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using C4F.DevKit.PreviewHandler.Service.Logger;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;
using WSUI.Infrastructure.Controls.ProgressManager;
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
        private readonly IUnityContainer _container;
        private readonly IRegionManager _regionManager;
        private IKindItem _currentItem = null;
        private bool _enabled = true;
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
            MainDataSource = new List<BaseSearchData>();
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

        public ObservableCollection<IWSCommand> Commands
        {
            get { return _currentItem != null ? _currentItem.Commands : null; }
        } 

        public virtual void Init()
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                                                                      {
                                                                          InitializeInThread();
                                                                          if (_listItems.Count > 0)
                                                                          {
                                                                              _listItems.ForEach(k => KindsCollection.Add(k));
                                                                              OnChoose(_listItems[0]);
                                                                          }
                                                                          OnPropertyChanged(() => KindsCollection);
                                                                          
                                                                      }), null);
        }

        public bool Enabled
        {
            get { return _enabled; }
            set 
            {
                _enabled = value;
                OnPropertyChanged(() => Enabled);
            }
        }

        #region private

        private void InitializeInThread()
        {
            _listItems = new List<IKindItem>();
            KindsCollection = new ObservableCollection<IKindItem>();
            GetAllKinds();
        }

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
                    kind.Parent = this;
                    kind.Choose += (o, e) => OnChoose(o);
                    if (kind is INotifyPropertyChanged)
                        (kind as INotifyPropertyChanged).PropertyChanged += (o, e) => OnPropertyChanged(e.PropertyName);
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
            OnPropertyChanged(() => Commands);

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

            try
            {
                ProgressManager.Instance.StartOperation(new ProgressOperation()
                                                            {
                                                                Caption = "Loading...",
                                                                DelayTime = 250,
                                                                Canceled = false,
                                                                Location =  Application.Current.MainWindow.PointToScreen(new Point(0, 0)),
                                                                Size = new Size(Application.Current.MainWindow.ActualWidth, Application.Current.MainWindow.ActualHeight)
                                                            });
                var filename = SearchItemHelper.GetFileName(_currentData);
                if (PreviewView != null)
                {
                    if (!string.IsNullOrEmpty(filename))
                    {
                        PreviewView.SetSearchPattern(_currentItem != null
                                ? _currentItem.SearchString
                                : string.Empty);
                        PreviewView.SetPreviewFile(filename);
                    }
                    else
                        PreviewView.ClearPreview();
                }
            }
            catch (Exception ex)
            {
                WSSqlLogger.Instance.LogError(ex.Message);
            }
            finally
            {
                ProgressManager.Instance.StopOperation();
            }
        }

        private void OnStart(object sender, EventArgs e)
        {
            EventHandler temp = Start;
            if(temp != null)
                temp(null, null);
            Enabled = false;
        }

        private void OnComplete(object sender, EventArgs<bool> e)
        {
            EventHandler temp = Complete;
            if (temp != null)
                temp(null, null);
            Enabled = true;
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
            string searchString = string.Empty;
            if(_currentItem  != null)
                searchString = _currentItem.SearchString;
            _currentItem = newItem;
            Connect();
            CurrentKindChanged(_currentItem);
            if (!string.IsNullOrEmpty(searchString) && searchString != _currentItem.SearchString)
            {
                _currentItem.SearchString = searchString;
                _currentItem.FilterData();
            }
        }

        #endregion

        #region Implementation of IMainViewModel

        public event EventHandler Start;
        public event EventHandler Complete;
        public List<BaseSearchData> MainDataSource { get; protected set; }

        public void Clear()
        {
            TempFileManager.Instance.ClearTempFolder();
        }


        #endregion

    }
}
