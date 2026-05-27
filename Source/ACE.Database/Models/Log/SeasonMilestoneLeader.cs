using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ACE.Database.Models.Log
{
    /// <summary>
    /// One row per (milestone × category × rank) entry in the top-10 snapshot.
    /// Tracks whether the player has claimed their reward items.
    /// </summary>
    public partial class SeasonMilestoneLeader
    {
        public uint     Id            { get; set; }
        public ushort   MilestoneId   { get; set; }

        /// <summary>Denormalised from SeasonMilestone for easier queries.</summary>
        public ushort   WeekNumber    { get; set; }

        /// <summary>Category key, e.g. "1v1", "pk-kills", "overall".</summary>
        public string   Category      { get; set; }

        /// <summary>Leaderboard rank at snapshot time (1–10).</summary>
        public byte     Rank          { get; set; }

        public uint     CharacterId   { get; set; }
        public string   CharacterName { get; set; }

        /// <summary>Raw numeric score used for this category at snapshot time.</summary>
        public ulong    Score         { get; set; }

        public bool     RewardClaimed  { get; set; }
        public DateTime? ClaimedDatetime { get; set; }

        // ── Computed (not persisted) ─────────────────────────────────────────

        [NotMapped]
        public string RankDisplay => Rank switch
        {
            1 => "1st",
            2 => "2nd",
            3 => "3rd",
            _ => $"{Rank}th"
        };
    }
}
