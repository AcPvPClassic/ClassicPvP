using System.Collections.Generic;

namespace ACE.Database
{
    /// <summary>
    /// A player's rank and score in every season category.
    /// Not persisted — assembled in memory by LogDatabase.GetSeasonPlayerStanding.
    /// </summary>
    public class SeasonPlayerStanding
    {
        public uint   CharacterId   { get; set; }
        public string CharacterName { get; set; }

        /// <summary>
        /// Keyed by category constant (e.g. SeasonConfig.Cat_1v1).
        /// A missing key means the player has no data for that category.
        /// <see cref="SeasonLeaderEntry.Rank"/> == 0 means unranked.
        /// </summary>
        public Dictionary<string, SeasonLeaderEntry> CategoryStandings { get; set; } = new();
    }
}
