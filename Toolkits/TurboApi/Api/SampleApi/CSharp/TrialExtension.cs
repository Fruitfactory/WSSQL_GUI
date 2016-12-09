using System;
using System.Windows.Forms;
using wyDay.TurboActivate;

public partial class TrialExtension : Form
{
    private readonly TurboActivate ta;
    private readonly TA_Flags trialFlags;

    public TrialExtension(TurboActivate ta, TA_Flags useTrialFlags)
    {
        this.ta = ta;
        this.trialFlags = useTrialFlags;

        InitializeComponent();
    }

    void btnCancel_Click(object sender, EventArgs e)
    {
        Close();
    }

    void btnOK_Click(object sender, EventArgs e)
    {
        try
        {
            // try to extend the trial and close the form
            ta.ExtendTrial(txtExtension.Text, trialFlags);
            DialogResult = DialogResult.OK;
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Trial extension failed.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            txtExtension.Focus();
        }
    }
}