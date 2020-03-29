using System;
using System.Runtime.Remoting.Messaging;
using System.Windows;
using OF.Control;
using OF.Core.Data;
using OF.Core.Enums;
using OF.Core.Helpers;
using OF.Core.Logger;
using OF.Core.Extensions;
using OFOutlookPlugin.Interfaces;
using OFOutlookPlugin.TaskPane;

namespace OFOutlookPlugin
{
    public partial class OFSidebar :  ISidebarForm
    {
	    private Microsoft.Office.Tools.CustomTaskPane _officeTaskPane;
	    private OFTaskPane _oftaskPane;
        private IPluginBootStraper _wsuiBootStraper = null;
        private bool _isSecondInstance = false;

        public OFSidebar(IPluginBootStraper bootStraper)
        {
            Init(bootStraper);
        }

        private void OnAdxAfterFormHide(object sender, EventArgs e)
        {
            if (!_isSecondInstance)
                Globals.ThisAddIn.IsMainUIVisible = false;
            _isSecondInstance = false;
        }

        //TODO:
        bool ISidebarForm.IsDisposed
        {
            get { return true; }
        }

        public void Minimize()
        {
            throw new NotImplementedException();
        }

        public void SendAction(OFActionType actionType)
        {
            _wsuiBootStraper.PassAction(new OFAction(actionType, null));
        }

        public void Hide ( )
        {
            if(_officeTaskPane == null) return;
	        _officeTaskPane.Visible = false;
        }

        public void Show ( )
        {
	        if (_officeTaskPane == null) return;
	        _officeTaskPane.Visible = true;
        }

        private void Init(IPluginBootStraper bootStraper)
        {
	        if (bootStraper == null)
	        {
		        OFLogger.Instance.LogDebug("Bootstraper eqaul 'NULL'.");
		        return;
	        }

	        _wsuiBootStraper = bootStraper;
	        UIElement el = _wsuiBootStraper.View as UIElement;
	        var parent = el.GetParentProperty();
	        if (el != null && parent == null)
	        {
		        _oftaskPane = new OFTaskPane();
		        _oftaskPane.SetChild(null);
		        _oftaskPane.SetChild(el);

		        _officeTaskPane = Globals.ThisAddIn.CustomTaskPanes.Add(_oftaskPane, "OutlookFinderTaskPane",
			        Globals.ThisAddIn.Application.ActiveExplorer());
		        Show();
		        if (!OFRegistryHelper.Instance.GetIsPluginUiVisible())
		        {
			        Hide();
			        Globals.ThisAddIn.IsMainUIVisible = false;
		        }
		        else
		        {
			        Globals.ThisAddIn.IsMainUIVisible = true;
		        }
	        }
	        else
	        {
		        _isSecondInstance = true;
		        Hide();
	        }
        }

    }
}