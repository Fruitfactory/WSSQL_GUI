using System;
using System.Collections.Generic;
using Microsoft.Practices.Unity;
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

        protected override ISearchSystem GetPreviewSystem(IUnityContainer container)
        {
            var searchSystem = new ContactAttachmentSearchSystem(LockObject);
            searchSystem.Init(container);
            searchSystem.SetProcessingRecordCount(10,0);
            return searchSystem;
        }

        protected override ISearchSystem GetMainSystem(IUnityContainer container)
        {
            var searchSystem = new ContactAttachmentSearchSystem(LockObject);
            searchSystem.Init(container);
            return searchSystem;
        }
    }
}