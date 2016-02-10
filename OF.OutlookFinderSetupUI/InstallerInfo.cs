using System.Windows;
using WixWPF;

namespace OF.OutlookFinderSetupUI
{
    /// <summary>The main installer data context.</summary>
    public class InstallerInfo : BaseBAObject
    {

        #region Member Variables

        /// <summary>Indicates if the product</summary>
        private bool _isInstalled = false;

        /// <summary>Indicates if the installer is busy.</summary>
        private bool _isBusy = false;

        /// <summary>The progress message.</summary>
        private string _message = string.Empty;

        private double _progress;


        public InstallerInfo()
        {
            IsApplied = false;
        }

        #endregion Member Variables

        #region Properties

        #region [isapplied]

        public bool IsApplied { get; set; }

        #endregion

        #region CanInstall
        /// <summary>Indicates if the product can be installed.</summary>
        public bool CanInstall { get { return !(IsInstalled); } }
        #endregion CanInstall

        #region Install Visibility

        public Visibility InstallVisibility
        {
            get { return !IsApplied && CanInstall ? Visibility.Visible : Visibility.Collapsed; }
        }

        #endregion 

        #region CanUninstall
        /// <summary>Indicates if the product can be uninstalled.</summary>
        public bool CanUninstall { get { return (IsInstalled); } }
        #endregion CanUninstall

        #region Unistall Visibility

        public Visibility UnistallVisibility
        {
            get { return !IsApplied && IsInstalled ? Visibility.Visible : Visibility.Collapsed; }
        }

        #endregion

        #region IsInstalled

        /// <summary>Indicates if the product is installed.</summary>
        public bool IsInstalled
        {
            get { return _isInstalled; }
            set
            {
                _isInstalled = value; 
                OnPropertiesChanged("IsInstalled", "CanInstall", "CanUninstall", "InstallVisibility", "UnistallVisibility", "CancelButtonName");
            }
        }
        #endregion IsInstalled

        #region IsBusy
        /// <summary>Indicates if the installer is busy.</summary>
        public bool IsBusy { get { return _isBusy; } set { _isBusy = value; OnPropertiesChanged("IsBusy", "IsEnabled"); } }
        #endregion IsBusy

        public bool IsEnabled { get { return !IsBusy; } }

        public string CancelButtonName { get { return "Done"; } }


        #region Message
        /// <summary>The progress message.</summary>
        public string Message { get { return _message; } set { _message = value; OnPropertiesChanged("Message"); } }
        #endregion Message

        #region [progress]

        public double Progress
        {
            get { return _progress; }
            set
            {
                _progress = value;
                OnPropertyChanged("Progress");
            }
        }

        #endregion

        #endregion Properties
    }
}
