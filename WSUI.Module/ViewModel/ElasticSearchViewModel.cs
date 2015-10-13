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
using OF.Core.Core.ElasticSearch;
using OF.Core.Core.MVVM;
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
        private IAttachmentReader _attachmentReader;
        private Timer _timer;
        private bool _isFinishing = false;

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
            CheckServices();
            InstallServiceCommand = new OFRelayCommand(InstallServiceCommandExecute);
            RunServiceCommand = new OFRelayCommand(RunServiceCommandExecute);
            CreateIndexCommand = new OFRelayCommand(CreateIndexCommandExecute);
            LetsGoCommand = new OFRelayCommand(o => this.Close());
            ForceCommand = new OFRelayCommand(ForceCommandExecute);
            CreateIndexVisibility = Visibility.Visible;
            ShowProgress = Visibility.Collapsed;
            FinishedStepVisibility = Visibility.Collapsed;
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

        public string CountEmailsAttachments
        {
            get { return Get(() => CountEmailsAttachments); }
            set { Set(() => CountEmailsAttachments, value); }
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
                var resp = ElasticSearchClient.IndexExists(OFElasticSearchClientBase.DefaultInfrastructureName);
                IsIndexExisted = resp.Exists;//false;//
                var riverStatusResp = ElasticSearchClient.GetRiverStatus();
                IsInitialIndexinginProgress = riverStatusResp.Response.IsNotNull() &&
                                              riverStatusResp.Response.Status == OFRiverStatus.InitialIndexing;
                _eventAggregator.GetEvent<OFMenuEnabling>().Publish(resp.Exists);
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
            //CheckServices();
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
                OFLogger.Instance.LogError(ex.Message);
            } 
        }

        private void InitElasticSearch()
        {
            try
            {
                var list = OFOutlookHelper.GetOutlookFiles();
                ElasticSearchClient.CreateInfrastructure(list);
                Thread.Sleep(1000);
                _timer = new Timer(TimerProgressCallback,null,1000,2000);
                OnIndexingStarted();
            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(ex.Message);
            }
        }

        private void StartReadingAttachment()
        {
            OFLogger.Instance.LogDebug("Create Attachment Reader...");
            _attachmentReader = _unityContainer.Resolve<IAttachmentReader>();
            _attachmentReader.Start(null);
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
                if (response.Response.Items.All(s => s.Status == PstReaderStatus.Finished) && _attachmentReader.IsNotNull() && _attachmentReader.Status == PstReaderStatus.Finished)
                {
                    _timer.Change(Timeout.Infinite, Timeout.Infinite);
                    IsIndexExisted = true;
                    FinishedStepVisibility = Visibility.Visible;
                    _eventAggregator.GetEvent<OFMenuEnabling>().Publish(true);
                    OnIndexingFinished();
                    return;
                }

                if (response.Response.Items.All(i => i.Status == PstReaderStatus.Finished) &&
                    _attachmentReader.IsNotNull() && _attachmentReader.IsSuspended &&
                    _attachmentReader.Status != PstReaderStatus.Finished)
                {
                    _attachmentReader.Resume();
                }
                if (response.Response.Items.Any(i => i.Status == PstReaderStatus.Busy))
                {
                    lock (_lock)
                    {
                        if (_attachmentReader.IsNull())
                        {
                            StartReadingAttachment();
                        }
                        else if (_attachmentReader.IsNotNull() && _attachmentReader.IsSuspended && _attachmentReader.Status != PstReaderStatus.Finished)
                        {
                            _attachmentReader.Resume();
                        }    
                    }
                    IsBusy = true;
                    ShowProgress = Visibility.Visible;
                    double sumAll = (double)response.Response.Items.Sum(s => s.Count);
                    double sumProcessing = response.Response.Items.Sum(s => s.Processing);
                    double sumAttachments = response.Response.Items.Sum(s => s.Attachment) + _attachmentReader.Count;
                    var busyReader = response.Response.Items.FirstOrDefault(r => r.Status == PstReaderStatus.Busy);
                    CurrentFolder = busyReader.IsNotNull() ? busyReader.Folder : "";
                    CurrentProgress = (sumProcessing / sumAll) * 100.0;
                    CountEmailsAttachments = string.Format("{0} / {1}", sumProcessing, sumAttachments);
                }
                lock (_lock)
                {
                    if (response.Response.Items.Any(i => i.Status == PstReaderStatus.Suspended ))
                    {
                        if (_attachmentReader.IsNotNull() && !_attachmentReader.IsSuspended &&
                            _attachmentReader.Status != PstReaderStatus.Finished)
                        {
                            _attachmentReader.Suspend();
                        }
                        IsBusy = false;
                    }    
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