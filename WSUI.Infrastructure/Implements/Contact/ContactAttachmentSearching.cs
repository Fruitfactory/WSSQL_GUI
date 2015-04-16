using System;
using System.Collections.Generic;
using OF.Core.Interfaces;
using OF.Infrastructure.Implements.Systems;
using OF.Infrastructure.Interfaces.Search;

namespace OF.Infrastructure.Implements.Contact
{
    public class ContactAttachmentSearching : BaseContactSearching
    {
        public ContactAttachmentSearching(object Lock) : base(Lock)
        {
        }

        protected override ISearchSystem GetPreviewSystem()
        {
            var searchSystem = new ContactAttachmentSearchSystem(LockObject);
            searchSystem.Init();
            searchSystem.SetProcessingRecordCount(10,0);
            return searchSystem;
        }

        protected override ISearchSystem GetMainSystem()
        {
            var searchSystem = new ContactAttachmentSearchSystem(LockObject);
            searchSystem.Init();
            return searchSystem;
        }
    }
}