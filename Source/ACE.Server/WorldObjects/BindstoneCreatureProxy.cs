using System;

using log4net;

using ACE.Entity;
using ACE.Entity.Enum;
using ACE.Entity.Models;
using ACE.Server.Entity;
using ACE.Server.Entity.AllegianceHometown;
using ACE.Server.Managers;
using ACE.Server.Network.GameMessages.Messages;

namespace ACE.Server.WorldObjects
{
    /// <summary>
    /// Temporary Creature spawned at the bindstone position during Phase 2 of
    /// Allegiance Hometown Capture.  Because the AC client only sends
    /// TargetedMeleeAttack packets for Creature-type objects, we cannot make the
    /// static Bindstone WorldObject directly attackable.  This proxy looks like
    /// the bindstone (same Setup model) and receives all standard melee damage.
    /// When destroyed it triggers the attacker-victory path instead of the normal
    /// creature-death path (no corpse, no XP, no loot).
    /// </summary>
    public class BindstoneCreatureProxy : Creature
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public byte TownId { get; set; }

        private DateTime _lastPhase2Broadcast = DateTime.MinValue;

        public BindstoneCreatureProxy(Weenie weenie, ObjectGuid guid) : base(weenie, guid) { }

        // -----------------------------------------------------------------------
        // Death — trigger attacker victory instead of normal creature death
        // -----------------------------------------------------------------------

        public override DeathMessage OnDeath(DamageHistoryInfo lastDamager, DamageType damageType, bool criticalHit = false)
        {
            // Skip XP grants and kill quests; just log and proceed
            OnMovementStopped();
            return GetDeathMessage(lastDamager, damageType, criticalHit);
        }

        private bool _dieEntered = false;

        protected override void Die(DamageHistoryInfo lastDamager, DamageHistoryInfo topDamager)
        {
            if (_dieEntered) return;
            _dieEntered = true;

            UpdateVital(Health, 0);

            // No corpse, no loot — just despawn after death animation
            CurrentMotionState = new Motion(MotionStance.NonCombat, MotionCommand.Ready);
            PhysicsObj?.StopCompletely(true);

            var motionDeath = new Motion(MotionStance.NonCombat, MotionCommand.Dead);
            var deathAnimLength = ExecuteMotion(motionDeath);

            var chain = new ACE.Server.Entity.Actions.ActionChain();
            chain.AddDelaySeconds(Math.Max(deathAnimLength, 0.5));
            chain.AddAction(this, () =>
            {
                AllegianceHometownManager.HandleAttackerVictory(TownId);
                AllegianceHometownManager.UnregisterPhase2Proxy(TownId);
                Destroy();
            });
            chain.EnqueueChain();
        }

        // -----------------------------------------------------------------------
        // Kill effects called from Player_Death
        // -----------------------------------------------------------------------

        /// <summary>A defender (owner allegiance member) was killed — bindstone loses 5% max HP.</summary>
        public void OnDefenderKilled()
        {
            if (!IsAlive) return;
            var loss = AllegianceHometownManager.GetKillEffect((int)Health.MaxValue);
            var newHp = (uint)Math.Max(1, (int)Health.Current - loss);
            UpdateVital(Health, newHp);
            BroadcastStatus();

            if (Health.Current <= 1)
                TakeDamage(this, DamageType.Bludgeon, 1);
        }

        /// <summary>An attacker (attacking allegiance member) was killed — bindstone heals 5% max HP.</summary>
        public void OnAttackerKilled()
        {
            if (!IsAlive) return;
            var heal = AllegianceHometownManager.GetKillEffect((int)Health.MaxValue);
            var newHp = (uint)Math.Min(Health.MaxValue, Health.Current + heal);
            UpdateVital(Health, newHp);
            BroadcastStatus();
        }

        // -----------------------------------------------------------------------
        // Broadcast HP status after each hit
        // -----------------------------------------------------------------------

        public void BroadcastStatus()
        {
            if (Health.MaxValue == 0) return;
            var pct = (float)Health.Current / Health.MaxValue * 100f;
            var msg = new GameMessageSystemChat(
                $"[Bind Stone] HP: {Health.Current:N0} / {Health.MaxValue:N0} ({pct:0.0}%)",
                ChatMessageType.WorldBroadcast);
            CurrentLandblock?.EnqueueBroadcast(null, false, Location, null, msg);
        }

        // -----------------------------------------------------------------------
        // Heartbeat — 30-min Phase 2 timeout + 60-s global broadcast
        // -----------------------------------------------------------------------

        public override void Heartbeat(double currentUnixTime)
        {
            if (IsAlive)
            {
                var entry = AllegianceHometownRegistry.GetByLandblock(CurrentLandblock?.Id.Landblock ?? 0);
                if (entry != null)
                {
                    var town = AllegianceHometownManager.GetTown(entry.TownId);
                    if (town?.ConflictPhase == 2 && town.Phase2StartTime.HasValue)
                    {
                        var elapsed       = DateTime.UtcNow - town.Phase2StartTime.Value;
                        var phase2Duration = TimeSpan.FromMinutes(30);

                        if (elapsed >= phase2Duration)
                        {
                            AllegianceHometownManager.HandleDefenderVictory(entry.TownId);
                            AllegianceHometownManager.UnregisterPhase2Proxy(entry.TownId);
                            Destroy();
                            return;
                        }

                        var now = DateTime.UtcNow;
                        if ((now - _lastPhase2Broadcast).TotalSeconds >= 60)
                        {
                            _lastPhase2Broadcast = now;
                            var remaining = phase2Duration - elapsed;
                            var hpPct     = Health.MaxValue > 0
                                ? (float)Health.Current / Health.MaxValue * 100f
                                : 0f;
                            var timeStr = AllegianceHometownManager.FormatTimeSpan(remaining);
                            PlayerManager.BroadcastToAll(
                                new GameMessageSystemChat(
                                    $"[{entry.TownName}] The Bind Stone is under attack by {town.ConflictAttackerName}! " +
                                    $"Time remaining: {timeStr} — Bind Stone HP: {hpPct:0.0}%",
                                    ChatMessageType.WorldBroadcast));
                        }
                    }
                }
            }

            base.Heartbeat(currentUnixTime);
        }
    }
}
