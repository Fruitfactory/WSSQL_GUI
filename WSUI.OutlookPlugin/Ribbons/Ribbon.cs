using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Practices.Prism.Events;
using OF.Core.Data;
using OF.Core.Enums;
using OF.Core.Logger;
using OFOutlookPlugin.About;
using OFOutlookPlugin.Core;
using Office = Microsoft.Office.Core;

// TODO:  Follow these steps to enable the Ribbon (XML) item:

// 1: Copy the following code block into the ThisAddin, ThisWorkbook, or ThisDocument class.

//  protected override Microsoft.Office.Core.IRibbonExtensibility CreateRibbonExtensibilityObject()
//  {
//      return new Ribbon();
//  }

// 2. Create callback methods in the "Ribbon Callbacks" region of this class to handle user
//    actions, such as clicking a button. Note: if you have exported this Ribbon from the Ribbon designer,
//    move your code from the event handlers to the callback methods and modify the code to work with the
//    Ribbon extensibility (RibbonX) programming model.

// 3. Assign attributes to the control tags in the Ribbon XML file to identify the appropriate callback methods in your code.  

// For more information, see the Ribbon XML documentation in the Visual Studio Tools for Office Help.


namespace OFOutlookPlugin.Ribbons
{
	[ComVisible(true)]
	public class Ribbon: OFSearchCommandManager,  Office.IRibbonExtensibility
	{
		private Office.IRibbonUI ribbon;

		public Ribbon() 
        {

		}

		#region IRibbonExtensibility Members

		public string GetCustomUI(string ribbonID) 
		{
			if (ribbonID == "Microsoft.Outlook.Explorer")
				return GetResourceText("OFOutlookPlugin.Ribbons.Ribbon.xml");
			return string.Empty;
		}

		#endregion

		#region Ribbon Callbacks
		//Create callback methods here. For more information about adding callback methods, visit https://go.microsoft.com/fwlink/?LinkID=271226

		public void Ribbon_Load(Office.IRibbonUI ribbonUI) 
		{

			this.ribbon = ribbonUI;
		}

		public void OnShowHide(Office.IRibbonControl control) 
		{
			InternalShowHidePublish();
		}

		public void OnSettings(Office.IRibbonControl control) 
		{
			Globals.ThisAddIn.BootStraper.PassAction(new OFAction(OFActionType.Settings, null));
		}

		public void OnSendLogFiles(Office.IRibbonControl control) 
		{
			Globals.ThisAddIn.BootStraper.PassAction(new OFAction(OFActionType.SendLogFile, null));
		}

		public void OnHelp(Office.IRibbonControl control)
		{
			try
			{
				string helpurl = Properties.Settings.Default.HelpUrl;
				if (string.IsNullOrEmpty(helpurl))
				{
					OFLogger.Instance.LogError("Run help: {0}", "Help url is empty");
					return;
				}

				Process.Start(helpurl);
			}
			catch (Exception ex)
			{
				OFLogger.Instance.LogError("Run help: {0}", ex.Message);
			}
		}

		public void OnAbout(Office.IRibbonControl control)
		{
			OFAbout frmAbout = new OFAbout();
			frmAbout.ShowDialog();
		}

		public System.Drawing.Bitmap GetCustomImage(Office.IRibbonControl control)
		{
			switch (control.Id)
			{
				case "btnSHButton":
				case "btnShowHide":
					return Properties.Resources.logo_64;
				case "btnHelp":
				case "buttonHelp":
					return Properties.Resources.question;
				case "menuMore":
				case "menu1":
					return Properties.Resources.gear;
			}

			return null;
		}

		#endregion

		#region Helpers

		private static string GetResourceText(string resourceName) 
		{
			Assembly asm = Assembly.GetExecutingAssembly();
			string[] resourceNames = asm.GetManifestResourceNames();
			for(int i = 0;i < resourceNames.Length;++i) {
				if(string.Compare(resourceName,resourceNames[i],StringComparison.OrdinalIgnoreCase) == 0) {
					using(StreamReader resourceReader = new StreamReader(asm.GetManifestResourceStream(resourceNames[i]))) {
						if(resourceReader != null) {
							return resourceReader.ReadToEnd();
						}
					}
				}
			}
			return null;
		}

		#endregion
	}
}
