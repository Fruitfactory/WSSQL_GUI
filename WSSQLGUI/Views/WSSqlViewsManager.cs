using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MVCSharp.Winforms;
using MVCSharp.Core.Views;
using System.Windows.Forms;
using MVCSharp.Winforms.Configuration;
using MVCSharp.Core.Configuration;
using System.Reflection;
using WSSQLGUI.Controllers;

namespace WSSQLGUI.Views
{
    internal class WSSqlViewsManager : WinformsViewsManager
    {
        static SearchForm _mainFrm;

        protected override void ActivateUserControlView(IView view)
        {
            base.ActivateUserControlView(view);
            (view as Control).BringToFront();
        }

        protected override void InitializeUserControlView(UserControl userControlView)
        {
          
            base.InitializeUserControlView(userControlView);

            string name = userControlView.GetType().Name;
            if (name.Contains("Settings"))
            {
                _mainFrm.SettingsPanel.Controls.Add(userControlView);
                if (_mainFrm.Controller != null)
                    (_mainFrm.Controller as SearchController).SetSettingsView(userControlView);
            }
            else if (name.Contains("Data"))
            {
                _mainFrm.DataPanel.Controls.Add(userControlView);
                if (_mainFrm.Controller != null)
                    (_mainFrm.Controller as SearchController).SetDataView(userControlView);
                userControlView.ContextMenuStrip = _mainFrm.contextMenu;
            }
            
            userControlView.Dock = DockStyle.Fill;
        }

        protected override void InitializeFormView(Form form, WinformsViewInfo viewInf)
        {
            _mainFrm = form as SearchForm;
            base.InitializeFormView(form, viewInf);
            
        }

        new public static MVCConfiguration GetDefaultConfig()
        {
            MVCConfiguration defaultConf = WinformsViewsManager.GetDefaultConfig();
            defaultConf.ViewsAssembly = Assembly.GetCallingAssembly();
            defaultConf.ViewsManagerType = typeof(WSSqlViewsManager);
            return defaultConf;
        }
    }
}
