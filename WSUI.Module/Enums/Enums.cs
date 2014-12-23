using WSUI.Core.Core.Attributes;

namespace WSUI.Module.Enums
{
    public enum UiSlideDirection
    {
        DataToPreview,
        PreviewToData
    }

    internal enum PhoneType
    {
        None,
        Business,
        Home,
        Mobile
    }

    public enum AdvancedSearchCriteriaType
    {
        [EnumPrefix("none")]
        None,
        [EnumPrefix("to")]
        To,
        [EnumPrefix("folder")]
        Folder,
        [EnumPrefix("body")]
        Body
    }

}