using System.Collections.Generic;

namespace ACE.Database
{
    /// <summary>
    /// Central configuration for the Season leaderboard system.
    /// Category weights, XP reward multipliers, and item weenie IDs are all
    /// defined here as constants — analogous to <see cref="ArenaRanking"/>.
    ///
    /// <para>
    /// <b>Before server launch:</b> fill in the three weenie ID constants
    /// (<see cref="ABox_WeenieId"/>, <see cref="PhialOfBloodyTears_WeenieId"/>,
    /// <see cref="PkTrophy_WeenieId"/>).  Any weenie ID left at 0 is silently skipped
    /// at reward claim time and logged as a warning.
    /// </para>
    /// </summary>
    public static class SeasonConfig
    {
        // ── Category keys ────────────────────────────────────────────────────
        // Canonical string identifiers used throughout the season system.

        public const string Cat_1v1          = "1v1";
        public const string Cat_2v2          = "2v2";
        public const string Cat_Ffa          = "ffa";
        public const string Cat_Tugak        = "tugak";
        public const string Cat_Group        = "group";
        public const string Cat_ArenaWins    = "arena-wins";
        public const string Cat_ArenaKills   = "arena-kills";
        public const string Cat_ArenaMatches = "arena-matches";
        public const string Cat_Bounty       = "bounty";
        public const string Cat_PkKills      = "pk-kills";
        public const string Cat_PkKd         = "pk-kd";
        public const string Cat_PkStreak     = "pk-streak";
        public const string Cat_Overall      = "overall";

        /// <summary>All scoreable categories in display order (excludes overall).</summary>
        public static readonly IReadOnlyList<string> ScoredCategories = new[]
        {
            Cat_1v1, Cat_2v2, Cat_Ffa, Cat_Tugak, Cat_Group,
            Cat_ArenaWins, Cat_ArenaKills, Cat_ArenaMatches,
            Cat_Bounty, Cat_PkKills, Cat_PkKd, Cat_PkStreak
        };

        // ── Overall score weights ────────────────────────────────────────────
        // For each category a player's rank (1-based) is converted to rank-points:
        //   rankPoints = max(0, 11 - rank)   →  rank 1 = 10 pts, rank 10 = 1 pt
        // overall score = Σ (rankPoints × weight) across all ScoredCategories.

        public static readonly IReadOnlyDictionary<string, double> CategoryWeights =
            new Dictionary<string, double>
            {
                [Cat_1v1]          = 3.0,   // Prestigious 1v1 format
                [Cat_2v2]          = 2.5,
                [Cat_PkKills]      = 2.5,   // Open-world PK activity
                [Cat_Ffa]          = 2.0,
                [Cat_Group]        = 2.0,
                [Cat_PkKd]         = 2.0,
                [Cat_Bounty]       = 2.0,   // Bounty system engagement
                [Cat_Tugak]        = 1.5,
                [Cat_PkStreak]     = 1.5,
                [Cat_ArenaWins]    = 1.5,
                [Cat_ArenaKills]   = 1.0,
                [Cat_ArenaMatches] = 0.5,   // Participation metric, lowest weight
            };

        /// <summary>
        /// Converts a 1-based leaderboard rank into raw rank-points before weight is applied.
        /// Rank 1 = 10 pts, rank 10 = 1 pt, rank 11+ = 0 pts.
        /// </summary>
        public static int RankToPoints(int rank) => System.Math.Max(0, 11 - rank);

        // ── Minimum thresholds ───────────────────────────────────────────────
        // pk-kd requires at least this many kills to appear on the leaderboard.
        public const int PkKd_MinKills = 10;

        // ── Weekly milestone: XP grants ──────────────────────────────────────
        // Uses GrantLevelProportionalXp(multiplier, 0, 0) — same pattern as ArenaLocation.
        // multiplier = fraction of XP to the next level (0.10 = 10%).
        public const double Weekly_Rank1_XpMultiplier     = 0.10;
        public const double Weekly_Rank2_XpMultiplier     = 0.07;
        public const double Weekly_Rank3_XpMultiplier     = 0.05;
        public const double Weekly_Rank4to10_XpMultiplier = 0.02;

        // ── Weekly milestone: reward item weenie IDs ─────────────────────────
        public const uint ABox_WeenieId               = 510000;  // "A Box"
        public const uint PhialOfBloodyTears_WeenieId = 1000003; // "Phial of Bloody Tears"
        public const uint PkTrophy_WeenieId           = 1000002; // "PK Trophy"

        // ── Weekly milestone: item bundles per rank ──────────────────────────
        // Each tuple is (weenieId, quantity).

        public static readonly (uint weenieId, int qty)[] Weekly_Rank1_Items =
        {
            (ABox_WeenieId,        3),
            (PhialOfBloodyTears_WeenieId, 2),
            (PkTrophy_WeenieId,           1),
        };

        public static readonly (uint weenieId, int qty)[] Weekly_Rank2_Items =
        {
            (ABox_WeenieId,        2),
            (PhialOfBloodyTears_WeenieId, 1),
        };

        public static readonly (uint weenieId, int qty)[] Weekly_Rank3_Items =
        {
            (ABox_WeenieId,        1),
            (PkTrophy_WeenieId,           1),
        };

        public static readonly (uint weenieId, int qty)[] Weekly_Rank4to10_Items =
        {
            (PhialOfBloodyTears_WeenieId, 1),
        };

        // ── Accessors ────────────────────────────────────────────────────────

        public static double GetXpMultiplier(int rank) => rank switch
        {
            1 => Weekly_Rank1_XpMultiplier,
            2 => Weekly_Rank2_XpMultiplier,
            3 => Weekly_Rank3_XpMultiplier,
            _ => Weekly_Rank4to10_XpMultiplier
        };

        public static (uint weenieId, int qty)[] GetItems(int rank) => rank switch
        {
            1 => Weekly_Rank1_Items,
            2 => Weekly_Rank2_Items,
            3 => Weekly_Rank3_Items,
            _ => Weekly_Rank4to10_Items
        };

        // ── Display helpers ──────────────────────────────────────────────────

        public static string GetCategoryDisplayName(string category) => category switch
        {
            Cat_1v1          => "1v1 Arena",
            Cat_2v2          => "2v2 Arena",
            Cat_Ffa          => "FFA Arena",
            Cat_Tugak        => "Tugak Arena",
            Cat_Group        => "Group Arena",
            Cat_ArenaWins    => "Arena Wins",
            Cat_ArenaKills   => "Arena Kills",
            Cat_ArenaMatches => "Arena Matches",
            Cat_Bounty       => "Bounty Hunter",
            Cat_PkKills      => "PK Kills",
            Cat_PkKd         => "K/D Ratio",
            Cat_PkStreak     => "Kill Streak",
            Cat_Overall      => "Season Champion",
            _                => category
        };

        /// <summary>
        /// Maps user-typed aliases to canonical category keys.
        /// Returns null if the alias is not recognised.
        /// </summary>
        public static string ResolveAlias(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return null;
            return input.ToLower().Replace(" ", "-") switch
            {
                "1v1"                           => Cat_1v1,
                "2v2"                           => Cat_2v2,
                "ffa"                           => Cat_Ffa,
                "tugak"                         => Cat_Tugak,
                "group"                         => Cat_Group,
                "arena-wins" or "arenawins"
                    or "wins"                   => Cat_ArenaWins,
                "arena-kills" or "arenakills"
                    or "slayer" or "arenaslayer" => Cat_ArenaKills,
                "arena-matches" or "arenamatch"
                    or "matches" or "veteran"
                    or "arenavet"               => Cat_ArenaMatches,
                "bounty" or "bountyhunter"
                    or "bounty-hunter"          => Cat_Bounty,
                "pk-kills" or "pkkills"
                    or "reaper" or "kills"      => Cat_PkKills,
                "pk-kd" or "pkkd" or "kd"
                    or "ratio" or "precision"   => Cat_PkKd,
                "pk-streak" or "pkstreak"
                    or "streak" or "unstoppable" => Cat_PkStreak,
                "overall" or "champion"
                    or "season"                 => Cat_Overall,
                _                               => null
            };
        }
    }
}
