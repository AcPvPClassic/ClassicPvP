using ACE.Common;
using ACE.Common.Extensions;
using ACE.Database;
using ACE.Entity.Enum;
using ACE.Server.Network.GameMessages.Messages;
using ACE.Server.Physics;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Joins;
using System.Text;

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
            new HotDungeonEntry { Landblock = 0x019E, MinLevel =  1, MaxLevel =  35, XpMultiplier = 4.0f, BoxDropChance = 0.03f }, //Drudge Hideout
            new HotDungeonEntry { Landblock = 0x0162, MinLevel =  1, MaxLevel =  35, XpMultiplier = 3.5f, BoxDropChance = 0.03f }, //Cave of Alabree
            new HotDungeonEntry { Landblock = 0x0153, MinLevel =  1, MaxLevel =  35, XpMultiplier = 3.5f, BoxDropChance = 0.03f }, //Shreth Hive
            new HotDungeonEntry { Landblock = 0x0156, MinLevel =  1, MaxLevel =  35, XpMultiplier = 3.5f, BoxDropChance = 0.03f }, //Sea Temple Catacombs
            new HotDungeonEntry { Landblock = 0x0163, MinLevel =  1, MaxLevel =  35, XpMultiplier = 4.0f, BoxDropChance = 0.03f }, //Holtburg Redoubt
            new HotDungeonEntry { Landblock = 0x0158, MinLevel =  1, MaxLevel =  35, XpMultiplier = 3.5f, BoxDropChance = 0.03f }, //Lytaway Dungeon
            new HotDungeonEntry { Landblock = 0x0154, MinLevel =  1, MaxLevel =  35, XpMultiplier = 3.5f, BoxDropChance = 0.03f }, //Stone Cathedral
            new HotDungeonEntry { Landblock = 0x02C6, MinLevel =  1, MaxLevel =  35, XpMultiplier = 2.5f, BoxDropChance = 0.03f }, //Lightless Tunnels
            new HotDungeonEntry { Landblock = 0x5749, MinLevel =  1, MaxLevel =  35, XpMultiplier = 3.5f, BoxDropChance = 0.03f }, //Tusker Lodge
            new HotDungeonEntry { Landblock = 0x01D9, MinLevel =  1, MaxLevel =  35, XpMultiplier = 4.5f, BoxDropChance = 0.03f }, //A Red Rat Lair
            new HotDungeonEntry { Landblock = 0x01F6, MinLevel =  1, MaxLevel =  35, XpMultiplier = 3.5f, BoxDropChance = 0.03f }, //Holtburg Dungeon

            new HotDungeonEntry { Landblock = 0x01D7, MinLevel =  10, MaxLevel =  40, XpMultiplier = 2.5f, BoxDropChance = 0.005f }, //Sylsfear
            new HotDungeonEntry { Landblock = 0x013B, MinLevel =  10, MaxLevel =  40, XpMultiplier = 2.5f, BoxDropChance = 0.005f }, //Folthid Cellar
            new HotDungeonEntry { Landblock = 0x01CC, MinLevel =  10, MaxLevel =  40, XpMultiplier = 2.5f, BoxDropChance = 0.005f }, //Halls of the Helm
            new HotDungeonEntry { Landblock = 0x01B4, MinLevel =  10, MaxLevel =  40, XpMultiplier = 2.5f, BoxDropChance = 0.005f }, //Golem Burial Ground
            new HotDungeonEntry { Landblock = 0x017A, MinLevel =  10, MaxLevel =  40, XpMultiplier = 2.5f, BoxDropChance = 0.005f }, //Banderling Hovel
            new HotDungeonEntry { Landblock = 0x5643, MinLevel =  10, MaxLevel =  40, XpMultiplier = 2.5f, BoxDropChance = 0.005f }, //Gromnie Clan Training Camp

            new HotDungeonEntry { Landblock = 0x5B46, MinLevel =  12, MaxLevel =  40, XpMultiplier = 2.5f, BoxDropChance = 0.005f }, //Tusker Cave
            new HotDungeonEntry { Landblock = 0x5C47, MinLevel =  12, MaxLevel =  40, XpMultiplier = 2.5f, BoxDropChance = 0.005f }, //Tusker Grotto

            new HotDungeonEntry { Landblock = 0x0188, MinLevel =  14, MaxLevel =  40, XpMultiplier = 2.5f, BoxDropChance = 0.005f }, //Mines of Despair

            new HotDungeonEntry { Landblock = 0x02F4, MinLevel =  15, MaxLevel =  60, XpMultiplier = 2.5f, BoxDropChance = 0.005f }, //Halls of Metos (AB)
            new HotDungeonEntry { Landblock = 0x01C1, MinLevel =  15, MaxLevel =  60, XpMultiplier = 2.5f, BoxDropChance = 0.005f }, //Tumerok Mine
            new HotDungeonEntry { Landblock = 0x018D, MinLevel =  15, MaxLevel =  60, XpMultiplier = 2.5f, BoxDropChance = 0.005f }, //Olthoi Tunnels
            new HotDungeonEntry { Landblock = 0x5C43, MinLevel =  15, MaxLevel =  60, XpMultiplier = 2.5f, BoxDropChance = 0.005f }, //Tusker Cavern
            new HotDungeonEntry { Landblock = 0x0140, MinLevel =  15, MaxLevel =  60, XpMultiplier = 2.5f, BoxDropChance = 0.005f }, //Virindi Fort
            new HotDungeonEntry { Landblock = 0x01A2, MinLevel =  15, MaxLevel =  60, XpMultiplier = 2.5f, BoxDropChance = 0.005f }, //Swamp Temple (Yanshi)
            new HotDungeonEntry { Landblock = 0x01AA, MinLevel =  15, MaxLevel =  60, XpMultiplier = 2.5f, BoxDropChance = 0.005f }, //Lair of Death

            new HotDungeonEntry { Landblock = 0x01A9, MinLevel =  18, MaxLevel =  60, XpMultiplier = 2.5f, BoxDropChance = 0.005f }, //Ancient Lighthouse            

            new HotDungeonEntry { Landblock = 0x02DB, MinLevel =  20, MaxLevel =  60, XpMultiplier = 2.5f, BoxDropChance = 0.005f }, //Fenmalain Vestibule
            new HotDungeonEntry { Landblock = 0x5772, MinLevel =  20, MaxLevel =  60, XpMultiplier = 2.5f, BoxDropChance = 0.005f }, //Olthoi Brood Hive (20+)
            new HotDungeonEntry { Landblock = 0x0105, MinLevel =  20, MaxLevel =  70, XpMultiplier = 2.5f, BoxDropChance = 0.005f }, //Black Spawn Den (AB)
            new HotDungeonEntry { Landblock = 0x5E45, MinLevel =  20, MaxLevel =  70, XpMultiplier = 2.5f, BoxDropChance = 0.005f }, //Black Death Catacombs
            new HotDungeonEntry { Landblock = 0x02FD, MinLevel =  20, MaxLevel =  70, XpMultiplier = 2.5f, BoxDropChance = 0.005f }, //Olthoi Horde Nest
            new HotDungeonEntry { Landblock = 0x5D4A, MinLevel =  20, MaxLevel =  70, XpMultiplier = 2.5f, BoxDropChance = 0.005f }, //Olthoi Chasm
            new HotDungeonEntry { Landblock = 0x01A6, MinLevel =  20, MaxLevel =  70, XpMultiplier = 2.5f, BoxDropChance = 0.005f }, //Disaster Maze
            new HotDungeonEntry { Landblock = 0x0290, MinLevel =  20, MaxLevel =  70, XpMultiplier = 2.5f, BoxDropChance = 0.005f }, //Stable Rift
            new HotDungeonEntry { Landblock = 0x5C45, MinLevel =  20, MaxLevel =  70, XpMultiplier = 2.5f, BoxDropChance = 0.005f }, //Tusker Abode
            new HotDungeonEntry { Landblock = 0x01BE, MinLevel =  20, MaxLevel =  70, XpMultiplier = 2.5f, BoxDropChance = 0.005f }, //Tumerok Fortress
            new HotDungeonEntry { Landblock = 0x0138, MinLevel =  20, MaxLevel =  70, XpMultiplier = 2.5f, BoxDropChance = 0.005f }, //Lost City of Frore

            new HotDungeonEntry { Landblock = 0x02A9, MinLevel =  20, MaxLevel =  275, XpMultiplier = 4.0f, BoxDropChance = 0.02f }, //Hieromancers' Halls

            new HotDungeonEntry { Landblock = 0x03A1, MinLevel =  25, MaxLevel =  70, XpMultiplier = 2.5f, BoxDropChance = 0.005f }, //Singular Pyreal Repository
            new HotDungeonEntry { Landblock = 0x5B47, MinLevel =  25, MaxLevel =  70, XpMultiplier = 2.5f, BoxDropChance = 0.005f }, //Tusker Habitat
            new HotDungeonEntry { Landblock = 0x0109, MinLevel =  25, MaxLevel =  70, XpMultiplier = 2.5f, BoxDropChance = 0.005f }, //Dungeon of Corpses

            new HotDungeonEntry { Landblock = 0x5B4A, MinLevel =  30, MaxLevel =  70, XpMultiplier = 2.5f, BoxDropChance = 0.005f }, //Tusker Quarters
            new HotDungeonEntry { Landblock = 0x02FD, MinLevel =  30, MaxLevel =  70, XpMultiplier = 2.5f, BoxDropChance = 0.005f }, //Olthoi Horde Nest (Bandit Castle)
            new HotDungeonEntry { Landblock = 0x0179, MinLevel =  30, MaxLevel =  70, XpMultiplier = 2.5f, BoxDropChance = 0.005f }, //Umbral Hall
            new HotDungeonEntry { Landblock = 0x02C9, MinLevel =  30, MaxLevel =  70, XpMultiplier = 2.5f, BoxDropChance = 0.005f }, //Sotiris Dungeon

            new HotDungeonEntry { Landblock = 0x02A1, MinLevel =  32, MaxLevel =  70, XpMultiplier = 2.5f, BoxDropChance = 0.005f }, //Panopticon
            new HotDungeonEntry { Landblock = 0x03A0, MinLevel =  35, MaxLevel =  70, XpMultiplier = 2.5f, BoxDropChance = 0.005f }, //singular chorizite repository
            new HotDungeonEntry { Landblock = 0x01F8, MinLevel =  35, MaxLevel =  70, XpMultiplier = 2.5f, BoxDropChance = 0.005f }, //Mite Maze

            new HotDungeonEntry { Landblock = 0x027C, MinLevel =  40, MaxLevel =  90, XpMultiplier = 2.5f, BoxDropChance = 0.005f }, //Ancient Empyrean Grotto
            new HotDungeonEntry { Landblock = 0x6346, MinLevel =  40, MaxLevel =  90, XpMultiplier = 2.5f, BoxDropChance = 0.005f }, //Matron Hive South
            new HotDungeonEntry { Landblock = 0x02DC, MinLevel =  40, MaxLevel =  90, XpMultiplier = 2.5f, BoxDropChance = 0.005f }, //Caulnalain Vestibule
            new HotDungeonEntry { Landblock = 0x5E4A, MinLevel =  40, MaxLevel =  90, XpMultiplier = 2.5f, BoxDropChance = 0.005f }, //Olthoi Brood Hive
            new HotDungeonEntry { Landblock = 0x5A48, MinLevel =  40, MaxLevel =  90, XpMultiplier = 2.5f, BoxDropChance = 0.005f }, //Tusker Barracks
            new HotDungeonEntry { Landblock = 0x017D, MinLevel =  40, MaxLevel =  90, XpMultiplier = 2.5f, BoxDropChance = 0.005f }, //Hidden Entrance
            new HotDungeonEntry { Landblock = 0x017B, MinLevel =  40, MaxLevel =  90, XpMultiplier = 2.5f, BoxDropChance = 0.005f }, //Watery Grotto

            new HotDungeonEntry { Landblock = 0x017D, MinLevel =  45, MaxLevel =  120, XpMultiplier = 2.5f, BoxDropChance = 0.005f }, //Singular Obsidian Repository
            new HotDungeonEntry { Landblock = 0x5A4A, MinLevel =  45, MaxLevel =  120, XpMultiplier = 2.5f, BoxDropChance = 0.005f }, //Tusker Pits
            new HotDungeonEntry { Landblock = 0x5D46, MinLevel =  45, MaxLevel =  120, XpMultiplier = 2.5f, BoxDropChance = 0.005f }, //Vengeance Caverns

            new HotDungeonEntry { Landblock = 0x5A4A, MinLevel =  50, MaxLevel =  275, XpMultiplier = 2.5f, BoxDropChance = 0.005f }, //Tusker Armory
            new HotDungeonEntry { Landblock = 0x0291, MinLevel =  50, MaxLevel =  275, XpMultiplier = 2.5f, BoxDropChance = 0.005f }, //Singularity Bore
            new HotDungeonEntry { Landblock = 0x027D, MinLevel =  50, MaxLevel =  275, XpMultiplier = 2.5f, BoxDropChance = 0.005f }, //Lair of the Eviscerators
            new HotDungeonEntry { Landblock = 0x5F46, MinLevel =  50, MaxLevel =  275, XpMultiplier = 2.5f, BoxDropChance = 0.005f }, //Heart of Innocence

            new HotDungeonEntry { Landblock = 0x6248, MinLevel =  60, MaxLevel =  275, XpMultiplier = 2.5f, BoxDropChance = 0.005f }, //Renegade Fortress
            new HotDungeonEntry { Landblock = 0x604A, MinLevel =  60, MaxLevel =  275, XpMultiplier = 2.5f, BoxDropChance = 0.005f }, //Sclavus Temple
            new HotDungeonEntry { Landblock = 0x02E0, MinLevel =  60, MaxLevel =  275, XpMultiplier = 2.5f, BoxDropChance = 0.005f }, //Shendolain Vestibule
            new HotDungeonEntry { Landblock = 0x5B48, MinLevel =  60, MaxLevel =  275, XpMultiplier = 2.5f, BoxDropChance = 0.005f }, //Tusker Holding           
            new HotDungeonEntry { Landblock = 0x5E4A, MinLevel =  60, MaxLevel =  275, XpMultiplier = 2.5f, BoxDropChance = 0.005f }, //Olthoi Brood Hive (60+)
            new HotDungeonEntry { Landblock = 0x01A3, MinLevel =  60, MaxLevel =  275, XpMultiplier = 2.5f, BoxDropChance = 0.005f }, //The Pit

            new HotDungeonEntry { Landblock = 0x6678, MinLevel =  70, MaxLevel =  275, XpMultiplier = 2.5f, BoxDropChance = 0.005f }, //Olthoi Warrior Nest
            new HotDungeonEntry { Landblock = 0x5D45, MinLevel =  70, MaxLevel =  275, XpMultiplier = 2.5f, BoxDropChance = 0.005f }, //Tusker Tunnels
            new HotDungeonEntry { Landblock = 0x0005, MinLevel =  70, MaxLevel =  275, XpMultiplier = 2.5f, BoxDropChance = 0.005f }, //Southern Power Forge

            new HotDungeonEntry { Landblock = 0x0005, MinLevel =  75, MaxLevel =  275, XpMultiplier = 2.5f, BoxDropChance = 0.005f }, //Plated Rifts

            new HotDungeonEntry { Landblock = 0x594D, MinLevel =  80, MaxLevel =  275, XpMultiplier = 2.5f, BoxDropChance = 0.005f }, //Ancient Temple
            new HotDungeonEntry { Landblock = 0x01E4, MinLevel =  80, MaxLevel =  275, XpMultiplier = 2.5f, BoxDropChance = 0.005f }, //North Glenden Prison
            new HotDungeonEntry { Landblock = 0x5D4D, MinLevel =  80, MaxLevel =  275, XpMultiplier = 2.5f, BoxDropChance = 0.005f }, //Cavernous Olthoi Chasm
            new HotDungeonEntry { Landblock = 0x5F48, MinLevel =  80, MaxLevel =  275, XpMultiplier = 2.5f, BoxDropChance = 0.005f }, //Hidden Cavern
            new HotDungeonEntry { Landblock = 0x5A49, MinLevel =  80, MaxLevel =  275, XpMultiplier = 2.5f, BoxDropChance = 0.005f }, //Tusker Honeycombs
            new HotDungeonEntry { Landblock = 0x667A, MinLevel =  80, MaxLevel =  275, XpMultiplier = 2.5f, BoxDropChance = 0.005f }, //Mutilator Tunnels
            new HotDungeonEntry { Landblock = 0x0010, MinLevel =  80, MaxLevel =  275, XpMultiplier = 2.5f, BoxDropChance = 0.005f }, //War Room
            new HotDungeonEntry { Landblock = 0x0003, MinLevel =  80, MaxLevel =  275, XpMultiplier = 2.5f, BoxDropChance = 0.005f }, //Niffis Fighting Pits

            new HotDungeonEntry { Landblock = 0x5849, MinLevel =  85, MaxLevel =  275, XpMultiplier = 2.5f, BoxDropChance = 0.005f }, //Tusker Assault

            new HotDungeonEntry { Landblock = 0x6145, MinLevel =  90, MaxLevel =  275, XpMultiplier = 2.5f, BoxDropChance = 0.005f }, //Matron Hive West
            new HotDungeonEntry { Landblock = 0x6548, MinLevel =  90, MaxLevel =  275, XpMultiplier = 2.5f, BoxDropChance = 0.005f }, //The Orphanage

            new HotDungeonEntry { Landblock = 0x0110, MinLevel =  100, MaxLevel =  275, XpMultiplier = 2.5f, BoxDropChance = 0.005f }, //Nexus
            new HotDungeonEntry { Landblock = 0x5949, MinLevel =  100, MaxLevel =  275, XpMultiplier = 2.5f, BoxDropChance = 0.005f }, //Tusker Lacuna
            new HotDungeonEntry { Landblock = 0x0017, MinLevel =  100, MaxLevel =  275, XpMultiplier = 2.5f, BoxDropChance = 0.005f }, //Vile Sanctuary
            new HotDungeonEntry { Landblock = 0x0023, MinLevel =  100, MaxLevel =  275, XpMultiplier = 2.5f, BoxDropChance = 0.005f }, //Sezzherei's Lair

            new HotDungeonEntry { Landblock = 0x0023, MinLevel =  120, MaxLevel =  275, XpMultiplier = 2.5f, BoxDropChance = 0.005f }, //Matron Hive East

            new HotDungeonEntry { Landblock = 0x624A, MinLevel =  60, MaxLevel =  275, XpMultiplier = 2.5f, BoxDropChance = 0.005f }, //Burun Cavern
            new HotDungeonEntry { Landblock = 0x6147, MinLevel =  60, MaxLevel =  275, XpMultiplier = 2.5f, BoxDropChance = 0.005f }, //Mountain Citadel





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
