using System;

using ACE.DatLoader;

namespace ACE.Server.Managers
{
    /// <summary>
    /// Manages the server-wide rolling level cap for ClassicPvP.
    ///
    /// The cap starts at level 15 on the configured start date and increases
    /// daily according to the following schedule:
    ///   Days  0-14:  +4 levels/day
    ///   Days 15-41:  +3.5 levels/day
    ///   Days 42-69:  +3.25 levels/day
    ///   Days 70-83:  +1.35 levels/day
    ///   Day  84+:    cap no longer increases
    ///
    /// Relevant server configs:
    ///   rolling_level_cap_enabled       (bool)  — master switch
    ///   rolling_level_cap_start_timestamp (long) — Unix timestamp of season day 0
    ///   rolling_level_cap               (long)  — computed level cap (managed automatically)
    ///   rolling_level_cap_timestamp     (long)  — last update time   (managed automatically)
    /// </summary>
    public static class RollingLevelCapManager
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static DateTime LastTickDateTime = DateTime.MinValue;

        /// <summary>
        /// Called from WorldManager.Tick(). Throttled to once every 15 minutes.
        /// Recomputes and persists the rolling_level_cap if a new calendar day has started.
        /// </summary>
        public static void Tick()
        {
            if (DateTime.Now.AddMinutes(-15) < LastTickDateTime)
                return;

            LastTickDateTime = DateTime.Now;

            if (!PropertyManager.GetBool("rolling_level_cap_enabled").Item)
                return;

            var startTimestamp = PropertyManager.GetLong("rolling_level_cap_start_timestamp").Item;
            if (startTimestamp <= 0)
                return;

            // Only recalculate once per calendar day (UTC midnight boundary)
            var lastUpdateTimestamp = PropertyManager.GetLong("rolling_level_cap_timestamp").Item;
            var todayMidnightUtc = new DateTimeOffset(DateTime.UtcNow.Date, TimeSpan.Zero).ToUnixTimeSeconds();
            if (lastUpdateTimestamp >= todayMidnightUtc)
                return;

            UpdateLevelCap(startTimestamp);
        }

        private static void UpdateLevelCap(long startTimestamp)
        {
            try
            {
                var startDate = DateTimeOffset.FromUnixTimeSeconds(startTimestamp).UtcDateTime.Date;
                var daysSinceStart = (DateTime.UtcNow.Date - startDate).Days;

                if (daysSinceStart < 0)
                    daysSinceStart = 0;

                double newLevelCap = 15;
                for (int i = 0; i < daysSinceStart; i++)
                {
                    if (i < 15)
                        newLevelCap += 4;
                    else if (i < 42)
                        newLevelCap += 3.5;
                    else if (i < 70)
                        newLevelCap += 3.25;
                    else if (i < 84)
                        newLevelCap += 1.35;
                    // After day 83 the cap stops increasing
                }

                var levelCap = Convert.ToInt64(Math.Ceiling(newLevelCap));

                // Clamp to the actual maximum level the XP table supports
                var maxPossibleLevel = (long)DatManager.PortalDat.XpTable.CharacterLevelXPList.Count - 1;
                if (levelCap > maxPossibleLevel)
                    levelCap = maxPossibleLevel;

                PropertyManager.ModifyLong("rolling_level_cap", levelCap);
                PropertyManager.ModifyLong("rolling_level_cap_timestamp", DateTimeOffset.UtcNow.ToUnixTimeSeconds());

                log.Info($"RollingLevelCapManager: Updated rolling_level_cap to {levelCap} (day {daysSinceStart} since season start).");
            }
            catch (Exception ex)
            {
                log.Error($"RollingLevelCapManager.UpdateLevelCap exception: {ex}");
            }
        }

        /// <summary>
        /// Returns the current rolling level cap, or 0 if the system is disabled or not yet configured.
        /// </summary>
        public static long GetCurrentLevelCap()
        {
            if (!PropertyManager.GetBool("rolling_level_cap_enabled").Item)
                return 0;

            var startTimestamp = PropertyManager.GetLong("rolling_level_cap_start_timestamp").Item;
            if (startTimestamp <= 0)
                return 0;

            return PropertyManager.GetLong("rolling_level_cap").Item;
        }

        /// <summary>
        /// Forces an immediate recalculation of the level cap and persists it.
        /// Useful after changing rolling_level_cap_start_timestamp via admin command.
        /// </summary>
        public static void ForceRecalculate()
        {
            var startTimestamp = PropertyManager.GetLong("rolling_level_cap_start_timestamp").Item;
            if (startTimestamp <= 0)
            {
                log.Warn("RollingLevelCapManager.ForceRecalculate: rolling_level_cap_start_timestamp is not set.");
                return;
            }

            // Reset the timestamp so UpdateLevelCap always runs
            PropertyManager.ModifyLong("rolling_level_cap_timestamp", 0);
            UpdateLevelCap(startTimestamp);
        }
    }
}
