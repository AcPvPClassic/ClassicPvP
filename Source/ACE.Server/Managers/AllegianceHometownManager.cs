using System;
using System.Collections.Generic;
using System.Linq;

using log4net;

using ACE.Common;
using ACE.Database;
using ACE.Database.Models.Log;
using ACE.Entity.Enum;
using ACE.Server.Entity.AllegianceHometown;
using ACE.Server.Factories;
using ACE.Server.Network.GameMessages.Messages;
using ACE.Server.WorldObjects;

namespace ACE.Server.Managers
{
    /// <summary>
    /// Manages in-memory state for Allegiance Hometown capture.
    /// All hot-path reads go through in-memory dictionaries; DB is only hit during
    /// Initialize() and on explicit saves.
    /// </summary>
    public static class AllegianceHometownManager
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static bool IsInitialized { get; private set; }

        // town_id → current DB row (mutable, kept in sync with DB)
        private static readonly Dictionary<byte, AllegianceHometownTown> _towns = new();

        // monarch_id → set of town_ids that allegiance currently owns
        private static readonly Dictionary<uint, HashSet<byte>> _ownedByMonarch = new();

        // monarch_id → active conflict town_ids (phase 1 or 2 in progress)
        private static readonly Dictionary<uint, HashSet<byte>> _activeConflictsByMonarch = new();

        // cooldowns: (monarchId, townId) → DateTime when cooldown expires
        // covers both phase-1-timeout cooldowns and phase-2-failure cooldowns
        private static readonly Dictionary<(uint, byte), DateTime> _attackerCooldowns = new();

        // per-town: monarch_id → cooldown expiry after a successful capture (24h default)
        private static readonly Dictionary<byte, DateTime> _captureProtection = new();

        // latest event per town (for audit log cache)
        private static readonly Dictionary<byte, AllegianceHometownEvent> _latestEventByTown = new();

        // town_id → active Phase 2 creature proxy world object
        private static readonly Dictionary<byte, BindstoneCreatureProxy> _phase2Proxies = new();

        // town_id → real Bindstone WO cloaked during Phase 2
        private static readonly Dictionary<byte, WorldObjects.Bindstone> _phase2CloakedBindstones = new();

        // monarch IDs permanently banned from attacking towns
        private static readonly System.Collections.Generic.HashSet<uint> _blacklist = new();
        private static readonly System.Collections.Generic.Dictionary<uint, ACE.Database.Models.Log.AllegianceHometownBlacklist> _blacklistEntries = new();

        public const int MaxSimultaneousAttacks = 2;

        // -----------------------------------------------------------------------
        // Initialization
        // -----------------------------------------------------------------------

        public static void Initialize()
        {
            try
            {
                _towns.Clear();
                _ownedByMonarch.Clear();
                _activeConflictsByMonarch.Clear();
                _attackerCooldowns.Clear();
                _captureProtection.Clear();
                _latestEventByTown.Clear();
                _phase2Proxies.Clear();
                _blacklist.Clear();
                _blacklistEntries.Clear();

                foreach (var bl in DatabaseManager.Log.GetAllAllegianceHometownBlacklist())
                {
                    _blacklist.Add(bl.MonarchId);
                    _blacklistEntries[bl.MonarchId] = bl;
                }

                var rows = DatabaseManager.Log.GetAllAllegianceHometownTowns();

                // Index by town_id; fill in any missing rows from registry
                foreach (var entry in AllegianceHometownRegistry.All.Values)
                {
                    var row = rows.FirstOrDefault(r => r.TownId == entry.TownId);
                    if (row == null)
                    {
                        row = new AllegianceHometownTown { TownId = entry.TownId, TownName = entry.TownName };
                    }
                    _towns[entry.TownId] = row;

                    // Rebuild ownership index
                    if (row.OwnerMonarchId.HasValue)
                        GetOrCreateSet(_ownedByMonarch, row.OwnerMonarchId.Value).Add(row.TownId);

                    // Any town that was in-conflict when server shut down: clear conflict state
                    // (Phase 1/2 cannot survive a server restart gracefully)
                    if (row.ConflictPhase != 0)
                    {
                        row.ConflictPhase             = 0;
                        row.ConflictAttackerMonarchId = null;
                        row.ConflictAttackerName      = null;
                        row.ConflictStartTime         = null;
                        row.Phase2StartTime           = null;
                        DatabaseManager.Log.UpdateAllegianceHometownTown(row);
                    }

                    // Restore capture protection: if captured_at is within the protection window, honour it
                    if (row.CapturedAt.HasValue)
                    {
                        var protectionWindowHours = PropertyManager.GetDouble("ah_capture_protection_hours", 24.0).Item;
                        var expiry = row.CapturedAt.Value.AddHours(protectionWindowHours);
                        if (expiry > DateTime.UtcNow)
                            _captureProtection[row.TownId] = expiry;
                    }
                }

                IsInitialized = true;
                log.Info($"[AllegianceHometown] Initialized with {_towns.Count} town(s).");
            }
            catch (Exception ex)
            {
                log.Error($"[AllegianceHometown] Initialize failed. Ex: {ex}");
            }
        }

        // -----------------------------------------------------------------------
        // Town reads
        // -----------------------------------------------------------------------

        public static AllegianceHometownTown GetTown(byte townId)
        {
            _towns.TryGetValue(townId, out var t);
            return t;
        }

        public static IReadOnlyCollection<AllegianceHometownTown> GetAllTowns() => _towns.Values;

        public static IReadOnlyCollection<byte> GetOwnedTownIds(uint monarchId)
        {
            _ownedByMonarch.TryGetValue(monarchId, out var s);
            return (IReadOnlyCollection<byte>)s ?? Array.Empty<byte>();
        }

        public static int GetOwnedTownCount(uint monarchId)
        {
            _ownedByMonarch.TryGetValue(monarchId, out var s);
            return s?.Count ?? 0;
        }

        public static bool IsInConflict(byte townId)
        {
            _towns.TryGetValue(townId, out var t);
            return t != null && t.ConflictPhase != 0;
        }

        public static bool IsTownProtected(byte townId) =>
            _captureProtection.TryGetValue(townId, out var exp) && exp > DateTime.UtcNow;

        public static bool ClearTownProtection(byte townId)
        {
            return _captureProtection.Remove(townId);
        }

        /// <summary>
        /// Returns a human-readable block reason if this monarch cannot attack the town due to
        /// blacklist, protection, or cooldown. Returns null if no persistent block exists.
        /// Does NOT modify any state.
        /// </summary>
        public static string GetAttackBlockReason(byte townId, uint attackerMonarchId)
        {
            if (!_towns.TryGetValue(townId, out var town)) return null;

            if (_blacklist.Contains(attackerMonarchId))
                return "Your allegiance has been suspended from hometown warfare.";

            if (town.OwnerMonarchId == attackerMonarchId)
                return null; // owner — handled separately by caller

            if (IsTownProtected(townId))
            {
                var remaining = _captureProtection[townId] - DateTime.UtcNow;
                return $"This town cannot be attacked for another {FormatTimeSpan(remaining)}.";
            }

            if (HasAttackerCooldown(attackerMonarchId, townId, out var cd))
                return $"Your allegiance cannot attack {town.TownName} for another {FormatTimeSpan(cd)}.";

            return null;
        }

        // -----------------------------------------------------------------------
        // Cooldown checks
        // -----------------------------------------------------------------------

        public static bool HasAttackerCooldown(uint monarchId, byte townId, out TimeSpan remaining)
        {
            if (_attackerCooldowns.TryGetValue((monarchId, townId), out var exp) && exp > DateTime.UtcNow)
            {
                remaining = exp - DateTime.UtcNow;
                return true;
            }
            remaining = TimeSpan.Zero;
            return false;
        }

        public static int GetActiveConflictCount(uint monarchId)
        {
            _activeConflictsByMonarch.TryGetValue(monarchId, out var s);
            return s?.Count ?? 0;
        }

        // -----------------------------------------------------------------------
        // Phase 1 — start / tick / timeout
        // -----------------------------------------------------------------------

        /// <summary>
        /// Attempts to start Phase 1 for the given attacker allegiance on the given town.
        /// Returns false (with reason message) if blocked by cooldowns, protection, or conflict limits.
        /// </summary>
        public static bool TryStartPhase1(byte townId, uint attackerMonarchId, string attackerName,
            out string failReason)
        {
            failReason = null;

            if (!_towns.TryGetValue(townId, out var town))
            {
                failReason = "Unknown town.";
                return false;
            }

            // Blacklisted allegiances cannot attack
            if (_blacklist.Contains(attackerMonarchId))
            {
                failReason = "Your allegiance has been suspended from hometown warfare.";
                return false;
            }

            // Can't attack your own town
            if (town.OwnerMonarchId == attackerMonarchId)
            {
                failReason = "Your allegiance already owns that town.";
                return false;
            }

            // Town already in conflict
            if (town.ConflictPhase != 0)
            {
                failReason = "That town is already under attack.";
                return false;
            }

            // Town is under capture protection
            if (IsTownProtected(townId))
            {
                var remaining = _captureProtection[townId] - DateTime.UtcNow;
                failReason = $"That town cannot be attacked for another {FormatTimeSpan(remaining)}.";
                return false;
            }

            // Attacker cooldown (phase-1 timeout or phase-2 failure)
            if (HasAttackerCooldown(attackerMonarchId, townId, out var cd))
            {
                failReason = $"Your allegiance cannot attack {town.TownName} for another {FormatTimeSpan(cd)}.";
                return false;
            }

            // Simultaneous attack limit
            if (GetActiveConflictCount(attackerMonarchId) >= MaxSimultaneousAttacks)
            {
                failReason = $"Your allegiance is already attacking {MaxSimultaneousAttacks} towns.";
                return false;
            }

            var defenderName = town.OwnerAllegianceName;

            town.ConflictPhase             = 1;
            town.ConflictAttackerMonarchId = attackerMonarchId;
            town.ConflictAttackerName      = attackerName;
            town.ConflictStartTime         = DateTime.UtcNow;
            town.Phase2StartTime           = null;
            SaveTown(town);

            GetOrCreateSet(_activeConflictsByMonarch, attackerMonarchId).Add(townId);

            // Log event
            var evt = DatabaseManager.Log.StartAllegianceHometownEvent(
                townId, attackerMonarchId, attackerName,
                town.OwnerMonarchId, defenderName);
            if (evt != null)
                _latestEventByTown[townId] = evt;

            // Global announcement
            var ownerPart = town.OwnerMonarchId.HasValue
                ? $" (owned by {defenderName})"
                : " (unowned)";
            GlobalBroadcast($"{attackerName} is assaulting {town.TownName}{ownerPart}! Phase 1 has begun.");

            return true;
        }

        /// <summary>
        /// Called every 5 seconds by the landblock tick for capturable-town landblocks.
        /// Handles Phase 1 accumulation, interruption, and timeout.
        /// Returns true if Phase 2 should now start.
        /// </summary>
        public static Phase1TickResult TickPhase1(byte townId,
            int attackersWithinRange,   // attacking allegiance members within 5m
            bool enemyOnLandblock,      // any non-attacker PK on the landblock
            ref double accumulatedSeconds)
        {
            if (!_towns.TryGetValue(townId, out var town) || town.ConflictPhase != 1)
                return Phase1TickResult.NotActive;

            // Timeout check
            if (DateTime.UtcNow - town.ConflictStartTime.Value > TimeSpan.FromHours(1))
            {
                HandlePhase1Timeout(town);
                accumulatedSeconds = 0;
                return Phase1TickResult.TimedOut;
            }

            if (enemyOnLandblock)
            {
                // Interrupt: reset progress, keep the attempt alive
                accumulatedSeconds = 0;
                return Phase1TickResult.Interrupted;
            }

            if (attackersWithinRange >= 2)
            {
                accumulatedSeconds += 5;
                if (accumulatedSeconds >= 60) // 1 minute (testing)
                    return Phase1TickResult.PhaseComplete;
            }

            return Phase1TickResult.Progressing;
        }

        private static void HandlePhase1Timeout(AllegianceHometownTown town)
        {
            var attackerMonarchId = town.ConflictAttackerMonarchId!.Value;
            var attackerName      = town.ConflictAttackerName;

            // Set 3-hour cooldown
            _attackerCooldowns[(attackerMonarchId, town.TownId)] = DateTime.UtcNow.AddHours(3);

            // Close audit event
            CloseLatestEvent(town.TownId, outcome: 2);

            // Clear conflict state
            ClearConflict(town);

            GlobalBroadcast($"{attackerName} failed to assault {town.TownName}! The attack has been repelled.");
        }

        // -----------------------------------------------------------------------
        // Phase 2 — start / outcome
        // -----------------------------------------------------------------------

        public static void StartPhase2(byte townId)
        {
            if (!_towns.TryGetValue(townId, out var town) || town.ConflictPhase != 1) return;

            town.ConflictPhase  = 2;
            town.Phase2StartTime = DateTime.UtcNow;
            SaveTown(town);

            if (_latestEventByTown.TryGetValue(townId, out var evt))
            {
                evt.Phase2StartTime = town.Phase2StartTime;
                DatabaseManager.Log.UpdateAllegianceHometownEvent(evt);
            }

            GlobalBroadcast($"{town.ConflictAttackerName} has breached Phase 2 in {town.TownName}! The Bind Stone is under attack!");
        }

        /// <summary>Called when bindstone HP reaches 0 — attacker wins.</summary>
        public static void HandleAttackerVictory(byte townId)
        {
            if (!_towns.TryGetValue(townId, out var town)) return;

            var attackerMonarchId = town.ConflictAttackerMonarchId!.Value;
            var attackerName      = town.ConflictAttackerName;
            var prevOwnerName     = town.OwnerAllegianceName;
            var prevOwnerId       = town.OwnerMonarchId;

            // Transfer ownership
            if (prevOwnerId.HasValue)
                GetOrCreateSet(_ownedByMonarch, prevOwnerId.Value).Remove(town.TownId);
            GetOrCreateSet(_ownedByMonarch, attackerMonarchId).Add(town.TownId);

            town.OwnerMonarchId       = attackerMonarchId;
            town.OwnerAllegianceName  = attackerName;
            town.CapturedAt           = DateTime.UtcNow;

            // Apply capture protection
            var protectionHours = PropertyManager.GetDouble("ah_capture_protection_hours", 24.0).Item;
            _captureProtection[town.TownId] = DateTime.UtcNow.AddHours(protectionHours);

            CloseLatestEvent(town.TownId, outcome: 0);

            UncloakPhase2Bindstone(townId);

            // Capture rewards: attackers win, defenders smited
            DistributeRewards(townId, attackerMonarchId, prevOwnerId);

            ClearConflict(town);

            var ownerPart = prevOwnerId.HasValue ? $" from {prevOwnerName}" : "";
            GlobalBroadcast($"{attackerName} has captured {town.TownName}{ownerPart}!");
        }

        /// <summary>Called when Phase 2 timer expires with bindstone still alive — defender wins.</summary>
        public static void HandleDefenderVictory(byte townId)
        {
            if (!_towns.TryGetValue(townId, out var town)) return;

            var attackerMonarchId = town.ConflictAttackerMonarchId!.Value;
            var attackerName      = town.ConflictAttackerName;

            // 6-hour cooldown on the attacking allegiance for this town
            _attackerCooldowns[(attackerMonarchId, town.TownId)] = DateTime.UtcNow.AddHours(6);

            UncloakPhase2Bindstone(townId);

            // Defense rewards: defenders win, attackers smited
            var defenderMonarchId = town.OwnerMonarchId;
            DistributeRewards(townId, defenderMonarchId ?? 0, attackerMonarchId);

            CloseLatestEvent(town.TownId, outcome: 1);
            ClearConflict(town);

            GlobalBroadcast($"{town.TownName} has been defended! {attackerName} failed to capture the town.");
        }

        // -----------------------------------------------------------------------
        // Free claim (unowned town)
        // -----------------------------------------------------------------------

        public static void ClaimTown(byte townId, uint monarchId, string allegianceName)
        {
            if (!_towns.TryGetValue(townId, out var town)) return;

            GetOrCreateSet(_ownedByMonarch, monarchId).Add(town.TownId);

            town.OwnerMonarchId      = monarchId;
            town.OwnerAllegianceName = allegianceName;
            town.CapturedAt          = DateTime.UtcNow;
            SaveTown(town);

            GlobalBroadcast($"{allegianceName} has claimed {town.TownName}!");
        }

        // -----------------------------------------------------------------------
        // Reward distribution
        // -----------------------------------------------------------------------

        /// <summary>
        /// Distributes capture/defense rewards to all eligible PK players currently on
        /// the town landblock (and adjacent landblocks for smite).
        /// <para>
        /// <paramref name="winnerMonarchId"/> identifies whose allegiance wins and receives rewards.
        /// <paramref name="loserMonarchId"/> identifies whose allegiance loses (smited and receives no reward).
        /// </para>
        /// </summary>
        public static void DistributeRewards(byte townId, uint winnerMonarchId, uint? loserMonarchId)
        {
            if (!_towns.TryGetValue(townId, out var town)) return;

            var entry = AllegianceHometownRegistry.GetById(townId);
            if (entry == null) return;

            var mainLb = LandblockManager.GetLandblock(new ACE.Entity.LandblockId(entry.LandblockId), false);
            if (mainLb == null) return;

            // Collect all landblocks in the smite zone (main + adjacent)
            var allLbs = new System.Collections.Generic.List<Entity.Landblock> { mainLb };
            foreach (var adjId in LandblockManager.GetAdjacentIDs(mainLb))
            {
                var adjLb = LandblockManager.GetLandblock(adjId, false);
                if (adjLb != null) allLbs.Add(adjLb);
            }

            // Collect winner players within 100m of the bindstone (main + adjacent landblocks)
            var winnerPlayers = new System.Collections.Generic.List<Player>();
            var bindstonePos  = entry.BindstonePosition;

            foreach (var lb in allLbs)
            {
                foreach (var p in lb.GetPlayers())
                {
                    if (!p.IsPK) continue;
                    var monarchId = p.MonarchId ?? p.Guid.Full;
                    if (monarchId == winnerMonarchId && p.Location.DistanceTo(bindstonePos) <= 100f)
                        winnerPlayers.Add(p);
                }
            }

            // Smite losing allegiance members within 100m of the bindstone
            if (loserMonarchId.HasValue)
            {
                foreach (var lb in allLbs)
                {
                    foreach (var p in lb.GetPlayers())
                    {
                        if (!p.IsPK) continue;
                        var monarchId = p.MonarchId ?? p.Guid.Full;
                        if (monarchId == loserMonarchId.Value && p.Location.DistanceTo(bindstonePos) <= 100f)
                            p.Smite(p);
                    }
                }
            }

            if (winnerPlayers.Count == 0) return;

            // Pool: 40–120 PK Trophies, 10–30 MMDs — split by player count
            int totalTrophies = ThreadSafeRandom.Next(40, 120);
            int totalMmds     = ThreadSafeRandom.Next(10, 30);
            int perTrophies   = Math.Max(1, totalTrophies / winnerPlayers.Count);
            int perMmds       = Math.Max(1, totalMmds     / winnerPlayers.Count);

            foreach (var winner in winnerPlayers)
            {
                // PK Trophies (stackable)
                GiveStacked(winner, CustomWeenieId.PkTrophy, perTrophies);

                // MMDs (stackable)
                GiveStacked(winner, 20630u, perMmds);

                // 1 Phial of Bloody Tears
                GiveSingle(winner, CustomWeenieId.PhialOfBloodyTears);

                // 3 Darkbeat Keys
                for (int k = 0; k < 3; k++)
                    GiveSingle(winner, CustomWeenieId.DarkbeatKey);

                // 10% of XP to next level
                var level = winner.Level ?? 1;
                var xpBand = (long)winner.GetXPBetweenLevels(level, level + 1);
                var bonusXp = (long)Math.Round(xpBand * 0.10);
                if (bonusXp > 0)
                    winner.GrantXP(bonusXp, XpType.Kill, ACE.Entity.Enum.ShareType.None, "hometown capture reward");

                winner.Session.Network.EnqueueSend(new GameMessageSystemChat(
                    $"[Hometown] You received {perTrophies} PK Trophy/Trophies, {perMmds} MMD(s), 1 Phial of Bloody Tears, and 3 Darkbeat Keys for your service!",
                    ChatMessageType.Magic));
            }
        }

        private static void GiveStacked(Player player, uint wcid, int amount)
        {
            var wo = WorldObjectFactory.CreateNewWorldObject(wcid);
            if (wo == null) return;
            wo.SetStackSize(amount);
            if (!player.TryCreateInInventoryWithNetworking(wo))
            {
                // Drop at player's feet if inventory full
                wo.Location = new ACE.Entity.Position(player.Location);
                player.CurrentLandblock?.AddWorldObject(wo);
            }
        }

        private static void GiveSingle(Player player, uint wcid)
        {
            var wo = WorldObjectFactory.CreateNewWorldObject(wcid);
            if (wo == null) return;
            if (!player.TryCreateInInventoryWithNetworking(wo))
            {
                wo.Location = new ACE.Entity.Position(player.Location);
                player.CurrentLandblock?.AddWorldObject(wo);
            }
        }

        // -----------------------------------------------------------------------
        // Phase 2 creature proxy registry
        // -----------------------------------------------------------------------

        public static void RegisterPhase2Proxy(byte townId, BindstoneCreatureProxy proxy)
        {
            _phase2Proxies[townId] = proxy;
        }

        public static void UnregisterPhase2Proxy(byte townId)
        {
            _phase2Proxies.Remove(townId);
        }

        public static BindstoneCreatureProxy GetPhase2Proxy(byte townId)
        {
            _phase2Proxies.TryGetValue(townId, out var proxy);
            return proxy;
        }

        // -----------------------------------------------------------------------
        // Blacklist management
        // -----------------------------------------------------------------------

        public static bool IsBlacklisted(uint monarchId) => _blacklist.Contains(monarchId);

        public static bool AddBlacklist(uint monarchId, string allegianceName, string reason, string addedBy)
        {
            if (_blacklist.Contains(monarchId)) return false;
            var entry = new ACE.Database.Models.Log.AllegianceHometownBlacklist
            {
                MonarchId      = monarchId,
                AllegianceName = allegianceName,
                Reason         = reason,
                AddedBy        = addedBy,
                AddedAt        = DateTime.UtcNow
            };
            _blacklist.Add(monarchId);
            _blacklistEntries[monarchId] = entry;
            DatabaseManager.Log.AddAllegianceHometownBlacklist(entry);
            return true;
        }

        public static bool RemoveBlacklist(uint monarchId)
        {
            if (!_blacklist.Remove(monarchId)) return false;
            _blacklistEntries.Remove(monarchId);
            DatabaseManager.Log.RemoveAllegianceHometownBlacklist(monarchId);
            return true;
        }

        public static System.Collections.Generic.IReadOnlyDictionary<uint, ACE.Database.Models.Log.AllegianceHometownBlacklist> GetBlacklist()
            => _blacklistEntries;

        public static void RegisterPhase2CloakedBindstone(byte townId, WorldObjects.Bindstone bindstone)
        {
            _phase2CloakedBindstones[townId] = bindstone;
        }

        public static void UncloakPhase2Bindstone(byte townId)
        {
            if (_phase2CloakedBindstones.TryGetValue(townId, out var bs))
            {
                bs.Visibility  = false;
                bs.Cloaked     = (bool?)false;
                bs.Ethereal    = (bool?)false;
                bs.NoDraw      = (bool?)false;
                bs.Attackable  = true;
                bs.EnqueueBroadcastPhysicsState();
                // Send CreateObject so all nearby clients who lack it in their tracking get it back
                bs.EnqueueBroadcast(false, new GameMessageCreateObject(bs));
                _phase2CloakedBindstones.Remove(townId);

                // Lift permaload so the landblock can unload normally when empty again
                var lb = bs.CurrentLandblock;
                if (lb != null)
                    lb.Permaload = false;
            }
        }

        // -----------------------------------------------------------------------
        // Helpers
        // -----------------------------------------------------------------------

        public static void SaveTown(AllegianceHometownTown town)
        {
            _towns[town.TownId] = town;
            DatabaseManager.Log.UpdateAllegianceHometownTown(town);
        }

        private static void ClearConflict(AllegianceHometownTown town)
        {
            if (town.ConflictAttackerMonarchId.HasValue)
                GetOrCreateSet(_activeConflictsByMonarch, town.ConflictAttackerMonarchId.Value).Remove(town.TownId);

            town.ConflictPhase             = 0;
            town.ConflictAttackerMonarchId = null;
            town.ConflictAttackerName      = null;
            town.ConflictStartTime         = null;
            town.Phase2StartTime           = null;
            SaveTown(town);
        }

        private static void CloseLatestEvent(byte townId, byte outcome)
        {
            if (!_latestEventByTown.TryGetValue(townId, out var evt)) return;
            evt.EventEndTime = DateTime.UtcNow;
            evt.Outcome      = outcome;
            DatabaseManager.Log.UpdateAllegianceHometownEvent(evt);
        }

        private static void GlobalBroadcast(string message)
        {
            PlayerManager.BroadcastToAll(
                new GameMessageSystemChat(message, ChatMessageType.WorldBroadcast));
        }

        private static HashSet<T> GetOrCreateSet<TKey, T>(Dictionary<TKey, HashSet<T>> dict, TKey key)
        {
            if (!dict.TryGetValue(key, out var set))
            {
                set = new HashSet<T>();
                dict[key] = set;
            }
            return set;
        }

        public static string FormatTimeSpan(TimeSpan ts)
        {
            if (ts.TotalHours >= 1)
                return $"{(int)ts.TotalHours}h {ts.Minutes}m";
            return $"{ts.Minutes}m {ts.Seconds}s";
        }

        // -----------------------------------------------------------------------
        // HP formula
        // -----------------------------------------------------------------------

        /// <summary>
        /// Computes the bindstone's starting HP based on the current level cap.
        /// Designed so 3 players attacking unopposed can destroy it in ~22 minutes.
        /// </summary>
        public static int ComputeBindstoneHp()
        {
            var xpCap    = RollingLevelCapManager.GetCurrentXpCap();
            int levelCap = RollingLevelCapManager.GetCurrentLevelCap(xpCap);
            float dpsPerPlayer = Math.Clamp(25f + 75f * (levelCap - 15f) / 115f, 25f, 100f);
            return (int)(dpsPerPlayer * 3f * 1350f);
        }

        /// <summary>
        /// Damage/heal amount applied to the bindstone per player kill during Phase 2 (5% of max HP).
        /// </summary>
        public static int GetKillEffect(int bindstoneMaxHp) => (int)(bindstoneMaxHp * 0.05f);

        /// <summary>
        /// Distance-based damage multiplier for attacks on the bindstone.
        /// 100% at ≤5m, halves every 5m beyond that.
        /// </summary>
        public static float GetDistanceMultiplier(float distanceMeters)
        {
            if (distanceMeters <= 5f) return 1f;
            return MathF.Pow(0.5f, (distanceMeters - 5f) / 5f);
        }
    }

    public enum Phase1TickResult
    {
        NotActive,
        Progressing,
        Interrupted,
        TimedOut,
        PhaseComplete,
    }
}
