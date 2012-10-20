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
    public partial class ProgressForm : Form
    {
        private SynchronizationContext _currentContext;

        public ProgressForm()
        {
            InitializeComponent();
            _currentContext = SynchronizationContext.Current;
        }

        public void SetMessage(string message)
        {
            labelMessage.Text = message;
        }

        public void CloseExt()
        {
            _currentContext.Send(state => Close(),null);    
        }

    }
}
