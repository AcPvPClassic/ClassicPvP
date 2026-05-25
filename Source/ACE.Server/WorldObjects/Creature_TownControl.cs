using System;
using System.Collections.Generic;
using System.Linq;

using log4net;

using ACE.Common;
using ACE.Entity;
using ACE.Entity.Enum;
using ACE.Server.Entity;
using ACE.Server.Entity.TownControl;
using ACE.Server.Factories;
using ACE.Server.Managers;
using ACE.Server.Network.GameMessages.Messages;

namespace ACE.Server.WorldObjects
{
    partial class Creature
    {
        private static readonly ILog tcLog = LogManager.GetLogger("TownControl");

        // -----------------------------------------------------------------------
        // Per-instance boss state flags
        // -----------------------------------------------------------------------

        /// <summary>True while this creature is a Town Control boss (init or conflict).</summary>
        public bool IsTownControlBoss         => TownControlBosses.IsTownControlBoss(WeenieClassId);

        /// <summary>True while this creature is specifically the conflict-phase boss.</summary>
        public bool IsTownControlConflictBoss => TownControlBosses.IsTownControlConflictBoss(WeenieClassId);

        // One-shot broadcast sentinels — reset on respawn
        public bool TownControl_ConflictBroadcast1Sent   { get; set; } = false;
        public bool TownControl_ConflictBroadcast2Sent   { get; set; } = false;
        public bool TownControl_ConflictBroadcast3Sent   { get; set; } = false;
        public bool TownControl_InitLowHpBroadcastSent   { get; set; } = false;

        // -----------------------------------------------------------------------
        // Death handler — called from OnDeath when IsTownControlBoss is true
        // -----------------------------------------------------------------------

        public void HandleTownControlBossDeath()
        {
            tcLog.Info($"Creature_TownControl.HandleTownControlBossDeath — WeenieId={WeenieClassId}");

            try
            {
                var deadBoss = TownControlBosses.Get(WeenieClassId);
                if (deadBoss == null) return;

                var killer = DamageHistory.TopDamager;
                var town   = TownControlManager.GetTown(deadBoss.TownId);
                if (town == null)
                {
                    tcLog.Error($"[TownControl] No town found for TownId={deadBoss.TownId}");
                    return;
                }

                // Resolve boss display name
                var bossName = Name ?? deadBoss.TownName + " Boss";

                // ---------------------------------------------------------------
                // Init Boss killed → attempt to start conflict
                // ---------------------------------------------------------------
                if (deadBoss.BossType == TownControlBossType.InitiationBoss)
                {
                    tcLog.Debug($"[TownControl] {town.TownName} init boss killed by guid={killer?.Guid}");
                    bool abort = false;

                    if (killer == null || !killer.IsPlayer)
                    {
                        tcLog.Debug($"[TownControl] {town.TownName} init boss killer is not a player — conflict not started.");
                        abort = true;
                    }

                    uint killerMonarchId    = 0;
                    string killerAllegName  = null;

                    if (!abort)
                    {
                        var killerPlayer     = PlayerManager.FindByGuid(killer.Guid);
                        var killerAllegiance = AllegianceManager.GetAllegiance(killerPlayer);

                        if (killerAllegiance?.MonarchId == null)
                        {
                            tcLog.Debug($"[TownControl] {town.TownName} init boss killer has no allegiance — conflict not started.");
                            CurrentLandblock?.EnqueueBroadcast(null, true, null, null,
                                new GameMessageSystemChat(
                                    $"{bossName} has been killed by a lone wolf with no allegiance. {town.TownName} will not yield.",
                                    ChatMessageType.Broadcast));
                            abort = true;
                        }
                        else
                        {
                            killerMonarchId  = killerAllegiance.MonarchId.Value;
                            killerAllegName  = killerAllegiance.Monarch?.Player?.Name ?? "Unknown";

                            if (!TownControlAllegiances.IsAllowedAllegiance(killerMonarchId))
                            {
                                tcLog.Debug($"[TownControl] {town.TownName} init boss killer allegiance {killerMonarchId} is not whitelisted.");
                                CurrentLandblock?.EnqueueBroadcast(null, true, null, null,
                                    new GameMessageSystemChat(
                                        $"{bossName} has been killed by an allegiance that is not enabled for town control. " +
                                        "Please contact an admin to enable your clan.",
                                        ChatMessageType.Broadcast));
                                abort = true;
                            }
                        }
                    }

                    // Resolve defending clan (current owner)
                    uint? defendingClanId      = null;
                    string defendingClanName   = null;

                    if (!abort && town.OwnerId.HasValue)
                    {
                        var owningMonarch = PlayerManager.FindByGuid(town.OwnerId.Value);
                        if (owningMonarch != null)
                        {
                            var owningAlleg = AllegianceManager.GetAllegiance(owningMonarch);
                            if (owningAlleg?.Monarch != null && owningAlleg.Monarch.PlayerGuid.Full == owningMonarch.Guid.Full)
                            {
                                defendingClanId   = owningAlleg.MonarchId;
                                defendingClanName = owningAlleg.Monarch.Player?.Name ?? "Unknown";
                                tcLog.Debug($"[TownControl] {town.TownName} defenders: id={defendingClanId}, name={defendingClanName}");
                            }
                        }
                    }

                    if (!abort && defendingClanId.HasValue && defendingClanId.Value == killerMonarchId)
                    {
                        tcLog.Debug($"[TownControl] {town.TownName} owner clan killed init boss — conflict not started.");
                        CurrentLandblock?.EnqueueBroadcast(null, true, null, null,
                            new GameMessageSystemChat(
                                $"{bossName} has been killed by a member of the clan that owns {town.TownName}. No conflict will be started.",
                                ChatMessageType.Broadcast));
                        abort = true;
                    }

                    if (!abort)
                    {
                        // Respite check
                        var recentAttackerEvent = TownControlManager.GetLatestEventByAttacker(killerMonarchId, deadBoss.TownId);
                        if (recentAttackerEvent?.EventStartTime != null)
                        {
                            var respiteSeconds = town.ConflictRespiteLength ?? 0u;
                            var respiteExpiry  = recentAttackerEvent.EventStartTime.Value.AddSeconds(respiteSeconds);
                            if (DateTime.UtcNow < respiteExpiry)
                            {
                                var remaining = respiteExpiry - DateTime.UtcNow;
                                tcLog.Debug($"[TownControl] {town.TownName} respite timer still active for clan {killerMonarchId}.");
                                CurrentLandblock?.EnqueueBroadcast(null, true, null, null,
                                    new GameMessageSystemChat(
                                        $"{bossName} has been killed by clan {killerAllegName}, but they attacked {town.TownName} too recently. " +
                                        $"Respite expires in {remaining:h'h 'm'm 's's'}.",
                                        ChatMessageType.Broadcast));
                                abort = true;
                            }
                        }
                    }

                    // Stop event either way — clean up content-generated event
                    if (TownControlLandblocks.TryGetEventName(CurrentLandblock?.Id.Landblock ?? 0, out var evtNameAbort))
                        EventManager.StopEvent(evtNameAbort, null, null);

                    if (abort) return;

                    // --- Start the conflict ---
                    var tcEvent = TownControlManager.StartEvent(deadBoss.TownId, killerMonarchId, killerAllegName, defendingClanId, defendingClanName);
                    if (tcEvent == null)
                    {
                        tcLog.Error($"[TownControl] StartEvent returned null for {town.TownName}.");
                        return;
                    }

                    town.IsInConflict = true;
                    town.LastConflictStartTime = tcEvent.EventStartTime;
                    TownControlManager.SaveTown(town);

                    // Start content event for conflict phase
                    if (TownControlLandblocks.TryGetEventName(CurrentLandblock?.Id.Landblock ?? 0, out var evtNameStart))
                        EventManager.StartEvent(evtNameStart, null, null);

                    string startMsg = defendingClanId.HasValue
                        ? $"{killer?.Name} has slain {bossName}, throwing {town.TownName} into conflict! Clan {killerAllegName} challenges clan {defendingClanName} for ownership of {town.TownName}!"
                        : $"{killer?.Name} has slain {bossName}, throwing {town.TownName} into conflict! Clan {killerAllegName} moves to claim ownership of {town.TownName}!";

                    PlayerManager.BroadcastToAll(new GameMessageSystemChat(startMsg, ChatMessageType.Broadcast));
                    DiscordWebhookManager.SendTownControl(startMsg);
                    tcLog.Info($"[TownControl] {town.TownName} conflict started. Attacker={killerAllegName} ({killerMonarchId}).");
                }

                // ---------------------------------------------------------------
                // Conflict Boss killed → resolve the conflict
                // ---------------------------------------------------------------
                else
                {
                    var tcEvent = TownControlManager.GetLatestEventByTown(deadBoss.TownId);

                    // Always stop the content event
                    if (TownControlLandblocks.TryGetEventName(CurrentLandblock?.Id.Landblock ?? 0, out var evtName))
                        EventManager.StopEvent(evtName, null, null);

                    if (!town.IsInConflict || tcEvent == null || !tcEvent.EventStartTime.HasValue || tcEvent.EventEndTime.HasValue)
                    {
                        tcLog.Debug($"[TownControl] {town.TownName} conflict boss killed but no active event found.");
                        CurrentLandblock?.EnqueueBroadcast(null, true, null, null,
                            new GameMessageSystemChat(
                                $"{bossName} has been defeated but no active conflict event was found for {town.TownName}.",
                                ChatMessageType.Broadcast));
                        return;
                    }

                    var expiry = tcEvent.EventStartTime.Value.AddSeconds(town.ConflictLength);
                    string endMsg;

                    if (DateTime.UtcNow < expiry)
                    {
                        // ---- Attackers win ----
                        tcLog.Debug($"[TownControl] {town.TownName} conflict boss killed within time limit. Attackers win.");

                        town.OwnerId      = tcEvent.AttackerId;
                        town.IsInConflict = false;
                        TownControlManager.SaveTown(town);

                        tcEvent.IsAttackSuccess = true;
                        tcEvent.EventEndTime    = DateTime.UtcNow;
                        TownControlManager.SaveEvent(tcEvent);

                        // Update quest stamps — remove old ownership stamp from all online players,
                        // grant it to attackers who are online
                        string questName = town.TownName.Trim().Replace(" ", "") + "TownControlOwner";
                        RefreshTownControlQuestStamps(questName, tcEvent.AttackerId);

                        // Award attackers in the reward landblocks
                        AwardTownControlRewards(town, tcEvent, isAttackerVictory: true);

                        endMsg = tcEvent.DefenderId.HasValue
                            ? $"The battle for {town.TownName} is over! Clan {tcEvent.AttackerClanName} has slain {bossName} and wrested control from clan {tcEvent.DefenderClanName}!"
                            : $"The battle for {town.TownName} is over! Clan {tcEvent.AttackerClanName} has slain {bossName} and claimed ownership of {town.TownName}!";
                    }
                    else
                    {
                        // ---- Defenders win ----
                        tcLog.Debug($"[TownControl] {town.TownName} conflict boss killed after time limit. Defenders win.");

                        town.IsInConflict = false;
                        TownControlManager.SaveTown(town);

                        tcEvent.IsAttackSuccess = false;
                        tcEvent.EventEndTime    = DateTime.UtcNow;
                        TownControlManager.SaveEvent(tcEvent);

                        AwardTownControlRewards(town, tcEvent, isAttackerVictory: false);

                        endMsg = tcEvent.DefenderId.HasValue
                            ? $"The battle for {town.TownName} has ended! Clan {tcEvent.AttackerClanName} failed to slay {bossName} in time. Clan {tcEvent.DefenderClanName} retains ownership of {town.TownName}!"
                            : $"The battle for {town.TownName} has ended! Clan {tcEvent.AttackerClanName} failed to slay {bossName} in time. {town.TownName} remains unclaimed.";
                    }

                    PlayerManager.BroadcastToAll(new GameMessageSystemChat(endMsg, ChatMessageType.Broadcast));
                    DiscordWebhookManager.SendTownControl(endMsg);
                }
            }
            catch (Exception ex)
            {
                tcLog.Error($"Exception in Creature_TownControl.HandleTownControlBossDeath. WeenieId={WeenieClassId}. Ex: {ex}");
            }
        }

        // -----------------------------------------------------------------------
        // Heartbeat (conflict progress + stale boss cleanup)
        // -----------------------------------------------------------------------

        /// <summary>
        /// Called from Creature.Heartbeat every ~5 s when IsTownControlBoss is true.
        /// </summary>
        public void HandleTownControlBossHeartbeat()
        {
            try
            {
                var boss = TownControlBosses.Get(WeenieClassId);
                if (boss == null) return;

                var town = TownControlManager.GetTown(boss.TownId);
                if (town == null) return;

                if (IsTownControlConflictBoss)
                {
                    var tcEvent = TownControlManager.GetLatestEventByTown(boss.TownId);

                    if (!town.IsInConflict || tcEvent == null || !tcEvent.EventStartTime.HasValue || tcEvent.EventEndTime.HasValue)
                    {
                        // Stale conflict boss — no active event, destroy it
                        tcLog.Debug($"[TownControl] Conflict boss WeenieId={WeenieClassId} exists with no active event. Destroying.");
                        var dmgHistory = new DamageHistoryInfo(this);
                        OnDeath(dmgHistory, DamageType.Bludgeon);
                        Die(dmgHistory, dmgHistory);
                        return;
                    }

                    var expiry = tcEvent.EventStartTime.Value.AddSeconds(town.ConflictLength);

                    if (DateTime.UtcNow > expiry)
                    {
                        // Event expired with boss alive — force-kill (triggers Creature_Death → HandleTownControlBossDeath → defender win)
                        var dmgHistory = new DamageHistoryInfo(this);
                        OnDeath(dmgHistory, DamageType.Bludgeon);
                        Die(dmgHistory, dmgHistory);
                        return;
                    }

                    // Send periodic progress broadcasts
                    if (!TownControl_ConflictBroadcast1Sent &&
                        (Health.Percent < 0.75 || DateTime.UtcNow > tcEvent.EventStartTime.Value.AddSeconds(town.ConflictLength / 4)))
                    {
                        Broadcast_TCConflictProgress(town, tcEvent, expiry, 1);
                        TownControl_ConflictBroadcast1Sent = true;
                    }
                    else if (TownControl_ConflictBroadcast1Sent && !TownControl_ConflictBroadcast2Sent &&
                             (Health.Percent < 0.50 || DateTime.UtcNow > tcEvent.EventStartTime.Value.AddSeconds(town.ConflictLength / 2)))
                    {
                        Broadcast_TCConflictProgress(town, tcEvent, expiry, 2);
                        TownControl_ConflictBroadcast2Sent = true;
                    }
                    else if (TownControl_ConflictBroadcast2Sent && !TownControl_ConflictBroadcast3Sent &&
                             (Health.Percent < 0.25 || DateTime.UtcNow > tcEvent.EventStartTime.Value.AddSeconds(town.ConflictLength / 1.25)))
                    {
                        Broadcast_TCConflictProgress(town, tcEvent, expiry, 3);
                        TownControl_ConflictBroadcast3Sent = true;
                    }
                }
                else
                {
                    // Init boss low-HP broadcast
                    if (!TownControl_InitLowHpBroadcastSent && Health.Percent < 0.33f)
                    {
                        var msg = $"{town.TownName} is under attack! {Name} is faltering! All who seek to rule {town.TownName} must come at once!";
                        PlayerManager.BroadcastToAll(new GameMessageSystemChat(msg, ChatMessageType.Broadcast));
                        TownControl_InitLowHpBroadcastSent = true;
                    }
                    else if (TownControl_InitLowHpBroadcastSent && Health.Percent > 0.95f)
                    {
                        TownControl_InitLowHpBroadcastSent = false;
                    }
                }
            }
            catch (Exception ex)
            {
                tcLog.Error($"Exception in Creature_TownControl.HandleTownControlBossHeartbeat. WeenieId={WeenieClassId}. Ex: {ex}");
            }
        }

        private void Broadcast_TCConflictProgress(
            ACE.Database.Models.Log.TownControlTown town,
            ACE.Database.Models.Log.TownControlEvent tcEvent,
            DateTime expiry,
            int broadcastNumber)
        {
            var minutesLeft = Math.Round((expiry - DateTime.UtcNow).TotalMinutes, 0, MidpointRounding.AwayFromZero);
            var hpPct       = Math.Round(Health.Percent * 100, 0);
            var msg = $"{town.TownName} is under attack! Clan {tcEvent.AttackerClanName} has reduced the town's defenses to " +
                      $"{hpPct}% with {minutesLeft} minute(s) remaining.";
            PlayerManager.BroadcastToAll(new GameMessageSystemChat(msg, ChatMessageType.Broadcast));
        }

        // -----------------------------------------------------------------------
        // Quest stamp management
        // -----------------------------------------------------------------------

        /// <summary>
        /// Strips the TC ownership quest stamp from all online players, then
        /// grants it to every online member of the winning allegiance.
        /// Offline players will have their stamps corrected on login via
        /// Player_TownControl.ValidateTownControlQuestStamps().
        /// </summary>
        private static void RefreshTownControlQuestStamps(string questName, uint winnerMonarchId)
        {
            // Strip from everyone online
            foreach (var player in PlayerManager.GetAllOnline())
            {
                if (player.QuestManager.HasQuest(questName))
                    player.QuestManager.Erase(questName);
            }

            // Grant to online members of the winning allegiance
            var winnerMembers = AllegianceManager.FindAllPlayers(new ObjectGuid(winnerMonarchId));
            if (winnerMembers == null) return;

            foreach (var member in winnerMembers)
            {
                var online = PlayerManager.GetOnlinePlayer(member.Guid);
                if (online != null)
                    online.QuestManager.Update(questName);
            }
        }

        // -----------------------------------------------------------------------
        // Reward distribution
        // -----------------------------------------------------------------------

        private const uint WcidPkTrophy = 1000002;
        private const uint WcidABox     = 510000;
        private const uint WcidHeraKey  = 490364;

        private static void AwardTownControlRewards(
            ACE.Database.Models.Log.TownControlTown town,
            ACE.Database.Models.Log.TownControlEvent tcEvent,
            bool isAttackerVictory)
        {
            try
            {
                uint? rewardMonarchId = isAttackerVictory ? (uint?)tcEvent.AttackerId : tcEvent.DefenderId;
                if (!rewardMonarchId.HasValue) return;

                ushort awardIndividual = isAttackerVictory ? town.AttackerAwardsIndividual : town.DefenderAwardsIndividual;
                ushort awardTotal      = isAttackerVictory ? town.AttackerAwardsTotal      : town.DefenderAwardsTotal;

                // Collect eligible players in the reward landblocks
                var rewardLandblocks = TownControlLandblocks.GetRewardLandblocks(town.TownId);
                if (rewardLandblocks == null || rewardLandblocks.Length == 0) return;

                var eligiblePlayers = new List<Player>();
                foreach (var lbId in rewardLandblocks)
                {
                    var lb = LandblockManager.GetLandblock(new LandblockId(lbId << 16), false);
                    if (lb == null) continue;

                    foreach (var player in lb.GetPlayers())
                    {
                        var alleg = AllegianceManager.GetAllegiance(player);
                        if (alleg?.MonarchId == rewardMonarchId && !eligiblePlayers.Contains(player))
                            eligiblePlayers.Add(player);
                    }
                }

                if (eligiblePlayers.Count == 0) return;

                // Trophy count = min(total / headcount, individual cap)
                int trophiesPerPlayer = (int)Math.Min(awardTotal / eligiblePlayers.Count, awardIndividual);

                foreach (var player in eligiblePlayers)
                {
                    bool doubleReward = ThreadSafeRandom.Next(0, 100) > 85;
                    int trophyCount   = doubleReward ? trophiesPerPlayer * 2 : trophiesPerPlayer;

                    // PK Trophies
                    if (trophyCount > 0)
                    {
                        var trophy = WorldObjectFactory.CreateNewWorldObject(WcidPkTrophy);
                        if (trophy != null)
                        {
                            trophy.SetStackSize(trophyCount);
                            if (player.TryCreateInInventoryWithNetworking(trophy))
                            {
                                player.Session.Network.EnqueueSend(new GameMessageCreateObject(trophy));
                                player.Session.Network.EnqueueSend(new GameMessageSystemChat(
                                    $"{(doubleReward ? "Lucky day — your reward was doubled! " : "")}You received {trophyCount} PK Trophies for {(isAttackerVictory ? "attacking" : "defending")} {town.TownName}.",
                                    ChatMessageType.Broadcast));
                            }
                        }
                    }

                    // Hera's Vault Key
                    var heraKey = WorldObjectFactory.CreateNewWorldObject(WcidHeraKey);
                    if (heraKey != null && player.TryCreateInInventoryWithNetworking(heraKey))
                    {
                        player.Session.Network.EnqueueSend(new GameMessageCreateObject(heraKey));
                        player.Session.Network.EnqueueSend(new GameMessageSystemChat(
                            $"You received a Hera's Vault Key for {(isAttackerVictory ? "attacking" : "defending")} {town.TownName}.",
                            ChatMessageType.Broadcast));
                    }

                    // A Box — attackers get 1, defenders get 5
                    int boxCount = isAttackerVictory ? 1 : 5;
                    for (int i = 0; i < boxCount; i++)
                    {
                        var box = WorldObjectFactory.CreateNewWorldObject(WcidABox);
                        if (box != null && player.TryCreateInInventoryWithNetworking(box))
                        {
                            player.Session.Network.EnqueueSend(new GameMessageCreateObject(box));
                            if (i == 0)
                            {
                                player.Session.Network.EnqueueSend(new GameMessageSystemChat(
                                    $"You received {(boxCount > 1 ? boxCount + "x " : "")}A Box for {(isAttackerVictory ? "attacking" : "defending")} {town.TownName}.",
                                    ChatMessageType.Broadcast));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                tcLog.Error($"Exception in Creature_TownControl.AwardTownControlRewards for TownId={town.TownId}. Ex: {ex}");
            }
        }
    }
}
