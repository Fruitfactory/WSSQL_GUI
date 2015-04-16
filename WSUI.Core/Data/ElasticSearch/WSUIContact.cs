using System;
using Elasticsearch.Net;
using Nest;

namespace OF.Core.Data.ElasticSearch
{
    [ElasticType(Name = "contact")]
    public class OFContact : OFElasticSearchBaseEntity
    {
        public string Firstname { get; set; }
        
        public string Lastname { get; set; }
        
        public string Emailaddress1 { get; set; }
        
        public string Emailaddress2 { get; set; }
        
        public string Emailaddress3 { get; set; }

        public string Businesstelephone { get; set; }
        public string Hometelephone { get; set; }

        public string Mobiletelephone { get; set; }

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

        public string Addresstype { get; set; }


    }
}