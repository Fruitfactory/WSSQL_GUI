using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using WSSQLGUI.Controllers;
using WSSQLGUI.Views;
using MVCSharp.Core.Configuration;
using MVCSharp.Core.Tasks;
using MVCSharp.Winforms;

namespace WSSQLGUI
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            TasksManager tasksManager = new TasksManager(WinformsViewsManager.GetDefaultConfig());
            tasksManager.StartTask(typeof(MainTask));

            Application.Run(Application.OpenForms[0]);
        }
    }
}
