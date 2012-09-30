using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MVCSharp.Core;
using MVCSharp.Core.Views;
using MVCSharp.Core.CommandManager;
using WSSQLGUI.Models;
using WSSQLGUI.Services;
using System.Data.OleDb;
using System.Data;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;
using Outlook = Microsoft.Office.Interop.Outlook;
using System.Text.RegularExpressions;
using WSSQLGUI.Services.Helpers;
using WSSQLGUI.Services.Enums;
using C4F.DevKit.PreviewHandler.Service.Logger;
using System.Reflection;
using WSSQLGUI.Core;
using WSSQLGUI.Views;
using MVCSharp.Core.Tasks;

namespace WSSQLGUI.Controllers
{
    class SearchController : ControllerBase
    {
        #region const

        private const string connectionString = "Provider=Search.CollatorDSO;Extended Properties=\"Application=Windows\"";
        private const string qyeryTemplate = "SELECT System.ItemName, System.ItemUrl, System.IsAttachment, System.Message.ConversationID, System.Message.DateReceived  FROM SystemIndex WHERE Contains(*,'{0}*')";
        private const string emailQueryTemplate = "GROUP ON System.Message.ConversationID OVER( SELECT System.Subject,System.ItemName,System.ItemUrl,System.Message.ToAddress,System.Message.DateReceived, System.Message.ConversationID FROM SystemIndex WHERE System.Kind = 'email' AND CONTAINS(*,'{0}*') AND CONTAINS(System.ItemPathDisplay,'Входящие*',1033) ORDER BY System.Message.DateReceived DESC) ";//Входящие  //Inbox
        private const string qyeryAnd = " AND Contains(*,'{0}*')";
        private const string Interface = "WSSQLGUI.Core.IKindItem";
        

        #endregion

        #region fields

        private DelegateCommand _openFileCommand;
        private DelegateCommand _searchCommand;
        private List<IKindItem> _listKind = new List<IKindItem>();
        private BaseKindItemStrategy _currentKind = null;
        private Dictionary<string, List<ITask>> _tasks = new Dictionary<string, List<ITask>>();

        #endregion

        #region events

        public event EventHandler OnStartSearch;
        public event EventHandler<EventArgs<bool>> OnCompleteSearch;
        public event EventHandler<EventArgs<SearchItem>> OnAddSearchItem;

        #endregion

        #region commands

        public ICommand OpenFileCommand
        {
            get
            {
                if (_openFileCommand == null)
                    _openFileCommand = new DelegateCommand("Preview", CanOpenFile, OpenCurrentFile);
                return _openFileCommand;
            }
        }

        

        #endregion


        #region properties

        public override IView View
        {
            get
            {
                return base.View;
            }
            set
            {
                base.View = value;
            }
        }

        public BaseSearchData CurrenItem { get; private set; }
        public string FileName { get; private set;}

        #endregion

        #region public methods
       
        public void CurrentSearchItemChanged(SearchItem item)
        {
        }

        public List<string> GetAllKinds()
        {
            var currentAssembly = Assembly.GetExecutingAssembly();
            var list = new List<string>();
            foreach (var type in currentAssembly.GetTypes())
            {
                if (type.IsClass && !type.IsAbstract && type.GetInterface(Interface, true) != null)
                {
                    var kind = (IKindItem)currentAssembly.CreateInstance(type.FullName);
                    if (kind == null)
                        continue;
                    if (string.IsNullOrEmpty(kind.Name))
                        continue;
                    list.Add(kind.Name);
                    _listKind.Add(kind);
                }
            }
            return list;
        }

        public void CurrentKindChanged(string name)
        {
            var item = _listKind.OfType<BaseKindItemStrategy>().Where(k => k.Name == name).ToList();
            if (item == null || item.Count == 0)
                return;
            _currentKind = item[0];
            if (_currentKind.SettingsTaskType == null ||
                _currentKind.DataTaskType == null)
                return;
            if (!_tasks.ContainsKey(name))
            {

                _tasks[name] = new List<ITask>() { StartNewTask(_currentKind.SettingsTaskType), StartNewTask(_currentKind.DataTaskType) };
            }
            else
            {
                _tasks[name].ForEach(it => it.OnStart(null));
            }
        }

        public void SetSettingsView(UserControl settings)
        {
            if (_currentKind == null)
                return;
            _currentKind.ConnectWithSettingsView(settings as IView);
        }

        public void SetDataView(UserControl data)
        {
            if (_currentKind == null)
                return;
            _currentKind.ConnectWithDataView(data as IView);
        }

        #endregion

        #region private

        private ITask StartNewTask(Type tastType)
        {
            return Task.TasksManager.StartTask(tastType);
        }

        private void OpenCurrentFile()
        {

            if (CurrenItem == null)
                return;

            if (string.IsNullOrEmpty(FileName) ||
                FileService.IsDirectory(FileName))
                return;
            try
            {
                Process.Start(FileName);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                WSSqlLogger.Instance.LogError(string.Format("{0}: {1}", "Open preview", ex.Message));
            }
        }

        private bool CanOpenFile()
        {
            return CurrenItem != null;
        }

       

        #endregion


    }
}
