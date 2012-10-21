using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace WSSQLGUI.Controls.ProgressControl
{
    internal partial class ProgressForm : Form, IProgressForm
    {
        private SynchronizationContext _currentContext;
        private ProgressOperation _currentOperation;
        private Action _cancelAction;

        public ProgressForm()
        {
            InitializeComponent();
            _currentContext = SynchronizationContext.Current;
        }

        #region implement IProgressForm

        public void CloseExt()
        {
            _currentContext.Send(state => Close(),null);    
        }

        public void ProcessCommand(ProgressFormCommand cmd, object arg)
        {
            switch(cmd)
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

        public FormWindowState State
        {
            get { return base.WindowState; }
            set { base.WindowState = value; }
        }

        public new Point Location
        {
            get { return base.Location; }
            set { base.Location = value; }
        }

        public new int Width
        {
            get { return base.Width; }
            set { base.Width = value; }
        }

        public new int Height
        {
            get { return base.Height; }
            set { base.Height = value; }
        }

        #endregion

        #region private 

        private void ProcessSettings(object arg)
        {
            ProgressOperation operation = arg as ProgressOperation;
            if(operation == null)
                return;
            _currentOperation = operation;
            TopMost = true;
            labelMessage.Text = _currentOperation.Caption;
            buttonCancel.Visible = _currentOperation.Canceled;
            progress.Style = _currentOperation.Style;
            if (_currentOperation.Style == ProgressBarStyle.Blocks)
            {
                progress.Minimum = _currentOperation.Min;
                progress.Maximum = _currentOperation.Max;
            }
            _cancelAction = _currentOperation.CancelAction;
            if(_cancelAction != null)
            {
                buttonCancel.Click += (o, e) => this.Invoke(_cancelAction);
            }

        }

        private void ProcessActivate()
        {
            TopMost = false;
            TopMost = true;
            Activate();
        }

        private void ProcessProgress(object arg)
        {
            if(arg == null)
                return;

            _currentContext.Send(o => progress.Value = (int)o,arg);
        }

        #endregion
    }
}
