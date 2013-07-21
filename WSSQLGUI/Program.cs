using System;
using System.Windows.Forms;
using WSPreview.PreviewHandler.Service.Preview;
using WSSQLGUI.Controllers.Tasks;
using WSSQLGUI.Views;
using MVCSharp.Core.Tasks;

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

            TasksManager tasksManager = new TasksManager(WSSqlViewsManager.GetDefaultConfig());
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
