using System;
using ACE.Entity.Enum.Properties;

namespace ACE.Server.Entity.Bounties
{
    public static class IPlayerBountyExtensions
    {
        public static bool IsHighPriorityTarget(this IPlayer target) =>
            target.GetProperty(PropertyBool.IsBountyHighPriorityTarget) ?? false;

        public static string GetPriorityOwnerName(this IPlayer target) =>
            target.GetProperty(PropertyString.BountyPriorityOwnerName)
            ?? throw new InvalidOperationException("BountyPriorityOwnerName is missing");

        public static int GetPriorityCurrency(this IPlayer target) =>
            target.GetProperty(PropertyInt.BountyPriorityCurrency)
            ?? throw new InvalidOperationException("BountyPriorityCurrency is missing");

        public static int GetPriorityRewardAmount(this IPlayer target) =>
            target.GetProperty(PropertyInt.BountyPriorityTargetRewardAmount)
            ?? throw new InvalidOperationException("BountyPriorityTargetRewardAmount is missing");

        public static int GetTargetKillStreak(this IPlayer target) =>
            target.GetProperty(PropertyInt.PlayerKillStreak) ?? 0;

        public static void ClearHighPriorityBountyProps(this IPlayer player)
        {
            player.RemoveProperty(PropertyBool.IsBountyHighPriorityTarget);
            player.RemoveProperty(PropertyInt.BountyPriorityTargetRewardAmount);
            player.RemoveProperty(PropertyInt.BountyPriorityCurrency);
            player.RemoveProperty(PropertyString.BountyPriorityOwnerName);
        }
    }
}
