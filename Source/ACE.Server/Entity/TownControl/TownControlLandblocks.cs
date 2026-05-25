using System.Collections.Generic;

namespace ACE.Server.Entity.TownControl
{
    /// <summary>
    /// Static registry of landblock IDs that belong to each Town Control town,
    /// which landblocks award rewards, and which content-generated events map to each landblock.
    /// </summary>
    public static class TownControlLandblocks
    {
        // Landblocks that are part of each town's TC area (keyed by TownId).
        private static readonly Dictionary<ushort, uint[]> _townLandblocks = new Dictionary<ushort, uint[]>
        {
            [72]  = new uint[] { 0xA5B4, 0x4FF1 },                          // Holtburg
            [91]  = new uint[] { 0xDE51, 0xE9F1, 0xE8F1, 0xE9F0 },          // Shoushi
            [102] = new uint[] { 0x8164, 0x00AB },                           // Yaraq
        };

        // Sub-set of the above that qualify for victory rewards.
        private static readonly Dictionary<ushort, uint[]> _rewardLandblocks = new Dictionary<ushort, uint[]>
        {
            [72]  = new uint[] { 0x4FF1 },                                   // Holtburg
            [91]  = new uint[] { 0xE9F0, 0xE9F1, 0xE8F1 },                  // Shoushi
            [102] = new uint[] { 0x00AB },                                   // Yaraq
        };

        // Content-generated event name for each landblock.
        private static readonly Dictionary<uint, string> _landblockEvents = new Dictionary<uint, string>
        {
            [0xDE51] = "towncontrol3",  // Shoushi
            [0xE9F1] = "towncontrol3",
            [0xE8F1] = "towncontrol3",
            [0xE9F0] = "towncontrol3",
            [0x8164] = "towncontrol1",  // Yaraq
            [0x00AB] = "towncontrol1",
            [0xA5B4] = "towncontrol2",  // Holtburg
            [0x4FF1] = "towncontrol2",
        };

        // Cached flat lists — built lazily.
        private static HashSet<uint> _allTcLandblocks;
        private static HashSet<uint> _allRewardLandblocks;

        private static HashSet<uint> AllTcLandblocks
        {
            get
            {
                if (_allTcLandblocks == null)
                {
                    _allTcLandblocks = new HashSet<uint>();
                    foreach (var arr in _townLandblocks.Values)
                        foreach (var lb in arr)
                            _allTcLandblocks.Add(lb);
                }
                return _allTcLandblocks;
            }
        }

        private static HashSet<uint> AllRewardLandblocks
        {
            get
            {
                if (_allRewardLandblocks == null)
                {
                    _allRewardLandblocks = new HashSet<uint>();
                    foreach (var arr in _rewardLandblocks.Values)
                        foreach (var lb in arr)
                            _allRewardLandblocks.Add(lb);
                }
                return _allRewardLandblocks;
            }
        }

        public static bool IsTownControlLandblock(uint landblockId)       => AllTcLandblocks.Contains(landblockId);
        public static bool IsTownControlRewardLandblock(uint landblockId) => AllRewardLandblocks.Contains(landblockId);

        public static ushort? GetTownIdByLandblockId(uint landblockId)
        {
            foreach (var kv in _townLandblocks)
                foreach (var lb in kv.Value)
                    if (lb == landblockId)
                        return kv.Key;
            return null;
        }

        /// <summary>Returns the reward landblock ID array for a town, or null if not found.</summary>
        public static uint[] GetRewardLandblocks(ushort townId)
        {
            _rewardLandblocks.TryGetValue(townId, out var arr);
            return arr;
        }

        public static bool TryGetEventName(uint landblockId, out string eventName)
            => _landblockEvents.TryGetValue(landblockId, out eventName);
    }
}
