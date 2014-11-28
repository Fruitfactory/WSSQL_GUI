using System;
using System.Collections.Generic;
using WSUI.Core.Interfaces;
using WSUI.Infrastructure.Implements.Systems;
using WSUI.Infrastructure.Interfaces.Search;

namespace WSUI.Infrastructure.Implements.Contact
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