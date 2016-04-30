using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WixWPF;
using Wix = Microsoft.Tools.WindowsInstallerXml.Bootstrapper;

namespace OF.OutlookFinderSetupUI
{
    /// <summary>Interaction logic for MainWindow.xaml</summary>
    public partial class MainWindow : BaseBAWindow
    {
        #region Member Variables

        /// <summary>The detected package states.</summary>
        private Dictionary<string, Wix.PackageState> _packageStates = new Dictionary<string, Wix.PackageState>();

        #endregion Member Variables

        #region Constructors

        /// <summary>Creates a new instance of <see cref="MainWindow" />.</summary>
        public MainWindow()
        {
            InitializeComponent();
            InstallData = new InstallerInfo();
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
            //System.Diagnostics.Debugger.Launch();
        }


        private void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            Bootstrapper.WriteToLog(Wix.LogLevel.Error, unhandledExceptionEventArgs.ExceptionObject.ToString() + "   " + unhandledExceptionEventArgs.ToString());
        }

        #endregion Constructors

        #region Event Handlers

        #region OnButtonClick
        /// <summary>Handles the event of a button being clicked.</summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The arguments of the event.</param>
        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            InstallData.IsBusy = true;
            Button btn = sender as Button;

            if (btn != null && btn.Content != null && Bootstrapper != null && Bootstrapper.Engine != null)
            {
                switch (btn.Content.ToString().ToUpperInvariant())
                {
                    case "INSTALL": { Bootstrapper.Engine.Plan(Wix.LaunchAction.Install); } break;
                    case "UNINSTALL": { Bootstrapper.Engine.Plan(Wix.LaunchAction.Uninstall); } break;
                    case "DONE":
                    case "CANCEL":
                    case "QUIT": { Close(); } break;
                    default: break;
                }
            }
            else { InstallData.IsBusy = false; }
        }
        #endregion OnButtonClick

        #region OnWindowLoaded
        /// <summary>Raised after the window has been loaded.</summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The arguments of the event.</param>
        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            
        }
        #endregion OnWindowLoaded

        #region OnApplyComplete
        /// <summary>Raised by the bootstrapper when the it is notified that the apply stage is complete.</summary>
        /// <param name="args">The arguments of the event.</param>
        public override void OnApplyComplete(WPFBootstrapperEventArgs<Wix.ApplyCompleteEventArgs> args)
        {
            if (IsValid(args) && Wix.Result.None.Equals(args.Arguments.Result))
            {
                InstallData.IsApplied = true;
                args.Cancel = true;
                Bootstrapper.Engine.Detect();
            }
        }
        #endregion OnApplyComplete

        #region OnDetectComplete
        /// <summary>Raised by the bootstrapper when the it is notified that the detection stage is complete.</summary>
        /// <param name="args">The arguments of the event.</param>
        public override void OnDetectComplete(WPFBootstrapperEventArgs<Wix.DetectCompleteEventArgs> args)
        {
            
            InstallData.IsBusy = false;
            InstallData.IsInstalled = _packageStates.All(x => Wix.PackageState.Present.Equals(x.Value));
        }
        #endregion OnDetectComplete

        

        #region OnDetectPackageComplete
        /// <summary>Called when a packages source is being resolved.</summary>
        /// <param name="args">The arguments of the event.</param>
        public override void OnDetectPackageComplete(WPFBootstrapperEventArgs<Wix.DetectPackageCompleteEventArgs> args)
        {
            if (IsValid(args))
            {
                _packageStates[args.Arguments.PackageId] = args.Arguments.State;
            }
        }
        #endregion OnDetectPackageComplete

        #region OnExecuteMsiMessage
        /// <summary>Called when Windows Installer sends an installation message.</summary>
        /// <param name="args">The arguments of the event.</param>
        //public override void OnExecuteMsiMessage(WPFBootstrapperEventArgs<Wix.ExecuteMsiMessageEventArgs> args)
        //{
        //    if (IsValid(args))
        //    {
        //        InstallData.Message = args.Arguments.Message;
        //    }
        //}

        #endregion OnExecuteMsiMessage

        protected override void OnClosing(CancelEventArgs e)
        {
            Bootstrapper.WriteToLog(Wix.LogLevel.Standard,"Closing");
            base.OnClosing(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            Bootstrapper.WriteToLog(Wix.LogLevel.Standard,"Closed");
            base.OnClosed(e);
        }

        public override void OnExecuteComplete(WPFBootstrapperEventArgs<Wix.ExecuteCompleteEventArgs> args)
        {
            //InstallData.IsBusy = false;
            base.OnExecuteComplete(args);
        }

        public override void OnExecutePackageComplete(WPFBootstrapperEventArgs<Wix.ExecutePackageCompleteEventArgs> args)
        {
            base.OnExecutePackageComplete(args);
        }

        public override void OnShutdown(WPFBootstrapperEventArgs<Wix.ShutdownEventArgs> args)
        {
            base.OnShutdown(args);
        }

        public override void OnSystemShutdown(WPFBootstrapperEventArgs<Wix.SystemShutdownEventArgs> args)
        {
            base.OnSystemShutdown(args);
        }

        #region OnPlanComplete
        /// <summary>Called when the engine has completed planning the installation.</summary>
        /// <param name="args">The arguments of the event.</param>
        public override void OnPlanComplete(WPFBootstrapperEventArgs<Wix.PlanCompleteEventArgs> args)
        {
            if (IsValid(args) && args.Arguments.Status >= 0 && Bootstrapper != null && Bootstrapper.Engine != null)
            {
                Bootstrapper.Engine.Apply(IntPtr.Zero);
            }
            else { InstallData.IsBusy = false; }
        }
        #endregion OnPlanComplete

        public override void OnExecuteProgress(WPFBootstrapperEventArgs<Wix.ExecuteProgressEventArgs> args)
        {
            if (IsValid(args))
            {
                InstallData.Progress = args.Arguments.ProgressPercentage;
                InstallData.Message = args.Arguments.PackageId;    
            }
        }

        #endregion Event Handlers

        #region Properties

        #region InstallData
        /// <summary>The installer data.</summary>
        public InstallerInfo InstallData { get { return (DataContext as InstallerInfo); } set { DataContext = value; } }
        #endregion InstallData

        #endregion Properties
    }
}