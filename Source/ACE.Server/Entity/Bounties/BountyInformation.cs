using System;
using System.Collections.Generic;

namespace ACE.Server.Entity.Bounties
{
    /// <summary>
    /// Per-player lifetime and daily bounty statistics.
    /// Serialized to JSON and stored in PropertyString.BountyInformationsSerialized.
    /// </summary>
    public class BountyInformation
    {
        public Dictionary<uint, BountyTargetInfo> BountyTargets { get; set; } = new Dictionary<uint, BountyTargetInfo>();

        // --- Lifelong stats (never reset) ---
        public uint TotalBountiesCompleted              { get; set; } = 0;
        public uint TotalBountyExpirationsCount         { get; set; } = 0;
        public uint TotalHighPriorityBountiesCompleted  { get; set; } = 0;
        public uint TotalDamageDealtToBountyTargets     { get; set; } = 0;
        public uint TotalDamageReceived                 { get; set; } = 0;
        public uint TotalKillStreakCompletions           { get; set; } = 0;
        public uint HighestKillStreakBroken              { get; set; } = 0;

        // --- Daily-reset stats ---
        public DateTime LastBountyQuestResetDate                  { get; set; } = DateTime.MinValue;
        public HashSet<uint> UniqueBountyTargets                  { get; set; } = new HashSet<uint>();
        public Dictionary<uint, uint> RepeatKillCounts            { get; set; } = new Dictionary<uint, uint>();
        public Dictionary<uint, uint> DailyTargetDamageDealt      { get; set; } = new Dictionary<uint, uint>();
        public uint TotalDailyHighPriorityBountiesCompleted        { get; set; } = 0;
        public uint TotalDailyDamageDealt                          { get; set; } = 0;
        public List<DateTime> BountyCompletionTimestamps           { get; set; } = new List<DateTime>();

        public double GetLastCompletedTimestamp(uint targetGuid)
        {
            return BountyTargets.TryGetValue(targetGuid, out var target) && target != null
                ? target.LastCompletedTimestamp
                : -1;
        }

        public BountyTargetInfo GetOrCreateTarget(uint targetGuid)
        {
            if (!BountyTargets.TryGetValue(targetGuid, out var target) || target == null)
            {
                target = new BountyTargetInfo { TargetGuid = targetGuid };
                BountyTargets[targetGuid] = target;
            }

            return target;
        }

        public void PruneEmptyTargets()
        {
            var emptyTargetGuids = new List<uint>();

            foreach (var entry in BountyTargets)
            {
                if (IsEmpty(entry.Value))
                    emptyTargetGuids.Add(entry.Key);
            }

            foreach (var targetGuid in emptyTargetGuids)
                BountyTargets.Remove(targetGuid);
        }

        private static bool IsEmpty(BountyTargetInfo target)
        {
            return target == null ||
                   target.TotalCompletions == 0 &&
                   target.TotalExpirations == 0 &&
                   target.TotalHighPriorityCompletions == 0 &&
                   target.TotalDamageReceived == 0 &&
                   target.TotalKillStreakCompletions == 0 &&
                   target.HighestKillStreakBroken == 0 &&
                   target.LastCompletedTimestamp == -1;
        }
    }
}
