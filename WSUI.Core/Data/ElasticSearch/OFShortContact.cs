using Nest;

namespace OF.Core.Data.ElasticSearch
{
    [ElasticsearchType(Name = "shortcontact")]    
    public class OFShortContact
    {
        public string Name { get; set; }

        public string Email { get; set; }

        public override string ToString()
        {
            return $"{Name} - {Email}";
        }
    }
}