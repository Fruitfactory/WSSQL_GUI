using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using OF.Core.Core.ElasticSearch;
using OF.Core.Core.MVVM;
using OF.Core.Data.ElasticSearch;
using OF.Core.Data.ElasticSearch.Response;
using OF.Core.Enums;
using OF.Core.Events;
using OF.Core.Extensions;
using OF.Core.Helpers;
using OF.Core.Interfaces;
using OF.Core.Logger;
using OF.Core.Utils.Dialog;
using OF.Infrastructure;
using OF.Infrastructure.MVVM.StatusItem;
using OF.Module.Interface.Service;
using OF.Module.Interface.View;
using OF.Module.Interface.ViewModel;

namespace OF.Module.ViewModel
{
    public class ElasticSearchViewModel : OFViewModelBase, IElasticSearchViewModel
    {

        private const string ElasticSearchService = "elasticsearch";

        private IEventAggregator _eventAggregator;
        private IUnityContainer _unityContainer;
        private IRegionManager _regionManager;
        private Timer _timer;

        private readonly object _lock = new object();


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
            CheckServicesAndIndex();
            InstallServiceCommand = new OFRelayCommand(InstallServiceCommandExecute);
            RunServiceCommand = new OFRelayCommand(RunServiceCommandExecute);
            CreateIndexCommand = new OFRelayCommand(CreateIndexCommandExecute);
            LetsGoCommand = new OFRelayCommand(o => this.Close());
            ForceCommand = new OFRelayCommand(ForceCommandExecute);
            CreateIndexVisibility = Visibility.Visible;
            ShowProgress = Visibility.Collapsed;
            FinishedStepVisibility = Visibility.Collapsed;
            WarmingVisibility = Visibility.Collapsed;
            WarmSecond = 10;
            if (IsIndexExisted)
            {
                ElasticSearchClient.WarmUp();
            }
            
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

        public bool IsInitialIndexinginProgress
        {
            get { return Get(() => IsInitialIndexinginProgress); }
            private set { Set(() => IsInitialIndexinginProgress, value); }
        }

        public bool IsBusy
        {
            get { return Get(() => IsBusy); }
            private set { Set(() => IsBusy, value); }
        }

        public bool IsVisible
        {
            get { return Get(() => IsVisible); }
            set { Set(() => IsVisible, value); }
        }

        public void Show( bool showJustProgress = false)
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
            if (showJustProgress)
            {
                IsIndexExisted = false;
                CreateIndexVisibility = Visibility.Collapsed;
                ShowProgress = Visibility.Visible;
                FinishedStepVisibility = Visibility.Collapsed;
                _timer = new Timer(TimerProgressCallback, null, 0, 2000);
            }
            IsVisible = true;
        }

        public void Close()
        {
            IRegion region = _regionManager.Regions[RegionNames.ElasticSearchRegion];
            if (region == null || !region.Views.Contains(View))
            {
                return;
            }
            IsVisible = false;
            region.Deactivate(View);
            region.Remove(View);
            OnClosed();
        }

        public event EventHandler IndexingStarted;
        public event EventHandler IndexingFinished;
        public event EventHandler Closed;

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

        public string CountEmailsAttachments
        {
            get { return Get(() => CountEmailsAttachments); }
            set { Set(() => CountEmailsAttachments, value); }
        }

        public Visibility WarmingVisibility
        {
            get { return Get(() => WarmingVisibility); }
            set { Set(() => WarmingVisibility, value); }
        }

        public int WarmSecond
        {
            get { return Get(() => WarmSecond); }
            set { Set(() => WarmSecond, value); }
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

        public ICommand ForceCommand
        {
            get { return Get(() => ForceCommand); }
            set { Set(() => ForceCommand, value); }
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

        private void OnClosed()
        {
            var temp = this.Closed;
            if (temp != null)
            {
                temp(this,EventArgs.Empty);
            }
        }

        private void CheckServicesAndIndex()
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
//#if DEBUG
//            IsServiceRunning = IsServiceInstalled = true;
//#endif
            if (IsServiceRunning)
            {
                var resp = ElasticSearchClient.IndexExists(OFElasticSearchClientBase.DefaultInfrastructureName);
                IsIndexExisted = resp.Exists;//false;//
                var riverStatusResp = ElasticSearchClient.GetRiverStatus();
                IsInitialIndexinginProgress = riverStatusResp.Response.IsNotNull() &&
                                              riverStatusResp.Response.Status == OFRiverStatus.InitialIndexing;
                OFLogger.Instance.LogInfo("STATUS: {0}",riverStatusResp.Response.Status);
                _eventAggregator.GetEvent<OFMenuEnabling>().Publish(resp.Exists);
                if (IsIndexExisted)
                {
                    ElasticSearchClient.CheckAndCreareWarms();
                }
            }
        }

        private void InstallServiceCommandExecute(object arg)
        {
            ExecuteCommandForService("install");
            CheckServicesAndIndex();                
        }

        private void RunServiceCommandExecute(object arg)
        {
            ExecuteCommandForService("start");
            WarmingVisibility = Visibility.Visible;

            Dispatcher disp = Dispatcher.CurrentDispatcher;
            

            var backGround = new BackgroundWorker();
            backGround.DoWork += (sender, args) =>
            {
                for (int i = WarmSecond - 1; i >= 0; i--)
                {
                    WarmSecond = i + 1;
                    Thread.Sleep(1000);
                }
                disp.BeginInvoke((Action) (() =>
                {
                    CheckServicesAndIndex();
                    if (IsServiceRunning && IsIndexExisted && !IsInitialIndexinginProgress)
                    {
                        Close();
                    }
                }));
            };
            backGround.RunWorkerAsync();

        }

        private void CreateIndexCommandExecute(object arg)
        {
            InitElasticSearch();
            CreateIndexVisibility = Visibility.Collapsed;
            ShowProgress = Visibility.Visible;
            //CheckServicesAndIndex();
        }

        private void ForceCommandExecute(object arg)
        {
            var forceClient = _unityContainer.Resolve<IElasticSearchForceClient>();
            if (forceClient.IsNull())
            {
                return;
            }
            forceClient.Force();
        }

        private void ExecuteCommandForService(string command)
        {
            string path = OFRegistryHelper.Instance.GetElasticSearchpath();
            string javaHome = OFRegistryHelper.Instance.GetJavaInstallationPath();
            if (path.IsEmpty() || command.IsEmpty() || javaHome.IsEmpty())
            {
                OFLogger.Instance.LogDebug("Path or Command or JavaHome was empty");
                return;
            }
            const string serviceBat = "service.bat";
            try
            {
                Process p = new Process();
                p.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                p.StartInfo.FileName = string.Format("{0}\\bin\\{1}", path, serviceBat);
                p.StartInfo.Arguments = string.Format(" {0} \"{1}\"", command, javaHome);
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.Verb = "runas";
                p.StartInfo.WorkingDirectory = string.Format("{0}{1}", path, "\\bin");
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.RedirectStandardOutput = true;

                p.Start();
                p.WaitForExit();

                Debug.WriteLine(string.Format("!!!!!!!! {0}", p.StandardOutput != null ? p.StandardOutput.ReadToEnd() : ""));
                Debug.WriteLine(string.Format("!!!!!!!! {0}", p.StandardError != null ? p.StandardError.ReadToEnd() : ""));
            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(ex.Message);
            } 
        }

        private void InitElasticSearch()
        {
            try
            {
                ElasticSearchClient.CreateInfrastructure();
                Thread.Sleep(1000);
                _timer = new Timer(TimerProgressCallback,null,1000,2000);
                OnIndexingStarted();
            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(ex.Message);
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

                if (response.Response.Items.All(i => i.Status == PstReaderStatus.NonStarted))
                {
                    return;
                }
                if (response.Response.Items.All(s => s.Status == PstReaderStatus.Finished))
                {
                    _timer.Change(Timeout.Infinite, Timeout.Infinite);
                    IsIndexExisted = true;
                    FinishedStepVisibility = Visibility.Visible;
                    _eventAggregator.GetEvent<OFMenuEnabling>().Publish(true);
                    OnIndexingFinished();
                    return;
                }
                if (response.Response.Items.Any(i => i.Status == PstReaderStatus.Busy))
                {
                    IsBusy = true;
                    ShowProgress = Visibility.Visible;
                    var emailCount = ElasticSearchClient.GetTypeCount<OFEmail>();
                    var attachmentCount = ElasticSearchClient.GetTypeCount<OFAttachmentContent>();
                    double sumAll = (double)response.Response.Items.Sum(s => s.Count);
                    double sumProcessing = emailCount + attachmentCount;
                    var busyReader = response.Response.Items.FirstOrDefault(r => r.Status == PstReaderStatus.Busy);
                    CurrentFolder = busyReader.IsNotNull() ? busyReader.Folder : "";
                    CurrentProgress = (sumProcessing / sumAll) * 100.0;
                    CountEmailsAttachments = string.Format("{0} / {1}", emailCount, attachmentCount);
                }
                if (response.Response.Items.Any(i => i.Status == PstReaderStatus.Suspended))
                {
                    IsBusy = false;
                }
            }
            catch (WebException w)
            {
                OFLogger.Instance.LogError("[Web] " + w.Message);
            }
            catch (Exception e)
            {
                OFLogger.Instance.LogError(e.Message);
            }
        }

       


        #endregion

    }
}