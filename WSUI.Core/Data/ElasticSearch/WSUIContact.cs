using System;
using Elasticsearch.Net;
using Nest;

namespace WSUI.Core.Data.ElasticSearch
{
    [ElasticType(Name = "contact")]
    public class WSUIContact : WSUIElasticSearchBaseEntity
    {
        public string FirstName { get; set; }
        
        public string LastName { get; set; }
        
        public string EmailAddress1 { get; set; }
        
        public string EmailAddress2 { get; set; }
        
        public string EmailAddress3 { get; set; }

        public string BusinessTelephone { get; set; }
        public string HomeTelephone { get; set; }

        public string MobileTelephone { get; set; }

        #region [home]

        public string HomeAddressCity { get; set; }

        public string HomeAddressCountry { get; set; }

        public string HomeAddressPostalCode { get; set; }

        public string HomeAddressState { get; set; }

        public string HomeAddressStreet { get; set; }

        public string HomeAddressPostOfficeBox { get; set; }

        #endregion

        #region [business]

        public string BusinessAddressCity { get; set; }

        public string BusinessAddressCountry { get; set; }

        public string BusinessAddressState { get; set; }

        public string BusinessAddressStreet { get; set; }

        public string BusinessAddressPostOfficeBox { get; set; }

        #endregion

        public string Keyword { get; set; }

        public string Location { get; set; }

        public string CompanyName { get; set; }

        public string Title { get; set; }

        public string DepartmentName { get; set; }

        public string MiddleName { get; set; }

        public string DisplyNamePrefix { get; set; }

        public string Profession { get; set; }

        public string Note { get; set; }

        public string HomeAddress { get; set; }

        public string WorkAddress { get; set; }

        public string OtherAddress { get; set; }

        public DateTime Birthday { get; set; }


    }
}