using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MVCSharp.Winforms;

namespace WSSQLGUI.Views
{
    public partial class BaseSettingsView : WinUserControlView
    {
        public BaseSettingsView()
        {
            InitializeComponent();
            OnInit();
        }

        protected  virtual void OnInit()
        {
           
        }


    }
}
