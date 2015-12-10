using System;
using OF.Core.Core.ElasticSearch;
using OF.Core.Interfaces;
using OF.Core.Logger;

namespace OF.Infrastructure.Implements.ElasticSearch.Clients
{
    public class OFElasticSearchForceClient : OFElasticSearchClientBase, IElasticSearchForceClient
    {
        public void Force()
        {
            try
            {
                Raw.IndexPut("_river", DefaultInfrastructureName, "force", new object());
            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(ex.ToString());
            }
        }
    }
}