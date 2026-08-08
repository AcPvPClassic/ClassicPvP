using System.Collections.Generic;
using System.Linq;

using ACE.Common;
using ACE.Database;

namespace ACE.Server.Entity.DungeonBoss
{
    /// <summary>
    /// A single Dungeon Boss archetype. Each entry maps to one authored boss weenie
    /// (see Content/sql/weenies/DungeonBosses). The weenie is authored at the reference
    /// difficulty (<see cref="DungeonBosses.ReferenceLevel"/>); the per-archetype
    /// multipliers below are applied on top of the level-cap scaling performed by
    /// <c>DungeonBossManager.ScaleBossToCap</c>, so each boss feels distinct
    /// (glass-cannon caster, armor wall, fast striker, etc.).
    /// </summary>
    public class DungeonBossDef
    {
        public uint WeenieId { get; set; }

        /// <summary>Display name, also used in the global spawn/slain broadcasts.</summary>
        public string Name { get; set; }

        /// <summary>
        /// Global flavor text broadcast when this boss spawns. Must NOT reveal the
        /// location — players are meant to hunt for it.
        /// </summary>
        public string SpawnMessage { get; set; }

        // ── Archetype multipliers (1.0 = neutral) ─────────────────────────────
        /// <summary>Scales MaxHealth on top of level-cap scaling.</summary>
        public float HealthMult { get; set; } = 1.0f;

        /// <summary>Scales melee (body-part) damage. Dominant for melee bosses.</summary>
        public float DamageMult { get; set; } = 1.0f;

        /// <summary>Scales natural armor / resistances.</summary>
        public float ArmorMult { get; set; } = 1.0f;

        /// <summary>Scales offensive skills (attack skills + magic schools). Dominant for casters.</summary>
        public float OffenseMult { get; set; } = 1.0f;

        /// <summary>Scales defensive skills (melee/missile/magic defense = evasion).</summary>
        public float DefenseMult { get; set; } = 1.0f;
    }

    public static class DungeonBosses
    {
        /// <summary>
        /// Level the boss weenies are authored at. Caps below this scale the boss
        /// down; caps above (the post-126 grind, display level up to ~275) scale it up.
        /// </summary>
        public const int ReferenceLevel = 126;

        private static List<DungeonBossDef> _roster;

        public static IReadOnlyList<DungeonBossDef> Roster
        {
            get
            {
                if (_roster == null)
                {
                    _roster = new List<DungeonBossDef>
                    {
                        new DungeonBossDef
                        {
                            WeenieId     = CustomWeenieId.BossGravewalker,
                            Name         = "The Gravewalker",
                            SpawnMessage = "A cold that no fire can touch seeps into the deep places of the land. Somewhere in the dark, old bones remember how to hate, and The Gravewalker rises to walk again. It hungers for the warmth of the living.",
                            HealthMult   = 1.50f,
                            DamageMult   = 1.05f,
                            ArmorMult    = 1.20f,
                            OffenseMult  = 1.00f,
                            DefenseMult  = 0.90f,   // evasion/resist frequency
                        },
                        new DungeonBossDef
                        {
                            WeenieId     = CustomWeenieId.BossEmberlord,
                            Name         = "Vaeth'ren the Emberlord",
                            SpawnMessage = "The air turns to smoke and the stone begins to sweat. A voice of crackling embers laughs from beneath the world as Vaeth'ren the Emberlord kindles his wrath. Seek him only if you would burn.",
                            HealthMult   = 0.70f,
                            DamageMult   = 1.10f,
                            ArmorMult    = 0.80f,
                            OffenseMult  = 1.30f,
                            DefenseMult  = 0.90f,
                        },
                        new DungeonBossDef
                        {
                            WeenieId     = CustomWeenieId.BossRendmaw,
                            Name         = "Rendmaw",
                            SpawnMessage = "A shriek of raw hunger tears through the tunnels, closer than it has any right to be. Rendmaw has caught a scent, and nothing it hunts has ever outrun it. Move quickly, or do not move at all.",
                            HealthMult   = 0.85f,
                            DamageMult   = 1.15f,
                            ArmorMult    = 0.90f,
                            OffenseMult  = 1.10f,
                            DefenseMult  = 1.10f,   // the evasive one, but not oppressive
                        },
                        new DungeonBossDef
                        {
                            WeenieId     = CustomWeenieId.BossAggregate,
                            Name         = "Aggregate Prime",
                            SpawnMessage = "Deep beneath the earth, ten thousand shards of stone and steel grind together and stand as one. Aggregate Prime has assembled itself once more. It does not tire, it does not yield, and it is already moving.",
                            HealthMult   = 1.80f,
                            DamageMult   = 0.90f,
                            ArmorMult    = 1.60f,
                            OffenseMult  = 0.90f,
                            DefenseMult  = 0.85f,
                        },
                        new DungeonBossDef
                        {
                            WeenieId     = CustomWeenieId.BossWhisperer,
                            Name         = "Nharim Dul, the Whispering Death",
                            SpawnMessage = "A whisper finds every ear at once, patient and certain, promising an end to all striving. Nharim Dul, the Whispering Death, has slipped between the shadows of the world. Those who follow the whisper are never heard from again.",
                            HealthMult   = 1.00f,
                            DamageMult   = 1.00f,
                            ArmorMult    = 1.00f,
                            OffenseMult  = 1.15f,
                            DefenseMult  = 1.00f,
                        },
                    };
                }

                return _roster;
            }
        }

        public static bool IsDungeonBoss(uint weenieId)
        {
            return Roster.Any(b => b.WeenieId == weenieId);
        }

        public static DungeonBossDef Get(uint weenieId)
        {
            return Roster.FirstOrDefault(b => b.WeenieId == weenieId);
        }

        /// <summary>
        /// Rolls a random boss whose weenie is not already active anywhere in the world.
        /// Returns null if every roster boss is currently active (so no duplicate weenie
        /// can ever be spawned at the same time).
        /// </summary>
        public static DungeonBossDef RollAvailableBoss(ICollection<uint> activeWeenieIds)
        {
            var available = Roster.Where(b => !activeWeenieIds.Contains(b.WeenieId)).ToList();
            if (available.Count == 0)
                return null;

            return available[ThreadSafeRandom.Next(0, available.Count - 1)];
        }
    }
}
