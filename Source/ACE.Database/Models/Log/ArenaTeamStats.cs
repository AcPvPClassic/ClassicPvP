using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ACE.Database.Models.Log
{
    /// <summary>
    /// Aggregate ranking row for a unique 2v2 team pair.
    /// The team is identified by <see cref="TeamKey"/> which is always
    /// the lower character ID followed by the higher one: "{minId}_{maxId}".
    /// </summary>
    public partial class ArenaTeamStats
    {
        public uint Id { get; set; }

        /// <summary>"{minCharacterId}_{maxCharacterId}" — unique per pair.</summary>
        public string TeamKey { get; set; }

        public uint CharacterIdA { get; set; }
        public string CharacterNameA { get; set; }
        public uint CharacterIdB { get; set; }
        public string CharacterNameB { get; set; }

        [NotMapped]
        public string TeamName => $"{CharacterNameA} & {CharacterNameB}";

        /// <summary>
        /// Raw team ELO.  ELO decay is applied daily by the background job and
        /// persisted here, so the stored value is always the current effective rating.
        /// </summary>
        public uint Elo { get; set; }

        /// <summary>
        /// Composite score snapshot as of the last match or last decay run.
        /// Useful for raw DB queries; authoritative ranking uses CompositeScore.
        /// </summary>
        public uint RankPoints { get; set; }

        public uint TotalMatches { get; set; }
        public uint TotalWins { get; set; }
        public uint TotalLosses { get; set; }
        public uint TotalDraws { get; set; }
        public uint TotalDisqualified { get; set; }

        /// <summary>
        /// Number of matches where both team members survived (were not eliminated)
        /// as part of the winning result.
        /// </summary>
        public uint TotalSurvived { get; set; }

        /// <summary>
        /// Timestamp of the most recent match for this team pair.
        /// </summary>
        public DateTime? LastMatchDatetime { get; set; }

        /// <summary>
        /// Timestamp of the last time ELO decay was written to the database for this team.
        /// Null means no decay has been applied since the most recent match.
        /// </summary>
        public DateTime? LastDecayDatetime { get; set; }

        // Computed in-memory by the database layer — not persisted.
        [NotMapped]
        public uint CompositeScore { get; set; }
    }
}
