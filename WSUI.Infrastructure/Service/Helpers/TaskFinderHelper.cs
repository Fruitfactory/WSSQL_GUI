using System;
using System.Collections.Generic;
using System.Reflection;

namespace WSUI.Infrastructure.Service.Helpers
{
    internal class TaskFinderHelper
    {
        #region fields

        private const string DataSuffix = "DataTask";
        private const string SettingsSuffix = "SettingsTask";


        private Dictionary<string, Type> _dictionarySettingsTasks = new Dictionary<string, Type>();
        private Dictionary<string, Type> _dictionaryDataTasks = new Dictionary<string, Type>();
        private static object _lock = new object();

        private static TaskFinderHelper _instance = null;

        #endregion

        private TaskFinderHelper()
        {
            LoadAllTasks();
        }

        #region instance 

        public static TaskFinderHelper Instance
        {
            get
            {
                lock (_lock)
                {
                    if(_instance == null)
                        _instance = new TaskFinderHelper();
                    return _instance;
                }
            }
        }

        #endregion

        #region private 

        private string GetDataPrefix(string name)
        {
            string res = name.Substring(0, name.IndexOf(DataSuffix));
            return res;
        }


        private string GetSettingsPrefix(string name)
        {
            string res = name.Substring(0, name.IndexOf(SettingsSuffix));
            return res;
        }


        private  void LoadAllTasks()
        {
            var currentAssembly = Assembly.GetExecutingAssembly();
            foreach (var type in currentAssembly.GetTypes())
            {
//                if (type.IsClass && !type.IsAbstract && type.BaseType == typeof(TaskBase))
//                {
//                    var name = type.Name;
//                    if (name.Contains(DataSuffix))
//                        _dictionaryDataTasks.Add(GetDataPrefix(name), type);
//                    else if (name.Contains(SettingsSuffix))
//                        _dictionarySettingsTasks.Add(GetSettingsPrefix(name), type);
//                }
            }
        }

        #endregion

        #region public 

        public Type GetDataTask(string prefix)
        {
            if (_dictionaryDataTasks.ContainsKey(prefix))
                return _dictionaryDataTasks[prefix];
            return null;
        }


        public Type GetSettingsTask(string prefix)
        {
            if (_dictionarySettingsTasks.ContainsKey(prefix))
                return _dictionarySettingsTasks[prefix];
            return null;
        }


        #endregion


    }
}
