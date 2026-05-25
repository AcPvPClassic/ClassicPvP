using System;
using System.Collections.Generic;
using System.Linq;

using ACE.Server.Managers;

namespace ACE.Server.Entity.TownControl
{
    /// <summary>
    /// Maintains the whitelist of allegiance monarch GUIDs that are permitted to
    /// initiate town control conflicts.  The list is read from the
    /// "town_control_alleglist" PropertyManager string (comma-separated uint values)
    /// and re-parsed whenever it changes.
    /// </summary>
    public static class TownControlAllegiances
    {
        private static List<uint> _allowed = new List<uint>();
        private static string _lastRaw = string.Empty;

        public static IReadOnlyList<uint> AllowedAllegianceList
        {
            get
            {
                try
                {
                    var raw = PropertyManager.GetString("town_control_alleglist").Item ?? string.Empty;
                    if (!raw.Equals(_lastRaw, StringComparison.Ordinal))
                    {
                        _lastRaw = raw;
                        var parsed = new List<uint>();
                        foreach (var part in raw.Split(','))
                        {
                            if (uint.TryParse(part.Trim(), out uint id))
                                parsed.Add(id);
                        }
                        _allowed = parsed;
                    }
                }
                catch
                {
                    return new List<uint>();
                }
                return _allowed;
            }
        }

        public static bool IsAllowedAllegiance(uint monarchId) => AllowedAllegianceList.Contains(monarchId);
    }
}
