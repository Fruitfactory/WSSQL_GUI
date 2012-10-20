using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using MVCSharp.Core.CommandManager;
using WSSQLGUI.Controls.ProgressControl;
using WSSQLGUI.Core;
using System.Text.RegularExpressions;

namespace WSSQLGUI.Controllers
{
	internal class AllFilesSettingsController : BaseSettingsController
	{
	    private DelegateCommand _testCommand;

        protected override bool OnCanSearch()
        {
            return !_isError && !_isLoading;
        }

	    public ICommand TestCommand
	    {
	        get
	        {
	            if(_testCommand == null)
                    _testCommand = new DelegateCommand("Test Progress",OnCanTestExecute,TestExecute);
	            return _testCommand;
	        }
	    }

        private bool OnCanTestExecute()
        {
            return true;
        }

        private void TestExecute()
        {
            ProgressManager.Instance.StartOperation(new ProgressOperation(){Name = "Test  Operation",DelayTime = 2500});
            Thread.Sleep(5000);
            ProgressManager.Instance.StopOperation();
        }

	}
}
