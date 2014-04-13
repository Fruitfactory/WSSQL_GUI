using System.Data;

namespace WSUI.Core.Interfaces
{
    internal interface IIndexerDataReader
    {
        DataTable GetDataByAdapter(string query);

        DataTable GetDataByReader(string query);
    }
}