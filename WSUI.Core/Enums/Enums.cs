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
}