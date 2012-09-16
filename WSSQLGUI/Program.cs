using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using WSSQLGUI.Controllers;
using WSSQLGUI.Views;
using MVCSharp.Core.Configuration;
using MVCSharp.Core.Tasks;
using MVCSharp.Winforms;
using C4F.DevKit.PreviewHandler;
using C4F.DevKit.PreviewHandler.Service.Logger;

namespace WSSQLGUI
{
    static class Program
    {
        private static string Registrekey = "/r";
        private static string Unregistrekey = "/u";


        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length == 1)
            {
                if (string.Equals(args[0], Registrekey))
                {
                    HelperPreviewHandlers.RegisterHandlers();
                    return;
                }
                else if (string.Equals(args[0], Unregistrekey))
                {
                    HelperPreviewHandlers.UnregisterHandlers();
                    return;
                }
            }
            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            TasksManager tasksManager = new TasksManager(WinformsViewsManager.GetDefaultConfig());
            tasksManager.StartTask(typeof(MainTask));
            Application.ApplicationExit += (o, e) =>
            {
                WSSQLGUI.Services.Helpers.TempFileManager.Instance.ClearTempFolder();
                WSSQLGUI.Services.Helpers.OutlookHelper.Instance.Logoff();
                WSSQLGUI.Services.Helpers.OutlookHelper.Instance.Dispose();
            };

            Application.Run(Application.OpenForms[0]);
            
        }

    }
}
