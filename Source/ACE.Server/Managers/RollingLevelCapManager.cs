using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

using ACE.Common;
using ACE.DatLoader;

namespace ACE.Server.Managers
{
    /// <summary>
    /// Manages the server-wide rolling XP cap for ClassicPvP (Infiltration ruleset).
    ///
    /// The cap is stored and enforced as a raw total-XP ceiling rather than a level
    /// number, so it can extend naturally past level 126 into the post-cap skill/attribute
    /// grind phase.
    ///
    /// Schedule (relative to rolling_level_cap_start_timestamp):
    ///
    ///   Phase 1  Days  0–14:  +3.00 levels/day  →  level  57 at end of week 2
    ///   Phase 2  Days 15–44:  +1.50 levels/day  →  level 101 at end of week 6
    ///   Phase 3  Days 45–59:  +1.40 levels/day  →  level 126 exactly at day 60
    ///   Phase 4  Days 60–120: linear XP growth from level-126 XP to season_max_xp
    ///   Day 121+:             cap frozen at season_max_xp
    ///
    ///   Week-by-week level milestones:
    ///     Week  1 (day  0): level  15
    ///     Week  2 (day  7): level  36
    ///     Week  3 (day 14): level  57
    ///     Week  4 (day 21): level  69
    ///     Week  5 (day 28): level  80
    ///     Week  6 (day 35): level  90
    ///     Week  7 (day 42): level 101
    ///     Week  8 (day 49): level 111
    ///     Week  9 (day 56): level 121
    ///     Week  9 (day 60): level 126 — post-level-cap XP phase begins
    ///     Week 18 (day120): season_max_xp reached
    ///
    /// Relevant server configs (PropertyManager):
    ///   rolling_level_cap_enabled         (bool)   — master on/off switch
    ///   rolling_level_cap_start_timestamp (long)   — Unix timestamp of season day 0
    ///   season_max_xp                     (long)   — total-XP ceiling at end of season;
    ///                                                should be enough for every template to
    ///                                                max all skills and attributes
    ///   rolling_xp_cap                    (long)   — computed XP cap (managed automatically)
    ///   rolling_xp_cap_timestamp          (long)   — last update time (managed automatically)
    ///   pvp_dmg_mod_preset_applied_level  (long)   — level threshold of last applied preset
    ///                                                (managed automatically)
    ///
    /// Note: allow_xp_at_max_level must be true (it is set automatically for Infiltration)
    /// for players at level 126 to continue earning XP during Phase 4.
    ///
    /// PvP damage modifier presets are defined in pvp_dmg_mod_presets.json in the server
    /// output directory.  The active preset (highest threshold &lt;= current level cap) is
    /// applied once per day alongside the XP cap update.  Use /reloadpvpdmgpresets to
    /// hot-reload the JSON without a server restart.
    /// </summary>
    public static class RollingLevelCapManager
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // Phase 3 ends (and Phase 4 begins) once the level-based computation first
        // reaches or exceeds the dat-file max level.  With the rates above this
        // occurs at exactly day 60; the code computes it dynamically so rate
        // changes above automatically propagate without touching this constant.
        private const int SEASON_END_DAY = 120;

        private static DateTime LastTickDateTime = DateTime.MinValue;

        // ── pvp_dmg_mod preset state ──────────────────────────────────────────────

        /// <summary>File name relative to the server's working / output directory.</summary>
        private const string PRESET_FILE_NAME = "pvp_dmg_mod_presets.json";

        /// <summary>In-memory preset list; replaced atomically on reload.</summary>
        private static List<PvpDmgModPreset> _presets = new List<PvpDmgModPreset>();

        // ── Tick ─────────────────────────────────────────────────────────────────

        /// <summary>
        /// Called from WorldManager.Tick(). Throttled to once every 15 minutes.
        /// Recomputes and persists rolling_xp_cap if a new calendar day has started,
        /// then applies any pending pvp_dmg_mod preset.
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

            // Only recalculate once per UTC calendar day.
            var lastUpdateTimestamp = PropertyManager.GetLong("rolling_xp_cap_timestamp").Item;
            var todayMidnightUtc = new DateTimeOffset(DateTime.UtcNow.Date, TimeSpan.Zero).ToUnixTimeSeconds();
            if (lastUpdateTimestamp >= todayMidnightUtc)
                return;

            UpdateXpCap(startTimestamp);
        }

        // ── Core computation ─────────────────────────────────────────────────────

        private static void UpdateXpCap(long startTimestamp)
        {
            try
            {
                var startDate = DateTimeOffset.FromUnixTimeSeconds(startTimestamp).UtcDateTime.Date;
                var daysSinceStart = Math.Max(0, (DateTime.UtcNow.Date - startDate).Days);

                var xpTable        = DatManager.PortalDat.XpTable.CharacterLevelXPList;
                var maxPossibleLvl = (int)xpTable.Count - 1;           // 126 for Infiltration dat
                var maxLevelXp     = (long)xpTable[maxPossibleLvl];    // XP required to reach level 126

                var seasonMaxXp = PropertyManager.GetLong("season_max_xp").Item;
                if (seasonMaxXp < maxLevelXp)
                    seasonMaxXp = maxLevelXp;   // floor: season cap is at least the level-126 threshold

                // ── Phase 1–3: level-based portion ───────────────────────────────
                // Compute what level the schedule would yield on this day.
                double newLevelCap = 15.0;
                for (int i = 0; i < daysSinceStart; i++)
                {
                    if      (i < 15) newLevelCap += 3.00;  // Phase 1: fast early gains
                    else if (i < 45) newLevelCap += 1.50;  // Phase 2: steady mid-game
                    else if (i < 60) newLevelCap += 1.40;  // Phase 3: approach to 126
                    // i >= 60: level increments exhausted; phase 4 handles XP directly
                }

                var levelCap = (int)Math.Ceiling(newLevelCap);
                if (levelCap > maxPossibleLvl)
                    levelCap = maxPossibleLvl;

                long xpCap;

                if (levelCap < maxPossibleLvl)
                {
                    // ── Phases 1–3: cap is a level-table XP milestone ─────────────
                    xpCap = (long)xpTable[levelCap];
                }
                else
                {
                    // ── Phase 4: post-level-cap linear XP growth ──────────────────
                    // Find the first day the level-based schedule hit maxPossibleLvl.
                    // We walk the same loop to locate it rather than hard-coding a magic
                    // constant, so changes to rates above automatically propagate.
                    int postCapStartDay = ComputePostCapStartDay(maxPossibleLvl);

                    int postCapDays      = daysSinceStart - postCapStartDay;
                    int totalPostCapDays = SEASON_END_DAY - postCapStartDay;  // window length

                    if (totalPostCapDays <= 0)
                    {
                        // Degenerate: schedule hit max level on or after the last day.
                        xpCap = seasonMaxXp;
                    }
                    else
                    {
                        double fraction = Math.Min(1.0, (double)postCapDays / totalPostCapDays);
                        xpCap = maxLevelXp + (long)(fraction * (seasonMaxXp - maxLevelXp));
                    }
                }

                PropertyManager.ModifyLong("rolling_xp_cap", xpCap);
                PropertyManager.ModifyLong("rolling_xp_cap_timestamp", DateTimeOffset.UtcNow.ToUnixTimeSeconds());

                log.Info($"RollingLevelCapManager: Updated rolling_xp_cap to {xpCap:N0} " +
                         $"(day {daysSinceStart}, {GetCapDescription(xpCap)}).");

                // ── Apply pvp_dmg_mod preset if the level cap has reached a new threshold ──
                ApplyPresetIfNeeded(levelCap);
            }
            catch (Exception ex)
            {
                log.Error($"RollingLevelCapManager.UpdateXpCap exception: {ex}");
            }
        }

        /// <summary>
        /// Returns the first daysSinceStart value at which the level-based schedule
        /// reaches or exceeds <paramref name="maxLevel"/>.  Used to locate the
        /// Phase 3 → Phase 4 transition day.
        /// </summary>
        private static int ComputePostCapStartDay(int maxLevel)
        {
            double v = 15.0;
            for (int i = 0; i < 200; i++)   // 200 is a safe upper bound
            {
                if      (i < 15) v += 3.00;
                else if (i < 45) v += 1.50;
                else if (i < 60) v += 1.40;
                else break;     // phases 1–3 are exhausted; post-cap already active

                if ((int)Math.Ceiling(v) >= maxLevel)
                    return i + 1;   // day *after* this increment
            }
            return 56;  // fallback — phases exhausted without hitting maxLevel
        }

        // ── Public API ───────────────────────────────────────────────────────────

        /// <summary>
        /// Returns the current raw total-XP cap, or 0 if the system is disabled
        /// or not yet configured.  The caller should treat 0 as "no active cap".
        /// </summary>
        public static long GetCurrentXpCap()
        {
            if (!PropertyManager.GetBool("rolling_level_cap_enabled").Item)
                return 0;

            if (PropertyManager.GetLong("rolling_level_cap_start_timestamp").Item <= 0)
                return 0;

            return PropertyManager.GetLong("rolling_xp_cap").Item;
        }

        /// <summary>
        /// Returns a human-readable description of what XP milestone <paramref name="xpCap"/>
        /// corresponds to — used in player-facing cap messages and admin status output.
        /// </summary>
        public static string GetCapDescription(long xpCap)
        {
            if (xpCap <= 0)
                return "no cap";

            try
            {
                var xpTable        = DatManager.PortalDat.XpTable.CharacterLevelXPList;
                var maxPossibleLvl = xpTable.Count - 1;

                // Walk the XP table to find the highest level whose XP ≤ xpCap.
                int impliedLevel = 0;
                for (int lvl = 1; lvl <= maxPossibleLvl; lvl++)
                {
                    if ((long)xpTable[lvl] <= xpCap)
                        impliedLevel = lvl;
                    else
                        break;
                }

                if (impliedLevel >= maxPossibleLvl)
                {
                    // Cap is at or above level 126 — express as XP beyond level cap.
                    long postCapXp = xpCap - (long)xpTable[maxPossibleLvl];
                    if (postCapXp <= 0)
                        return $"level {maxPossibleLvl} (XP: {xpCap:N0})";

                    return $"post-level-{maxPossibleLvl} (+{postCapXp:N0} XP beyond level cap, total {xpCap:N0})";
                }

                return $"level {impliedLevel} (XP: {xpCap:N0})";
            }
            catch
            {
                return $"XP {xpCap:N0}";
            }
        }

        /// <summary>
        /// Forces an immediate recalculation of the XP cap and persists it.
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

            // Reset the timestamp so UpdateXpCap always runs.
            PropertyManager.ModifyLong("rolling_xp_cap_timestamp", 0);
            UpdateXpCap(startTimestamp);
        }

        /// <summary>
        /// Returns the current season day number (0-based, UTC).
        /// Returns -1 if the season has not been started.
        /// </summary>
        public static int GetCurrentSeasonDay()
        {
            var startTimestamp = PropertyManager.GetLong("rolling_level_cap_start_timestamp").Item;
            if (startTimestamp <= 0) return -1;
            var startDate = DateTimeOffset.FromUnixTimeSeconds(startTimestamp).UtcDateTime.Date;
            return Math.Max(0, (DateTime.UtcNow.Date - startDate).Days);
        }

        /// <summary>
        /// Returns the maximum character level implied by <paramref name="xpCap"/> using
        /// the dat-file XP table.  Returns 0 if the dat is unavailable or xpCap is 0.
        /// </summary>
        public static int GetCurrentLevelCap(long xpCap)
        {
            if (xpCap <= 0) return 0;
            try
            {
                var xpTable        = DatManager.PortalDat.XpTable.CharacterLevelXPList;
                var maxPossibleLvl = (int)xpTable.Count - 1;
                int impliedLevel   = 0;
                for (int lvl = 1; lvl <= maxPossibleLvl; lvl++)
                {
                    if ((long)xpTable[lvl] <= xpCap)
                        impliedLevel = lvl;
                    else
                        break;
                }
                return impliedLevel;
            }
            catch { return 0; }
        }

        /// <summary>
        /// Returns the time remaining until the cap next advances (next UTC midnight).
        /// Returns <see cref="TimeSpan.Zero"/> if the season has ended or has not started.
        /// </summary>
        public static TimeSpan GetTimeUntilNextCapIncrease()
        {
            int day = GetCurrentSeasonDay();
            if (day < 0 || day >= SEASON_END_DAY) return TimeSpan.Zero;
            return DateTime.UtcNow.Date.AddDays(1) - DateTime.UtcNow;
        }

        // ── pvp_dmg_mod preset management ─────────────────────────────────────────

        /// <summary>
        /// Loads (or reloads) pvp_dmg_mod_presets.json from the server's working directory.
        /// Called once at startup and on demand via /reloadpvpdmgpresets.
        /// Returns a human-readable status string suitable for admin output.
        /// </summary>
        public static string LoadPresets()
        {
            string path = ResolvePresetFilePath();

            if (!File.Exists(path))
            {
                _presets = new List<PvpDmgModPreset>();
                log.Info($"RollingLevelCapManager: {PRESET_FILE_NAME} not found at '{path}'. No presets loaded.");
                return $"{PRESET_FILE_NAME} not found at '{path}'. No presets active.";
            }

            try
            {
                var json = File.ReadAllText(path);
                var config = JsonSerializer.Deserialize<PvpDmgModPresetConfig>(json, ConfigManager.SerializerOptions);

                if (config?.Presets == null || config.Presets.Count == 0)
                {
                    _presets = new List<PvpDmgModPreset>();
                    log.Info($"RollingLevelCapManager: {PRESET_FILE_NAME} loaded — no presets defined.");
                    return $"{PRESET_FILE_NAME} loaded — no presets defined.";
                }

                // Sort ascending by level threshold so the "highest ≤ cap" search is easy.
                config.Presets.Sort((a, b) => a.LevelThreshold.CompareTo(b.LevelThreshold));
                _presets = config.Presets;

                log.Info($"RollingLevelCapManager: Loaded {_presets.Count} pvp_dmg_mod preset(s) from '{path}'. " +
                         $"Thresholds: [{string.Join(", ", _presets.Select(p => p.LevelThreshold))}]");

                return $"Loaded {_presets.Count} preset(s). Thresholds: [{string.Join(", ", _presets.Select(p => p.LevelThreshold))}]";
            }
            catch (Exception ex)
            {
                log.Error($"RollingLevelCapManager: Failed to load {PRESET_FILE_NAME}: {ex.Message}");
                return $"Error loading {PRESET_FILE_NAME}: {ex.Message}";
            }
        }

        /// <summary>
        /// Returns a snapshot of the currently loaded presets (for admin display).
        /// </summary>
        public static IReadOnlyList<PvpDmgModPreset> GetLoadedPresets() => _presets;

        /// <summary>
        /// Returns the active preset for the given level cap, or null if none applies.
        /// The active preset is the one with the highest LevelThreshold &lt;= levelCap.
        /// </summary>
        public static PvpDmgModPreset GetActivePreset(int levelCap)
        {
            PvpDmgModPreset active = null;
            foreach (var p in _presets)
            {
                if (p.LevelThreshold <= levelCap)
                    active = p;
                else
                    break;  // list is sorted ascending; no point continuing
            }
            return active;
        }

        /// <summary>
        /// Checks whether the active preset for <paramref name="levelCap"/> differs from
        /// the last-applied one, and if so applies it.  Called automatically from
        /// <see cref="UpdateXpCap"/> each daily tick.
        /// </summary>
        private static void ApplyPresetIfNeeded(int levelCap)
        {
            if (_presets.Count == 0)
                return;

            var activePreset = GetActivePreset(levelCap);
            if (activePreset == null)
                return;

            var lastAppliedLevel = PropertyManager.GetLong("pvp_dmg_mod_preset_applied_level").Item;
            if (activePreset.LevelThreshold == lastAppliedLevel)
                return;  // already applied — nothing to do

            ApplyPreset(activePreset, reason: $"level cap reached {levelCap} (threshold {activePreset.LevelThreshold})");
        }

        /// <summary>
        /// Applies all properties in <paramref name="preset"/> via PropertyManager and
        /// records the threshold in pvp_dmg_mod_preset_applied_level.
        /// Returns a summary string suitable for admin output.
        /// </summary>
        public static string ApplyPreset(PvpDmgModPreset preset, string reason = "manual")
        {
            if (preset == null)
                return "No preset to apply.";

            int applied = 0, skipped = 0;
            var skippedKeys = new List<string>();

            foreach (var kvp in preset.Properties)
            {
                if (PropertyManager.ModifyDouble(kvp.Key, kvp.Value))
                    applied++;
                else
                {
                    skipped++;
                    skippedKeys.Add(kvp.Key);
                }
            }

            PropertyManager.ModifyLong("pvp_dmg_mod_preset_applied_level", preset.LevelThreshold);

            var desc = string.IsNullOrWhiteSpace(preset.Description) ? "(no description)" : preset.Description;
            var summary = $"Applied pvp_dmg_mod preset (threshold {preset.LevelThreshold} — \"{desc}\") " +
                          $"via {reason}: {applied} propert{(applied == 1 ? "y" : "ies")} set";

            if (skipped > 0)
                summary += $", {skipped} unknown key(s) skipped: [{string.Join(", ", skippedKeys)}]";

            log.Info($"RollingLevelCapManager: {summary}.");
            return summary;
        }

        // ── Helpers ───────────────────────────────────────────────────────────────

        private static string ResolvePresetFilePath()
        {
            // Prefer the directory of the executing assembly (the server output dir),
            // fall back to the current working directory.
            var assemblyDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            if (!string.IsNullOrEmpty(assemblyDir))
            {
                var p = Path.Combine(assemblyDir, PRESET_FILE_NAME);
                if (File.Exists(p)) return p;
            }
            return Path.Combine(Environment.CurrentDirectory, PRESET_FILE_NAME);
        }
    }
}
