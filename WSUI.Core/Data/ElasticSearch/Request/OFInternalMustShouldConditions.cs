using OF.Core.Data.ElasticSearch.Request.Base;

namespace OF.Core.Data.ElasticSearch.Request
{
    public class OFInternalMustShouldConditions<T> : OFBaseInternalConditions<T> where T : class
    {

        public OFInternalMustShouldConditions() { }


        public void AddMustCondition(T value)
        {
            AddCondition("must",value);
        }

        public void AddShouldCondiption(T value)
        {
            AddCondition("should",value);
        }
         
    }
}