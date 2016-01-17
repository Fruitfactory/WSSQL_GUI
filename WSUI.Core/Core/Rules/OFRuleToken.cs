using OF.Core.Enums;

namespace OF.Core.Core.Rules
{
    public class OFRuleToken
    {
        public OFRuleToken(ofRuleType type)
        {
            Type = type;
        }

        public string Result { get; set; }

        public ofRuleType Type { get; private set; }
     
    }
}