using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using WSSQLGUI.Core;
using WSSQLGUI.Views;
using WSSQLGUI.Controllers;
using System.Threading;
using System.Data.OleDb;
using System.Data;
using WSSQLGUI.Services.Enums;
using WSSQLGUI.Services.Helpers;

namespace WSSQLGUI.Kinds
{
	internal class AllFilesStrategy : BaseKindItemStrategy
	{

        public AllFilesStrategy()
        {
            // init
            _queryTemplate = "SELECT System.ItemName, System.ItemUrl FROM SystemIndex WHERE Contains(*,'{0}*')";
            _queryAnd = " AND Contains(*,'{0}*')";
            

        }

        public override void OnInit()
        {
            base.OnInit();
            ID = 0;
            SettingsView = new AllFilesSettingsView();
            _settingsController = SettingsView.Controller as AllFilesSettingsController;
            DataView = new AllFilesDataView();
            _dataController = DataView.Controller as AllFilesDataController;
            (_settingsController as AllFilesSettingsController).Search += (o, e) =>
            {
                Thread thread = new Thread(() => DoQuery());
                OnStart();
                (DataView as IDataView).IsLoading = true;
                thread.Start();
            };
            (_settingsController as AllFilesSettingsController).Error += (o, e) => { OnError(e.Value); };
            (DataView as IDataView).SelectedItemChanged += (o, e) => { OnCurrentItemChanged(e.Value); };

        }

        protected override void OnComplete(bool res)
        {
            base.OnComplete(res);
            (DataView as IDataView).IsLoading = false;
        }

        protected override void ReadData(IDataReader reader)
        {
            string name = reader[0] as string;
            string file = reader[1] as string;
            bool att = (bool)reader[2];
            TypeSearchItem type = SearchItemHelper.GetTypeItem(file);
            (_dataController as AllFilesDataController).SetData( new BaseSearchData() { Name = name, Path = file, Type = type, ID = Guid.NewGuid() });
        }

        protected override string CreateSqlQuery()
        {
            var searchCriteria = (SettingsView as IAllFilesSettingsView).SearchCriteria;
            string res = string.Empty;
            if (searchCriteria.IndexOf(' ') > -1)
            {
                StringBuilder temp = new StringBuilder();
                var list = searchCriteria.Split(' ').ToList();
                if (list == null || list.Count == 1)
                    return searchCriteria;
                res = string.Format(_queryTemplate, list[0]);
                for (int i = 1; i < list.Count; i++)
                {
                    temp.Append(string.Format(_queryAnd, list[i]));
                }
                res += temp.ToString();
            }
            else
                res = string.Format(_queryTemplate, searchCriteria);

            return res;
        }
	}
}
