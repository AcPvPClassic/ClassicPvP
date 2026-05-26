using System;
using ACE.Database.Models.Log;

namespace ACE.Database
{
    public static class ArenaRanking
    {
        // -----------------------------------------------------------------------
        // ELO helpers (shared by 1v1 and 2v2)
        // -----------------------------------------------------------------------

        public static float GetProbabilityWinning(float ratingPlayer1, float ratingPlayer2)
        {
            return 1f / (1f + MathF.Pow(10f, (ratingPlayer2 - ratingPlayer1) / 400f));
        }

        /// <summary>
        /// Returns how many ELO points transfer from the loser to the winner.
        /// </summary>
        public static int GetRankChange(uint winnerCurrentRank, uint loserCurrentRank, int multiplier)
        {
            float probabilityWinPlayer1 = GetProbabilityWinning(winnerCurrentRank, loserCurrentRank);
            return Convert.ToInt32(Math.Round(multiplier * (1 - probabilityWinPlayer1)));
        }

        // -----------------------------------------------------------------------
        // Composite score (1v1 and 2v2 individual)
        // -----------------------------------------------------------------------

        /// <summary>Added to composite score per ranked win.</summary>
        public const uint WinBonus = 8;

        /// <summary>Added to composite score per ranked match played.</summary>
        public const uint MatchBonus = 2;

        /// <summary>
        /// Added to composite score each time a 2v2 player survived the match
        /// (was not eliminated) as part of the winning team.
        /// </summary>
        public const uint SurviveBonus = 30;

        // -----------------------------------------------------------------------
        // ELO decay (persisted to the database once per day by ArenaManager)
        // -----------------------------------------------------------------------

        /// <summary>
        /// Number of consecutive days without a match in the same event category
        /// before ELO decay begins.  Playing a 2v2 does NOT reset the 1v1 decay
        /// clock and vice versa — each category is tracked independently.
        /// </summary>
        public const int EloDecayGracePeriodDays = 3;

        /// <summary>
        /// Fractional ELO reduction applied per day once the grace period expires.
        /// 0.03 = 3% per day.
        /// </summary>
        public const double EloDecayRatePerDay = 0.03;

        /// <summary>Minimum ELO after decay.  Players never fall below this value.</summary>
        public const uint EloFloor = 1500;

        /// <summary>
        /// Computes how many full days of decay are due and returns the updated ELO
        /// and the new last-decay timestamp.  Returns null when no decay is owed yet.
        ///
        /// <para>Decay begins after <see cref="EloDecayGracePeriodDays"/> days of
        /// inactivity within the same event category.  Each day of decay reduces the
        /// stored ELO by <see cref="EloDecayRatePerDay"/>, floored at
        /// <see cref="EloFloor"/>.</para>
        ///
        /// <para>The caller is responsible for persisting both the new ELO and the
        /// returned timestamp.  <paramref name="lastDecayDatetime"/> must be stored so
        /// that subsequent calls do not double-apply decay for the same days.</para>
        ///
        /// <para>The returned <c>newLastDecayDatetime</c> is advanced by exactly the
        /// number of integer days processed (not by DateTime.Now) to prevent drift
        /// when the job fires at slightly different times each day.</para>
        /// </summary>
        public static (uint newElo, DateTime newLastDecayDatetime)? ComputeAndApplyDecay(
            uint currentElo, DateTime? lastMatchDatetime, DateTime? lastDecayDatetime)
        {
            if (!lastMatchDatetime.HasValue || currentElo <= EloFloor)
                return null;

            var decayStartDate = lastMatchDatetime.Value.AddDays(EloDecayGracePeriodDays);
            var now = DateTime.Now;

            if (now < decayStartDate)
                return null; // still within the grace period

            // Start counting from the later of: when decay should have begun, or
            // when decay was last applied (so we never double-count days).
            var countFrom = lastDecayDatetime.HasValue && lastDecayDatetime.Value > decayStartDate
                ? lastDecayDatetime.Value
                : decayStartDate;

            int decayDays = (int)Math.Floor((now - countFrom).TotalDays);
            if (decayDays <= 0)
                return null; // no full day has elapsed since last decay run

            double decayed = currentElo * Math.Pow(1.0 - EloDecayRatePerDay, decayDays);
            uint newElo = (uint)Math.Max(EloFloor, Math.Round(decayed));
            DateTime newLastDecay = countFrom.AddDays(decayDays);

            return (newElo, newLastDecay);
        }

        /// <summary>
        /// Computes the full composite leaderboard score for a 1v1 or 2v2
        /// individual stats row.  ELO decay is applied daily by the background job
        /// and already reflected in <see cref="ArenaCharacterStats.Elo"/>, so no
        /// additional decay computation is needed here.
        /// Never call this for FFA or Tugak.
        /// </summary>
        public static uint ComputeCompositeScore(ArenaCharacterStats stats)
        {
            return stats.Elo
                + stats.TotalWins     * WinBonus
                + stats.TotalMatches  * MatchBonus
                + stats.TotalSurvived * SurviveBonus;
        }

        /// <summary>
        /// Computes the full composite leaderboard score for a 2v2 team stats row.
        /// </summary>
        public static uint ComputeCompositeScore(ArenaTeamStats stats)
        {
            return stats.Elo
                + stats.TotalWins     * WinBonus
                + stats.TotalMatches  * MatchBonus
                + stats.TotalSurvived * SurviveBonus;
        }

        // -----------------------------------------------------------------------
        // FFA / Tugak placement points
        // -----------------------------------------------------------------------

        /// <summary>1st-place points in FFA / Tugak events.</summary>
        public const uint FfaPoints_1st           = 100;
        /// <summary>2nd-place points.</summary>
        public const uint FfaPoints_2nd           = 50;
        /// <summary>3rd-place points.</summary>
        public const uint FfaPoints_3rd           = 25;
        /// <summary>Participation points (4th place and beyond).</summary>
        public const uint FfaPoints_Participation = 5;

        /// <summary>
        /// Returns how many ranking points to award for the given finish place
        /// in an FFA or Tugak event.  Disqualified players (finishPlace == -1)
        /// receive 0 points.
        /// </summary>
        public static uint GetFfaPlacementPoints(int finishPlace)
        {
            return finishPlace switch
            {
                1  => FfaPoints_1st,
                2  => FfaPoints_2nd,
                3  => FfaPoints_3rd,
                -1 => 0,                    // disqualified
                _  => FfaPoints_Participation
            };
        }
    }
}
