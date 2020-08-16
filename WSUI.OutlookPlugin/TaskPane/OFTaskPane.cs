using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace OFOutlookPlugin.TaskPane
{
	public partial class OFTaskPane:UserControl
	{
		public OFTaskPane()
		{
			InitializeComponent();
            this.BackColor = Color.White;
        }

		public void SetChild(UIElement child)
		{
			uiHost.Child = child;
		}

	}
}
