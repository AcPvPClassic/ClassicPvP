using System.Collections.Generic;

using ACE.Entity;
using ACE.Entity.Enum;
using ACE.Server.Entity;
using ACE.Server.Managers;
using ACE.Server.Network.GameMessages.Messages;

namespace ACE.Server.WorldObjects
{
    public partial class Player
    {
        public enum ZergEntryStatus
        {
            Allowed,
            SameAllegianceLimitReached
        }

        public class ZergEntryResult
        {
            public ZergEntryStatus Status { get; set; }

            public bool Success => Status == ZergEntryStatus.Allowed;

            public bool LimitReached => Status == ZergEntryStatus.SameAllegianceLimitReached;

            public bool Failure => !Success;
        }

        /// <summary>
        /// Returns true if newPosition is inside a ZergControlArea that this player should be
        /// evaluated against.  Admins and sentinels are exempt.
        /// </summary>
        public bool IsInZergRestrictedEntry(Position newPosition, out ZergControlArea area)
        {
            area = null;

            if (newPosition == null)
                return false;

            if (IsAdmin || IsSentinel)
                return false;

            area = ZergControlLandblocks.GetLandblockZergControlArea(newPosition.Landblock);

            return area != null;
        }

        /// <summary>
        /// Evaluates whether this player is allowed to enter the given ZergControlArea, based on
        /// how many players of the player's own allegiance are already inside the area.
        /// </summary>
        public ZergEntryResult EvaluateZergEntry(ZergControlArea area, Position newPosition)
        {
            var allegiance = AllegianceManager.GetAllegiance(this);

            // Players with no allegiance can't exceed a per-allegiance limit.
            if (allegiance == null || !allegiance.MonarchId.HasValue)
                return new ZergEntryResult { Status = ZergEntryStatus.Allowed };

            // Check if the player's allegiance already has the maximum number of players in this area.
            var sameAllegCount = CountPlayersInAreaWithAllegiance(area, allegiance.MonarchId.Value);

            if (sameAllegCount >= area.MaxPlayersPerAllegiance)
                return new ZergEntryResult { Status = ZergEntryStatus.SameAllegianceLimitReached };

            return new ZergEntryResult { Status = ZergEntryStatus.Allowed };
        }

        private int CountPlayersInAreaWithAllegiance(ZergControlArea area, uint monarchId)
        {
            var players = new HashSet<Player>();

            foreach (var landblockId in area.AreaLandblockIds)
            {
                var landblock = LandblockManager.GetLandblock(new LandblockId(landblockId << 16), false);
                foreach (var p in landblock.GetCurrentLandblockPlayers())
                {
                    var alleg = AllegianceManager.GetAllegiance(p);

                    if (alleg?.MonarchId == monarchId)
                        players.Add(p);
                }
            }

            return players.Count;
        }

        /// <summary>
        /// Side effect for a failed ZergControl entry: notify the player why they were turned away.
        /// (The caller is responsible for actually relocating them to their lifestone.)
        /// </summary>
        public void HandleZergEntryFailure(ZergEntryResult result, ZergControlArea zergArea, Position pos)
        {
            var allegiance = AllegianceManager.GetAllegiance(this);
            var playerAllegName = allegiance?.Monarch?.Player?.Name ?? "Unknown";

            if (result.Status == ZergEntryStatus.SameAllegianceLimitReached)
            {
                Session.Network.EnqueueSend(
                    new GameMessageSystemChat($"You have attempted to enter a zerg restricted area.  {playerAllegName} already has {zergArea.MaxPlayersPerAllegiance} players in this area, which is the maximum allowed per allegiance.  You have been redirected to your lifestone.", ChatMessageType.System));
            }
        }
    }
}
