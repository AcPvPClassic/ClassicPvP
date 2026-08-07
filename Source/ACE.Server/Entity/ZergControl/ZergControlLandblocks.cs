using System.Collections.Generic;

namespace ACE.Server.Entity
{
    /// <summary>
    /// Defines "zerg control" areas.  A ZergControlArea is a group of one or more landblocks
    /// that share a single per-allegiance player cap.  Enforcement happens in two places:
    ///   - On a landblock tick (Landblock.HandleZergControl): every player across all landblocks
    ///     in the area is bucketed by allegiance, and any allegiance over its cap has its excess
    ///     players (the most recently teleported ones) booted to their lifestone.
    ///   - When a player teleports into / logs into one of these landblocks
    ///     (Player.IsInZergRestrictedEntry / Player.EvaluateZergEntry): the player is blocked if
    ///     their own allegiance is already at the cap.
    ///
    /// NOTE: every landblock listed in a ZergControlArea's AreaLandblockIds must ALSO be added as
    /// a key in the map (pointing at the same area) so a tick on any of them enforces the whole area.
    /// </summary>
    public static class ZergControlLandblocks
    {
        private static Dictionary<uint, ZergControlArea> _zergControlLandblocksMap;

        public static Dictionary<uint, ZergControlArea> ZergControlLandblocksMap
        {
            get
            {
                if (_zergControlLandblocksMap == null)
                {
                    _zergControlLandblocksMap = new Dictionary<uint, ZergControlArea>();

                    var subway = new ZergControlArea();
                    subway.MaxPlayersPerAllegiance = 9;
                    subway.AreaLandblockIds = new uint[] { 0x01C9 };
                    _zergControlLandblocksMap.Add(0x01C9, subway);
                }

                return _zergControlLandblocksMap;
            }
        }

        public static bool IsZergControlLandblock(uint landblockId)
        {
            return ZergControlLandblocksMap.ContainsKey(landblockId);
        }

        public static ZergControlArea GetLandblockZergControlArea(uint landblockId)
        {
            if (IsZergControlLandblock(landblockId))
            {
                return ZergControlLandblocksMap[landblockId];
            }

            return null;
        }
    }

    public class ZergControlArea
    {
        /// <summary>
        /// All landblocks that make up this area.  Players across every one of these landblocks
        /// are counted together against MaxPlayersPerAllegiance.
        /// </summary>
        public uint[] AreaLandblockIds;

        /// <summary>
        /// The maximum number of players a single allegiance may have inside this area.
        /// </summary>
        public uint MaxPlayersPerAllegiance;
    }
}
