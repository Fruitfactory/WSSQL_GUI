using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using C4F.DevKit.PreviewHandler.Service;
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
using Application = System.Windows.Application;

namespace WSUI.Module.ViewModel
{
    public class MainViewModel : ViewModelBase, IMainViewModel
    {
        private List<LazyKind> _listItems;
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
            Host = ReferenceEquals(Application.Current.MainWindow, null) ? HostType.Plugin : HostType.Application;
        }


        public IKindsView KindsView
        {
            get; protected set;
        }

        public IPreviewView PreviewView
        {
            get; protected set;
        }

        public ObservableCollection<LazyKind> KindsCollection
        {
            get;
            protected set;
        }

        public ObservableCollection<IWSCommand> Commands
        {
            get { return _currentItem != null ? _currentItem.Commands : null; }
        } 

        public virtual void Init()
        {
            //switch(Host)
            //{
            //    case HostType.Plugin:
            //        Application.Current.Dispatcher.BeginInvoke(new Action(InitializeInThread), null);
            //        break;
            //    default:
            //        Application.Current.Dispatcher.BeginInvoke(new Action(InitializeInThread), null);
            //        break;
            //}
            Application.Current.Dispatcher.BeginInvoke(new Action(InitializeInThread), null);
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
            _listItems = new List<LazyKind>();
            KindsCollection = new ObservableCollection<LazyKind>();
            GetAllKinds();
            if (_listItems.Count > 0)
            {
                _listItems.ForEach(k => KindsCollection.Add(k));
                OnChoose(_listItems[0].Kind);
            }
            OnPropertyChanged(() => KindsCollection);
        }

        private void GetAllKinds()
        {
            //var currentAssembly = Assembly.GetExecutingAssembly();
            //foreach (var type in currentAssembly.GetTypes())
            //{
            //    if (type.IsClass && !type.IsAbstract && type.GetInterface(Interface, true) != null)
            //    {

            //        var kind = new LazyKind(_container,type,this,OnChoose,OnPropertyChanged);
            //        kind.Initialize();
            //        _listItems.Add(kind);
            //    }
            //}

            var types =
               AppDomain.CurrentDomain.GetAssemblies().SelectMany(
                   a => a.GetTypes().Where(t => !t.IsAbstract && t.IsClass && t.GetInterface(Interface, true) != null));

            foreach (var type in types)
            {
                var kind = new LazyKind(_container, type, this, OnChoose, OnPropertyChanged);
                kind.Initialize();
                _listItems.Add(kind);
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
                Tuple<Point, Size> mwi = GetMainWindowInfo();
                ProgressManager.Instance.StartOperation(new ProgressOperation()
                                                            {
                                                                Caption = "Loading...",
                                                                DelayTime = 250,
                                                                Canceled = false,
                                                                Location =  mwi.Item1,
                                                                Size = mwi.Item2
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
                WSSqlLogger.Instance.LogError(string.Format("{0} - {2}", "OnCurrentImageChanged", ex.Message));
            }
            finally
            {
                ProgressManager.Instance.StopOperation();
            }
        }

        private void OnStart(object sender, EventArgs e)
        {
            var temp = Start;
            if(temp != null)
                temp(this,null);
            Enabled = _currentItem.Enabled;
        }

        private void OnComplete(object sender, EventArgs<bool> e)
        {
            var temp = Complete;
            if (temp != null)
                temp(this,null);
            Enabled = _currentItem.Enabled;
        }

        private void OnError(object sender, EventArgs<bool> e)
        {

        }

        private void OnChoose(object sender)
        {
            var sendItem = sender as IKindItem;
            if(sendItem == null)
                return;
            var newItem = _listItems.Find(item => item.UIName == sendItem.UIName);
            if(newItem == null)
                return;
            newItem.Toggle = true;
            _listItems.ForEach(i => { if (i.UIName != newItem.UIName) i.Toggle = false; });
            Disconnect();
            string searchString = string.Empty;
            if(_currentItem  != null)
                searchString = _currentItem.SearchString;
            _currentItem = newItem.Kind;
            Connect();
            CurrentKindChanged(_currentItem);
            if (!string.IsNullOrEmpty(searchString) && searchString != _currentItem.SearchString)
            {
                _currentItem.SearchString = searchString;
                _currentItem.FilterData();
            }
        }

        private Tuple<Point,Size> GetMainWindowInfo()
        {
            Point pt = new Point();
            Size sz = new Size();
            switch (Host)
            {
                case HostType.Application:
                    pt = Application.Current.MainWindow.PointToScreen(new Point(0, 0));
                    sz = new Size(Application.Current.MainWindow.ActualWidth, Application.Current.MainWindow.ActualHeight);
                    
                    break;
                case HostType.Plugin:
                    FormCollection formsCollection = System.Windows.Forms.Application.OpenForms;
                    if (formsCollection.Count > 0)
                    {
                        pt = new Point(formsCollection[0].DesktopLocation.X,formsCollection[0].DesktopLocation.Y);
                        sz = new Size(formsCollection[0].DesktopBounds.Width, formsCollection[0].DesktopBounds.Height);                    
                    }
                    break;
            }
            
            return new Tuple<Point, Size>(pt,sz);
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

        public void SelectKind(string name)
        {
            if(string.IsNullOrEmpty(name))
                return;

            var lazyKind = _listItems.Find(lk => lk.UIName == name);
            if (lazyKind != null)
            {
                OnChoose(lazyKind.Kind);
            }
        }

        public void PassActionForPreview(WSActionType actionType)
        {
            PreviewView.PassActionForPreview(actionType);
        }

        #endregion

    }
}
