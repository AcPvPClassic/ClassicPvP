using System;
using System.Collections.Generic;
using System.Linq;

using ACE.Common;
using ACE.Entity;
using ACE.Entity.Enum;
using ACE.Entity.Enum.Properties;
using ACE.Entity.Models;
using ACE.Server.Entity.Actions;
using ACE.Server.Network.GameMessages.Messages;

namespace ACE.Server.WorldObjects
{
    public class Hotspot : WorldObject
    {
        public Hotspot(Weenie weenie, ObjectGuid guid) : base(weenie, guid)
        {
            SetEphemeralValues();
        }

        public Hotspot(Biota biota) : base(biota)
        {
            SetEphemeralValues();
        }

        private void SetEphemeralValues()
        {
            // If CycleTime is less than 1, player has a very bad time.
            if ((CycleTime ?? 0) < 1)
                CycleTime = 1;

            // A death zone's proximity scan is driven off Heartbeat. If this weenie has heartbeats
            // disabled, force them on so the scan loop can start (and self-heal) reliably.
            if (IsDeathZone && NextHeartbeatTime == double.MaxValue)
            {
                HeartbeatInterval = 1.0f;
                ReinitializeHeartbeats();
            }
        }

        private HashSet<ObjectGuid> Creatures = new HashSet<ObjectGuid>();

        private ActionChain ActionLoop = null;

        public override void OnCollideObject(WorldObject wo)
        {
            if (!(wo is Creature creature))
                return;

            if (!AffectsAis && !(wo is Player))
                return;

            if (!Creatures.Contains(creature.Guid))
            {
                //Console.WriteLine($"{Name} ({Guid}).OnCollideObject({creature.Name})");

                Creatures.Add(creature.Guid);
                if (wo.HotspotImmunityTimestamp <= Time.GetUnixTime())
                    Activate(wo as Creature); // Antecipate first activation to the moment of contact, much better feedback this way.
            }

            if (ActionLoop == null)
            {
                ActionLoop = NextActionLoop;
                NextActionLoop.EnqueueChain();
            }
        }

        public override void OnCollideObjectEnd(WorldObject wo)
        {
            /*if (!(wo is Player player))
                return;

            if (Players.Contains(player.Guid))
                Players.Remove(player.Guid);*/
        }

        private ActionChain NextActionLoop
        {
            get
            {
                ActionLoop = new ActionChain();
                ActionLoop.AddDelaySeconds(CycleTimeNext);
                ActionLoop.AddAction(this, () =>
                {
                    if (Creatures.Any())
                    {
                        Activate();
                        NextActionLoop.EnqueueChain();
                    }
                    else
                    {
                        ActionLoop = null;
                    }
                });
                return ActionLoop;
            }
        }

        private double CycleTimeNext
        {
            get
            {
                var max = CycleTime;
                var min = max * (1.0f - CycleTimeVariance ?? 0.0f);

                return ThreadSafeRandom.Next((float)min, (float)max);
            }
        }

        public double? CycleTime
        {
            get => GetProperty(PropertyFloat.HotspotCycleTime);
            set { if (value == null) RemoveProperty(PropertyFloat.HotspotCycleTime); else SetProperty(PropertyFloat.HotspotCycleTime, (double)value); }
        }

        public double? CycleTimeVariance
        {
            get => GetProperty(PropertyFloat.HotspotCycleTimeVariance) ?? 0;
            set { if (value == null) RemoveProperty(PropertyFloat.HotspotCycleTimeVariance); else SetProperty(PropertyFloat.HotspotCycleTimeVariance, (double)value); }
        }

        private float DamageNext
        {
            get
            {
                var r = GetBaseDamage();
                var p = (float)ThreadSafeRandom.Next(r.MinDamage, r.MaxDamage);
                return p;
            }
        }

        private int? _DamageType
        {
            get => GetProperty(PropertyInt.DamageType);
            set { if (value == null) RemoveProperty(PropertyInt.DamageType); else SetProperty(PropertyInt.DamageType, (int)value); }
        }

        public DamageType DamageType
        {
            get { return (DamageType)_DamageType; }
        }

        public bool IsHot
        {
            get => GetProperty(PropertyBool.IsHot) ?? false;
            set { if (!value) RemoveProperty(PropertyBool.IsHot); else SetProperty(PropertyBool.IsHot, value); }
        }

        public bool AffectsAis
        {
            get => GetProperty(PropertyBool.AffectsAis) ?? false;
            set { if (!value) RemoveProperty(PropertyBool.AffectsAis); else SetProperty(PropertyBool.AffectsAis, value); }
        }

        // --- Death Zone (proximity kill) ---------------------------------------------------------
        // A standard hotspot only harms creatures that physically collide with its geometry, so a
        // player who jumps clears the collision volume and passes through unharmed. When UseRadius is
        // set (> 0), this hotspot instead runs its own fast scan loop and kills any player within that
        // horizontal radius, independent of collision -- jumping no longer bypasses it.

        /// <summary>
        /// Horizontal radius (in meters) within which a death-zone hotspot instantly kills players.
        /// Backed by PropertyFloat.UseRadius. When null or &lt;= 0, this behaves as a normal hotspot.
        /// </summary>
        public double? DeathZoneRadius
        {
            get => GetProperty(PropertyFloat.UseRadius);
            set { if (value == null) RemoveProperty(PropertyFloat.UseRadius); else SetProperty(PropertyFloat.UseRadius, (double)value); }
        }

        public bool IsDeathZone => (DeathZoneRadius ?? 0) > 0;

        // How often the proximity scan runs. Fast enough that a sprinting player cannot cross the
        // zone between scans. Cheap for a handful of hallway blockers; do not set radius on hundreds.
        private const double DeathZoneScanInterval = 0.25;

        private bool DeathZoneLoopRunning = false;

        /// <summary>
        /// Proximity death zones self-drive on a fast scan loop rather than waiting on physics
        /// collision, so a jumping player who clears the collision volume is still killed. The loop is
        /// (re)started from Heartbeat, which makes it self-healing if it is ever interrupted.
        /// </summary>
        public override void Heartbeat(double currentUnixTime)
        {
            if (IsDeathZone && !DeathZoneLoopRunning)
                StartDeathZoneLoop();

            base.Heartbeat(currentUnixTime);
        }

        private void StartDeathZoneLoop()
        {
            DeathZoneLoopRunning = true;

            var loop = new ActionChain();
            loop.AddDelaySeconds(DeathZoneScanInterval);
            loop.AddAction(this, () =>
            {
                // Stop cleanly if the zone was disabled or the object left the world; the next
                // Heartbeat will restart the loop if the object is still around and still a death zone.
                if (!IsDeathZone || PhysicsObj == null || CurrentLandblock == null)
                {
                    DeathZoneLoopRunning = false;
                    return;
                }

                ScanAndKillNearbyPlayers();
                StartDeathZoneLoop();
            });
            loop.EnqueueChain();
        }

        private void ScanAndKillNearbyPlayers()
        {
            if (!IsHot) return;

            if (PhysicsObj?.ObjMaint == null || Location == null)
                return;

            var radius = DeathZoneRadius ?? 0;
            var radiusSq = radius * radius;

            var damageType = _DamageType.HasValue ? DamageType : DamageType.Bludgeon;

            foreach (var player in PhysicsObj.ObjMaint.GetVisiblePlayersValuesAsPlayer())
            {
                if (player == null || player.IsDead || player.Teleporting || player.Location == null)
                    continue;

                // Horizontal (X/Y) distance only -- Z is ignored so a jumping player is still caught.
                if (Location.Distance2DSquared(player.Location) > radiusSq)
                    continue;

                // Guaranteed kill. TakeDamage still honors Invincible / lifestone protection /
                // no-damage landblocks, so those safety systems are not overridden.
                player.TakeDamage(this, damageType, player.Health.Current, Server.Entity.BodyPart.Foot);

                if (!Visibility)
                    EnqueueBroadcast(new GameMessageSound(Guid, Sound.TriggerActivated, 1.0f));

                if (ActivationResponse.HasFlag(ActivationResponse.Emote))
                    OnEmote(player);
            }
        }

        private void Activate()
        {
            foreach (var creatureGuid in Creatures)
            {
                var creature = CurrentLandblock.GetObject(creatureGuid) as Creature;

                // verify current state of collision here
                if (creature == null || !creature.PhysicsObj.is_touching(PhysicsObj))
                {
                    //Console.WriteLine($"{Name} ({Guid}).OnCollideObjectEnd({creature?.Name})");
                    Creatures.Remove(creatureGuid);
                    continue;
                }

                Activate(creature);
            }
        }

        private void Activate(Creature creature)
        {
            if (!IsHot) return;

            var amount = DamageNext;
            var iAmount = (int)Math.Round(amount);

            var player = creature as Player;

            var currentTime = Time.GetUnixTime();

            if (creature.HotspotImmunityTimestamp > currentTime)
                return;
            else
            {
                var immunityTime = (CycleTime ?? 0) * (1.0f - CycleTimeVariance ?? 0.0f) * 0.9f; // Multiplying the minimum possible CycleTime by 0.9 just to be extra sure that we wont be immune for the next tick.
                creature.HotspotImmunityTimestamp = currentTime + immunityTime;
            }

            if (Common.ConfigManager.Config.Server.WorldRuleset == Common.Ruleset.CustomDM && WeenieClassId == 8127) // Menhir Mana Field
            {
                if (player != null)
                    player.AlignLeyLineAmulet(this);
            }

            switch (DamageType)
            {
                default:

                    if (creature.Invincible || creature.IsDead || creature.IsOnNoDamageLandblock) return;

                    amount *= creature.GetResistanceMod(DamageType, this, null);

                    if (player != null)
                        iAmount = player.TakeDamage(this, DamageType, amount, Server.Entity.BodyPart.Foot);
                    else
                        iAmount = (int)creature.TakeDamage(this, DamageType, amount);

                    if (creature.IsDead && Creatures.Contains(creature.Guid))
                        Creatures.Remove(creature.Guid);

                    break;

                case DamageType.Mana:
                    iAmount = creature.UpdateVitalDelta(creature.Mana, -iAmount);
                    break;

                case DamageType.Stamina:
                    iAmount = creature.UpdateVitalDelta(creature.Stamina, -iAmount);
                    break;

                case DamageType.Health:
                    iAmount = creature.UpdateVitalDelta(creature.Health, -iAmount);

                    if (iAmount > 0)
                        creature.DamageHistory.OnHeal((uint)iAmount);
                    else
                        creature.DamageHistory.Add(this, DamageType.Health, (uint)-iAmount);

                    break;
            }

            if (!Visibility)
                EnqueueBroadcast(new GameMessageSound(Guid, Sound.TriggerActivated, 1.0f));

            if (player != null && !string.IsNullOrWhiteSpace(ActivationTalk) && iAmount != 0)
                player.Session.Network.EnqueueSend(new GameMessageSystemChat(ActivationTalk.Replace("%i", Math.Abs(iAmount).ToString()), ChatMessageType.Broadcast));

            // perform activation emote
            if (ActivationResponse.HasFlag(ActivationResponse.Emote))
                OnEmote(creature);
        }
    }
}
