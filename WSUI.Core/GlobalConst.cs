using WSUI.Core.Enums;

namespace WSUI.Core
{
    public static class GlobalConst
    {
        public const string AdvancedSearchFormat = "{0}:({1}) ";


        private static OutlookVersions currentOutlookVersion = OutlookVersions.None;

        public static OutlookVersions CurrentOutlookVersion
        {
            get 
            { 
                return currentOutlookVersion;
            }
            set
            {
                if (currentOutlookVersion == OutlookVersions.None)
                {
                    currentOutlookVersion = value;
                }
            }
        }
    }
}