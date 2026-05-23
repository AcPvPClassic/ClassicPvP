namespace ACE.Entity.Enum
{
    /// <summary>
    /// For leveling up items,
    /// only kill and quest XP are taken into consideration
    /// </summary>
    public enum XpType
    {
        Kill,
        Quest,
        Proficiency,
        Fellowship,
        Allegiance,
        Admin,
        Emote,
        Exploration,
        /// <summary>
        /// XP earned through PvP activity: player kills, arena matches, and any
        /// other PvP-centered custom content.  Tracked in its own category bucket
        /// (CapPvpXp) with an independently tunable ratio (daily_pvp_xp_category_ratio).
        /// Award this type anywhere a player earns XP from defeating another player
        /// or completing PvP-specific objectives.
        /// </summary>
        PvP
    }
}
