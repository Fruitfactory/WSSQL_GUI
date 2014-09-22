using System;
using System.Collections.Generic;
using System.Linq;
using WSUI.Core.Data;
using WSUI.Infrastructure.Implements.Rules.BaseRules;

namespace WSUI.Infrastructure.Implements.Rules
{
    public class AttachmentFilenameSearchRule : BaseAttachmentSearchRule
    {
        public AttachmentFilenameSearchRule()
            : base()
        {
            CreateInit();
        }

        public AttachmentFilenameSearchRule(object lockObject)
            : base(lockObject)
        {
            CreateInit();
        }

        private void CreateInit()
        {
            Priority = 6;
        }

        protected override string GetSearchProperty()
        {
            return "System.ItemUrl";
        }

        public override void Init()
        {
            RuleName = "Attachment (Filename)";
            base.Init();
        }

        protected override IEnumerable<AttachmentSearchObject> GetSortedAttachmentSearchObjects(IEnumerable<AttachmentSearchObject> list)
        {
            var words = Query.Split(' ');
            return list.OrderBy(d => GetMinContainsIndex(d.ItemNameDisplay, words));
        }

        private int GetMinContainsIndex(string itemName, IEnumerable<string> words)
        {
            if (string.IsNullOrEmpty(itemName) || words == null)
                return int.MaxValue;
            int min = words.Min(w => itemName.IndexOf(w, StringComparison.InvariantCultureIgnoreCase));
            System.Diagnostics.Debug.WriteLine(string.Format("Min: {0}", min));
            return min == -1 ? int.MaxValue : min;
        }
    }
}