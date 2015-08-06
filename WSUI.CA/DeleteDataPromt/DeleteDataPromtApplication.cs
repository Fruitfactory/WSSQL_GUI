using System.Windows.Forms;
using OF.CA.Core;

namespace OF.CA.DeleteDataPromt
{
    public class DeleteDataPromtApplication : CoreSetupApplication
    {
        private DeleteDataFolderForm _form;

        public DeleteDataPromtApplication(string productName) : base(productName)
        {
        }

        public bool PromtDeleteFolder()
        {
            _form = new DeleteDataFolderForm();
            return ShowDialog();
        }

        private bool ShowDialog()
        {
            return _form.ShowDialog(new WindowWrapper(GetMainWindowHandle())) == DialogResult.OK;
        }
    }
}