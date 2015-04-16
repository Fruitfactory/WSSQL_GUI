using System;
using System.Threading;
using System.Windows.Threading;
using OF.Core.Logger;

namespace OF.Infrastructure.Controls.ProgressManager
{
    /// <summary>
    /// Interaction logic for ProgressWindow.xaml
    /// </summary>
    public partial class ProgressWindow : IProgressForm
    {
        private ProgressOperation _currentOperation;
        private Action _cancelAction;
        private SynchronizationContext _currentContext;

        public ProgressWindow()
        {
            InitializeComponent();
            _currentContext = SynchronizationContext.Current;
        }

        #region Implementation of IProgressForm

        public void CloseExt()
        {
            //DispatcherOperation dispatcherOperation = this.Dispatcher.BeginInvoke(new Action(Close));
            if (_currentContext != null)
                _currentContext.Send(state => Close(), null);
        }

        public void ProcessCommand(ProgressFormCommand cmd, object arg)
        {
            switch (cmd)
            {
                case ProgressFormCommand.Settings:
                    ProcessSettings(arg);
                    WSSqlLogger.Instance.LogInfo(arg.ToString());
                    break;
                case ProgressFormCommand.Activate:
                    ProcessActivate();
                    break;
                case ProgressFormCommand.Progress:
                    ProcessProgress(arg);
                    break;
            }
        }

        public new int Width
        {
            get { return (int)base.Width; }
            set { base.Width = value; }
        }
        public new int Height
        {
            get { return (int)base.Height; }
            set { base.Height = value; }
        }

        #endregion

        private void ProcessSettings(object arg)
        {
            ProgressOperation operation = arg as ProgressOperation;
            if (operation == null)
                return;
            _currentOperation = operation;
            textLabel.Text = _currentOperation.Caption;
            Left = ((operation.Size.Width - this.Width)/2) + operation.Location.X;
            Top = ((operation.Size.Height - this.Height)/2) + operation.Location.Y;
            _cancelAction = _currentOperation.CancelAction;
        }

        private void ProcessActivate()
        {
            Activate();
        }

        private void ProcessProgress(object arg)
        {
            if (arg == null)
                return;
        }

    }
}
