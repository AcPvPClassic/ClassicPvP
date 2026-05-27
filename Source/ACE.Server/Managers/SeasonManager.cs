using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using log4net;

using ACE.Database;
using ACE.Entity.Enum;
using ACE.Server.Entity;
using ACE.Server.Factories;
using ACE.Server.Network.GameMessages.Messages;
using ACE.Server.WorldObjects;

namespace ACE.Server.Managers
{
    /// <summary>
    /// Manages the Season leaderboard system.
    ///
    /// <para>Responsibilities:</para>
    /// <list type="bullet">
    ///   <item>Maintains an in-memory leaderboard cache per category (configurable TTL).</item>
    ///   <item>Tracks current open-world kill streaks in memory, persisted on each change.</item>
    ///   <item>Fires weekly Sunday milestone snapshots and broadcasts rewards.</item>
    ///   <item>Delivers reward items to players via <c>/season claim</c>.</item>
    /// </list>
    ///
    /// <para>Thread safety: all public methods are called from the world heartbeat thread
    /// (same thread as <see cref="ArenaManager"/>).  The cache dictionaries are not
    /// individually locked because ACE is single-threaded on the tick path.</para>
    /// </summary>
    public static class SeasonManager
    {
        private static readonly ILog log = LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // ── Cache ─────────────────────────────────────────────────────────────
        // Top-10 per category.  Refreshed lazily when stale.
        private static readonly Dictionary<string, (List<SeasonLeaderEntry> entries, DateTime refreshed)>
            _topCache = new();

        // Per-player standings cache.  Shorter TTL — invalidated immediately on
        // kill/death/bounty for the affected player.
        private static readonly Dictionary<uint, (SeasonPlayerStanding standing, DateTime refreshed)>
            _playerCache = new();

        // How long a cached leaderboard entry is considered fresh (minutes).
        private static int CacheTtlMinutes =>
            (int)Math.Max(1, PropertyManager.GetLong("season_cache_ttl_minutes").Item > 0
                ? PropertyManager.GetLong("season_cache_ttl_minutes").Item
                : 5);

        private const int PlayerCacheTtlSeconds = 60;

        // ── Kill streak tracking ──────────────────────────────────────────────
        // Loaded from DB on Initialize; persisted on each change.
        private static readonly Dictionary<uint, uint> _currentStreaks = new();

        // ── Milestone state ───────────────────────────────────────────────────
        private static DateTime _lastMilestoneDatetime = DateTime.MinValue;
        private static ushort   _currentWeekNumber     = 0;

        // ── Tick rate limiting ────────────────────────────────────────────────
        private static DateTime _lastTick = DateTime.MinValue;

        // ─────────────────────────────────────────────────────────────────────
        // Lifecycle
        // ─────────────────────────────────────────────────────────────────────

        public static void Initialize()
        {
            if (!LogDatabase.IsConfigured)
            {
                log.Warn("[Season] ace_log database not configured — Season leaderboard disabled.");
                return;
            }

            // Restore kill streaks from DB
            var streaks = DatabaseManager.Log.LoadAllCurrentStreaks();
            foreach (var kv in streaks)
                _currentStreaks[kv.Key] = kv.Value;

            // Restore milestone state
            _currentWeekNumber     = DatabaseManager.Log.GetLastMilestoneWeekNumber();
            _lastMilestoneDatetime = DatabaseManager.Log.GetLastMilestoneDatetime() ?? DateTime.MinValue;

            log.Info($"[Season] Initialized — week {_currentWeekNumber}, " +
                     $"last milestone {(_lastMilestoneDatetime == DateTime.MinValue ? "none" : _lastMilestoneDatetime.ToString("yyyy-MM-dd"))}, " +
                     $"{streaks.Count} active kill streak(s) restored.");
        }

        /// <summary>
        /// Called from WorldManager.Tick().  Rate-limited to once per minute.
        /// Fires the weekly Sunday milestone snapshot when due.
        /// </summary>
        public static void Tick()
        {
            if (!LogDatabase.IsConfigured) return;
            if (DateTime.Now < _lastTick.AddMinutes(1)) return;
            _lastTick = DateTime.Now;

            // Weekly milestone: fire on Sunday, at most once per calendar day.
            if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday
                && DateTime.Now.Date > _lastMilestoneDatetime.Date)
            {
                FireWeeklyMilestone();
            }
        }

        private static void FireWeeklyMilestone()
        {
            _currentWeekNumber++;
            log.Info($"[Season] Firing Week {_currentWeekNumber} milestone snapshot...");

            var milestoneId = DatabaseManager.Log.CaptureSeasonMilestone(_currentWeekNumber);
            if (milestoneId == 0)
            {
                log.Error("[Season] CaptureSeasonMilestone returned 0 — milestone NOT recorded.");
                _currentWeekNumber--;   // roll back so we retry next tick
                return;
            }

            _lastMilestoneDatetime = DateTime.Now;

            // Build the broadcast message
            var msg = BuildMilestoneMessage(_currentWeekNumber);

            // In-game broadcast
            PlayerManager.BroadcastToAll(new GameMessageSystemChat(msg, ChatMessageType.WorldBroadcast));

            // Discord webhook
            DiscordWebhookManager.SendSeasonMilestone(msg);

            // Flush the whole top cache so next query is fresh
            _topCache.Clear();

            log.Info($"[Season] Week {_currentWeekNumber} milestone complete (id={milestoneId}).");
        }

        private static string BuildMilestoneMessage(ushort week)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"*** Season Week {week} Milestone Reached! ***");
            sb.AppendLine("This week's leaders:");

            foreach (var cat in SeasonConfig.ScoredCategories)
            {
                var top = GetTopForCategory(cat, 1);
                if (top.Count == 0) continue;
                var leader = top[0];
                sb.AppendLine($"  {SeasonConfig.GetCategoryDisplayName(cat)}: {leader.CharacterName} ({leader.ScoreDisplay})");
            }

            sb.AppendLine("Use /season top to see the full leaderboards.");
            sb.AppendLine("Use /season claim to collect your weekly reward items!");
            return sb.ToString().TrimEnd();
        }

        // ─────────────────────────────────────────────────────────────────────
        // Event hooks (called by Player_Death and Player_BountyInformation)
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Called when a player makes an open-world PK kill (not in an arena match).
        /// </summary>
        public static void RecordPkKill(uint killerId, string killerName)
        {
            if (!LogDatabase.IsConfigured) return;

            _currentStreaks.TryGetValue(killerId, out var cur);
            var newStreak = cur + 1;
            _currentStreaks[killerId] = newStreak;

            DatabaseManager.Log.UpdateSeasonKill(killerId, killerName, newStreak);
            InvalidatePlayerCache(killerId);
        }

        /// <summary>
        /// Called when a player dies to another player in open-world PK.
        /// Resets the victim's kill streak.
        /// </summary>
        public static void RecordPkDeath(uint victimId, string victimName)
        {
            if (!LogDatabase.IsConfigured) return;

            _currentStreaks[victimId] = 0;

            DatabaseManager.Log.UpdateSeasonDeath(victimId, victimName);
            InvalidatePlayerCache(victimId);
        }

        /// <summary>
        /// Called when a player successfully completes and turns in a bounty contract.
        /// </summary>
        public static void RecordBountyCompleted(uint characterId, string characterName)
        {
            if (!LogDatabase.IsConfigured) return;

            DatabaseManager.Log.UpdateSeasonBounty(characterId, characterName);
            InvalidatePlayerCache(characterId);
        }

        /// <summary>
        /// Flushes all arena-category cache entries so the next query re-fetches
        /// from the DB.  Called by ArenaManager after a match ends.
        /// </summary>
        public static void InvalidateArenaCache()
        {
            _topCache.Remove(SeasonConfig.Cat_1v1);
            _topCache.Remove(SeasonConfig.Cat_2v2);
            _topCache.Remove(SeasonConfig.Cat_Ffa);
            _topCache.Remove(SeasonConfig.Cat_Tugak);
            _topCache.Remove(SeasonConfig.Cat_Group);
            _topCache.Remove(SeasonConfig.Cat_ArenaWins);
            _topCache.Remove(SeasonConfig.Cat_ArenaKills);
            _topCache.Remove(SeasonConfig.Cat_ArenaMatches);
            _topCache.Remove(SeasonConfig.Cat_Overall);
        }

        // ─────────────────────────────────────────────────────────────────────
        // Cache-aware reads (used by the /season command)
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Returns the cached top-<paramref name="count"/> entries for the given category,
        /// refreshing the cache if it is stale.
        /// </summary>
        public static List<SeasonLeaderEntry> GetTopForCategory(string category, int count = 10)
        {
            if (!LogDatabase.IsConfigured) return new List<SeasonLeaderEntry>();

            var now = DateTime.Now;
            if (_topCache.TryGetValue(category, out var cached)
                && now < cached.refreshed.AddMinutes(CacheTtlMinutes))
            {
                return cached.entries.Take(count).ToList();
            }

            var fresh = DatabaseManager.Log.GetSeasonTopForCategory(category, 10);
            _topCache[category] = (fresh, now);
            return fresh.Take(count).ToList();
        }

        /// <summary>
        /// Returns the cached per-player standings, refreshing the cache if stale.
        /// </summary>
        public static SeasonPlayerStanding GetPlayerStanding(uint characterId, string characterName)
        {
            if (!LogDatabase.IsConfigured)
                return new SeasonPlayerStanding { CharacterId = characterId, CharacterName = characterName };

            var now = DateTime.Now;
            if (_playerCache.TryGetValue(characterId, out var cached)
                && now < cached.refreshed.AddSeconds(PlayerCacheTtlSeconds))
            {
                return cached.standing;
            }

            var fresh = DatabaseManager.Log.GetSeasonPlayerStanding(characterId, characterName);
            _playerCache[characterId] = (fresh, now);
            return fresh;
        }

        private static void InvalidatePlayerCache(uint characterId) =>
            _playerCache.Remove(characterId);

        // ─────────────────────────────────────────────────────────────────────
        // Reward delivery  (called from /season claim handler)
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Delivers all unclaimed milestone rewards to the player and marks them claimed.
        /// Sends a summary message to the player when done.
        /// </summary>
        public static void ClaimRewards(Player player)
        {
            if (!LogDatabase.IsConfigured)
            {
                player.SendMessage("[Season] Reward system unavailable — ace_log database not configured.");
                return;
            }

            var unclaimed = DatabaseManager.Log.GetUnclaimedMilestoneLeaders(player.Guid.Full);
            if (unclaimed.Count == 0)
            {
                player.SendMessage("[Season] You have no pending rewards to collect.");
                return;
            }

            var sb = new StringBuilder();
            sb.AppendLine($"[Season] Collecting {unclaimed.Count} pending reward(s)...\n");

            foreach (var row in unclaimed)
            {
                sb.AppendLine($"  Week {row.WeekNumber} — {SeasonConfig.GetCategoryDisplayName(row.Category)} — {row.RankDisplay} Place:");

                // XP grant
                var xpMult = SeasonConfig.GetXpMultiplier(row.Rank);
                player.GrantLevelProportionalXp(xpMult, 0, 0);
                sb.AppendLine($"    +{xpMult * 100:0}% XP to next level");

                // Item grants
                foreach (var (weenieId, qty) in SeasonConfig.GetItems(row.Rank))
                {
                    if (weenieId == 0)
                    {
                        log.Warn($"[Season] Skipping unconfigured reward item (weenieId=0) " +
                                 $"for rank {row.Rank}, category '{row.Category}', milestoneId={row.MilestoneId}.");
                        continue;
                    }

                    var cachedWeenie = DatabaseManager.World.GetCachedWeenie(weenieId);
                    var weenieName = (cachedWeenie?.PropertiesString != null &&
                                      cachedWeenie.PropertiesString.TryGetValue(ACE.Entity.Enum.Properties.PropertyString.Name, out var wn))
                                     ? wn : $"Item #{weenieId}";

                    for (var i = 0; i < qty; i++)
                    {
                        var item = WorldObjectFactory.CreateNewWorldObject(weenieId);
                        if (item == null)
                        {
                            log.Error($"[Season] WorldObjectFactory returned null for weenieId={weenieId}.");
                            continue;
                        }

                        if (!player.TryAddToInventory(item))
                        {
                            log.Warn($"[Season] Could not add {weenieName} to {player.Name}'s inventory (full?). Item destroyed.");
                            item.Destroy();
                        }
                    }

                    sb.AppendLine($"    +{qty}x {weenieName}");
                }

                DatabaseManager.Log.MarkMilestoneLeaderClaimed(row.Id);
            }

            sb.AppendLine("\n[Season] Rewards collected!");
            player.SendMessage(sb.ToString().TrimEnd());
        }

        // ─────────────────────────────────────────────────────────────────────
        // Admin helpers
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>Forces an immediate milestone snapshot regardless of day.  GM use only.</summary>
        public static void ForceMilestone()
        {
            if (!LogDatabase.IsConfigured) return;
            FireWeeklyMilestone();
        }

        /// <summary>Flushes every cached leaderboard entry.  GM use only.</summary>
        public static void ResetCache()
        {
            _topCache.Clear();
            _playerCache.Clear();
            log.Info("[Season] Leaderboard cache cleared by admin.");
        }

        /// <summary>Returns a short status string for the /seasons status command.</summary>
        public static string GetStatusString() =>
            $"[Season] Week: {_currentWeekNumber}  " +
            $"Last milestone: {(_lastMilestoneDatetime == DateTime.MinValue ? "never" : _lastMilestoneDatetime.ToString("yyyy-MM-dd HH:mm"))}  " +
            $"Cache entries: {_topCache.Count} categories, {_playerCache.Count} players  " +
            $"Active streaks in memory: {_currentStreaks.Count(kv => kv.Value > 0)}";
    }
}
