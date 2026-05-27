namespace ACE.Database
{
    /// <summary>
    /// A single row in a season leaderboard query result.
    /// Not persisted — built in memory from DB query results.
    /// </summary>
    public class SeasonLeaderEntry
    {
        /// <summary>1-based rank on this leaderboard.</summary>
        public int    Rank          { get; set; }

        public uint   CharacterId   { get; set; }
        public string CharacterName { get; set; }

        /// <summary>
        /// Raw numeric score used for sorting/storage (category-specific units).
        /// For K/D this is KdRatio * 1000; for Overall it is weighted pts * 100.
        /// </summary>
        public ulong  Score         { get; set; }

        /// <summary>Human-readable score string shown in leaderboard output.</summary>
        public string ScoreDisplay  { get; set; }
    }
}
