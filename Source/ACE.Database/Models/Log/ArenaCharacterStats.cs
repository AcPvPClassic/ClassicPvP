using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ACE.Database.Models.Log
{
    public partial class ArenaCharacterStats
    {
        public uint Id { get; set; }
        public uint CharacterId { get; set; }
        public string CharacterName { get; set; }

        [NotMapped]
        public uint CharacterLevel { get; set; }

        public string EventType { get; set; }

        // Raw ELO rating — used by 1v1 and 2v2 individual rankings.
        // Updated per-match.  ELO decay is applied daily by the background job
        // and written back to this column, so the stored value is always the
        // current effective rating — no run-time adjustment is needed.
        // FFA/Tugak leave this at its default (1500) and do not use it.
        public uint Elo { get; set; }

        // Accumulated placement points — used by FFA and Tugak rankings.
        // 1v1/2v2 no longer maintain this column for ranking purposes
        // (composite score is computed from Elo + activity counters).
        public uint RankPoints { get; set; }

        // Timestamp of the most recent ranked match in this event category.
        // Playing a 1v1 does NOT reset the 2v2 decay clock; each event type
        // is tracked independently.
        public DateTime? LastMatchDatetime { get; set; }

        // Timestamp of the last time ELO decay was written to the database by
        // the daily background job.  Null means no decay has been applied since
        // the most recent match (the grace period has not yet expired, or the
        // player just played and reset the clock).
        public DateTime? LastDecayDatetime { get; set; }

        // Number of 2v2 matches where this player survived (was not eliminated)
        // as part of the winning team.  Feeds a bonus into the composite score.
        public uint TotalSurvived { get; set; }

        // Computed in-memory by the database layer (not persisted).
        // For 1v1/2v2: Elo + WinBonus + MatchBonus + SurviveBonus.
        // For FFA/Tugak: same as RankPoints.
        [NotMapped]
        public uint CompositeScore { get; set; }

        public uint TotalMatches { get; set; }
        public uint TotalWins { get; set; }
        public uint TotalDraws { get; set; }
        public uint TotalLosses { get; set; }
        public uint TotalDisqualified { get; set; }
        public uint TotalKills { get; set; }
        public uint TotalDeaths { get; set; }
        public uint TotalDmgReceived { get; set; }
        public uint TotalDmgDealt { get; set; }
    }
}
