using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using MVCSharp.Core.CommandManager;
using WSSQLGUI.Controls.ProgressControl;
using WSSQLGUI.Core;
using System.Text.RegularExpressions;

namespace WSSQLGUI.Controllers
{
	internal class AllFilesSettingsController : BaseSettingsController, IAllFilesSettingsController
	{
	    private DelegateCommand _testCommand;
	    private volatile bool _isCanceled = false;


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
            Action cancel = () =>
                                {
                                    _isCanceled = true;
                                };
            int min = 0;
            int max = 100000;
            ProgressManager.Instance.StartOperation(new ProgressOperation(){Caption = "Test  Operation",DelayTime = 0,Canceled = true,Style = ProgressBarStyle.Blocks,Min = min, Max = max,CancelAction = cancel});
            //Thread.Sleep(5000);
            for (int i = min; i < max;i++ )
            {
                if (_isCanceled)
                    break;
                ProgressManager.Instance.SetProgress(i);                    
            }
            ProgressManager.Instance.StopOperation();
            _isCanceled = false;
        }

	}
}
