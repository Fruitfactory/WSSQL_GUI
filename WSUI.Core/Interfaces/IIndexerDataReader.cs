using System.Collections.Generic;
using System.Data;

namespace WSUI.Core.Interfaces
{
    public interface IIndexerDataReader
    {
        DataTable GetDataByAdapter(string query);

        DataTable GetDataByReader(string query,IEnumerable<string> ingnoredFields);

        IIndexerDataReader GetInstance();

    }
}