namespace OF.Core.Data.ElasticSearch
{
    public class OFRecipient : OFElasticSearchBaseEntity
    {
        public string Name { get; set; }

        public string Address { get; set; }

        public string Emailaddresstype { get; set; }

        public override string ToString()
        {
            return string.Format("{0} <{1}>", Name, Address);
        }
    }
}