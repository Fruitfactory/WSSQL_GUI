using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.ServiceProcess;
using System.Threading;
using System.Windows.Documents.DocumentStructures;
using System.Windows.Input;
using System.Windows.Threading;
using Elasticsearch.Net.Serialization;
using MahApps.Metro.Controls;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;
using Nest;
using WSUI.Core.Core.ElasticSearch;
using WSUI.Core.Core.MVVM;
using WSUI.Core.Enums;
using WSUI.Core.Extensions;
using WSUI.Core.Helpers;
using WSUI.Core.Interfaces;
using WSUI.Core.Logger;
using WSUI.Core.Utils.Dialog;
using WSUI.Infrastructure;
using WSUI.Infrastructure.MVVM.StatusItem;
using WSUI.Module.Interface.View;
using WSUI.Module.Interface.ViewModel;

namespace WSUI.Module.ViewModel
{
    public class ElasticSearchViewModel : ViewModelBase, IElasticSearchViewModel
    {

        private const string ElasticSearchService = "elasticsearch";

        private IEventAggregator _eventAggregator;
        private IUnityContainer _unityContainer;
        private IRegionManager _regionManager;
        private Timer _timer;

        [Dependency]
        public IElasticSearchInitializationIndex ElasticSearchClient { get; set; }

        public ElasticSearchViewModel(IElasticSearchView view, IRegionManager regionManager, IUnityContainer unityContainer, IEventAggregator eventAggregator)
        {
            View = view;
            view.Model = this;
            _eventAggregator = eventAggregator;
            _regionManager = regionManager;
            _unityContainer = unityContainer;
        }

        public void Initialize()
        {
            CheckServices();
            InstallServiceCommand = new WSUIRelayCommand(InstallServiceCommandExecute);
            RunServiceCommand = new WSUIRelayCommand(RunServiceCommandExecute);
            CreateIndexCommand = new WSUIRelayCommand(CreateIndexCommandExecute);
            LetsGoCommand = new WSUIRelayCommand(o => this.Close());
            CreateIndexVisibility = Visibility.Visible;
            ShowProgress = Visibility.Collapsed;
            FinishedStepVisibility = Visibility.Collapsed;
            
        }

        public object View
        {
            get; private set;
        }

        public bool IsServiceInstalled
        {
            get { return Get(() => IsServiceInstalled); }
            private set { Set(() => IsServiceInstalled,value); }
        }

        public bool IsServiceRunning
        {
            get { return Get(() => IsServiceRunning); }
            private set { Set(() => IsServiceRunning,value); }
        }

        public bool IsIndexExisted
        {
            get { return Get(() => IsIndexExisted); }
            private set { Set(() => IsIndexExisted,value);}
        }

        public void Show()
        {
            IRegion region = _regionManager.Regions[RegionNames.ElasticSearchRegion];
            if (region == null || region.Views.Contains(View))
            {
                return;
            }
            region.Add(View);
            region.Activate(View);
            OnPropertyChanged(() => IsServiceInstalled);
            OnPropertyChanged(() => IsServiceRunning);
            OnPropertyChanged(() => IsIndexExisted);
        }

        public void Close()
        {
            IRegion region = _regionManager.Regions[RegionNames.ElasticSearchRegion];
            if (region == null || !region.Views.Contains(View))
            {
                return;
            }
            region.Deactivate(View);
            region.Remove(View);
        }

        public event EventHandler IndexingStarted;
        public event EventHandler IndexingFinished;

        #region [properties]

        public Visibility CreateIndexVisibility
        {
            get { return Get(() => CreateIndexVisibility); }
            set { Set(() => CreateIndexVisibility, value); }
        }

        public Visibility ShowProgress
        {
            get { return Get(() => ShowProgress); }
            set { Set(() => ShowProgress, value); }
        }

        public Visibility FinishedStepVisibility
        {
            get { return Get(() => FinishedStepVisibility); }
            set { Set(() => FinishedStepVisibility, value); }
        }

        public string CurrentFolder
        {
            get { return Get(() => CurrentFolder); }
            set { Set(() => CurrentFolder, value); }
        }

        public double CurrentProgress
        {
            get { return Get(() => CurrentProgress); }
            set { Set(() => CurrentProgress, value); }
        }

        #endregion

        #region [commands]

        public ICommand InstallServiceCommand
        {
            get { return Get(() => InstallServiceCommand); }
            private set {Set(() => InstallServiceCommand,value);}
        }

        public ICommand RunServiceCommand
        {
            get { return Get(() => RunServiceCommand); }
            private set { Set(() => RunServiceCommand,value);}
        }

        public ICommand CreateIndexCommand
        {
            get { return Get(() => CreateIndexCommand); } 
            private set {Set(() => CreateIndexCommand,value);}
        }


        public ICommand LetsGoCommand
        {
            get { return Get(() => LetsGoCommand); }
            set { Set(() => LetsGoCommand, value); }
        }


        #endregion


        #region [private]

        private void OnIndexingStarted()
        {
            var temp = this.IndexingStarted;
            if (temp != null)
            {
                temp(this,EventArgs.Empty);
            }
        }

        private void OnIndexingFinished()
        {
            var temp = this.IndexingFinished;
            if (temp != null)
            {
                temp(this, EventArgs.Empty);
            }
        }

        private void CheckServices()
        {
            ServiceController sct = ServiceController.GetServices().FirstOrDefault(s => s.ServiceName.IndexOf(ElasticSearchService,StringComparison.InvariantCultureIgnoreCase) > -1);
            if (sct == null)
            {
                IsServiceInstalled = false;
            }
            else
            {
                IsServiceInstalled = true;
                IsServiceRunning = sct.Status == ServiceControllerStatus.Running;
            }
#if DEBUG
            IsServiceRunning = IsServiceInstalled = true;
#endif
            if (IsServiceRunning)
            {
                var resp = ElasticSearchClient.IndexExists(WSUIElasticSearchClient.DefaultIndexName);
                IsIndexExisted = resp.Exists;//false;//
            }
        }

        private void InstallServiceCommandExecute(object arg)
        {
            ExecuteCommandForService("install");
            CheckServices();                
        }

        private void RunServiceCommandExecute(object arg)
        {
            ExecuteCommandForService("start");
            Thread.Sleep(3000);
            CheckServices();
        }

        private void CreateIndexCommandExecute(object arg)
        {
            InitElasticSearch();
            CreateIndexVisibility = Visibility.Collapsed;
            ShowProgress = Visibility.Visible;

            Thread.Sleep(2000);
            //CheckServices();
        }

        private void ExecuteCommandForService(string command)
        {
            string path = RegistryHelper.Instance.GetElasticSearchpath();
            if (path.IsEmpty() || command.IsEmpty())
                return;
            const string serviceinstallcommand = "service.bat";
            try
            {
                Process p = new Process();
                p.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                p.StartInfo.FileName = string.Format("{0}\\bin\\{1}", path, serviceinstallcommand);
                p.StartInfo.Arguments = command;
                p.StartInfo.Verb = "runas";
                p.Start();
                p.WaitForExit();
            }
            catch (Exception ex)
            {
                WSSqlLogger.Instance.LogError(ex.Message);
            } 
        }

        private void InitElasticSearch()
        {
            try
            {
                
                var list = GetOutlookFiles();
                var index = new
                {
                    type = "pst",
                    pst = new
                    {
                        update_rate = "1h",
                        pst_list = list
                    }
                };
                var body = ElasticSearchClient.Serializer.Serialize(index, SerializationFormatting.Indented);
                ElasticSearchClient.CreateIndex(body);
                _timer = new Timer(TimerProgressCallback,null,1000,2000);
                OnIndexingStarted();
                
            }
            catch (Exception ex)
            {
                WSSqlLogger.Instance.LogError(ex.Message);
            }
        }

        private void TimerProgressCallback(object state)
        {
            try
            {
                var response = ElasticSearchClient.GetIndexingProgress();
                if (response.Response.IsNull() || response.Response.Items.IsNull() || !response.Response.Items.Any())
                {
                    return;
                }

                double sumAll = (double)response.Response.Items.Sum(s => s.Count);
                double sumProcessing = response.Response.Items.Sum(s => s.Processing);
                int max = response.Response.Items.Max(i => i.Count);
                var item = response.Response.Items.FirstOrDefault(s => s.Count == max);
                bool isFinished = response.Response.Items.All(s => s.Status == PstReaderStatus.Finished);

                CurrentFolder = item.IsNotNull() ? item.Folder : "";
                CurrentProgress = (sumProcessing / sumAll) * 100.0;
                
                if (isFinished)
                {
                    _timer.Change(Timeout.Infinite, Timeout.Infinite);
                    IsIndexExisted = true;
                    FinishedStepVisibility = Visibility.Visible;
                    OnIndexingFinished();
                }
            }
            catch (WebException w)
            {
                WSSqlLogger.Instance.LogError("[Web] " + w.Message);
            }
            catch (Exception e)
            {
                WSSqlLogger.Instance.LogError(e.Message);
            }
        }

        private IEnumerable<string> GetOutlookFiles()
        {
            string path = string.Format("{0}\\Microsoft\\Outlook",
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
            if (Directory.Exists(path))
            {
                var files = Directory.GetFiles(path, "*.pst");//new List<string>() { "F:\\Visual\\iyariki.ya@gmail.com.ost", "F:\\Visual\\iyariki.ya@gmail.com.ost" }; //
                var files1 = Directory.GetFiles(path, "*.ost");
                var list = new List<string>(files);
                list.AddRange(files1);

                foreach (var file in list)
                {
                    System.Diagnostics.Debug.WriteLine(file);
                }
                return list;
            }
            return null;
        }


        #endregion

    }
}