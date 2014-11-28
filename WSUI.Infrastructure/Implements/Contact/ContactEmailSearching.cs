using System;
using System.Collections.Generic;
using WSUI.Core.Interfaces;
using WSUI.Infrastructure.Implements.Systems;
using WSUI.Infrastructure.Interfaces.Search;

namespace WSUI.Infrastructure.Implements.Contact
{
    public class ContactEmailSearching : BaseContactSearching
    {
        protected override ISearchSystem GetPreviewSystem()
        {
            var searchsystem = new ContactEmailSearchSystem();
            searchsystem.Init();
            searchsystem.SetProcessingRecordCount(30,0);
            return searchsystem;
        }

        protected override ISearchSystem GetMainSystem()
        {
            var searchSystem = new ContactEmailSearchSystem();
            searchSystem.Init();
            return searchSystem;
        }
    }
}