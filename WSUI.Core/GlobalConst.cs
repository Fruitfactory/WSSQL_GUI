using OF.Core.Enums;

namespace OF.Core
{
    public static class GlobalConst
    {
        public static readonly string AdvancedSearchFormat = "{0}:({1}) ";


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

        public static readonly string ExchangeEmailType = "EX";

        public static readonly string SettingsRiverFile = "riversettings";

        public static readonly string RiverMetaMutex = "RiverMetaMutex";

    }
}