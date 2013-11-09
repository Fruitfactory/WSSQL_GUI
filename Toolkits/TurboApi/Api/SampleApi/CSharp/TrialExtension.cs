using System;
using System.Windows.Forms;

public partial class TrialExtension : Form
{
    public TrialExtension()
    {
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
            TurboActivate.ExtendTrial(txtExtension.Text);
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