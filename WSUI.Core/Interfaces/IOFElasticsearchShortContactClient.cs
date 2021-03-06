﻿using System.Collections.Generic;
using OF.Core.Data.ElasticSearch;

namespace OF.Core.Interfaces
{
    public interface IOFElasticsearchShortContactClient
    {
        void SaveShortContacts(List<OFShortContact> contacts);
        IEnumerable<OFShortContact> GetAllSuggestionContacts();
    }
}