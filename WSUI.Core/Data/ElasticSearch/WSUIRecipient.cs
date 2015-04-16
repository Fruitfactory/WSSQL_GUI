namespace OF.Core.Data.ElasticSearch
{
    public class OFRecipient : OFElasticSearchBaseEntity
    {
        public string Name { get; set; }

        public string Address { get; set; }

        public string Emailaddresstype { get; set; }

    }
}