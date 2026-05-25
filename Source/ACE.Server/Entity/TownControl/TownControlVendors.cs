using System.Collections.Generic;

namespace ACE.Server.Entity.TownControl
{
    public class TownControlVendor
    {
        public uint   WeenieId { get; set; }
        public ushort TownId   { get; set; }
        public string TownName { get; set; }
    }

    public static class TownControlVendors
    {
        private static readonly Dictionary<uint, TownControlVendor> _map = new Dictionary<uint, TownControlVendor>
        {
            [42128707] = new TownControlVendor { WeenieId = 42128707, TownId = 91,  TownName = "Shoushi"  },
            [42128708] = new TownControlVendor { WeenieId = 42128708, TownId = 72,  TownName = "Holtburg" },
            [42128709] = new TownControlVendor { WeenieId = 42128709, TownId = 102, TownName = "Yaraq"    },
        };

        public static bool IsTownControlVendor(uint weenieId) => _map.ContainsKey(weenieId);

        public static TownControlVendor Get(uint weenieId) => _map.TryGetValue(weenieId, out var v) ? v : null;
    }
}
