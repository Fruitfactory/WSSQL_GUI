using OF.Core.Core.Attributes;

namespace OF.Core.Enums
{
    public enum AdvancedSearchCriteriaType
    {
        [OFEnumPrefix("none")]
        None,
        [OFEnumPrefix("to")]
        To,
        [OFEnumPrefix("folder")]
        Folder,
        [OFEnumPrefix("body")]
        Body,
        [OFEnumPrefix("sortby")]
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
        Finished,
        Suspended
    }

    public enum ofRuleType
    {
        None,
        Quote,
        Word,
        Amount,
        Price
    }

    public enum ofUserActivityState
    {
        Unknown,
        Online,
        Idle,
        Away,
        Night
    }

    public enum ofServiceApplicationMessageType
    {
        None, 
        StartIndexing,
        ForceIndexing,
        ControllerStatus,
        OfPluginState
    }

    public enum ofServerResponseStatus
    {
        None, 
        Ok,
        Failed
    }

}