using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace CSharp
{
    public partial class Form1 : Form
    {
        bool isActivated;

        public Form1()
        {
            InitializeComponent();

            //TODO: goto the version page at LimeLM and paste this GUID here
            TurboActivate.VersionGUID = "17738358944b7a7316ec5fe9.23132283";

            try
            {
                // Check if we're activated, and every 90 days verify it with the activation servers
                // In this example we won't show an error if the activation was done offline
                // (see the 3rd parameter of the IsGenuine() function) -- http://wyday.com/limelm/help/offline-activation/
                IsGenuineResult gr = TurboActivate.IsGenuine(90, 14, true);

                isActivated = gr == IsGenuineResult.Genuine ||
                              gr == IsGenuineResult.GenuineFeaturesChanged ||

                              // an internet error means the user is activated but
                              // TurboActivate failed to contact the LimeLM servers
                              gr == IsGenuineResult.InternetError;

                if (gr == IsGenuineResult.InternetError)
                {
                    //TODO: give the user the option to retry the genuine checking immediately
                    //      For example a dialog box. In the dialog call IsGenuine() to retry immediately
                }
            }
            catch (TurboActivateException ex)
            {
                MessageBox.Show("Failed to check if activated: " + ex.Message);
            }

            ShowTrial(!isActivated);

            // if this app is activated then you can get a feature value
            // See: http://wyday.com/limelm/help/license-features/
            /*
            if (isActivated)
            {
                string featureValue = TurboActivate.GetFeatureValue("your feature name");

                //TODO: do something with the featureValue
            }
            */
        }

        void mnuActDeact_Click(object sender, EventArgs e)
        {
            if (isActivated)
            {
                // deactivate product without deleting the product key
                // allows the user to easily reactivate
                TurboActivate.Deactivate(false);
                isActivated = false;
                ShowTrial(true);
            }
            else
            {
                //Note: you can launch the TurboActivate wizard or you can create you own interface

                // launch TurboActivate.exe to get the product key from the user
                Process TAProcess = new Process
                {
                    StartInfo =
                    {
                        FileName = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "TurboActivate.exe")
                    },
                    EnableRaisingEvents = true
                };

                TAProcess.Exited += p_Exited;
                TAProcess.Start();
            }
        }

        /// <summary>This event handler is called when TurboActivate.exe closes.</summary>
        void p_Exited(object sender, EventArgs e)
        {
            // remove the event
            ((Process) sender).Exited -= p_Exited;

            // the UI thread is running asynchronous to TurboActivate closing
            // that's why we can't call CheckIfActivated(); directly
            Invoke(new IsActivatedDelegate(CheckIfActivated));
        }

        delegate void IsActivatedDelegate();

        /// <summary>Rechecks if we're activated -- if so enable the app features.</summary>
        void CheckIfActivated()
        {
            // recheck if activated
            if (TurboActivate.IsActivated())
            {
                isActivated = true;
                ReEnableAppFeatures();
                ShowTrial(false);
            }
        }

        /// <summary>Put this app in either trial mode or "full mode"</summary>
        /// <param name="show">If true show the trial, otherwise hide the trial.</param>
        void ShowTrial(bool show)
        {
            lblTrialMessage.Visible = show;
            btnExtendTrial.Visible = show;

            mnuActDeact.Text = show ? "Activate..." : "Deactivate";

            if (show)
            {
                int trialDaysRemaining = 0;

                try
                {
                    TurboActivate.UseTrial();

                    // get the number of remaining trial days
                    trialDaysRemaining = TurboActivate.TrialDaysRemaining(); 
                }
                catch { }

                // if no more trial days then disable all app features
                if (trialDaysRemaining == 0)
                    DisableAppFeatures();
                else
                    lblTrialMessage.Text = "Your trial expires in " + trialDaysRemaining + " days.";
            }
        }

        /// <summary>Change this function to disable the features of your app.</summary>
        void DisableAppFeatures()
        {
            //TODO: disable all the features of the program
            txtMain.Enabled = false;

            lblTrialMessage.Text = "The trial has expired. Get an extension at Example.com";
        }

        /// <summary>Change this function to re-enable the features of your app.</summary>
        void ReEnableAppFeatures()
        {
            //TODO: re-enable all the features of the program
            txtMain.Enabled = true;
        }

        void btnExtendTrial_Click(object sender, EventArgs e)
        {
            TrialExtension trialExt = new TrialExtension();

            if (trialExt.ShowDialog(this) == DialogResult.OK)
            {
                // get the number of remaining trial days
                int trialDaysRemaining = 0;

                try
                {
                    trialDaysRemaining = TurboActivate.TrialDaysRemaining();
                }
                catch { }

                // if more trial days then re-enable all app features
                if (trialDaysRemaining > 0)
                {
                    ReEnableAppFeatures();
                    lblTrialMessage.Text = "Your trial expires in " + trialDaysRemaining + " days.";
                }
            }
        }
    }
}
