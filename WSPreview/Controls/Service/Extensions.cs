using System.Linq;
using HtmlAgilityPack;

namespace GDIDraw.Service
{
    public static class Extensions
    {
        #region [public const]

        public const string LinkTag = "a";

        #endregion

        #region [private const]

        private const string TargetAttributeName = "target";

        #endregion

        public static void RemoveTargetFromTagA(this HtmlNode node)
        {
            if (node == null)
                return;
            var attribute = node.Attributes.FirstOrDefault(attr => attr.Name == TargetAttributeName);
            if (attribute != null)
            {
                node.Attributes.Remove(attribute);
            }
        }

    }
}