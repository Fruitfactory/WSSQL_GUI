using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WSUI.Infrastructure.Controls.ProgressManager
{
    /// <summary>
    /// Interaction logic for ProgressWindow.xaml
    /// </summary>
    public partial class ProgressWindow : IProgressForm
    {
        private SynchronizationContext _currentContext;
        private ProgressOperation _currentOperation;
        private Action _cancelAction;

        
        public ProgressWindow()
        {
            InitializeComponent();
            _currentContext = SynchronizationContext.Current;
        }

        #region Implementation of IProgressForm

        public void CloseExt()
        {
            _currentContext.Send(state => Close(), null); 
        }

        public void ProcessCommand(ProgressFormCommand cmd, object arg)
        {
            switch (cmd)
            {
                case ProgressFormCommand.Settings:
                    ProcessSettings(arg);
                    break;
                case ProgressFormCommand.Activate:
                    ProcessActivate();
                    break;
                case ProgressFormCommand.Progress:
                    ProcessProgress(arg);
                    break;
            }
        }

        public int Width { get; set; }
        public int Height { get; set; }

        #endregion

        private void ProcessSettings(object arg)
        {
            ProgressOperation operation = arg as ProgressOperation;
            if (operation == null)
                return;
            _currentOperation = operation;
            textLabel.Text = _currentOperation.Caption;
            //buttonCancel.Visible = _currentOperation.Canceled;
            //progress.Style = _currentOperation.Style;
            //if (_currentOperation.Style == ProgressBarStyle.Blocks)
            //{
            //    progress.Minimum = _currentOperation.Min;
            //    progress.Maximum = _currentOperation.Max;
            //}
            _cancelAction = _currentOperation.CancelAction;
            //if (_cancelAction != null)
            //{
            //    buttonCancel.Click += (o, e) => this.Invoke(_cancelAction);
            //}
        }

        private void ProcessActivate()
        {
            Activate();
        }

        private void ProcessProgress(object arg)
        {
            if (arg == null)
                return;
            //_currentContext.Send(o => progress.Value = (int)o, arg);
        }

    }
}
