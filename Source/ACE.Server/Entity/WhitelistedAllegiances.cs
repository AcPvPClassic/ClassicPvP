using System;
using System.Collections.Generic;

using ACE.Server.Managers;

namespace ACE.Server.Entity
{
    /// <summary>
    /// Maintains the runtime-cached list of whitelisted MonarchIDs.
    /// The list is stored in the "whitelisted_allegiances" server config as a
    /// comma-separated string of uint character IDs and re-evaluated whenever
    /// the config value changes.
    /// </summary>
    public static class WhitelistedAllegiances
    {
        private static List<int> _allegianceList;
        private static string _allegianceListString = string.Empty;

        public static List<int> AllowedAllegianceList
        {
            get
            {
                try
                {
                    var configString = PropertyManager.GetString("whitelisted_allegiances").Item ?? string.Empty;

                    if (_allegianceList == null || !configString.Equals(_allegianceListString))
                    {
                        _allegianceListString = configString;
                        _allegianceList = new List<int>();

                        foreach (var entry in configString.Split(','))
                        {
                            if (int.TryParse(entry.Trim(), out int monarchId))
                                _allegianceList.Add(monarchId);
                        }
                    }
                }
                catch (Exception)
                {
                    return new List<int>();
                }

                return _allegianceList;
            }
        }

        public static bool IsAllowedAllegiance(int monarchId)
        {
            return AllowedAllegianceList.Contains(monarchId);
        }
    }
}
