using System;
using System.Collections.Generic;
using WSUI.Core.Interfaces;
using WSUI.Infrastructure.Implements.Systems;
using WSUI.Infrastructure.Interfaces.Search;

namespace WSUI.Infrastructure.Implements.Contact
{
    public class ContactAttachmentSearching : BaseContactSearching
    {
        protected override ISearchSystem GetPreviewSystem()
        {
            var searchSystem = new ContactAttachmentSearchSystem();
            searchSystem.Init();
            searchSystem.SetProcessingRecordCount(10,0);
            return searchSystem;
        }

        protected override ISearchSystem GetMainSystem()
        {
            var searchSystem = new ContactAttachmentSearchSystem();
            searchSystem.Init();
            return searchSystem;
        }
    }
}