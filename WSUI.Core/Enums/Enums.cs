using WSUI.Core.Core.Attributes;

namespace WSUI.Core.Enums
{
    public enum AdvancedSearchCriteriaType
    {
        [EnumPrefix("none")]
        None,
        [EnumPrefix("to")]
        To,
        [EnumPrefix("folder")]
        Folder,
        [EnumPrefix("body")]
        Body,
        [EnumPrefix("sortby")]
        SortBy
    }

    public enum AdvancedSearchSortByType
    {
        Relevance,
        NewestToOldest,
        OldestToNewest
    }

    public enum OutlookVersions
    {
        None = 0,
        Outlook2007 = 12,
        Outlook2010 = 14,
        Otlook2013 = 15
    }

    public enum PstReaderStatus
    {
        None,
        NonStarted,
        Busy,
        Finished
    }
}