using System;
using System.Collections.Generic;
using Microsoft.Practices.Unity;
using OF.Core.Interfaces;
using OF.Infrastructure.Implements.Systems;
using OF.Infrastructure.Interfaces.Search;

namespace OF.Infrastructure.Implements.Contact
{
    public class ContactEmailSearching : BaseContactSearching
    {
        public ContactEmailSearching(object Lock) : base(Lock)
        {
        }

        protected override ISearchSystem GetPreviewSystem(IUnityContainer container)
        {
            var searchsystem = new ContactEmailSearchSystem(LockObject);
            searchsystem.Init(container);
            searchsystem.SetProcessingRecordCount(30,0);
            return searchsystem;
        }

        protected override ISearchSystem GetMainSystem(IUnityContainer container)
        {
            var searchSystem = new ContactEmailSearchSystem(LockObject);
            searchSystem.Init(container);
            return searchSystem;
        }
    }
}