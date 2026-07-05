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
        private static DateTime _lastMilestoneDatetime  = DateTime.MinValue;
        private static ushort   _currentWeekNumber      = 0;

        // ── Daily status state ────────────────────────────────────────────────
        private static DateTime _lastDailyStatusDatetime = DateTime.MinValue;

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

            // Daily status: fire once per calendar day on any day.
            if (DateTime.Now.Date > _lastDailyStatusDatetime.Date)
            {
                FireDailyStatus();
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

            // In-game broadcast — keep it terse (#1 per category); the full top-10
            // would flood in-game chat.
            PlayerManager.BroadcastToAll(new GameMessageSystemChat(msg, ChatMessageType.WorldBroadcast));

            // Discord webhook — post the full top-10-per-category announcement with
            // the reward legend (split across messages to fit Discord's size limit).
            AnnounceLeaderboards(_currentWeekNumber);

            // Flush the whole top cache so next query is fresh
            _topCache.Clear();

            log.Info($"[Season] Week {_currentWeekNumber} milestone complete (id={milestoneId}).");
        }

        private static void FireDailyStatus()
        {
            _lastDailyStatusDatetime = DateTime.Now;

            var msg = BuildDailyStatusMessage();
            if (msg == null) return;

            DiscordWebhookManager.SendSeasonMilestone(msg);
            log.Info("[Season] Daily status posted to Discord.");
        }

        private static string BuildDailyStatusMessage()
        {
            var day = RollingLevelCapManager.GetCurrentSeasonDay();
            if (day < 0) return null;   // season not started

            var daysRemaining = Math.Max(0, RollingLevelCapManager.SeasonEndDay - day);

            // XP cap — current and tomorrow
            var xpCap         = RollingLevelCapManager.GetCurrentXpCap();
            var tomorrowXpCap = RollingLevelCapManager.ComputeXpCapForDay(day + 1);
            var capNow        = RollingLevelCapManager.GetCapDescription(xpCap);
            var capTomorrow   = RollingLevelCapManager.GetCapDescription(tomorrowXpCap);

            // XP modifier
            var xpMod = PropertyManager.GetDouble("xp_modifier").Item;

            var sb = new StringBuilder();
            // Display is 1-based (opening day is "Day 1"); the raw `day` above stays
            // 0-based for XP-cap phase math and daysRemaining.
            sb.AppendLine($"📅 **Season Day {day + 1} of {RollingLevelCapManager.SeasonEndDay + 1}**  ·  Week {_currentWeekNumber}  ·  {daysRemaining} days remaining");
            sb.AppendLine();
            sb.AppendLine("**XP Cap**");
            sb.AppendLine($"  Current:   {capNow}");
            sb.AppendLine($"  Tomorrow:  {capTomorrow}");
            sb.AppendLine();
            sb.AppendLine($"**XP Modifier**  ·  {xpMod:0.00}×");
            sb.AppendLine();
            sb.AppendLine("**Current Leaders**");

            // Overall first, then each scored category
            var categoriesToShow = new[] { SeasonConfig.Cat_Overall }
                .Concat(SeasonConfig.ScoredCategories);

            foreach (var cat in categoriesToShow)
            {
                var top = GetTopForCategory(cat, 1);
                var displayName = SeasonConfig.GetCategoryDisplayName(cat);
                if (top.Count == 0)
                {
                    sb.AppendLine($"  {displayName}:  —");
                }
                else
                {
                    var leader = top[0];
                    sb.AppendLine($"  {displayName}:  {leader.CharacterName}  ·  {leader.ScoreDisplay}");
                }
            }

            return sb.ToString().TrimEnd();
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
            sb.AppendLine("Use /season rewards to collect your weekly reward items!");
            return sb.ToString().TrimEnd();
        }

        // ─────────────────────────────────────────────────────────────────────
        // Full top-10 leaderboard announcement (Discord)
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Discord content limit is 2000 chars per message; a full top-10 across every
        /// category is far larger, so blocks are packed into messages up to this size.
        /// </summary>
        private const int DiscordMessageBudget = 1900;

        /// <summary>
        /// Posts the full top-10 for every category (Overall first) plus the weekly
        /// reward legend to the Season webhook, split across as few messages as the
        /// Discord size limit allows.  Called automatically at the weekly milestone
        /// and on demand via <c>/seasons announce</c>.
        /// </summary>
        public static void AnnounceLeaderboards(ushort week)
        {
            if (!LogDatabase.IsConfigured) return;

            var messages = BuildLeaderboardAnnouncement(week);
            if (messages.Count == 0) return;

            DiscordWebhookManager.SendSeasonMilestoneMulti(messages);
            log.Info($"[Season] Posted Week {week} top-10 leaderboard announcement to Discord ({messages.Count} message(s)).");
        }

        /// <summary>
        /// Builds the ordered list of Discord messages for the weekly announcement:
        /// a title, one block per category (Overall + each scored category), and a
        /// reward legend.  Category blocks are packed together up to the Discord size
        /// budget so we send as few messages as possible.
        /// </summary>
        private static List<string> BuildLeaderboardAnnouncement(ushort week)
        {
            // One self-contained block per category (header + up to 10 entries).
            var blocks = new List<string>();

            var categories = new[] { SeasonConfig.Cat_Overall }
                .Concat(SeasonConfig.ScoredCategories);

            foreach (var cat in categories)
            {
                var top = GetTopForCategory(cat, 10);
                if (top.Count == 0) continue;

                var block = new StringBuilder();
                block.AppendLine($"**{SeasonConfig.GetCategoryDisplayName(cat)}**");
                foreach (var entry in top)
                    block.AppendLine($"`{entry.Rank,2}.` {entry.CharacterName} — {entry.ScoreDisplay}");

                blocks.Add(block.ToString().TrimEnd());
            }

            var messages = new List<string>();

            // Title always leads.
            messages.Add(
                $"🏆 **Season Week {week} — Weekly Leaderboards** 🏆\n" +
                "Top 10 in every category. Rewards are now claimable in-game with `/season rewards`.");

            // Pack category blocks into as few messages as the budget allows.
            var current = new StringBuilder();
            foreach (var block in blocks)
            {
                if (current.Length > 0 && current.Length + block.Length + 2 > DiscordMessageBudget)
                {
                    messages.Add(current.ToString().TrimEnd());
                    current.Clear();
                }

                if (current.Length > 0) current.AppendLine();
                current.AppendLine(block);
            }
            if (current.Length > 0)
                messages.Add(current.ToString().TrimEnd());

            // Reward legend closes the announcement.
            messages.Add(BuildRewardLegend());

            return messages;
        }

        /// <summary>
        /// Builds the per-rank reward legend shown at the end of the announcement.
        /// Reward tiers apply per category, so this is a single shared block.
        /// </summary>
        private static string BuildRewardLegend()
        {
            var sb = new StringBuilder();
            sb.AppendLine("**Weekly Rewards** — by finishing rank, in every category");
            sb.AppendLine($"🥇 1st — {FormatRewardBundle(1)}");
            sb.AppendLine($"🥈 2nd — {FormatRewardBundle(2)}");
            sb.AppendLine($"🥉 3rd — {FormatRewardBundle(3)}");
            sb.AppendLine($"🎖️ 4th–10th — {FormatRewardBundle(4)}");
            sb.Append("Collect yours in-game with `/season rewards`.");
            return sb.ToString();
        }

        /// <summary>Formats a single rank's reward bundle (XP grant + item list) for display.</summary>
        private static string FormatRewardBundle(int rank)
        {
            var parts = new List<string> { $"+{SeasonConfig.GetXpMultiplier(rank) * 100:0}% XP" };

            foreach (var (weenieId, qty) in SeasonConfig.GetItems(rank))
            {
                if (weenieId == 0) continue;
                parts.Add($"{qty}× {GetWeenieName(weenieId)}");
            }

            return string.Join(" · ", parts);
        }

        /// <summary>
        /// Resolves a weenie's display name from the world cache, falling back to
        /// <c>Item #id</c> when the weenie is not cached.
        /// </summary>
        private static string GetWeenieName(uint weenieId)
        {
            var cachedWeenie = DatabaseManager.World.GetCachedWeenie(weenieId);
            return (cachedWeenie?.PropertiesString != null &&
                    cachedWeenie.PropertiesString.TryGetValue(ACE.Entity.Enum.Properties.PropertyString.Name, out var wn))
                   ? wn : $"Item #{weenieId}";
        }

        /// <summary>
        /// Resolves a weenie's MaxStackSize from the world cache, returning 1 for
        /// non-stackable items or when the weenie is not cached.
        /// </summary>
        private static int GetWeenieMaxStackSize(uint weenieId)
        {
            var cachedWeenie = DatabaseManager.World.GetCachedWeenie(weenieId);
            if (cachedWeenie?.PropertiesInt != null &&
                cachedWeenie.PropertiesInt.TryGetValue(ACE.Entity.Enum.Properties.PropertyInt.MaxStackSize, out var max) &&
                max > 1)
                return max;

            return 1;
        }

        /// <summary>
        /// Computes how many inventory slots a rank's reward bundle needs, counting one
        /// slot per stack (a quantity larger than an item's MaxStackSize spills into
        /// additional stacks).  Mirrors how <see cref="GrantRewardStacks"/> lays items out,
        /// so the pre-check in <see cref="ClaimRewards"/> is exact.
        /// </summary>
        private static int GetRequiredInventorySlots(int rank)
        {
            var slots = 0;

            foreach (var (weenieId, qty) in SeasonConfig.GetItems(rank))
            {
                if (weenieId == 0 || qty <= 0) continue;

                var maxStack = GetWeenieMaxStackSize(weenieId);
                slots += (qty + maxStack - 1) / maxStack; // ceil(qty / maxStack)
            }

            return slots;
        }

        /// <summary>
        /// Grants <paramref name="qty"/> of a weenie to <paramref name="player"/> as
        /// properly-sized, client-networked stacks, splitting across multiple stacks
        /// when <paramref name="qty"/> exceeds the item's MaxStackSize.
        /// Returns the quantity actually delivered; a shortfall means the inventory filled up.
        /// </summary>
        /// <remarks>
        /// Uses <see cref="Player.TryCreateInInventoryWithNetworking(WorldObject)"/> — the same
        /// path the PK-quest rewards use — so the client receives the create/stack packets
        /// immediately.  The old code created one WorldObject per unit and called
        /// <c>TryAddToInventory</c> directly, which (a) handed out unstacked singletons and
        /// (b) never sent the create packet, so items appeared bugged until a relog forced a
        /// full inventory re-send.
        /// </remarks>
        private static int GrantRewardStacks(Player player, uint weenieId, int qty, string weenieName)
        {
            var delivered = 0;

            while (delivered < qty)
            {
                var item = WorldObjectFactory.CreateNewWorldObject(weenieId);
                if (item == null)
                {
                    log.Error($"[Season] WorldObjectFactory returned null for weenieId={weenieId}.");
                    break;
                }

                var maxStack = Math.Max(1, (int)(item.MaxStackSize ?? 1));
                var thisStack = Math.Min(qty - delivered, maxStack);
                if (thisStack > 1)
                    item.SetStackSize(thisStack);

                if (!player.TryCreateInInventoryWithNetworking(item))
                {
                    log.Warn($"[Season] Could not add {weenieName} to {player.Name}'s inventory " +
                             $"(full?). {qty - delivered} not delivered.");
                    item.Destroy();
                    break;
                }

                delivered += thisStack;
            }

            return delivered;
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

            var claimed = 0;
            var deferred = 0;

            foreach (var row in unclaimed)
            {
                var header = $"  Week {row.WeekNumber} — {SeasonConfig.GetCategoryDisplayName(row.Category)} — {row.RankDisplay} Place:";

                // Pre-check: a bundle is delivered all-or-nothing.  If the whole bundle
                // won't fit, leave it unclaimed (no XP, no items) so nothing is lost, and
                // tell the player how much room to free.  Re-checked live so earlier
                // deliveries this run are accounted for.
                var requiredSlots = GetRequiredInventorySlots(row.Rank);
                if (player.GetFreeInventorySlots() < requiredSlots)
                {
                    deferred++;
                    sb.AppendLine($"{header} DEFERRED");
                    sb.AppendLine($"    Needs {requiredSlots} free inventory slot(s) — free up space and run /season rewards again.");
                    continue;
                }

                sb.AppendLine(header);

                // XP grant
                var xpMult = SeasonConfig.GetXpMultiplier(row.Rank);
                player.GrantLevelProportionalXp(xpMult, 0, 0);
                sb.AppendLine($"    +{xpMult * 100:0}% XP to next level");

                // Item grants — guaranteed to fit by the pre-check above.
                foreach (var (weenieId, qty) in SeasonConfig.GetItems(row.Rank))
                {
                    if (weenieId == 0)
                    {
                        log.Warn($"[Season] Skipping unconfigured reward item (weenieId=0) " +
                                 $"for rank {row.Rank}, category '{row.Category}', milestoneId={row.MilestoneId}.");
                        continue;
                    }

                    var weenieName = GetWeenieName(weenieId);
                    var delivered = GrantRewardStacks(player, weenieId, qty, weenieName);

                    if (delivered < qty)
                        sb.AppendLine($"    +{delivered}x {weenieName} (inventory full — {qty - delivered} not delivered)");
                    else
                        sb.AppendLine($"    +{qty}x {weenieName}");
                }

                DatabaseManager.Log.MarkMilestoneLeaderClaimed(row.Id);
                claimed++;
            }

            if (deferred > 0)
                sb.AppendLine($"\n[Season] Collected {claimed} reward(s); {deferred} deferred for inventory space. " +
                              "Free up slots and run /season rewards again to collect the rest.");
            else
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

        /// <summary>
        /// Re-posts the full top-10 leaderboard announcement to Discord for the current
        /// week without capturing a new milestone snapshot.  GM use only.
        /// </summary>
        public static void AnnounceLeaderboardsNow() => AnnounceLeaderboards(_currentWeekNumber);

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
