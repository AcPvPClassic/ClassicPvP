using System;
using System.Linq;
using ACE.Common;
using ACE.Server.Entity;
using ACE.Server.Entity.Bounties;
using ACE.Server.Managers;
using Newtonsoft.Json;

namespace ACE.Server.WorldObjects
{
    partial class Player
    {
        private BountyInformation _bountyInformation;

        private BountyInformation BountyInformation
        {
            get
            {
                if (_bountyInformation == null)
                    _bountyInformation = LoadBountyInformation();
                return _bountyInformation;
            }
        }

        private BountyInformation LoadBountyInformation()
        {
            try
            {
                var serialized = GetProperty(ACE.Entity.Enum.Properties.PropertyString.BountyInformationsSerialized);
                if (!string.IsNullOrEmpty(serialized))
                    return JsonConvert.DeserializeObject<BountyInformation>(serialized) ?? new BountyInformation();
            }
            catch (Exception ex)
            {
                log.Error($"Exception loading BountyInformation for player {Name}: {ex}");
            }
            return new BountyInformation();
        }

        private void SaveBountyInformation()
        {
            try
            {
                var serialized = JsonConvert.SerializeObject(_bountyInformation ?? BountyInformation);
                SetProperty(ACE.Entity.Enum.Properties.PropertyString.BountyInformationsSerialized, serialized);
            }
            catch (Exception ex)
            {
                log.Error($"Exception saving BountyInformation for player {Name}: {ex}");
            }
        }

        private double GetBountyCooldown(uint targetGuid)
        {
            var target = GetBountyTargetInfo(targetGuid);
            return target.LastCompletedTimestamp;
        }

        private int GetBountiesCompletedInLastMinutes(int minutes)
        {
            var cutoffTime = DateTime.UtcNow.AddMinutes(-minutes);
            return BountyInformation.BountyCompletionTimestamps.Count(t => t >= cutoffTime);
        }

        private void SaveBountyExpiration(uint targetGuid)
        {
            var info = BountyInformation;
            var target = GetBountyTargetInfo(targetGuid);
            target.TotalExpirations++;
            info.TotalBountyExpirationsCount++;
            SaveBountyInformation();
        }

        private class BountyCompletionResult
        {
            public uint TargetGuid;
            public bool IsNewUniqueTarget;
            public bool IsHighPriorityTarget;
            public bool IsKillStreakCompletion;
            public uint RepeatCount;
            public uint DailyHighPriorityCount;
            public uint KillStreak;
            public uint DamageDealt       { get; set; }
            public uint TotalDamageDealt  { get; set; }
            public uint TotalDamageReceived { get; set; }
            public uint DamageReceived    { get; set; }
            public int CountLast30Min;
            public int CountLast60Min;
            public int CountLast90Min;
        }

        private BountyCompletionResult UpdateCompletedBountyInformation(IPlayer bountyTarget, BountyContract contract)
        {
            var targetGuid = bountyTarget.Guid.Full;
            var info       = BountyInformation;
            var targetInfo = GetBountyTargetInfo(targetGuid);

            info.TotalBountiesCompleted++;
            targetInfo.LastCompletedTimestamp = Time.GetUnixTime();
            targetInfo.TotalCompletions++;

            // Keep a rolling window of the last 30 timestamps
            info.BountyCompletionTimestamps.Add(DateTime.UtcNow);
            if (info.BountyCompletionTimestamps.Count > 30)
                info.BountyCompletionTimestamps.RemoveAt(0);

            // Repeat kill count for this specific target
            if (!info.RepeatKillCounts.TryGetValue(targetGuid, out var repeatCount))
                repeatCount = 0;
            repeatCount++;
            info.RepeatKillCounts[targetGuid] = repeatCount;

            // Unique target tracking
            var isNewUnique = info.UniqueBountyTargets.Add(targetGuid);

            // High priority target
            var isHighPriorityTarget = bountyTarget.IsHighPriorityTarget();
            if (isHighPriorityTarget)
            {
                targetInfo.TotalHighPriorityCompletions++;
                info.TotalHighPriorityBountiesCompleted++;
                info.TotalDailyHighPriorityBountiesCompleted++;
            }

            // Kill streak
            var killStreakCount     = contract.BountyKillStreakCount;
            var killStreakMinimum   = PropertyManager.GetLong("bounty_kill_streak_minimum").Item;
            var isKillStreakCompletion = killStreakCount > killStreakMinimum;

            if (isKillStreakCompletion)
            {
                info.TotalKillStreakCompletions++;
                targetInfo.TotalKillStreakCompletions++;

                if ((uint)killStreakCount > info.HighestKillStreakBroken)
                    info.HighestKillStreakBroken = (uint)killStreakCount;
                if ((uint)killStreakCount > targetInfo.HighestKillStreakBroken)
                    targetInfo.HighestKillStreakBroken = (uint)killStreakCount;
            }

            return new BountyCompletionResult
            {
                TargetGuid             = targetInfo.TargetGuid,
                IsNewUniqueTarget      = isNewUnique,
                IsKillStreakCompletion = isKillStreakCompletion,
                IsHighPriorityTarget   = isHighPriorityTarget,
                KillStreak             = (uint)killStreakCount,
                RepeatCount            = repeatCount,
                DailyHighPriorityCount = info.TotalDailyHighPriorityBountiesCompleted,
                CountLast30Min         = GetBountiesCompletedInLastMinutes(30),
                CountLast60Min         = GetBountiesCompletedInLastMinutes(60),
                CountLast90Min         = GetBountiesCompletedInLastMinutes(90)
            };
        }

        private BountyTargetInfo GetBountyTargetInfo(uint targetGuid)
        {
            if (!BountyInformation.BountyTargets.TryGetValue(targetGuid, out var info))
            {
                info = new BountyTargetInfo { TargetGuid = targetGuid };
                BountyInformation.BountyTargets[targetGuid] = info;
            }
            return info;
        }

        private void ResetDailyBountyQuestsIfNeeded()
        {
            var today = DateTime.Today;
            if (BountyInformation.LastBountyQuestResetDate >= today)
                return;

            BountyInformation.UniqueBountyTargets.Clear();
            BountyInformation.RepeatKillCounts.Clear();
            BountyInformation.BountyCompletionTimestamps.Clear();
            BountyInformation.LastBountyQuestResetDate = today;
            BountyInformation.TotalDailyHighPriorityBountiesCompleted = 0;
            BountyInformation.DailyTargetDamageDealt.Clear();
            BountyInformation.TotalDailyDamageDealt = 0;

            SaveBountyInformation();
        }
    }
}
