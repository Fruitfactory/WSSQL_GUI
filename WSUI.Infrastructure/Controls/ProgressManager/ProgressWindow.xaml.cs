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
using System.Windows.Threading;

namespace WSUI.Infrastructure.Controls.ProgressManager
{
    /// <summary>
    /// Interaction logic for ProgressWindow.xaml
    /// </summary>
    public partial class ProgressWindow : IProgressForm
    {
        private ProgressOperation _currentOperation;
        private Action _cancelAction;


        public ProgressWindow()
        {
            InitializeComponent();
        }

        #region Implementation of IProgressForm

        public void CloseExt()
        {
            DispatcherOperation dispatcherOperation = this.Dispatcher.BeginInvoke(new Action(Close));
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

        public int Width
        {
            get { return (int)base.Width; }
            set { base.Width = value; }
        }
        public int Height
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
