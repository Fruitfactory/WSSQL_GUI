using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;

using MVCSharp.Core.Configuration.Tasks;
using MVCSharp.Core.Tasks;
using WSSQLGUI.Core;
using WSSQLGUI.Services;
using WSSQLGUI.Services.Helpers;
using Threading = System.Threading.Tasks;

namespace WSSQLGUI.Controllers
{
    internal class ContactSettingsController : BaseSettingsController, IContactSettingsController,IEmailSettings
    {
    	#region fields
    	private readonly string _query = "SELECT System.ItemName, System.Contact.FirstName, System.Contact.LastName FROM SystemIndex WHERE System.Kind = 'contact' AND ( CONTAINS(System.Contact.FirstName,'\"{0}*\"') OR CONTAINS(System.Contact.LastName,'\"{1}*\"') )";
        private readonly string _connectionString = "Provider=Search.CollatorDSO;Extended Properties=\"Application=Windows\"";
		
		private volatile string _searchCriteria = String.Empty;
        
    	#endregion
    	
    	#region events
        public event EventHandler<EventArgs<List<string>>> Suggest;
    	#endregion
    	
    	#region public
    	
    	public void StartSuggesting(string text)
    	{
    		if(string.IsNullOrEmpty(text))
    			return;
    		_searchCriteria = text;
    	    Threading.Task.Factory.StartNew(() => DoSuggest());
    	}
    	

        public List<string> GetFolders()
        {
            var list = OutlookHelper.Instance.GetFolderList();
            return list;
        }


    	#endregion
    	
    	#region private
    	
    	private void DoSuggest()
    	{
            List<string> list = null;
            var q = CreateQuery(_searchCriteria.Trim());
            OleDbConnection con = new OleDbConnection(_connectionString);
            OleDbDataReader reader = null;
            OleDbCommand cmd = new OleDbCommand(q, con);
            try
            {
                con.Open();
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var name = reader[0].ToString();
                    var first = reader[1].ToString();
                    var last = reader[2].ToString();
                    if (list == null)
                        list = new List<string>();
                    list.Add(string.Format("{0} {1}",first,last));
                }
            }
            finally
            {
                reader.Close();
                con.Close();
                EventHandler<EventArgs<List<string>>> temp = Suggest;
                if(temp != null)
                {
                	temp(null,new EventArgs<List<string>>(list));
                }
            }

    	}
    	
    	private string CreateQuery(string criteria)
        {
            var searchCriteria = criteria;

            string res = string.Empty;
            res = string.Format(_query, searchCriteria,searchCriteria);

            return res;
        }
    	
    	
    	#endregion
    }
}
