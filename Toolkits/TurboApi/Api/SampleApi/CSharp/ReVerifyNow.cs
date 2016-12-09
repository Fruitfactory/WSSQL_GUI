using System;
using System.Windows.Forms;
using wyDay.TurboActivate;

partial class ReVerifyNow : Form
{
    private TurboActivate ta;
    bool inGrace = false;
    public readonly uint GenuineDaysLeft;

    public bool noLongerActivated = false;

    public ReVerifyNow(TurboActivate ta, uint DaysBetweenChecks, uint GracePeriodLength)
    {
        // Set the TurboActivate instance from the main form
        this.ta = ta;

        InitializeComponent();

        // Use the days between checks and grace period from
        // the main form
        GenuineDaysLeft = ta.GenuineDays(DaysBetweenChecks, GracePeriodLength, ref inGrace);

        if (GenuineDaysLeft == 0)
            lblDescr.Text = "You must re-verify with the activation servers to continue using this app.";
        else
            lblDescr.Text = "You have " + GenuineDaysLeft +  " days to re-verify with the activation servers.";
    }

    private void btnReverify_Click(object sender, EventArgs e)
    {
        try
        {
            switch (ta.IsGenuine())
            {
                case IsGenuineResult.Genuine:
                case IsGenuineResult.GenuineFeaturesChanged:

                    DialogResult = DialogResult.OK;
                    Close();

                    break;

                case IsGenuineResult.NotGenuine:
                case IsGenuineResult.NotGenuineInVM:

                    noLongerActivated = true;
                    DialogResult = DialogResult.Cancel;
                    Close();

                    break;

                case IsGenuineResult.InternetError:

                    MessageBox.Show("Failed to connect with the activation servers.");
                    break;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Failed to re-verify with the activation servers. Full error: " + ex.Message);
        }
    }

    private void btnExit_Click(object sender, EventArgs e)
    {
        DialogResult = DialogResult.Cancel;
        Close();
    }
}