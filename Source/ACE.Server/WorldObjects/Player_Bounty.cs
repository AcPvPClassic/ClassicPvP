using System;
using System.Collections.Generic;
using System.Linq;
using ACE.Common;
using ACE.Database;
using ACE.Entity;
using ACE.Entity.Enum;
using ACE.Entity.Models;
using ACE.Server.Entity;
using ACE.Server.Entity.Actions;
using ACE.Server.Entity.Bounties;
using ACE.Server.Entity.PKQuests;
using ACE.Server.Factories;
using ACE.Server.Managers;
using ACE.Server.Network.GameMessages.Messages;

namespace ACE.Server.WorldObjects
{
    partial class Player
    {
        private bool BountiesInitialized = false;
        private double? LastBountyHunterSeenTimestamp;
        private BountyCollection BountyCollection;

        /// <summary>
        /// Returns true when this player is an eligible bounty target.
        /// ClassicPvP: bounty_allow_all_locations defaults to true so no landblock restriction is applied.
        /// </summary>
        private bool IsValidBountyTarget
        {
            get
            {
                var validLocation = CurrentLandblock?.IsBountyLocation ?? false;
                var validLogout   = !IsLoggingOut;

                if (PropertyManager.GetBool("bounty_allow_all_locations").Item)
                    validLocation = true;
                if (PropertyManager.GetBool("bounty_allow_logged_out").Item)
                    validLogout = true;

                var minimumPlayerLevel = PropertyManager.GetLong("bounty_minimum_player_level").Item;

                return
                    !IsAdmin &&
                    validLogout &&
                    validLocation &&
                    Level >= minimumPlayerLevel &&
                    this.IsAllegianceWhitelisted() &&
                    IsPK;
            }
        }

        private void InitializeBounties()
        {
            var bountyContracts = GetInventoryItemsOfWCID(BountyContract.BountyContractWcid)
                .OfType<BountyContract>()
                .ToList();

            foreach (var contract in bountyContracts)
                contract.UpdateUiEffects(this);

            BountyCollection  = new BountyCollection(this, bountyContracts);
            BountiesInitialized = true;
        }

        private void BountyTick()
        {
            if (!BountiesInitialized || BountyCollection == null || !BountyContract.IsBountySystemEnabled)
                return;
            BountyCollection.CheckContracts(this);
        }

        // --- BountyCollection queries and mutations ---

        private bool TryGetBountyContract(uint targetGuid, out BountyContract contract)
        {
            contract = null;
            if (!BountiesInitialized || BountyCollection == null)
                return false;
            return BountyCollection.TryGetBountyContract(targetGuid, out contract);
        }

        private bool TryGetActiveBountyContract(uint targetGuid, out BountyContract contract)
        {
            contract = null;
            if (!BountiesInitialized || BountyCollection == null)
                return false;
            return BountyCollection.TryGetActiveBountyContract(targetGuid, this, out contract);
        }

        private bool TryAddCompletedBountyContract(BountyContract contract)
        {
            if (!BountiesInitialized || BountyCollection == null || contract.IsInvalidContract)
                return false;
            return BountyCollection.CompleteContract((uint)contract.BountyTargetGuid.Value, this);
        }

        private bool TryAddActiveBountyContract(BountyContract contract)
        {
            if (!BountiesInitialized || BountyCollection == null || contract.IsInvalidContract)
                return false;
            return BountyCollection.AddBountyContract((uint)contract.BountyTargetGuid.Value, contract);
        }

        public bool TryRemoveBountyContract(uint targetGuid)
        {
            if (!BountiesInitialized || BountyCollection == null)
                return false;
            return BountyCollection.RemoveBountyContract(targetGuid);
        }

        // --- NPC transactions ---

        private NpcTransactionResult CheckBountyTransactions(WorldObject npc, WorldObject itemToGive)
        {
            if (npc.WeenieClassId != BountyContract.BountyNPCWcid)
                return NpcTransactionResult.Ignore;

            var now             = Time.GetUnixTime();
            var cooldownSeconds = PropertyManager.GetDouble("bounty_npc_use_cooldown_seconds").Item;

            if (BountyLastUsedNpcTimestamp.HasValue && now - BountyLastUsedNpcTimestamp.Value < cooldownSeconds)
            {
                SendNpcResponse(npc, $"To prevent abuse, you must wait {cooldownSeconds} seconds after every transaction. Try again in a few seconds.");
                return NpcTransactionResult.Return;
            }

            BountyLastUsedNpcTimestamp = now;

            var result = CheckBountyContractTurnIns(npc, itemToGive);
            if (result != NpcTransactionResult.Ignore) return result;

            result = CheckBountyPurchase(npc, itemToGive);
            if (result != NpcTransactionResult.Ignore) return result;

            result = CheckWritOfPursuit(npc, itemToGive);
            if (result != NpcTransactionResult.Ignore) return result;

            return NpcTransactionResult.Ignore;
        }

        /// <summary>
        /// Turn in a completed or expired bounty contract.
        /// </summary>
        private NpcTransactionResult CheckBountyContractTurnIns(WorldObject npc, WorldObject item)
        {
            if (npc.WeenieClassId != BountyContract.BountyNPCWcid || item is not BountyContract contract)
                return NpcTransactionResult.Ignore;

            try
            {
                if (!BountyContract.IsBountySystemEnabled)
                {
                    SendNpcResponse(npc, "Bounties are not enabled on this server.");
                    return NpcTransactionResult.Return;
                }

                if (!BountiesInitialized)
                {
                    SendNpcResponse(npc, "Bounties have not finished initializing yet, please try again in a few seconds.");
                    return NpcTransactionResult.Return;
                }

                if (contract.IsInvalidContract)
                {
                    SendNpcResponse(npc, "This contract is invalid and cannot be turned in. Destroying the contract now.");
                    return NpcTransactionResult.Consume;
                }

                var targetGuid = (uint)contract.BountyTargetGuid.Value;

                if (contract.IsBountyExpired)
                {
                    var bountyCurrencyWeenie      = BountyContract.BountyCurrencyWeenie;
                    var bountyCurrencyReturnAmount = BountyContract.BountyCurrencyReturnAmount;

                    TryRemoveBountyContract(targetGuid);
                    SaveBountyExpiration(targetGuid);

                    SendNpcResponse(npc,
                        $"Your bounty contract has expired; you will be compensated {bountyCurrencyWeenie.BuildAmountString(bountyCurrencyReturnAmount)} for your time.",
                        () => GiveFromEmote(npc, BountyContract.BountyCurrencyWcid, amount: (int)bountyCurrencyReturnAmount));

                    return NpcTransactionResult.Consume;
                }

                if (!contract.IsCompletedState)
                {
                    SendNpcResponse(npc, "Your bounty contract is not completed yet. Keep hunting!");
                    return NpcTransactionResult.Return;
                }

                var bountyTarget = PlayerManager.FindByGuid(new ObjectGuid(targetGuid));
                if (bountyTarget == null || bountyTarget.IsPendingDeletion || bountyTarget.IsDeleted)
                {
                    SendNpcResponse(npc, "The bounty target for this contract no longer exists.");
                    return NpcTransactionResult.Return;
                }

                var result       = UpdateCompletedBountyInformation(bountyTarget, contract);
                HandleBountyQuests(result);
                SaveBountyInformation();

                var rewardAmount       = bountyTarget.GetProperty(ACE.Entity.Enum.Properties.PropertyInt.BountyPriorityTargetRewardAmount);
                var rewardCurrencyWcid = bountyTarget.GetProperty(ACE.Entity.Enum.Properties.PropertyInt.BountyPriorityCurrency);

                bountyTarget.ClearHighPriorityBountyProps();
                TryRemoveBountyContract(targetGuid);
                BountyEndTimestamp = Time.GetUnixTime();

                SendNpcResponse(npc, $"You have successfully turned in your bounty for player \"{bountyTarget.Name}\".");

                if (result.IsHighPriorityTarget && rewardAmount.HasValue && rewardCurrencyWcid.HasValue)
                {
                    var wopCurrencyWeenie  = DatabaseManager.World.GetOrThrowCachedWeenie((uint)rewardCurrencyWcid.Value);
                    var rewardedAmountStr  = wopCurrencyWeenie.BuildAmountString(rewardAmount.Value);

                    SendNpcResponse(npc,
                        $"This bounty was a high priority target; you have been rewarded {rewardedAmountStr}.",
                        () => GiveFromEmote(npc, (uint)rewardCurrencyWcid.Value, amount: rewardAmount.Value));

                    PlayerManager.BroadcastToAll(new GameMessageSystemChat(
                        $"{Name} has completed a bounty contract for high priority target {bountyTarget.Name} and received {rewardedAmountStr}!",
                        ChatMessageType.WorldBroadcast));

                    foreach (var p in PlayerManager.GetAllOnline())
                    {
                        if (p.TryGetActiveBountyContract(targetGuid, out var activeContract))
                        {
                            activeContract.SetUiEffect(
                                ACE.Entity.Enum.UiEffects.Fire, p,
                                message: $"{bountyTarget.Name} is no longer a high priority target; your contract has returned to normal.");
                        }
                    }
                }

                return NpcTransactionResult.Consume;
            }
            catch (Exception ex)
            {
                log.Error($"Error in CheckBountyContractTurnIns for {Name} ({Guid.Full}): ", ex);
                SendChatMessage("An error occurred while processing the bounty turn-in. Please notify an admin.", ChatMessageType.System);
                return NpcTransactionResult.Consume;
            }
        }

        /// <summary>
        /// Process a Writ of Pursuit turned in to the bounty NPC.
        /// </summary>
        private NpcTransactionResult CheckWritOfPursuit(WorldObject npc, WorldObject item)
        {
            if (npc.WeenieClassId != BountyContract.BountyNPCWcid ||
                item.WeenieClassId != BountyContract.WritOfPursuitWcid)
                return NpcTransactionResult.Ignore;

            try
            {
                if (!BountyContract.IsBountySystemEnabled)
                {
                    SendNpcResponse(npc, "Bounties are not enabled on this server.");
                    return NpcTransactionResult.Return;
                }

                if (!BountiesInitialized)
                {
                    SendNpcResponse(npc, "Bounties have not finished initializing yet, please try again in a few seconds.");
                    return NpcTransactionResult.Return;
                }

                if (!BountyContract.IsWritOfPursuitEnabled)
                {
                    SendNpcResponse(npc, "Writs of Pursuit are not enabled on this server.");
                    return NpcTransactionResult.Return;
                }

                if (!this.IsAllegianceWhitelisted())
                {
                    SendNpcResponse(npc, "You must be in a whitelisted allegiance to purchase a bounty.");
                    return NpcTransactionResult.Return;
                }

                var wopCurrencyWeenie = BountyContract.BountyWopCurrencyWeenie;

                if (!TryParseBountyWrit(item.Inscription, out var targetName, out var rewardAmount))
                {
                    SendNpcResponse(npc,
                        "Unable to parse your Writ of Pursuit. Please inscribe it with the player's name and reward amount in this format: <Name>:<Amount>.");
                    SendNpcResponse(npc,
                        $"The reward amount must be between 1 and {wopCurrencyWeenie.GetMaxStackSize()}.");
                    return NpcTransactionResult.Return;
                }

                var player = PlayerManager.FindByName(targetName);
                if (player == null)
                {
                    SendNpcResponse(npc, "Your Writ of Pursuit does not contain a valid player name.");
                    return NpcTransactionResult.Return;
                }

                if (!player.IsAllegianceWhitelisted())
                {
                    SendNpcResponse(npc, $"The player \"{targetName}\" is not in a whitelisted allegiance and cannot be targeted with a Writ of Pursuit.");
                    return NpcTransactionResult.Return;
                }

                if (player.Guid.Full == Guid.Full)
                {
                    SendNpcResponse(npc, "You cannot target yourself with a Writ of Pursuit.");
                    return NpcTransactionResult.Return;
                }

                var minimumPlayerLevel = PropertyManager.GetLong("bounty_minimum_player_level").Item;
                if (player.Level <= minimumPlayerLevel)
                {
                    SendNpcResponse(npc, $"The player \"{targetName}\" does not meet the minimum level requirement of {minimumPlayerLevel} to be targeted with a Writ of Pursuit.");
                    return NpcTransactionResult.Return;
                }

                if (this.IsSameAllegiance(player))
                {
                    SendNpcResponse(npc, "You cannot target a player of the same allegiance with a Writ of Pursuit.");
                    return NpcTransactionResult.Return;
                }

                if (player.GetProperty(ACE.Entity.Enum.Properties.PropertyBool.IsBountyHighPriorityTarget) == true)
                {
                    var ownerName = player.GetProperty(ACE.Entity.Enum.Properties.PropertyString.BountyPriorityOwnerName);
                    SendNpcResponse(npc, $"The player \"{targetName}\" is already a high priority target for {ownerName}; wait until someone completes a bounty for them.");
                    return NpcTransactionResult.Return;
                }

                var wopInventoryCount = GetNumInventoryItemsOfWCID(BountyContract.BountyWopCurrencyWcid);
                if (wopInventoryCount < rewardAmount)
                {
                    SendNpcResponse(npc,
                        $"You do not have enough {wopCurrencyWeenie.GetPluralName()} to turn in your Writ of Pursuit. You have {wopInventoryCount} but require {rewardAmount}.");
                    return NpcTransactionResult.Return;
                }

                if (!TryConsumeFromInventoryWithNetworking(BountyContract.BountyWopCurrencyWcid, rewardAmount))
                    return NpcTransactionResult.Return;

                // Mark the target as high priority
                player.SetProperty(ACE.Entity.Enum.Properties.PropertyBool.IsBountyHighPriorityTarget, true);
                player.SetProperty(ACE.Entity.Enum.Properties.PropertyInt.BountyPriorityTargetRewardAmount, rewardAmount);
                player.SetProperty(ACE.Entity.Enum.Properties.PropertyInt.BountyPriorityCurrency, (int)BountyContract.BountyWopCurrencyWcid);
                player.SetProperty(ACE.Entity.Enum.Properties.PropertyString.BountyPriorityOwnerName, Name);

                SendNpcResponse(npc,
                    $"You have successfully submitted your Writ of Pursuit for player \"{player.Name}\" with a reward of {wopCurrencyWeenie.BuildAmountString(rewardAmount)}.");

                var targetGuid = player.Guid.Full;
                foreach (var p in PlayerManager.GetAllOnline())
                {
                    if (p.TryGetActiveBountyContract(targetGuid, out var activeContract))
                    {
                        activeContract.SetUiEffect(
                            ACE.Entity.Enum.UiEffects.Magical, p,
                            message: $"{targetName} is now a high priority bounty target; your contract has been upgraded.");
                    }
                }

                return NpcTransactionResult.Consume;
            }
            catch (Exception ex)
            {
                log.Error($"Error in CheckWritOfPursuit for {Name} ({Guid.Full}): ", ex);
                SendChatMessage("An error occurred while trying to turn in a Writ of Pursuit. Please notify an admin.", ChatMessageType.System);
                return NpcTransactionResult.Consume;
            }
        }

        private bool TryParseBountyWrit(string inscription, out string playerName, out int reward)
        {
            playerName = null;
            reward     = 0;
            var wopCurrencyWeenie = BountyContract.BountyWopCurrencyWeenie;

            if (string.IsNullOrWhiteSpace(inscription))
                return false;

            var parts = inscription.Split(':');
            if (parts.Length != 2)
                return false;

            playerName = parts[0].Trim();
            if (string.IsNullOrWhiteSpace(playerName) || playerName.Length > 32)
                return false;

            if (!int.TryParse(parts[1].Trim(), out reward) || reward <= 0)
                return false;

            if (reward > wopCurrencyWeenie.GetMaxStackSize())
                return false;

            return true;
        }

        /// <summary>
        /// Purchase a new bounty contract via a Bounty Purchase Token.
        /// </summary>
        private NpcTransactionResult CheckBountyPurchase(WorldObject npc, WorldObject item)
        {
            if (npc.WeenieClassId != BountyContract.BountyNPCWcid ||
                item.WeenieClassId != BountyContract.BountyPurchaseTokenWcid)
                return NpcTransactionResult.Ignore;

            try
            {
                if (!BountyContract.IsBountySystemEnabled)
                {
                    SendNpcResponse(npc, "Bounties are not enabled on this server.");
                    return NpcTransactionResult.Return;
                }

                if (!BountiesInitialized)
                {
                    SendNpcResponse(npc, "Bounties have not finished initializing yet, please try again in a few seconds.");
                    return NpcTransactionResult.Return;
                }

                if (!this.IsAllegianceWhitelisted())
                {
                    SendNpcResponse(npc, "You must be in a whitelisted allegiance to purchase a bounty.");
                    return NpcTransactionResult.Return;
                }

                if (!HasCooldownPenaltyExpiredForHunter())
                {
                    SendNpcResponse(npc, "You currently have a global bounty cooldown after turning in a completed contract.");
                    SendNpcResponse(npc, $"Cooldown remaining: {BountyCooldownTimeRemaining}.");
                    return NpcTransactionResult.Return;
                }

                if (BountyCollection.GetTotalCount() >= BountyContract.MaxBountyContracts)
                {
                    SendNpcResponse(npc,
                        $"You have reached the maximum number of active bounty contracts ({BountyContract.MaxBountyContracts}). " +
                        "Please complete your current bounties before purchasing new ones.");
                    return NpcTransactionResult.Return;
                }

                var eligibleTargets = PlayerManager.GetAllOnline().Where(p =>
                    p != null &&
                    p.Guid.Full != Guid.Full &&
                    p.IsValidBountyTarget &&
                    !TryGetBountyContract(p.Guid.Full, out _) &&
                    !this.IsSameAllegiance(p) &&
                    IsDifferentIPAddress(p) &&
                    !IsOnHardCooldown(p)).ToList();

                if (eligibleTargets.Count == 0)
                {
                    SendNpcResponse(npc, "There are currently no eligible bounty targets available.");
                    return NpcTransactionResult.Return;
                }

                var bountyTarget = SelectRandomWeightedTarget(eligibleTargets);
                var contract     = WorldObjectFactory.CreateNewWorldObject(BountyContract.BountyContractWcid) as BountyContract;

                if (contract == null)
                    return NpcTransactionResult.Consume;

                var now = Time.GetUnixTime();

                contract.BountyOwnerGuid        = (int?)Guid.Full;
                contract.BountyOwnerName        = Name;
                contract.BountyTargetGuid       = (int?)bountyTarget.Guid.Full;
                contract.BountyTargetName       = bountyTarget.Name;
                contract.BountyCreationTimestamp = now;
                contract.Name                   = $"Bounty Contract: {bountyTarget.Name}";

                if (bountyTarget.IsHighPriorityTarget())
                {
                    contract.SetUiEffect(
                        ACE.Entity.Enum.UiEffects.Magical, this,
                        message: $"{bountyTarget.Name} is a high priority target; this contract has a higher reward than normal!");
                }

                if (!TryCreateForGive(npc, contract))
                {
                    contract.Destroy();
                    return NpcTransactionResult.Return;
                }

                contract.SetState(BountyContract.BountyContractState.Active, this);

                if (!TryAddActiveBountyContract(contract))
                {
                    TryConsumeFromInventoryWithNetworking(contract);
                    return NpcTransactionResult.Return;
                }

                SendNpcResponse(npc,
                    $"A new bounty for player \"{bountyTarget.Name}\" has been assigned to you. A bounty contract has been added to your inventory.");

                return NpcTransactionResult.Consume;
            }
            catch (Exception ex)
            {
                log.Error($"Error in CheckBountyPurchase for {Name} ({Guid.Full}): ", ex);
                SendChatMessage("An error occurred while trying to purchase a bounty. Please notify an admin.", ChatMessageType.System);
                return NpcTransactionResult.Consume;
            }
        }

        // --- Weighted target selection ---

        private Player SelectRandomWeightedTarget(List<Player> players)
        {
            var weightedPlayers = new List<(Player player, double weight)>();
            double totalWeight  = 0;

            foreach (var p in players)
            {
                var w = GetBountyWeight(p);
                totalWeight += w;
                weightedPlayers.Add((p, w));
            }

            if (totalWeight <= 0)
                return players[ThreadSafeRandom.Next(0, players.Count - 1)];

            double roll       = ThreadSafeRandom.Next(0f, (float)totalWeight);
            double cumulative = 0;

            foreach (var entry in weightedPlayers)
            {
                cumulative += entry.weight;
                if (roll <= cumulative)
                    return entry.player;
            }

            return weightedPlayers.Last().player;
        }

        private double GetBountyWeight(Player p)
        {
            double weight = 1;

            if (p.IsBountyHighPriorityTarget)
                weight *= 2;

            var bountyWeightExponent      = PropertyManager.GetDouble("bounty_weight_exponent").Item;
            var bountyWeightMultiplier    = PropertyManager.GetDouble("bounty_weight_multiplier").Item;
            var bountyWeightMaxStackScale = PropertyManager.GetDouble("bounty_weight_maxstack_scale").Item;

            bountyWeightMultiplier    = Math.Max(0, bountyWeightMultiplier);
            bountyWeightMaxStackScale = Math.Clamp(bountyWeightMaxStackScale, 0.01, 1.0);
            bountyWeightExponent      = Math.Clamp(bountyWeightExponent, 0.25, 1.0);

            if (p.BountyPriorityTargetRewardAmount.HasValue)
            {
                var wopCurrencyWeenie   = BountyContract.BountyWopCurrencyWeenie;
                var rewardAmount        = p.BountyPriorityTargetRewardAmount.Value;
                var maxStack            = wopCurrencyWeenie.GetMaxStackSize();
                if (maxStack <= 0) maxStack = 1;

                var effectiveMaxStack = Math.Max(1, maxStack * bountyWeightMaxStackScale);
                var normalized        = Math.Clamp(rewardAmount / effectiveMaxStack, 0.0, 1.0);

                weight += Math.Pow(normalized, bountyWeightExponent) * bountyWeightMultiplier;
            }

            var killStreak        = p.PlayerKillStreak;
            var killStreakMinimum = PropertyManager.GetLong("bounty_kill_streak_minimum").Item;

            if (killStreak > killStreakMinimum)
            {
                var normalized = Math.Clamp(
                    (killStreak - killStreakMinimum) / (double)Math.Max(1, killStreakMinimum),
                    0.0, 1.0);
                weight += Math.Pow(normalized, bountyWeightExponent) * bountyWeightMultiplier;
            }

            return weight;
        }

        // --- PK death integration ---

        /// <summary>
        /// Called from Player_Death when a PK kill occurs.
        /// Marks the hunter's contract as complete if they have an active contract on this target.
        /// </summary>
        public void MarkBountyComplete(Player bountyTarget, double damageDealt, double maxHealth, double currentHealth)
        {
            var targetGuid = bountyTarget.Guid.Full;
            var killStreak = bountyTarget.PlayerKillStreak;

            if (!TryGetActiveBountyContract(targetGuid, out var contract))
                return;

            if (contract.IsInvalidContract || contract.IsBountyExpired || !contract.IsActiveState)
                return;

            contract.SetState(BountyContract.BountyContractState.Completed, this);
            contract.BountyKillStreakCount = killStreak;

            TryAddCompletedBountyContract(contract);
        }

        // --- Tracking integration ---

        /// <summary>
        /// Called on teleport completion. Checks all visible players and refreshes the PK timer
        /// on any player who is either hunting this player or being hunted by this player.
        /// This prevents the target from recalling or using portals for pk_bounty_timer seconds (default 2 min).
        /// </summary>
        private void CheckVisibleBounties()
        {
            if (!BountyContract.IsBountySystemEnabled || !BountyContract.IsBountyPkTimerActiveEnabled)
                return;

            var visiblePlayers = PhysicsObj.ObjMaint.GetVisibleObjectsValuesOfTypeCreature().OfType<Player>();

            foreach (var player in visiblePlayers)
            {
                TryMarkHunterTarget(player, this); // player hunts me
                TryMarkHunterTarget(this, player); // I hunt player
            }
        }

        private static void TryMarkHunterTarget(Player hunter, Player target)
        {
            if (!hunter.TryGetActiveBountyContract(target.Guid.Full, out _))
                return;
            target.RefreshBountyPkTimer();
        }

        public void RefreshBountyPkTimer()
        {
            LastBountyHunterSeenTimestamp = Time.GetUnixTime();
            UpdatePKTimer();
        }

        // --- Cooldown helpers ---

        private string BountyCooldownTimeRemaining
        {
            get
            {
                if (!BountyEndTimestamp.HasValue)
                    return "ready";

                var endTime          = Time.GetDateTimeFromTimestamp(BountyEndTimestamp.Value);
                var cooldownMinutes  = PropertyManager.GetLong("bounty_cooldown_expiration_time").Item;
                return Time.GetTimeRemainingString(endTime, TimeSpan.FromMinutes(cooldownMinutes), expiredText: "ready");
            }
        }

        private bool HasCooldownPenaltyExpiredForHunter()
        {
            if (!BountyEndTimestamp.HasValue)
                return true;

            var cooldownMinutes = PropertyManager.GetLong("bounty_cooldown_expiration_time").Item;
            return DateTime.UtcNow - Time.GetDateTimeFromTimestamp(BountyEndTimestamp.Value)
                > TimeSpan.FromMinutes(cooldownMinutes);
        }

        private bool IsOnHardCooldown(Player p)
        {
            var cooldown = GetBountyCooldown(p.Guid.Full);
            if (cooldown == -1)
                return false;

            var durationMinutes  = PropertyManager.GetLong("bounty_cooldown_target_expiration_time").Item;
            var cooldownDatetime = Time.GetDateTimeFromTimestamp(cooldown);
            var elapsed          = DateTime.UtcNow - cooldownDatetime;

            var onCooldown = elapsed.TotalMinutes < durationMinutes;

            if (onCooldown)
            {
                SendChatMessage(
                    $"A potential bounty target is currently on cooldown for you. Time remaining: {Time.GetTimeRemainingString(cooldownDatetime, TimeSpan.FromMinutes(durationMinutes))}.",
                    ChatMessageType.System);
            }

            return onCooldown;
        }

        // --- Quest completions ---

        private void HandleBountyQuests(BountyCompletionResult result)
        {
            // Any bounty
            CompletePkQuestTasks(PKQuests.PKQuests_BountyAny);

            // Unique targets
            if (result.IsNewUniqueTarget)
                CompletePkQuestTasks(PKQuests.PKQuests_BountyUnique);

            // Repeat kills (same player)
            if (result.RepeatCount == 3) CompletePkQuestTask("BOUNTY_REPEAT_3", 1);
            if (result.RepeatCount == 5) CompletePkQuestTask("BOUNTY_REPEAT_5", 1);

            // Speed-based
            if (result.CountLast30Min >= 2) CompletePkQuestTask("BOUNTY_FAST_2", 1);
            if (result.CountLast30Min >= 3) CompletePkQuestTask("BOUNTY_FAST_3", 1);
            if (result.CountLast60Min >= 8) CompletePkQuestTask("BOUNTY_FAST_8", 1);
            if (result.CountLast90Min >= 12) CompletePkQuestTask("BOUNTY_FAST_12", 1);

            // High priority
            if (result.IsHighPriorityTarget)
                CompletePkQuestTasks(PKQuests.PKQuests_BountyPriority);

            // Kill streaks
            if (result.IsKillStreakCompletion)
            {
                CompletePkQuestTasks(PKQuests.PKQuests_BountyKillStreak);

                if (result.KillStreak >= 5)  CompletePkQuestTask("BOUNTY_STREAKBREAKER_5",  1);
                if (result.KillStreak >= 10) CompletePkQuestTask("BOUNTY_STREAKBREAKER_10", 1);
            }
        }

        // --- Messaging ---

        private void SendNpcResponse(WorldObject npc, string message, Action action = null, double delay = 0)
        {
            SendChatMessage($"{npc.Name} tells you, \"{message}\"", ChatMessageType.Tell, action: action, delaySeconds: delay);
        }

        public void SendChatMessage(string message, ChatMessageType type = ChatMessageType.System, double delaySeconds = 0, Action action = null)
        {
            if (delaySeconds > 0)
            {
                var actionChain = new ActionChain();
                actionChain.AddDelaySeconds(delaySeconds);
                actionChain.AddAction(this, () =>
                {
                    Session?.Network?.EnqueueSend(new GameMessageSystemChat(message, type));
                    action?.Invoke();
                });
                actionChain.EnqueueChain();
                return;
            }

            Session?.Network?.EnqueueSend(new GameMessageSystemChat(message, type));
            action?.Invoke();
        }

        public void SendBountyMessage(string message, Action action = null, double delay = 0)
        {
            SendChatMessage($"[BOUNTY] {message}", ChatMessageType.System, delay, action);
        }
    }
}
