using System;

using log4net;

using ACE.Entity.Enum;
using ACE.Entity.Enum.Properties;
using ACE.Server.Entity;
using ACE.Server.Entity.Actions;
using ACE.Server.Entity.AllegianceHometown;
using ACE.Server.Managers;
using ACE.Server.Network.GameEvent.Events;
using ACE.Server.Network.GameMessages.Messages;

namespace ACE.Server.WorldObjects
{
    /// <summary>
    /// Allegiance Hometown Phase 2 behaviour for Bindstone world objects.
    /// Partial class extends Bindstone with HP tracking, damage reception,
    /// and Phase 2 heartbeat logic.
    /// </summary>
    public partial class Bindstone
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // -----------------------------------------------------------------------
        // Phase 2 state
        // -----------------------------------------------------------------------

        public int BindstoneMaxHp { get; private set; }
        public int BindstoneCurrentHp { get; private set; }

        public bool IsPhase2Active => Attackable == true && BindstoneMaxHp > 0;

        // -----------------------------------------------------------------------
        // Phase 1: ActOnUse dispatch
        // -----------------------------------------------------------------------

        private void HandleHometownUse(Player player)
        {
            var entry = AllegianceHometownRegistry.GetByLandblock(CurrentLandblock?.Id.Landblock ?? 0);
            if (entry == null)
            {
                // Not a hometown bindstone — fall through to original sanctuary logic
                HandleSanctuaryUse(player);
                return;
            }

            if (player.Allegiance == null)
            {
                player.Session.Network.EnqueueSend(new GameEventWeenieError(player.Session, WeenieError.YouAreNotInAllegiance));
                return;
            }

            if (player.AllegiancePermissionLevel < AllegiancePermissionLevel.Seneschal)
            {
                player.Session.Network.EnqueueSend(new GameEventWeenieError(player.Session, WeenieError.YouDoNotHaveAuthorityInAllegiance));
                return;
            }

            if (!player.IsPK)
            {
                player.SendTransientError("Only Player Killers can claim or contest hometowns.");
                return;
            }

            var monarchId = player.Allegiance.MonarchId ?? player.Guid.Full;
            var allegianceName = player.Allegiance.AllegianceName ?? player.Name;
            var town = AllegianceHometownManager.GetTown(entry.TownId);

            if (town == null)
            {
                player.SendTransientError("Hometown data is unavailable. Please try again later.");
                return;
            }

            // Phase 2 in progress — cannot use while bindstone is under attack
            if (town.ConflictPhase == 2)
            {
                player.SendTransientError("The bind stone is currently under siege.");
                return;
            }

            // Unowned town — free claim
            if (!town.OwnerMonarchId.HasValue)
            {
                AllegianceHometownManager.ClaimTown(entry.TownId, monarchId, allegianceName);
                player.Session.Network.EnqueueSend(new GameMessageSystemChat(
                    $"Your allegiance has claimed {entry.TownName}!", ChatMessageType.Magic));
                return;
            }

            // Owned by this allegiance
            if (town.OwnerMonarchId == monarchId)
            {
                player.Session.Network.EnqueueSend(new GameMessageSystemChat(
                    $"Your allegiance already owns {entry.TownName}.", ChatMessageType.Magic));
                return;
            }

            // Phase 1 already in progress on this town
            if (town.ConflictPhase == 1)
            {
                player.SendTransientError($"{entry.TownName} is already under attack.");
                return;
            }

            // Attempt to start Phase 1
            if (!AllegianceHometownManager.TryStartPhase1(entry.TownId, monarchId, allegianceName, out var failReason))
            {
                player.SendTransientError(failReason);
                return;
            }

            player.Session.Network.EnqueueSend(new GameMessageSystemChat(
                $"Phase 1 assault on {entry.TownName} has begun! Hold this position with at least 2 allies for 4 minutes.",
                ChatMessageType.Magic));
        }

        private void HandleSanctuaryUse(Player player)
        {
            // Original Bindstone sanctuary-set logic preserved for non-hometown bindstones
            if (player.Allegiance == null)
            {
                player.Session.Network.EnqueueSend(new GameEventWeenieError(player.Session, WeenieError.YouAreNotInAllegiance));
                return;
            }

            if (player.AllegiancePermissionLevel < AllegiancePermissionLevel.Seneschal)
            {
                player.Session.Network.EnqueueSend(new GameEventWeenieError(player.Session, WeenieError.YouDoNotHaveAuthorityInAllegiance));
                return;
            }

            var actionChain = new ActionChain();
            if (player.CombatMode != CombatMode.NonCombat)
            {
                var stanceTime = player.SetCombatMode(CombatMode.NonCombat);
                actionChain.AddDelaySeconds(stanceTime);
                player.LastUseTime += stanceTime;
            }

            actionChain.AddAction(this, () => EnqueueBroadcastMotion(new Motion(MotionStance.NonCombat, MotionCommand.Twitch1)));
            player.LastUseTime += player.EnqueueMotion(actionChain, MotionCommand.Sanctuary);

            actionChain.AddAction(this, () =>
            {
                if (player.IsWithinUseRadiusOf(this))
                {
                    player.Allegiance.Sanctuary = new ACE.Entity.Position(player.Location);
                    player.Allegiance.SaveBiotaToDatabase();
                    player.Session.Network.EnqueueSend(new GameMessageSystemChat(
                        GetProperty(PropertyString.UseMessage), ChatMessageType.Magic));
                }
                else
                    player.Session.Network.EnqueueSend(new GameEventWeenieError(player.Session, WeenieError.YouHaveMovedTooFar));
            });

            actionChain.EnqueueChain();
        }

        // -----------------------------------------------------------------------
        // Phase 2: activation / deactivation
        // -----------------------------------------------------------------------

        public void ActivatePhase2(int maxHp)
        {
            BindstoneMaxHp     = maxHp;
            BindstoneCurrentHp = maxHp;
            Attackable         = true;

            BroadcastHpStatus();
        }

        public void DeactivatePhase2()
        {
            Attackable         = false;
            BindstoneCurrentHp = BindstoneMaxHp;

            BroadcastHpStatus();

            BindstoneMaxHp     = 0;
            BindstoneCurrentHp = 0;
        }

        // -----------------------------------------------------------------------
        // Phase 2: damage reception
        // -----------------------------------------------------------------------

        /// <summary>
        /// Called by the melee/missile attack loops when a player swings at this
        /// bindstone during Phase 2.
        /// </summary>
        public void ReceiveAttack(Player attacker, float powerLevel)
        {
            if (!IsPhase2Active) return;

            var entry = AllegianceHometownRegistry.GetByLandblock(CurrentLandblock?.Id.Landblock ?? 0);
            if (entry == null) return;

            var town = AllegianceHometownManager.GetTown(entry.TownId);
            if (town == null || town.ConflictPhase != 2) return;

            // Only the attacking allegiance can damage the bindstone
            var attackerMonarchId = attacker.Allegiance?.MonarchId ?? attacker.Guid.Full;
            if (attackerMonarchId != town.ConflictAttackerMonarchId)
            {
                attacker.SendTransientError("Only the attacking allegiance can damage the bind stone.");
                return;
            }

            // Distance multiplier
            var dist       = attacker.Location.DistanceTo(Location);
            var distMult   = AllegianceHometownManager.GetDistanceMultiplier(dist);
            var damage     = (int)MathF.Round(ComputeHitDamage(attacker, powerLevel, distMult));
            if (damage < 1) damage = 1;

            BindstoneCurrentHp -= damage;

            // Floating damage text for the attacker
            attacker.Session.Network.EnqueueSend(
                new GameMessageSystemChat(
                    $"You deal {damage} damage to the bind stone. ({BindstoneCurrentHp:N0}/{BindstoneMaxHp:N0} HP remaining)",
                    ChatMessageType.Combat));

            BroadcastHpStatus();

            if (BindstoneCurrentHp <= 0)
            {
                BindstoneCurrentHp = 0;
                // Attacker victory — manager handles ownership transfer, cooldowns, broadcast
                AllegianceHometownManager.HandleAttackerVictory(entry.TownId);
                AllegianceHometownManager.UnregisterPhase2Bindstone(entry.TownId);
                DeactivatePhase2();
            }
        }

        // -----------------------------------------------------------------------
        // Phase 2: kill effects (called from Player_Death)
        // -----------------------------------------------------------------------

        /// <summary>
        /// A defender (enemy allegiance member) was killed in the Phase 2 zone.
        /// The bindstone loses 5% of max HP as a bonus for the attackers.
        /// </summary>
        public void OnDefenderKilled()
        {
            if (!IsPhase2Active) return;

            var bonus = AllegianceHometownManager.GetKillEffect(BindstoneMaxHp);
            BindstoneCurrentHp -= bonus;

            BroadcastHpStatus();

            if (BindstoneCurrentHp <= 0)
            {
                var entry = AllegianceHometownRegistry.GetByLandblock(CurrentLandblock?.Id.Landblock ?? 0);
                if (entry != null)
                {
                    BindstoneCurrentHp = 0;
                    AllegianceHometownManager.HandleAttackerVictory(entry.TownId);
                    AllegianceHometownManager.UnregisterPhase2Bindstone(entry.TownId);
                }
                DeactivatePhase2();
            }
        }

        /// <summary>
        /// An attacker (attacking allegiance member) was killed in the Phase 2 zone.
        /// The bindstone heals 5% of max HP as a bonus for the defenders.
        /// </summary>
        public void OnAttackerKilled()
        {
            if (!IsPhase2Active) return;

            var heal = AllegianceHometownManager.GetKillEffect(BindstoneMaxHp);
            BindstoneCurrentHp = Math.Min(BindstoneMaxHp, BindstoneCurrentHp + heal);

            BroadcastHpStatus();
        }

        // -----------------------------------------------------------------------
        // Phase 2: 30-minute timeout heartbeat
        // -----------------------------------------------------------------------

        public override void Heartbeat(double currentUnixTime)
        {
            if (IsPhase2Active)
            {
                var entry = AllegianceHometownRegistry.GetByLandblock(CurrentLandblock?.Id.Landblock ?? 0);
                if (entry != null)
                {
                    var town = AllegianceHometownManager.GetTown(entry.TownId);
                    if (town?.ConflictPhase == 2 && town.Phase2StartTime.HasValue)
                    {
                        var elapsed = DateTime.UtcNow - town.Phase2StartTime.Value;
                        if (elapsed >= TimeSpan.FromMinutes(30))
                        {
                            // Defender victory — bindstone survives
                            AllegianceHometownManager.HandleDefenderVictory(entry.TownId);
                            AllegianceHometownManager.UnregisterPhase2Bindstone(entry.TownId);
                            DeactivatePhase2();
                        }
                    }
                }
            }

            base.Heartbeat(currentUnixTime);
        }

        // -----------------------------------------------------------------------
        // Helpers
        // -----------------------------------------------------------------------

        private float ComputeHitDamage(Player attacker, float powerLevel, float distMultiplier)
        {
            var xpCap    = RollingLevelCapManager.GetCurrentXpCap();
            var levelCap = RollingLevelCapManager.GetCurrentLevelCap(xpCap);

            float dpsPerPlayer = Math.Clamp(25f + 75f * (levelCap - 15f) / 115f, 25f, 100f);

            var weaponSkill   = attacker.GetCurrentWeaponSkill();
            var skill         = attacker.GetCreatureSkill(weaponSkill).Current;
            float expectedSkill = Math.Max(1f, levelCap * 2.5f);
            float skillRatio    = Math.Clamp(skill / expectedSkill, 0.1f, 2.0f);

            // Assumes ~3-second hit interval at full power — scales with power level
            return skillRatio * dpsPerPlayer * 3f * powerLevel * distMultiplier;
        }

        private void BroadcastHpStatus()
        {
            // Send HP fraction to all players on the landblock as system chat
            // (WorldObjects don't have a native HP bar visible to players)
            if (BindstoneMaxHp <= 0) return;

            var pct     = (float)BindstoneCurrentHp / BindstoneMaxHp * 100f;
            var msg     = new GameMessageSystemChat(
                $"[Bind Stone] HP: {BindstoneCurrentHp:N0} / {BindstoneMaxHp:N0} ({pct:0.0}%)",
                ChatMessageType.WorldBroadcast);

            CurrentLandblock?.EnqueueBroadcast(null, false, Location, null, msg);
        }
    }
}
