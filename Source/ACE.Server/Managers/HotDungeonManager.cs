using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ACE.Common;
using ACE.Common.Extensions;
using ACE.Database;
using ACE.Server.Network.GameMessages.Messages;
using ACE.Entity.Enum;

using log4net;

namespace ACE.Server.Managers
{
    public class HotDungeonEntry
    {
        public ushort Landblock    { get; set; }
        public int    MinLevel     { get; set; }
        public int    MaxLevel     { get; set; }
        public float  XpMultiplier { get; set; }
        public float  BoxDropChance { get; set; }
    }

    public class ActiveHotDungeon
    {
        public ushort Landblock     { get; set; }
        public string Name          { get; set; }
        public string Directions    { get; set; }
        public float  XpMultiplier  { get; set; }
        public float  BoxDropChance { get; set; }
        public double ExpiresAt     { get; set; }
        public double NextAnnounceAt { get; set; }
    }

    public static class HotDungeonManager
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public const uint PhialOfBloodyTearsWcid = 1000003;
        public const uint BoxWcid = 510000;

        private const int    MaxActive            = 3;
        private const double MinDurationSeconds   = 86400;   // 24 h
        private const double MaxDurationSeconds   = 172800;  // 48 h
        private const double MinRollIntervalSecs  = 43200;   // 12 h
        private const double MaxRollIntervalSecs  = 129600;  // 36 h
        private const double AnnounceIntervalSecs = 3600;    // 60 min

        // ── Hardcoded dungeon pool ────────────────────────────────────────────
        // TODO: Replace placeholder entries with real dungeon data before launch.
        // Landblock  = upper-16-bit landblock ID (matches LandblockId.Landblock)
        // MinLevel   = minimum current level-cap required for this dungeon to be eligible
        // MaxLevel   = maximum current level-cap (0 = no upper limit)
        // XpMultiplier = multiplied against kill XP while dungeon is hot (e.g. 1.5 = 1.5x)
        // BoxDropChance = per-kill probability (0.0–1.0) that a monster drops A Box
        public static readonly List<HotDungeonEntry> PossibleDungeons = new List<HotDungeonEntry>
        {
            new HotDungeonEntry { Landblock = 0x019E, MinLevel =  1, MaxLevel =  30, XpMultiplier = 2.5f, BoxDropChance = 0.05f }, //Drudge Hideout
            new HotDungeonEntry { Landblock = 0x0162, MinLevel =  1, MaxLevel =  30, XpMultiplier = 2.5f, BoxDropChance = 0.05f }, //Cave of Alabree
            new HotDungeonEntry { Landblock = 0x0153, MinLevel =  1, MaxLevel =  30, XpMultiplier = 2.5f, BoxDropChance = 0.05f }, //Shreth Hive
            new HotDungeonEntry { Landblock = 0x0156, MinLevel =  1, MaxLevel =  30, XpMultiplier = 2.5f, BoxDropChance = 0.05f }, //Sea Temple Catacombs
            new HotDungeonEntry { Landblock = 0x0163, MinLevel =  1, MaxLevel =  30, XpMultiplier = 2.5f, BoxDropChance = 0.05f }, //Holtburg Redoubt
            new HotDungeonEntry { Landblock = 0x0158, MinLevel =  1, MaxLevel =  30, XpMultiplier = 2.5f, BoxDropChance = 0.05f }, //Lytaway Dungeon
            new HotDungeonEntry { Landblock = 0x0154, MinLevel =  1, MaxLevel =  30, XpMultiplier = 2.5f, BoxDropChance = 0.05f }, //Stone Cathedral
            new HotDungeonEntry { Landblock = 0x02C6, MinLevel =  1, MaxLevel =  30, XpMultiplier = 2.5f, BoxDropChance = 0.05f }, //Lightless Tunnels
            new HotDungeonEntry { Landblock = 0x5749, MinLevel =  1, MaxLevel =  30, XpMultiplier = 2.5f, BoxDropChance = 0.05f }, //Tusker Lodge

            new HotDungeonEntry { Landblock = 0x01D7, MinLevel =  10, MaxLevel =  40, XpMultiplier = 2.5f, BoxDropChance = 0.05f }, //Sylsfear
            new HotDungeonEntry { Landblock = 0x013B, MinLevel =  10, MaxLevel =  40, XpMultiplier = 2.5f, BoxDropChance = 0.05f }, //Folthid Cellar
            new HotDungeonEntry { Landblock = 0x01CC, MinLevel =  10, MaxLevel =  40, XpMultiplier = 2.5f, BoxDropChance = 0.05f }, //Halls of the Helm
            new HotDungeonEntry { Landblock = 0x01B4, MinLevel =  10, MaxLevel =  40, XpMultiplier = 2.5f, BoxDropChance = 0.05f }, //Golem Burial Ground
            new HotDungeonEntry { Landblock = 0x017A, MinLevel =  10, MaxLevel =  40, XpMultiplier = 2.5f, BoxDropChance = 0.05f }, //Banderling Hovel

            new HotDungeonEntry { Landblock = 0x5B46, MinLevel =  12, MaxLevel =  40, XpMultiplier = 2.5f, BoxDropChance = 0.05f }, //Tusker Cave
            new HotDungeonEntry { Landblock = 0x5C47, MinLevel =  12, MaxLevel =  40, XpMultiplier = 2.5f, BoxDropChance = 0.05f }, //Tusker Grotto

            new HotDungeonEntry { Landblock = 0x0188, MinLevel =  14, MaxLevel =  40, XpMultiplier = 2.5f, BoxDropChance = 0.05f }, //Mines of Despair

            new HotDungeonEntry { Landblock = 0x02F4, MinLevel =  15, MaxLevel =  50, XpMultiplier = 2.5f, BoxDropChance = 0.05f }, //Halls of Metos (AB)
            new HotDungeonEntry { Landblock = 0x01C1, MinLevel =  15, MaxLevel =  50, XpMultiplier = 2.5f, BoxDropChance = 0.05f }, //Tumerok Mine
            new HotDungeonEntry { Landblock = 0x018D, MinLevel =  15, MaxLevel =  50, XpMultiplier = 2.5f, BoxDropChance = 0.05f }, //Olthoi Tunnels
            new HotDungeonEntry { Landblock = 0x5C43, MinLevel =  15, MaxLevel =  50, XpMultiplier = 2.5f, BoxDropChance = 0.05f }, //Tusker Cavern
            new HotDungeonEntry { Landblock = 0x0140, MinLevel =  15, MaxLevel =  50, XpMultiplier = 2.5f, BoxDropChance = 0.05f }, //Virindi Fort

            new HotDungeonEntry { Landblock = 0x01A9, MinLevel =  18, MaxLevel =  50, XpMultiplier = 2.5f, BoxDropChance = 0.05f }, //Ancient Lighthouse

            new HotDungeonEntry { Landblock = 0x02DB, MinLevel =  20, MaxLevel =  50, XpMultiplier = 2.5f, BoxDropChance = 0.05f }, //Fenmalain Vestibule
            new HotDungeonEntry { Landblock = 0x5772, MinLevel =  20, MaxLevel =  50, XpMultiplier = 2.5f, BoxDropChance = 0.05f }, //Olthoi Brood Hive (20+)


            new HotDungeonEntry { Landblock = 0x5E4A, MinLevel =  60, MaxLevel =  130, XpMultiplier = 2.5f, BoxDropChance = 0.05f }, //Olthoi Brood Hive (60+)






        };

        public static List<ActiveHotDungeon> ActiveDungeons { get; } = new List<ActiveHotDungeon>();

        private static double _nextRollAt = 0;

        public static void Initialize()
        {
            // Roll the first dungeon 30–180 minutes after server start so players
            // don't have to wait a full 12+ hours on a fresh boot.
            _nextRollAt = Time.GetFutureUnixTime(ThreadSafeRandom.Next(1800f, 10800f));
            log.Info($"HotDungeonManager: first roll scheduled in {TimeSpan.FromSeconds(_nextRollAt - Time.GetUnixTime()).GetFriendlyString()}.");
        }

        public static void Tick(double currentUnixTime)
        {
            if (!PropertyManager.GetBool("hot_dungeon_enabled").Item)
                return;

            if (Common.ConfigManager.Config.Server.WorldRuleset != Common.Ruleset.Infiltration)
                return;

            // Expire dungeons and re-announce active ones
            for (int i = ActiveDungeons.Count - 1; i >= 0; i--)
            {
                var d = ActiveDungeons[i];
                if (d.ExpiresAt <= currentUnixTime)
                {
                    ActiveDungeons.RemoveAt(i);
                    Broadcast($"{d.Name} is no longer a Hot Dungeon.");
                    log.Info($"HotDungeonManager: {d.Name} (0x{d.Landblock:X4}) expired.");
                    continue;
                }

                if (d.NextAnnounceAt <= currentUnixTime)
                {
                    d.NextAnnounceAt = currentUnixTime + AnnounceIntervalSecs;
                    var remaining = TimeSpan.FromSeconds(d.ExpiresAt - currentUnixTime).GetFriendlyString();
                    Broadcast($"{d.Name} is still a Hot Dungeon! {d.XpMultiplier:0.##}x XP — {remaining} remaining. The entrance is located {d.Directions}!");
                }
            }

            // Try to select a new dungeon if under the cap and the roll timer has fired
            if (ActiveDungeons.Count >= MaxActive || _nextRollAt > currentUnixTime)
                return;

            RollOneDungeon(currentUnixTime);
        }

        private static void RollOneDungeon(double currentUnixTime)
        {
            var levelCap = GetCurrentLevelCap();

            var eligible = PossibleDungeons
                .Where(e => levelCap == 0 || (levelCap >= e.MinLevel && (e.MaxLevel == 0 || levelCap <= e.MaxLevel)))
                .Where(e => ActiveDungeons.All(a => a.Landblock != e.Landblock))
                .ToList();

            if (eligible.Count == 0)
            {
                ScheduleNextRoll(currentUnixTime);
                log.Debug("HotDungeonManager: no eligible dungeons for current level cap, rescheduling.");
                return;
            }

            var entry = eligible[ThreadSafeRandom.Next(0, eligible.Count - 1)];

            string name, directions;
            var lb = DatabaseManager.World.GetLandblockDescriptionsByLandblock(entry.Landblock).FirstOrDefault();
            if (lb != null)
            {
                name = lb.Name;
                if (!string.IsNullOrEmpty(lb.MicroRegion))
                    directions = $"{lb.Directions} {lb.Reference} in {lb.MicroRegion}";
                else if (!string.IsNullOrEmpty(lb.MacroRegion) && lb.MacroRegion != "Dereth")
                    directions = $"{lb.Directions} {lb.Reference} in {lb.MacroRegion}";
                else
                    directions = $"{lb.Directions} {lb.Reference}";
            }
            else
            {
                name       = $"Unknown Dungeon (0x{entry.Landblock:X4})";
                directions = "at an unknown location";
            }

            var duration  = ThreadSafeRandom.Next((float)MinDurationSeconds, (float)MaxDurationSeconds);
            var expiresAt = currentUnixTime + duration;

            var active = new ActiveHotDungeon
            {
                Landblock      = entry.Landblock,
                Name           = name,
                Directions     = directions,
                XpMultiplier   = entry.XpMultiplier,
                BoxDropChance  = entry.BoxDropChance,
                ExpiresAt      = expiresAt,
                NextAnnounceAt = currentUnixTime + AnnounceIntervalSecs,
            };

            ActiveDungeons.Add(active);
            ScheduleNextRoll(currentUnixTime);

            var durationStr = TimeSpan.FromSeconds(duration).GetFriendlyString();
            Broadcast($"{name} is now a Hot Dungeon! {entry.XpMultiplier:0.##}x XP for the next {durationStr}! The entrance is located {directions}!");
            log.Info($"HotDungeonManager: {name} (0x{entry.Landblock:X4}) is now hot for {durationStr} at {entry.XpMultiplier:0.##}x XP.");
        }

        private static void ScheduleNextRoll(double currentUnixTime)
        {
            var delay = ThreadSafeRandom.Next((float)MinRollIntervalSecs, (float)MaxRollIntervalSecs);
            _nextRollAt = currentUnixTime + delay;
        }

        private static int GetCurrentLevelCap()
        {
            var xpCap = PropertyManager.GetLong("rolling_xp_cap").Item;
            return xpCap > 0 ? RollingLevelCapManager.GetCurrentLevelCap(xpCap) : 0;
        }

        public static bool IsHotDungeon(ushort landblock, out ActiveHotDungeon hotDungeon)
        {
            foreach (var d in ActiveDungeons)
            {
                if (d.Landblock == landblock)
                {
                    hotDungeon = d;
                    return true;
                }
            }
            hotDungeon = null;
            return false;
        }

        /// <summary>
        /// Returns a formatted status string for the /hotdungeons player command.
        /// </summary>
        public static string GetStatusMessage()
        {
            if (ActiveDungeons.Count == 0)
                return "There are no Hot Dungeons active at this time.";

            var now = Time.GetUnixTime();
            var sb  = new StringBuilder();
            sb.AppendLine($"Active Hot Dungeons ({ActiveDungeons.Count}/{MaxActive}):");
            foreach (var d in ActiveDungeons)
            {
                var remaining = TimeSpan.FromSeconds(d.ExpiresAt - now).GetFriendlyString();
                sb.AppendLine($"  {d.Name} — {d.XpMultiplier:0.##}x XP — {remaining} remaining — {d.Directions}");
            }
            return sb.ToString().TrimEnd();
        }

        public static void AdminForceRoll()
        {
            RollOneDungeon(Time.GetUnixTime());
        }

        public static void AdminForceLandblock(ushort landblock)
        {
            if (ActiveDungeons.Any(a => a.Landblock == landblock))
                return;

            var entry = PossibleDungeons.FirstOrDefault(e => e.Landblock == landblock);
            float xpMul      = entry?.XpMultiplier  ?? 1.5f;
            float boxChance  = entry?.BoxDropChance  ?? 0.05f;

            var now      = Time.GetUnixTime();
            var duration = ThreadSafeRandom.Next((float)MinDurationSeconds, (float)MaxDurationSeconds);

            string name, directions;
            var lb = DatabaseManager.World.GetLandblockDescriptionsByLandblock(landblock).FirstOrDefault();
            if (lb != null)
            {
                name = lb.Name;
                directions = !string.IsNullOrEmpty(lb.MicroRegion)
                    ? $"{lb.Directions} {lb.Reference} in {lb.MicroRegion}"
                    : (!string.IsNullOrEmpty(lb.MacroRegion) && lb.MacroRegion != "Dereth"
                        ? $"{lb.Directions} {lb.Reference} in {lb.MacroRegion}"
                        : $"{lb.Directions} {lb.Reference}");
            }
            else
            {
                name       = $"Unknown Dungeon (0x{landblock:X4})";
                directions = "at an unknown location";
            }

            var active = new ActiveHotDungeon
            {
                Landblock      = landblock,
                Name           = name,
                Directions     = directions,
                XpMultiplier   = xpMul,
                BoxDropChance  = boxChance,
                ExpiresAt      = now + duration,
                NextAnnounceAt = now + AnnounceIntervalSecs,
            };

            ActiveDungeons.Add(active);
            var durationStr = TimeSpan.FromSeconds(duration).GetFriendlyString();
            Broadcast($"{name} is now a Hot Dungeon! {xpMul:0.##}x XP for the next {durationStr}! The entrance is located {directions}!");
            log.Info($"HotDungeonManager: admin forced {name} (0x{landblock:X4}) hot for {durationStr}.");
        }

        public static void AdminProlong()
        {
            var now = Time.GetUnixTime();
            foreach (var d in ActiveDungeons)
            {
                d.ExpiresAt      += 3600;
                d.NextAnnounceAt  = now + AnnounceIntervalSecs;
            }
            if (ActiveDungeons.Count > 0)
                Broadcast("All active Hot Dungeons have been extended by 1 hour.");
        }

        private static void Broadcast(string msg)
        {
            PlayerManager.BroadcastToAll(new GameMessageSystemChat(msg, ChatMessageType.WorldBroadcast));
            PlayerManager.LogBroadcastChat(Channel.AllBroadcast, null, msg);
        }
    }
}
