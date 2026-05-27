using System.ComponentModel.DataAnnotations.Schema;

namespace ACE.Database.Models.Log
{
    /// <summary>
    /// Per-character lifetime open-world PK stats and bounty completions for the
    /// Season leaderboard.  Populated incrementally by SeasonManager on each kill,
    /// death, and bounty completion.  Arena stats are not stored here — they are
    /// read directly from <see cref="ArenaCharacterStats"/>.
    /// </summary>
    public partial class SeasonCharacterStats
    {
        public uint   Id            { get; set; }
        public uint   CharacterId   { get; set; }
        public string CharacterName { get; set; }

        // ── Open-world PK ────────────────────────────────────────────────────
        /// <summary>Total open-world PK kills (arena kills excluded).</summary>
        public uint PkKills { get; set; }

        /// <summary>Total open-world PK deaths.</summary>
        public uint PkDeaths { get; set; }

        /// <summary>Best single kill streak (consecutive kills without dying) ever recorded.</summary>
        public uint PkKillStreakBest { get; set; }

        /// <summary>
        /// Current active kill streak.  Loaded into SeasonManager's in-memory dict
        /// on startup; persisted back here on each kill/death.
        /// </summary>
        public uint PkKillStreakCur { get; set; }

        // ── Bounty system ────────────────────────────────────────────────────
        /// <summary>Total bounty contracts successfully completed and turned in.</summary>
        public uint BountiesCompleted { get; set; }

        // ── Computed (not persisted) ─────────────────────────────────────────
        /// <summary>
        /// K/D ratio computed in memory at query time.
        /// Returns <see cref="PkKills"/> when <see cref="PkDeaths"/> is zero.
        /// </summary>
        [NotMapped]
        public double KdRatio => PkDeaths > 0 ? (double)PkKills / PkDeaths : PkKills;
    }
}
