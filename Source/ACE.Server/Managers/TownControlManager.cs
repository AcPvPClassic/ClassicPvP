using System;
using System.Collections.Generic;

using log4net;

using ACE.Database;
using ACE.Database.Models.Log;

namespace ACE.Server.Managers
{
    /// <summary>
    /// Manages in-memory state for Town Control.  All hot-path reads go through
    /// the in-memory dictionaries; database is only hit during Initialize() and
    /// on explicit saves.
    /// </summary>
    public static class TownControlManager
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static bool IsInitialized { get; private set; } = false;

        // --- In-memory caches ---
        private static readonly Dictionary<ushort, TownControlTown>  _towns  = new Dictionary<ushort, TownControlTown>();
        private static readonly Dictionary<ushort, TownControlEvent> _latestEventByTown  = new Dictionary<ushort, TownControlEvent>();

        // Maps attackerId → list of latest events by town (for respite checks)
        private static readonly Dictionary<uint, List<TownControlEvent>> _latestEventByAttacker = new Dictionary<uint, List<TownControlEvent>>();

        // -----------------------------------------------------------------------
        // Initialization
        // -----------------------------------------------------------------------

        public static void Initialize()
        {
            try
            {
                _towns.Clear();
                _latestEventByTown.Clear();
                _latestEventByAttacker.Clear();

                var towns = DatabaseManager.Log.GetAllTownControlTowns();
                foreach (var town in towns)
                    _towns[town.TownId] = town;

                // Load the latest event per town
                foreach (var town in towns)
                {
                    var latest = DatabaseManager.Log.GetLatestTownControlEventByTownId(town.TownId);
                    if (latest != null)
                        _latestEventByTown[town.TownId] = latest;
                }

                // Build attacker-indexed cache from existing latest-by-town events
                foreach (var evt in _latestEventByTown.Values)
                    AddToAttackerCache(evt);

                IsInitialized = true;
                log.Info($"[TownControl] TownControlManager initialized with {_towns.Count} town(s).");
            }
            catch (Exception ex)
            {
                log.Error($"[TownControl] TownControlManager.Initialize failed. Ex: {ex}");
            }
        }

        // -----------------------------------------------------------------------
        // Town reads
        // -----------------------------------------------------------------------

        public static TownControlTown GetTown(ushort townId)
        {
            _towns.TryGetValue(townId, out var town);
            return town;
        }

        public static IReadOnlyCollection<TownControlTown> GetAllTowns() => _towns.Values;

        // -----------------------------------------------------------------------
        // Town writes (cache + DB)
        // -----------------------------------------------------------------------

        /// <summary>
        /// Updates the town's mutable state both in cache and in the database.
        /// </summary>
        public static void SaveTown(TownControlTown town)
        {
            _towns[town.TownId] = town;
            DatabaseManager.Log.UpdateTownControlTown(town);
        }

        // -----------------------------------------------------------------------
        // Event reads
        // -----------------------------------------------------------------------

        /// <summary>
        /// Returns the most recent event for a town from the in-memory cache.
        /// Falls back to DB if the manager has not been initialized yet.
        /// </summary>
        public static TownControlEvent GetLatestEventByTown(ushort townId)
        {
            if (IsInitialized)
            {
                _latestEventByTown.TryGetValue(townId, out var evt);
                return evt;
            }
            // Fallback: hit DB (should not happen in production)
            return DatabaseManager.Log.GetLatestTownControlEventByTownId(townId);
        }

        /// <summary>
        /// Returns the most recent event for a specific attacker+town pair from cache.
        /// Falls back to DB if not yet initialized.
        /// </summary>
        public static TownControlEvent GetLatestEventByAttacker(uint attackerId, ushort townId)
        {
            if (IsInitialized)
            {
                if (_latestEventByAttacker.TryGetValue(attackerId, out var list))
                    return list.Find(e => e.TownId == townId);
                return null;
            }
            return DatabaseManager.Log.GetLatestTownControlEventByAttacker(attackerId, townId);
        }

        // -----------------------------------------------------------------------
        // Event writes (cache + DB)
        // -----------------------------------------------------------------------

        /// <summary>
        /// Inserts a new conflict event into the DB, caches it, and returns the populated record.
        /// </summary>
        public static TownControlEvent StartEvent(ushort townId, uint attackerId, string attackerClanName,
            uint? defenderId, string defenderClanName)
        {
            var evt = DatabaseManager.Log.StartTownControlEvent(townId, attackerId, attackerClanName, defenderId, defenderClanName);
            if (evt != null)
            {
                _latestEventByTown[townId] = evt;
                UpdateAttackerCache(evt);
            }
            return evt;
        }

        /// <summary>
        /// Persists updates to an existing event both to DB and cache.
        /// </summary>
        public static void SaveEvent(TownControlEvent evt)
        {
            DatabaseManager.Log.UpdateTownControlEvent(evt);
            _latestEventByTown[evt.TownId] = evt;
            UpdateAttackerCache(evt);
        }

        // -----------------------------------------------------------------------
        // Internal cache helpers
        // -----------------------------------------------------------------------

        private static void AddToAttackerCache(TownControlEvent evt)
        {
            if (!_latestEventByAttacker.TryGetValue(evt.AttackerId, out var list))
            {
                list = new List<TownControlEvent>();
                _latestEventByAttacker[evt.AttackerId] = list;
            }
            list.Add(evt);
        }

        private static void UpdateAttackerCache(TownControlEvent evt)
        {
            if (!_latestEventByAttacker.TryGetValue(evt.AttackerId, out var list))
            {
                list = new List<TownControlEvent>();
                _latestEventByAttacker[evt.AttackerId] = list;
            }
            list.RemoveAll(e => e.TownId == evt.TownId);
            list.Add(evt);
        }

        // -----------------------------------------------------------------------
        // Anomaly recovery — called by Landblock every ~30 s on TC landblocks
        // -----------------------------------------------------------------------

        /// <summary>
        /// If a conflict event is more than 2 minutes past its expiry with no resolution recorded,
        /// force-ends it as a defender victory (no rewards).  This handles cases where the normal
        /// Creature_Tick/Death path failed to close the event.
        /// </summary>
        public static void RecoverStaleEvent(ushort townId, Action<string> logWarningCallback = null)
        {
            var latestEvent = GetLatestEventByTown(townId);
            if (latestEvent == null) return;

            // Only care about events that haven't ended
            if (latestEvent.EventEndTime.HasValue || latestEvent.IsAttackSuccess.HasValue) return;

            var town = GetTown(townId);
            if (town == null) return;

            var expiry = latestEvent.EventStartTime.Value.AddSeconds(town.ConflictLength);
            if (DateTime.UtcNow <= expiry.AddMinutes(2)) return;

            // Force defender win
            var msg = $"[TownControl] Recovering stale event for {town.TownName} (EventId={latestEvent.EventId}). Forcing defender win.";
            log.Warn(msg);
            logWarningCallback?.Invoke(msg);

            town.IsInConflict = false;
            SaveTown(town);

            latestEvent.IsAttackSuccess = false;
            latestEvent.EventEndTime    = DateTime.UtcNow;
            SaveEvent(latestEvent);
        }
    }
}
