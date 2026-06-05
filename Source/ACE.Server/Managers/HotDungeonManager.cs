using ACE.Common;
using ACE.Common.Extensions;
using ACE.Database;
using ACE.Entity.Enum;
using ACE.Server.Network.GameMessages.Messages;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ACE.Server.Managers
{
    public class HotDungeonEntry
    {
        public ushort Landblock    { get; set; }
        public string Name         { get; set; }
        public int    MinLevel     { get; set; }
        public int    MaxLevel     { get; set; }
        public float  XpMultiplier { get; set; }
        public float  BoxDropChance { get; set; }
    }

    public class ActiveHotDungeon
    {
        public ushort Landblock     { get; set; }
        public string Name          { get; set; }
        public float  XpMultiplier  { get; set; }
        public float  BoxDropChance { get; set; }
        public double ExpiresAt     { get; set; }
        public double NextAnnounceAt { get; set; }
    }

    public static class HotDungeonManager
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private const int    MaxActive            = 3;
        private const double MinDurationSeconds   = 86400;   // 24 h
        private const double MaxDurationSeconds   = 172800;  // 48 h
        private const double MinRollIntervalSecs  = 43200;   // 12 h
        private const double MaxRollIntervalSecs  = 129600;  // 36 h
        private const double AnnounceIntervalSecs = 3600;    // 60 min

        // ── Hardcoded dungeon pool ────────────────────────────────────────────
        // Landblock    = upper-16-bit landblock ID (matches LandblockId.Landblock)
        // Name         = display name shown in global broadcasts and /hotdungeons
        // MinLevel     = minimum current level-cap for this dungeon to be eligible
        // MaxLevel     = maximum current level-cap (0 = no upper limit)
        // XpMultiplier = multiplied against kill XP while dungeon is hot (e.g. 1.5 = 1.5x)
        // BoxDropChance = per-kill probability (0.0–1.0) that a monster drops A Box
        public static readonly List<HotDungeonEntry> PossibleDungeons = new List<HotDungeonEntry>
        {
            new HotDungeonEntry { Landblock = 0x019E, Name = "Drudge Hideout",                  MinLevel =  1, MaxLevel =  35, XpMultiplier = 4.0f, BoxDropChance = 0.03f },
            new HotDungeonEntry { Landblock = 0x0162, Name = "Cave of Alabree",                 MinLevel =  1, MaxLevel =  35, XpMultiplier = 3.5f, BoxDropChance = 0.03f },
            new HotDungeonEntry { Landblock = 0x0153, Name = "Shreth Hive",                     MinLevel =  1, MaxLevel =  35, XpMultiplier = 3.5f, BoxDropChance = 0.03f },
            new HotDungeonEntry { Landblock = 0x0156, Name = "Sea Temple Catacombs",            MinLevel =  1, MaxLevel =  35, XpMultiplier = 3.5f, BoxDropChance = 0.03f },
            new HotDungeonEntry { Landblock = 0x0163, Name = "Holtburg Redoubt",               MinLevel =  1, MaxLevel =  35, XpMultiplier = 4.0f, BoxDropChance = 0.03f },
            new HotDungeonEntry { Landblock = 0x0158, Name = "Lytaway Dungeon",                MinLevel =  1, MaxLevel =  35, XpMultiplier = 3.5f, BoxDropChance = 0.03f },
            new HotDungeonEntry { Landblock = 0x0154, Name = "Stone Cathedral",                MinLevel =  1, MaxLevel =  35, XpMultiplier = 3.5f, BoxDropChance = 0.03f },
            new HotDungeonEntry { Landblock = 0x02C6, Name = "Lightless Tunnels",              MinLevel =  1, MaxLevel =  35, XpMultiplier = 2.5f, BoxDropChance = 0.03f },
            new HotDungeonEntry { Landblock = 0x5749, Name = "Tusker Lodge",                   MinLevel =  1, MaxLevel =  35, XpMultiplier = 3.5f, BoxDropChance = 0.03f },
            new HotDungeonEntry { Landblock = 0x01D9, Name = "A Red Rat Lair",                 MinLevel =  1, MaxLevel =  35, XpMultiplier = 4.5f, BoxDropChance = 0.03f },
            new HotDungeonEntry { Landblock = 0x01F6, Name = "Holtburg Dungeon",               MinLevel =  1, MaxLevel =  35, XpMultiplier = 3.5f, BoxDropChance = 0.03f },

            new HotDungeonEntry { Landblock = 0x01D7, Name = "Sylsfear",                       MinLevel = 10, MaxLevel =  40, XpMultiplier = 2.5f, BoxDropChance = 0.005f },
            new HotDungeonEntry { Landblock = 0x013B, Name = "Folthid Cellar",                 MinLevel = 10, MaxLevel =  40, XpMultiplier = 2.5f, BoxDropChance = 0.005f },
            new HotDungeonEntry { Landblock = 0x01CC, Name = "Halls of the Helm",              MinLevel = 10, MaxLevel =  40, XpMultiplier = 2.5f, BoxDropChance = 0.005f },
            new HotDungeonEntry { Landblock = 0x01B4, Name = "Golem Burial Ground",            MinLevel = 10, MaxLevel =  40, XpMultiplier = 2.5f, BoxDropChance = 0.005f },
            new HotDungeonEntry { Landblock = 0x017A, Name = "Banderling Hovel",               MinLevel = 10, MaxLevel =  40, XpMultiplier = 2.5f, BoxDropChance = 0.005f },
            new HotDungeonEntry { Landblock = 0x5643, Name = "Gromnie Clan Training Camp",     MinLevel = 10, MaxLevel =  40, XpMultiplier = 2.5f, BoxDropChance = 0.005f },

            new HotDungeonEntry { Landblock = 0x5B46, Name = "Tusker Cave",                    MinLevel = 12, MaxLevel =  40, XpMultiplier = 2.5f, BoxDropChance = 0.005f },
            new HotDungeonEntry { Landblock = 0x5C47, Name = "Tusker Grotto",                  MinLevel = 12, MaxLevel =  40, XpMultiplier = 2.5f, BoxDropChance = 0.005f },

            new HotDungeonEntry { Landblock = 0x0188, Name = "Mines of Despair",               MinLevel = 14, MaxLevel =  40, XpMultiplier = 2.5f, BoxDropChance = 0.005f },

            new HotDungeonEntry { Landblock = 0x02F4, Name = "Halls of Metos",                 MinLevel = 15, MaxLevel =  60, XpMultiplier = 2.5f, BoxDropChance = 0.005f },
            new HotDungeonEntry { Landblock = 0x01C1, Name = "Tumerok Mine",                   MinLevel = 15, MaxLevel =  60, XpMultiplier = 2.5f, BoxDropChance = 0.005f },
            new HotDungeonEntry { Landblock = 0x018D, Name = "Olthoi Tunnels",                 MinLevel = 15, MaxLevel =  60, XpMultiplier = 2.5f, BoxDropChance = 0.005f },
            new HotDungeonEntry { Landblock = 0x5C43, Name = "Tusker Cavern",                  MinLevel = 15, MaxLevel =  60, XpMultiplier = 2.5f, BoxDropChance = 0.005f },
            new HotDungeonEntry { Landblock = 0x0140, Name = "Virindi Fort",                   MinLevel = 15, MaxLevel =  60, XpMultiplier = 2.5f, BoxDropChance = 0.005f },
            new HotDungeonEntry { Landblock = 0x01A2, Name = "Swamp Temple",                   MinLevel = 15, MaxLevel =  60, XpMultiplier = 2.5f, BoxDropChance = 0.005f },
            new HotDungeonEntry { Landblock = 0x01AA, Name = "Lair of Death",                  MinLevel = 15, MaxLevel =  60, XpMultiplier = 2.5f, BoxDropChance = 0.005f },

            new HotDungeonEntry { Landblock = 0x01A9, Name = "Ancient Lighthouse",             MinLevel = 18, MaxLevel =  60, XpMultiplier = 2.5f, BoxDropChance = 0.005f },

            new HotDungeonEntry { Landblock = 0x02DB, Name = "Fenmalain Vestibule",            MinLevel = 20, MaxLevel =  60, XpMultiplier = 2.5f, BoxDropChance = 0.005f },
            new HotDungeonEntry { Landblock = 0x5772, Name = "Olthoi Brood Hive",              MinLevel = 20, MaxLevel =  60, XpMultiplier = 2.5f, BoxDropChance = 0.005f },
            new HotDungeonEntry { Landblock = 0x0105, Name = "Black Spawn Den",                MinLevel = 20, MaxLevel =  70, XpMultiplier = 2.5f, BoxDropChance = 0.005f },
            new HotDungeonEntry { Landblock = 0x5E45, Name = "Black Death Catacombs",          MinLevel = 20, MaxLevel =  70, XpMultiplier = 2.5f, BoxDropChance = 0.005f },
            new HotDungeonEntry { Landblock = 0x02FD, Name = "Olthoi Horde Nest",              MinLevel = 20, MaxLevel =  70, XpMultiplier = 2.5f, BoxDropChance = 0.005f },
            new HotDungeonEntry { Landblock = 0x5D4A, Name = "Olthoi Chasm",                   MinLevel = 20, MaxLevel =  70, XpMultiplier = 2.5f, BoxDropChance = 0.005f },
            new HotDungeonEntry { Landblock = 0x01A6, Name = "Disaster Maze",                  MinLevel = 20, MaxLevel =  70, XpMultiplier = 2.5f, BoxDropChance = 0.005f },
            new HotDungeonEntry { Landblock = 0x0290, Name = "Stable Rift",                    MinLevel = 20, MaxLevel =  70, XpMultiplier = 2.5f, BoxDropChance = 0.005f },
            new HotDungeonEntry { Landblock = 0x5C45, Name = "Tusker Abode",                   MinLevel = 20, MaxLevel =  70, XpMultiplier = 2.5f, BoxDropChance = 0.005f },
            new HotDungeonEntry { Landblock = 0x01BE, Name = "Tumerok Fortress",               MinLevel = 20, MaxLevel =  70, XpMultiplier = 2.5f, BoxDropChance = 0.005f },
            new HotDungeonEntry { Landblock = 0x0138, Name = "Lost City of Frore",             MinLevel = 20, MaxLevel =  70, XpMultiplier = 2.5f, BoxDropChance = 0.005f },

            new HotDungeonEntry { Landblock = 0x02A9, Name = "Hieromancers' Halls",            MinLevel = 20, MaxLevel = 275, XpMultiplier = 4.0f, BoxDropChance = 0.02f  },

            new HotDungeonEntry { Landblock = 0x03A1, Name = "Singular Pyreal Repository",     MinLevel = 25, MaxLevel =  70, XpMultiplier = 2.5f, BoxDropChance = 0.005f },
            new HotDungeonEntry { Landblock = 0x5B47, Name = "Tusker Habitat",                 MinLevel = 25, MaxLevel =  70, XpMultiplier = 2.5f, BoxDropChance = 0.005f },
            new HotDungeonEntry { Landblock = 0x0109, Name = "Dungeon of Corpses",             MinLevel = 25, MaxLevel =  70, XpMultiplier = 2.5f, BoxDropChance = 0.005f },

            new HotDungeonEntry { Landblock = 0x5B4A, Name = "Tusker Quarters",                MinLevel = 30, MaxLevel =  70, XpMultiplier = 2.5f, BoxDropChance = 0.005f },
            new HotDungeonEntry { Landblock = 0x0179, Name = "Umbral Hall",                    MinLevel = 30, MaxLevel =  70, XpMultiplier = 2.5f, BoxDropChance = 0.005f },
            new HotDungeonEntry { Landblock = 0x02C9, Name = "Sotiris Dungeon",                MinLevel = 30, MaxLevel =  70, XpMultiplier = 2.5f, BoxDropChance = 0.005f },

            new HotDungeonEntry { Landblock = 0x02A1, Name = "Panopticon",                     MinLevel = 32, MaxLevel =  70, XpMultiplier = 2.5f, BoxDropChance = 0.005f },
            new HotDungeonEntry { Landblock = 0x03A0, Name = "Singular Chorizite Repository",  MinLevel = 35, MaxLevel =  70, XpMultiplier = 2.5f, BoxDropChance = 0.005f },
            new HotDungeonEntry { Landblock = 0x01F8, Name = "Mite Maze",                      MinLevel = 35, MaxLevel =  70, XpMultiplier = 2.5f, BoxDropChance = 0.005f },

            new HotDungeonEntry { Landblock = 0x027C, Name = "Ancient Empyrean Grotto",        MinLevel = 40, MaxLevel =  90, XpMultiplier = 2.5f, BoxDropChance = 0.005f },
            new HotDungeonEntry { Landblock = 0x6346, Name = "Matron Hive South",              MinLevel = 40, MaxLevel =  90, XpMultiplier = 2.5f, BoxDropChance = 0.005f },
            new HotDungeonEntry { Landblock = 0x02DC, Name = "Caulnalain Vestibule",           MinLevel = 40, MaxLevel =  90, XpMultiplier = 2.5f, BoxDropChance = 0.005f },
            new HotDungeonEntry { Landblock = 0x5E4A, Name = "Olthoi Brood Hive",              MinLevel = 40, MaxLevel =  90, XpMultiplier = 2.5f, BoxDropChance = 0.005f },
            new HotDungeonEntry { Landblock = 0x5A48, Name = "Tusker Barracks",                MinLevel = 40, MaxLevel =  90, XpMultiplier = 2.5f, BoxDropChance = 0.005f },
            new HotDungeonEntry { Landblock = 0x017D, Name = "Hidden Entrance",                MinLevel = 40, MaxLevel =  90, XpMultiplier = 2.5f, BoxDropChance = 0.005f },
            new HotDungeonEntry { Landblock = 0x017B, Name = "Watery Grotto",                  MinLevel = 40, MaxLevel =  90, XpMultiplier = 2.5f, BoxDropChance = 0.005f },

            new HotDungeonEntry { Landblock = 0x5A4A, Name = "Tusker Pits",                    MinLevel = 45, MaxLevel = 120, XpMultiplier = 2.5f, BoxDropChance = 0.005f },
            new HotDungeonEntry { Landblock = 0x5D46, Name = "Vengeance Caverns",              MinLevel = 45, MaxLevel = 120, XpMultiplier = 2.5f, BoxDropChance = 0.005f },

            new HotDungeonEntry { Landblock = 0x0291, Name = "Singularity Bore",               MinLevel = 50, MaxLevel = 275, XpMultiplier = 2.5f, BoxDropChance = 0.005f },
            new HotDungeonEntry { Landblock = 0x027D, Name = "Lair of the Eviscerators",       MinLevel = 50, MaxLevel = 275, XpMultiplier = 2.5f, BoxDropChance = 0.005f },
            new HotDungeonEntry { Landblock = 0x5F46, Name = "Heart of Innocence",             MinLevel = 50, MaxLevel = 275, XpMultiplier = 2.5f, BoxDropChance = 0.005f },

            new HotDungeonEntry { Landblock = 0x6248, Name = "Renegade Fortress",              MinLevel = 60, MaxLevel = 275, XpMultiplier = 2.5f, BoxDropChance = 0.005f },
            new HotDungeonEntry { Landblock = 0x604A, Name = "Sclavus Temple",                 MinLevel = 60, MaxLevel = 275, XpMultiplier = 2.5f, BoxDropChance = 0.005f },
            new HotDungeonEntry { Landblock = 0x02E0, Name = "Shendolain Vestibule",           MinLevel = 60, MaxLevel = 275, XpMultiplier = 2.5f, BoxDropChance = 0.005f },
            new HotDungeonEntry { Landblock = 0x5B48, Name = "Tusker Holding",                 MinLevel = 60, MaxLevel = 275, XpMultiplier = 2.5f, BoxDropChance = 0.005f },
            new HotDungeonEntry { Landblock = 0x01A3, Name = "The Pit",                        MinLevel = 60, MaxLevel = 275, XpMultiplier = 2.5f, BoxDropChance = 0.005f },
            new HotDungeonEntry { Landblock = 0x624A, Name = "Burun Cavern",                   MinLevel = 60, MaxLevel = 275, XpMultiplier = 2.5f, BoxDropChance = 0.005f },
            new HotDungeonEntry { Landblock = 0x6147, Name = "Mountain Citadel",               MinLevel = 60, MaxLevel = 275, XpMultiplier = 2.5f, BoxDropChance = 0.005f },

            new HotDungeonEntry { Landblock = 0x6678, Name = "Olthoi Warrior Nest",            MinLevel = 70, MaxLevel = 275, XpMultiplier = 2.5f, BoxDropChance = 0.005f },
            new HotDungeonEntry { Landblock = 0x5D45, Name = "Tusker Tunnels",                 MinLevel = 70, MaxLevel = 275, XpMultiplier = 2.5f, BoxDropChance = 0.005f },
            new HotDungeonEntry { Landblock = 0x0005, Name = "Southern Power Forge",           MinLevel = 70, MaxLevel = 275, XpMultiplier = 2.5f, BoxDropChance = 0.005f },

            new HotDungeonEntry { Landblock = 0x594D, Name = "Ancient Temple",                 MinLevel = 80, MaxLevel = 275, XpMultiplier = 2.5f, BoxDropChance = 0.005f },
            new HotDungeonEntry { Landblock = 0x01E4, Name = "North Glenden Prison",           MinLevel = 80, MaxLevel = 275, XpMultiplier = 2.5f, BoxDropChance = 0.005f },
            new HotDungeonEntry { Landblock = 0x5D4D, Name = "Cavernous Olthoi Chasm",         MinLevel = 80, MaxLevel = 275, XpMultiplier = 2.5f, BoxDropChance = 0.005f },
            new HotDungeonEntry { Landblock = 0x5F48, Name = "Hidden Cavern",                  MinLevel = 80, MaxLevel = 275, XpMultiplier = 2.5f, BoxDropChance = 0.005f },
            new HotDungeonEntry { Landblock = 0x5A49, Name = "Tusker Honeycombs",              MinLevel = 80, MaxLevel = 275, XpMultiplier = 2.5f, BoxDropChance = 0.005f },
            new HotDungeonEntry { Landblock = 0x667A, Name = "Mutilator Tunnels",              MinLevel = 80, MaxLevel = 275, XpMultiplier = 2.5f, BoxDropChance = 0.005f },
            new HotDungeonEntry { Landblock = 0x0010, Name = "War Room",                       MinLevel = 80, MaxLevel = 275, XpMultiplier = 2.5f, BoxDropChance = 0.005f },
            new HotDungeonEntry { Landblock = 0x0003, Name = "Niffis Fighting Pits",           MinLevel = 80, MaxLevel = 275, XpMultiplier = 2.5f, BoxDropChance = 0.005f },

            new HotDungeonEntry { Landblock = 0x5849, Name = "Tusker Assault",                 MinLevel = 85, MaxLevel = 275, XpMultiplier = 2.5f, BoxDropChance = 0.005f },

            new HotDungeonEntry { Landblock = 0x6145, Name = "Matron Hive West",               MinLevel = 90, MaxLevel = 275, XpMultiplier = 2.5f, BoxDropChance = 0.005f },
            new HotDungeonEntry { Landblock = 0x6548, Name = "The Orphanage",                  MinLevel = 90, MaxLevel = 275, XpMultiplier = 2.5f, BoxDropChance = 0.005f },

            new HotDungeonEntry { Landblock = 0x0110, Name = "Nexus",                          MinLevel = 100, MaxLevel = 275, XpMultiplier = 2.5f, BoxDropChance = 0.005f },
            new HotDungeonEntry { Landblock = 0x5949, Name = "Tusker Lacuna",                  MinLevel = 100, MaxLevel = 275, XpMultiplier = 2.5f, BoxDropChance = 0.005f },
            new HotDungeonEntry { Landblock = 0x0017, Name = "Vile Sanctuary",                 MinLevel = 100, MaxLevel = 275, XpMultiplier = 2.5f, BoxDropChance = 0.005f },
            new HotDungeonEntry { Landblock = 0x0023, Name = "Sezzherei's Lair",               MinLevel = 100, MaxLevel = 275, XpMultiplier = 2.5f, BoxDropChance = 0.005f },

            new HotDungeonEntry { Landblock = 0x6245, Name = "Matron Hive East",               MinLevel = 120, MaxLevel = 275, XpMultiplier = 2.5f, BoxDropChance = 0.005f },
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
                    Broadcast($"{d.Name} is still a Hot Dungeon! {d.XpMultiplier:0.##}x XP — {remaining} remaining.");
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

            var entry    = eligible[ThreadSafeRandom.Next(0, eligible.Count - 1)];
            var duration = ThreadSafeRandom.Next((float)MinDurationSeconds, (float)MaxDurationSeconds);

            var active = new ActiveHotDungeon
            {
                Landblock      = entry.Landblock,
                Name           = entry.Name,
                XpMultiplier   = entry.XpMultiplier,
                BoxDropChance  = entry.BoxDropChance,
                ExpiresAt      = currentUnixTime + duration,
                NextAnnounceAt = currentUnixTime + AnnounceIntervalSecs,
            };

            ActiveDungeons.Add(active);
            ScheduleNextRoll(currentUnixTime);

            var durationStr = TimeSpan.FromSeconds(duration).GetFriendlyString();
            Broadcast($"{entry.Name} is now a Hot Dungeon! {entry.XpMultiplier:0.##}x XP for the next {durationStr}!");
            log.Info($"HotDungeonManager: {entry.Name} (0x{entry.Landblock:X4}) is now hot for {durationStr} at {entry.XpMultiplier:0.##}x XP.");
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
                sb.AppendLine($"  {d.Name} — {d.XpMultiplier:0.##}x XP — {remaining} remaining");
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

            var entry    = PossibleDungeons.FirstOrDefault(e => e.Landblock == landblock);
            var name     = entry?.Name ?? $"Unknown Dungeon (0x{landblock:X4})";
            var xpMul    = entry?.XpMultiplier  ?? 1.5f;
            var boxChance = entry?.BoxDropChance ?? 0.05f;
            var duration = ThreadSafeRandom.Next((float)MinDurationSeconds, (float)MaxDurationSeconds);
            var now      = Time.GetUnixTime();

            var active = new ActiveHotDungeon
            {
                Landblock      = landblock,
                Name           = name,
                XpMultiplier   = xpMul,
                BoxDropChance  = boxChance,
                ExpiresAt      = now + duration,
                NextAnnounceAt = now + AnnounceIntervalSecs,
            };

            ActiveDungeons.Add(active);
            var durationStr = TimeSpan.FromSeconds(duration).GetFriendlyString();
            Broadcast($"{name} is now a Hot Dungeon! {xpMul:0.##}x XP for the next {durationStr}!");
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
