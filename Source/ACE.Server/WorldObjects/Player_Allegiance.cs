using System;
using System.Collections.Generic;
using System.Linq;

using ACE.Common;
using ACE.Entity;
using ACE.Entity.Enum;
using ACE.Entity.Enum.Properties;
using ACE.Server.Entity;
using ACE.Server.Entity.Actions;
using ACE.Server.Managers;
using ACE.Server.Network.GameEvent.Events;
using ACE.Server.Network.GameMessages.Messages;
using ACE.Server.Network.Structure;

namespace ACE.Server.WorldObjects
{
    partial class Player
    {
        public Allegiance Allegiance { get; set; }
        public AllegianceNode AllegianceNode { get; set; }

        public bool HasAllegiance { get => Allegiance != null && AllegianceNode != null && Allegiance.TotalMembers > 1; }

        public ulong AllegianceXPCached
        {
            get => (ulong)(GetProperty(PropertyInt64.AllegianceXPCached) ?? 0);
            set { if (value == 0) RemoveProperty(PropertyInt64.AllegianceXPCached); else SetProperty(PropertyInt64.AllegianceXPCached, (long)value); }
        }

        public ulong AllegianceXPGenerated
        {
            get => (ulong)(GetProperty(PropertyInt64.AllegianceXPGenerated) ?? 0);
            set { if (value == 0) RemoveProperty(PropertyInt64.AllegianceXPGenerated); else SetProperty(PropertyInt64.AllegianceXPGenerated, (long)value); }
        }

        public ulong AllegianceXPReceived
        {
            get => (ulong)(GetProperty(PropertyInt64.AllegianceXPReceived) ?? 0);
            set { if (value == 0) RemoveProperty(PropertyInt64.AllegianceXPReceived); else SetProperty(PropertyInt64.AllegianceXPReceived, (long)value); }
        }

        public int? AllegianceRank
        {
            get => GetProperty(PropertyInt.AllegianceRank);
            set { if (!value.HasValue) RemoveProperty(PropertyInt.AllegianceRank); else SetProperty(PropertyInt.AllegianceRank, value.Value); }
        }

        public int? AllegianceOfficerRank
        {
            get => GetProperty(PropertyInt.AllegianceOfficerRank);
            set { if (!value.HasValue) RemoveProperty(PropertyInt.AllegianceOfficerRank); else SetProperty(PropertyInt.AllegianceOfficerRank, value.Value); }
        }

        /// <summary>
        /// Unix timestamp of the last time this character swore allegiance (first oath leaves this null).
        /// Used to enforce the 30-day re-swear cooldown.
        /// </summary>
        public double? AllegianceSwearTimestamp
        {
            get => GetProperty(PropertyFloat.AllegianceSwearTimestamp);
            set { if (!value.HasValue) RemoveProperty(PropertyFloat.AllegianceSwearTimestamp); else SetProperty(PropertyFloat.AllegianceSwearTimestamp, value.Value); }
        }

        /// <summary>
        /// When a character is force-broken from their allegiance due to a break cascade,
        /// this stores the monarch GUID they were broken from. A re-swear into that same
        /// monarch chain is permitted without consuming the cooldown.
        /// Cleared on the next successful swear.
        /// </summary>
        public uint? AllegianceForcedBreakMonarchId
        {
            get => GetProperty(PropertyInstanceId.AllegianceForcedBreakMonarchId);
            set { if (!value.HasValue) RemoveProperty(PropertyInstanceId.AllegianceForcedBreakMonarchId); else SetProperty(PropertyInstanceId.AllegianceForcedBreakMonarchId, value.Value); }
        }

        /// <summary>
        /// The monarch GUID this character's free same-allegiance re-swear allowance applies to.
        /// Set whenever a genuine (cooldown-consuming) swear completes. Breaking and re-swearing
        /// back into this same monarch's chain is free (does not start a cooldown) until
        /// AllegianceReswearCount reaches allegiance_free_same_chain_reswears, letting players
        /// re-arrange the order of their chain without penalty while still capping abuse.
        /// </summary>
        public uint? AllegianceReswearMonarchId
        {
            get => GetProperty(PropertyInstanceId.AllegianceReswearMonarchId);
            set { if (!value.HasValue) RemoveProperty(PropertyInstanceId.AllegianceReswearMonarchId); else SetProperty(PropertyInstanceId.AllegianceReswearMonarchId, value.Value); }
        }

        /// <summary>
        /// Number of free same-allegiance re-swears this character has consumed against
        /// AllegianceReswearMonarchId. Reset to 0 on a genuine allegiance change.
        /// </summary>
        public int AllegianceReswearCount
        {
            get => GetProperty(PropertyInt.AllegianceReswearCount) ?? 0;
            set { if (value == 0) RemoveProperty(PropertyInt.AllegianceReswearCount); else SetProperty(PropertyInt.AllegianceReswearCount, value); }
        }

        /// <summary>
        /// This flag indicates if a player can pass up allegiance XP
        /// </summary>
        public bool ExistedBeforeAllegianceXpChanges
        {
            get => GetProperty(PropertyBool.ExistedBeforeAllegianceXpChanges) ?? true;
            set { if (value) RemoveProperty(PropertyBool.ExistedBeforeAllegianceXpChanges); else SetProperty(PropertyBool.ExistedBeforeAllegianceXpChanges, value); }
        }

        /// <summary>
        /// Called when a player tries to Swear Allegiance to a target
        /// </summary>
        /// <param name="targetGuid">The target this player is attempting to swear allegiance to</param>
        public void HandleActionSwearAllegiance(uint targetGuid)
        {
            var patron = PlayerManager.GetOnlinePlayer(targetGuid);

            if (patron == null) return;

            if (!IsPledgable(patron)) return;

            // perform moveto / turnto
            CreateMoveToChain(patron, (success) => SwearAllegiance(patron.Guid.Full, success), Allegiance_MaxSwearDistance);
        }

        public void OfflineSwearAllegiance(uint targetGuid)
        {
            var patron = PlayerManager.GetOfflinePlayer(targetGuid);
            if (patron == null || patron.IsPendingDeletion || patron.IsDeleted)
                return;

            if (!IsPledgable(patron, isOfflineSwear: true)) return;

            log.Debug($"[ALLEGIANCE] {Name} ({Level}) swearing allegiance to {patron.Name} ({patron.Level})");

            PatronId = targetGuid;

            var monarchGuid = AllegianceManager.GetMonarch(patron).Guid.Full;

            UpdateProperty(PropertyInstanceId.Monarch, monarchGuid, true);

            ExistedBeforeAllegianceXpChanges = (patron.Level ?? 1) >= (Level ?? 1);

            // OfflineSwear (same-account organization) is exempt from the re-swear cooldown, so it
            // neither starts a cooldown (AllegianceSwearTimestamp is left untouched) nor is gated by one.

            // Clear forced-break exemption now that the swear succeeded
            AllegianceForcedBreakMonarchId = null;

            // handle special case: monarch swearing into another allegiance
            if (Allegiance != null && Allegiance.MonarchId == Guid.Full)
                HandleMonarchSwear();

            SaveBiotaToDatabase();

            // send message to vassal:
            // %patron% has accepted your oath of Allegiance!
            // Motion_Kneel
            Session.Network.EnqueueSend(new GameMessageSystemChat($"{patron.Name} has accepted your oath of Allegiance!", ChatMessageType.Broadcast));

            EnqueueBroadcastMotion(new Motion(MotionStance.NonCombat, MotionCommand.Kneel));

            // rebuild allegiance tree structure
            AllegianceManager.OnSwearAllegiance(this);

            AllegianceXPGenerated = 0;
            AllegianceOfficerRank = null;

            // refresh ui panel
            Session.Network.EnqueueSend(new GameEventAllegianceUpdate(Session, Allegiance, AllegianceNode), new GameEventAllegianceAllegianceUpdateDone(Session));

            if (GetCharacterOption(CharacterOption.ListenToAllegianceChat) && Allegiance != null)
                JoinTurbineChatChannel("Allegiance");
        }

        public void SwearAllegiance(uint targetGuid, bool success, bool confirmed = false)
        {
            if (!success) return;

            var patron = PlayerManager.GetOnlinePlayer(targetGuid);
            if (patron == null)
                return;

            if (!IsPledgable(patron)) return;

            if (!confirmed)
            {
                if (!patron.ConfirmationManager.EnqueueSend(new Confirmation_SwearAllegiance(patron.Guid, Guid), Name))
                {
                    Session.Network.EnqueueSend(new GameMessageSystemChat($"{patron.Name} is busy.", ChatMessageType.Broadcast));
                }
                return;
            }

            log.DebugFormat("[ALLEGIANCE] {0} ({1}) swearing allegiance to {2} ({3})", Name, Level, patron.Name, patron.Level);

            PatronId = targetGuid;

            var monarchGuid = AllegianceManager.GetMonarch(patron).Guid.Full;

            UpdateProperty(PropertyInstanceId.Monarch, monarchGuid, true);

            ExistedBeforeAllegianceXpChanges = (patron.Level ?? 1) >= (Level ?? 1);

            ApplySwearCooldown(monarchGuid);
            AllegianceForcedBreakMonarchId = null;

            // handle special case: monarch swearing into another allegiance
            if (Allegiance != null && Allegiance.MonarchId == Guid.Full)
                HandleMonarchSwear();

            SaveBiotaToDatabase();

            //Console.WriteLine("Patron: " + PlayerManager.GetOfflinePlayerByGuidId(Patron.Value).Name);
            //Console.WriteLine("Monarch: " + PlayerManager.GetOfflinePlayerByGuidId(Monarch.Value).Name);

            // send message to patron:
            // %vassal% has sworn Allegiance to you.
            patron.Session.Network.EnqueueSend(new GameMessageSystemChat($"{Name} has sworn Allegiance to you.", ChatMessageType.Broadcast));

            // send message to vassal:
            // %patron% has accepted your oath of Allegiance!
            // Motion_Kneel
            Session.Network.EnqueueSend(new GameMessageSystemChat($"{patron.Name} has accepted your oath of Allegiance!", ChatMessageType.Broadcast));

            EnqueueBroadcastMotion(new Motion(MotionStance.NonCombat, MotionCommand.Kneel));

            // rebuild allegiance tree structure
            AllegianceManager.OnSwearAllegiance(this);

            AllegianceXPGenerated = 0;
            AllegianceOfficerRank = null;

            // refresh ui panel
            Session.Network.EnqueueSend(new GameEventAllegianceUpdate(Session, Allegiance, AllegianceNode), new GameEventAllegianceAllegianceUpdateDone(Session));

            if (GetCharacterOption(CharacterOption.ListenToAllegianceChat) && Allegiance != null)
                JoinTurbineChatChannel("Allegiance");
        }

        /// <summary>
        /// Applies the re-swear cooldown bookkeeping after a successful online swear.
        /// Breaking and re-swearing back into the same monarch's chain (to re-arrange chain order)
        /// is free until the allegiance_free_same_chain_reswears allowance is exhausted; those free
        /// re-swears leave the cooldown timestamp untouched. A genuine swear (first oath, allegiance
        /// change, or a re-swear past the free allowance) starts the cooldown, and a genuine
        /// allegiance change re-bases the free-reswear allowance onto the new monarch.
        /// </summary>
        private void ApplySwearCooldown(uint monarchGuid)
        {
            // A forced-break re-swear into the original chain keeps its prior behavior: it is exempt
            // from being blocked (handled in IsPledgable) but still starts a fresh cooldown and does
            // not consume a voluntary same-chain re-swear. (AllegianceForcedBreakMonarchId is cleared
            // by the caller after this returns.)
            bool forcedBreakReswear = AllegianceForcedBreakMonarchId.HasValue &&
                                      AllegianceForcedBreakMonarchId.Value == monarchGuid;

            var freeReswears = PropertyManager.GetLong("allegiance_free_same_chain_reswears").Item;
            bool sameChain = AllegianceReswearMonarchId.HasValue &&
                             AllegianceReswearMonarchId.Value == monarchGuid;

            if (!forcedBreakReswear && sameChain && AllegianceReswearCount < freeReswears)
            {
                // Free same-allegiance re-swear: consume one, leave the cooldown timestamp untouched.
                AllegianceReswearCount++;
                return;
            }

            // Genuine swear, forced-break re-swear, or exhausted free re-swears: start the cooldown.
            AllegianceSwearTimestamp = Time.GetUnixTime();

            // A genuine allegiance change re-bases the free-reswear allowance onto the new monarch.
            if (!sameChain)
            {
                AllegianceReswearMonarchId = monarchGuid;
                AllegianceReswearCount = 0;
            }
        }

        /// <summary>
        /// Records the monarch of the allegiance a player is leaving on a break, so a later re-swear
        /// back into that same chain is recognized as a same-allegiance re-swear (free until the
        /// allegiance_free_same_chain_reswears cap). Called for whichever player is being detached, in
        /// both break directions. Without this the anchor is only set on a genuine swear, so anyone who
        /// last swore before this feature shipped (AllegianceReswearMonarchId == null) would be denied
        /// their free re-swears. Only (re)initializes when the anchor points at a different allegiance,
        /// so the free-reswear count survives repeated break/re-swear cycles within the same chain.
        /// </summary>
        private static void RecordReswearAnchorOnBreak(IPlayer player, uint priorMonarchId)
        {
            if (player == null || priorMonarchId == 0)
                return;

            if (player.GetProperty(PropertyInstanceId.AllegianceReswearMonarchId) != priorMonarchId)
            {
                player.SetProperty(PropertyInstanceId.AllegianceReswearMonarchId, priorMonarchId);
                player.RemoveProperty(PropertyInt.AllegianceReswearCount);
            }
        }

        /// <summary>
        /// Handle monarch swearing into another allegiance
        /// </summary>
        public void HandleMonarchSwear()
        {
            // walk the allegiance tree from this node, update monarch ids
            AllegianceNode.Walk((node) =>
            {
                node.Player.UpdateProperty(PropertyInstanceId.Monarch, MonarchId, true);

                node.Player.SaveBiotaToDatabase();

                // update node.Player.House.Monarch, if not null?
            });

            // TODO: allegiance officers should probably be stored in their own table
            foreach (var kvp in Allegiance.Officers)
            {
                var officer = PlayerManager.FindByGuid(kvp.Key);
                if (officer != null)
                    officer.AllegianceOfficerRank = null;

                officer.SaveBiotaToDatabase();
            }
        }

        /// <summary>
        /// Called when a player tries to break Allegiance to a target
        /// </summary>
        /// <param name="targetGuid">The target this player is attempting to break allegiance from</param>
        public void HandleActionBreakAllegiance(uint targetGuid)
        {
            if (!IsBreakable(targetGuid)) return;

            var target = PlayerManager.FindByGuid(targetGuid, out var targetIsOnline);

            if (target == null) return;

            log.DebugFormat("[ALLEGIANCE] {0} breaking allegiance to {1}", Name, target.Name);

            // Capture the current monarch BEFORE the break so we can detect account conflicts after
            var priorMonarchId = Allegiance?.MonarchId ?? 0;

            // target can be either patron or vassal
            var isPatron = PatronId == target.Guid.Full;
            var isVassal = target.PatronId == Guid.Full;

            // break ties
            if (isVassal)
            {
                // patron breaking from vassal
                target.PatronId = null;

                Allegiance.Members.TryGetValue(target.Guid, out var targetNode);

                var monarchId = targetNode.HasVassals ? (uint?)target.Guid.Full : null;

                target.UpdateProperty(PropertyInstanceId.Monarch, monarchId, true);

                // walk the allegiance tree from this node, update monarch ids
                targetNode.Walk((node) =>
                {
                    node.Player.UpdateProperty(PropertyInstanceId.Monarch, target.Guid.Full, true);

                    node.Player.SaveBiotaToDatabase();

                }, false);

                // Record the allegiance the kicked vassal is leaving so they can re-swear back for free
                RecordReswearAnchorOnBreak(target, priorMonarchId);

                target.SaveBiotaToDatabase();
            }
            else
            {
                // vassal breaking from patron
                PatronId = null;
                UpdateProperty(PropertyInstanceId.Monarch, null, true);

                // walk the allegiance tree from this node, update monarch ids
                AllegianceNode.Walk((node) =>
                {
                    node.Player.UpdateProperty(PropertyInstanceId.Monarch, Guid.Full, true);

                    node.Player.SaveBiotaToDatabase();

                }, false);

                // Record the allegiance we're leaving so we can re-swear back for free
                RecordReswearAnchorOnBreak(this, priorMonarchId);

                SaveBiotaToDatabase();
            }

            // send message to target if online
            if (targetIsOnline)
            {
                var onlineTarget = PlayerManager.GetOnlinePlayer(targetGuid);
                if (onlineTarget != null)
                    onlineTarget.Session.Network.EnqueueSend(new GameMessageSystemChat($"{Name} has broken their Allegiance to you!", ChatMessageType.Broadcast));
            }

            // send message to self
            Session.Network.EnqueueSend(new GameMessageSystemChat($"You have broken your Allegiance to {target.Name}!", ChatMessageType.Broadcast));

            // rebuild allegiance tree structures
            AllegianceManager.OnBreakAllegiance(this, target);

            if (isVassal)
            {
                // patron broke from vassal — target's subtree is now detached
                CheckAllegianceHouse(target.Guid);

                var vassalAllegiance = AllegianceManager.GetAllegiance(target);
                if (vassalAllegiance != null)
                {
                    vassalAllegiance.Monarch.Walk((node) => CheckAllegianceHouse(node.PlayerGuid), false);

                    // Resolve any account conflicts created by the detached subtree
                    if (priorMonarchId != 0)
                        ResolveAccountConflictsAfterBreak(vassalAllegiance.Monarch, priorMonarchId);
                }
            }
            else
            {
                // vassal broke from patron — self's subtree is now detached
                CheckAllegianceHouse(Guid);

                if (AllegianceNode != null)
                {
                    AllegianceNode.Walk((node) => CheckAllegianceHouse(node.PlayerGuid), false);

                    // Resolve any account conflicts created by the detached subtree
                    if (priorMonarchId != 0)
                        ResolveAccountConflictsAfterBreak(AllegianceNode, priorMonarchId);
                }
            }

            // refresh ui panel

            // move this to function below?
            Session.Network.EnqueueSend(new GameEventAllegianceUpdate(Session, Allegiance, AllegianceNode), new GameEventAllegianceAllegianceUpdateDone(Session));
        }

        public static void CheckAllegianceHouse(ObjectGuid playerGuid)
        {
            // handle player.House.Monarch updates?

            // instead of walking, would it be more appropriate for House
            // to boot any players who don't belong there?

            var player = PlayerManager.GetOnlinePlayer(playerGuid);
            if (player == null) return;

            if (player.CheckHouse())
                player.Session.Network.EnqueueSend(new GameMessageSystemChat("You have been booted from the allegiance house.", ChatMessageType.Broadcast));

            if (player.GetCharacterOption(CharacterOption.ListenToAllegianceChat))
            {
                if (player.Allegiance != null)
                {
                    player.LeaveTurbineChatChannel("Allegiance", true);
                    player.JoinTurbineChatChannel("Allegiance");
                }
                else
                    player.LeaveTurbineChatChannel("Allegiance");
            }
            else
                player.SendTurbineChatChannels();
        }

        //public static float Allegiance_MaxSwearDistance = 4.0f;
        public static float Allegiance_MaxSwearDistance = 2.0f;

        /// <summary>
        /// Returns TRUE if this player can swear to the target guid
        /// </summary>
        public bool IsPledgable(IPlayer target, bool isOfflineSwear = false)
        {
            // the client doesn't seem to display most of these werrors,
            // so we also send similar messages as text

            // An Olthoi player cannot swear allegiance to another player
            if (IsOlthoiPlayer)
            {
                //Session.Network.EnqueueSend(new GameMessageSystemChat($"The Olthoi only have an allegiance to the Olthoi Queen!", ChatMessageType.Broadcast));
                Session.Network.EnqueueSend(new GameEventWeenieError(Session, WeenieError.OlthoiCannotJoinAllegiance));
                return false;
            }

            if (target.IsOlthoiPlayer)
            {
                Session.Network.EnqueueSend(new GameMessageSystemChat($"The Olthoi have loyalty only to their Olthoi Queen!", ChatMessageType.Broadcast));
                SendWeenieError(WeenieError.None);
                return false;
            }

            if (target is Player onlinePlayer)
            {
                // check ignore allegiance requests
                if (onlinePlayer.GetCharacterOption(CharacterOption.IgnoreAllegianceRequests))
                {
                    Session.Network.EnqueueSend(new GameMessageSystemChat($"Your offer of allegiance was ignored.", ChatMessageType.Broadcast));
                    Session.Network.EnqueueSend(new GameEventWeenieError(Session, WeenieError.YourOfferOfAllegianceWasIgnored));
                    return false;
                }
            }

            // player already sworn?
            if (PatronId != null)
            {
                //Console.WriteLine(Name + " tried to swear to " + target.Name + ", but is already sworn to " + PlayerManager.GetOfflinePlayerByGuidId(Patron.Value).Name);
                Session.Network.EnqueueSend(new GameMessageSystemChat($"You've already sworn allegiance.", ChatMessageType.Broadcast));
                Session.Network.EnqueueSend(new GameEventWeenieError(Session, WeenieError.YouveAlreadySwornAllegiance));
                return false;
            }

            // player can't swear to themselves
            if (target.Guid == Guid)
            {
                //Console.WriteLine(Name + " tried to swear to themselves");
                Session.Network.EnqueueSend(new GameMessageSystemChat($"You cannot swear allegiance to yourself.", ChatMessageType.Broadcast));
                return false;
            }

            // Swearing to a lower-level patron is retail behavior: it is allowed, and no allegiance XP
            // passes up until the patron surpasses the vassal's level (handled automatically via
            // ExistedBeforeAllegianceXpChanges, set at swear time and flipped in AllegianceNode.OnLevelUp).
            // Server operators can disable this and require patron >= vassal level via allow_swear_to_lower_level.
            if (!PropertyManager.GetBool("allow_swear_to_lower_level").Item)
            {
                // patron must currently be greater or equal level
                if (target.Level < Level)
                {
                    //Console.WriteLine(Name + " tried to swear to a lower level character");
                    Session.Network.EnqueueSend(new GameMessageSystemChat($"You cannot swear to a lower level character.", ChatMessageType.Broadcast));
                    Session.Network.EnqueueSend(new GameEventWeenieError(Session, WeenieError.AllegianceIllegalLevel));
                    return false;
                }
            }

            var selfNode = AllegianceNode;
            var targetNode = target.AllegianceNode;

            if (targetNode != null)
            {
                // maximum # of direct vassals = 11
                if (targetNode.TotalVassals >= 11)
                {
                    //Console.WriteLine(target.Name + " already has the maximum # of vassals");
                    Session.Network.EnqueueSend(new GameMessageSystemChat($"{target.Name} already has the maximum # of vassals", ChatMessageType.Broadcast));
                    return false;
                }

                // 2 players can't swear to each other
                // prevent any loops in the allegiance chain
                if (selfNode != null && selfNode.IsMonarch)
                {
                    if (selfNode.PlayerGuid == targetNode.Monarch.PlayerGuid)
                    {
                        //Console.WriteLine(Name + " tried to swear to someone already in Allegiance: " + target.Name);
                        Session.Network.EnqueueSend(new GameMessageSystemChat($"You cannot swear allegiance to {target.Name}.", ChatMessageType.Broadcast));
                        return false;
                    }
                }

                if (targetNode.Allegiance.IsLocked && !targetNode.Allegiance.HasApprovedVassal(Guid.Full)
                    && (targetNode.Player.AllegianceOfficerRank ?? 0) < (int)AllegianceOfficerLevel.Castellan)
                {
                    //Console.WriteLine(Name + "tried to join locked allegiance, not in approved vassals list");
                    Session.Network.EnqueueSend(new GameMessageSystemChat($"{target.Name} is not accepting allegiance requests.", ChatMessageType.Broadcast));
                    return false;
                }

                if (targetNode.Allegiance.IsBanned(Guid.Full))
                {
                    //Console.WriteLine(Name + "tried to join allegiance, but was banned!");
                    Session.Network.EnqueueSend(new GameMessageSystemChat($"You are banned from joining {target.Name}'s allegiance.", ChatMessageType.Broadcast));
                    return false;
                }
            }

            // ensure this player doesn't own a monarch-only house
            if (House != null && House.SlumLord.HouseRequiresMonarch && House.HouseOwner == Guid.Full)
            {
                //Console.WriteLine(Name + "monarch tried to pledge allegiance, already owns a mansion");
                //Session.Network.EnqueueSend(new GameMessageSystemChat($"You cannot swear allegiance while owning a mansion.", ChatMessageType.Broadcast));
                Session.Network.EnqueueSend(new GameEventWeenieError(Session, WeenieError.CannotSwearAllegianceWhileOwningMansion));
                return false;
            }

            // Account-wide allegiance lock: all characters on this account must share the same monarch.
            var targetMonarchId = target.MonarchId ?? target.Guid.Full;
            var accountPlayers  = PlayerManager.GetAccountPlayers(Account.AccountId);
            foreach (var kvp in accountPlayers)
            {
                var sibling = kvp.Value;
                if (sibling.Guid == Guid) continue;

                var siblingMonarchId = sibling.MonarchId;
                if (!siblingMonarchId.HasValue) continue;

                // Skip siblings that belong to this player's own allegiance (their monarch is this
                // player). If this player is a monarch swearing into another allegiance,
                // HandleMonarchSwear cascades the entire sub-tree — including these account siblings —
                // to the new monarch, so they will share the same monarch once the swear completes.
                if (siblingMonarchId.Value == Guid.Full) continue;

                if (siblingMonarchId.Value != targetMonarchId)
                {
                    Session.Network.EnqueueSend(new GameMessageSystemChat(
                        "Another character on your account is sworn to a different allegiance. All characters on an account must belong to the same allegiance.",
                        ChatMessageType.Broadcast));
                    return false;
                }
            }

            // Swear cooldown: first oath (AllegianceSwearTimestamp == null) is always free.
            // Subsequent oaths require waiting allegiance_swear_cooldown_days days,
            // unless this player was force-broken from the same monarch chain (ForcedBreakMonarchId matches).
            // OfflineSwear (same-account organization) is always exempt from the cooldown.
            if (!isOfflineSwear && AllegianceSwearTimestamp.HasValue)
            {
                var cooldownDays   = PropertyManager.GetDouble("allegiance_swear_cooldown_days").Item;
                var cooldownExpiry = DateTimeOffset.FromUnixTimeSeconds((long)AllegianceSwearTimestamp.Value).UtcDateTime.AddDays(cooldownDays);

                if (DateTime.UtcNow < cooldownExpiry)
                {
                    bool forcedBreakExempt = AllegianceForcedBreakMonarchId.HasValue &&
                                             AllegianceForcedBreakMonarchId.Value == targetMonarchId;

                    // Re-swearing back into the same allegiance (to re-arrange chain order) is free
                    // until the free-reswear allowance is exhausted for this monarch.
                    var freeReswears = PropertyManager.GetLong("allegiance_free_same_chain_reswears").Item;
                    bool sameChainExempt = AllegianceReswearMonarchId.HasValue &&
                                           AllegianceReswearMonarchId.Value == targetMonarchId &&
                                           AllegianceReswearCount < freeReswears;

                    if (!forcedBreakExempt && !sameChainExempt)
                    {
                        var remaining = cooldownExpiry - DateTime.UtcNow;
                        var days      = (int)Math.Ceiling(remaining.TotalDays);
                        Session.Network.EnqueueSend(new GameMessageSystemChat(
                            $"You must wait {days} more day{(days == 1 ? "" : "s")} before swearing allegiance again.",
                            ChatMessageType.Broadcast));
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Returns TRUE if this player can break allegiance to the target guid
        /// </summary>
        public bool IsBreakable(uint targetGuid)
        {
            // players can break from either vassals or patrons

            // ensure target player exists
            var target = PlayerManager.FindByGuid(targetGuid);
            if (target == null)
            {
                //Console.WriteLine(Name + " tried to break allegiance to an unknown player guid: " + targetGuid.Full.ToString("X8"));
                return false;
            }

            // verify patron or vassal
            var isPatron = PatronId == target.Guid.Full;
            var isVassal = target.PatronId == Guid.Full;

            if (!isPatron && !isVassal)
            {
                //Console.WriteLine(Name + " tried to break allegiance from " + target.Name + ", but they aren't patron or vassal");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Called when a player logs in to handle allegiance events on login
        /// </summary>
        public void HandleAllegianceOnLogin()
        {
            var actionChain = new ActionChain();
            actionChain.AddDelaySeconds(3.0f);
            actionChain.AddAction(this, () =>
            {
                if (Allegiance != null && Allegiance.AllegianceMotd != null)
                    Session.Network.EnqueueSend(new GameMessageSystemChat($"\"{Allegiance.AllegianceMotd}\" -- {Allegiance.AllegianceMotdSetBy}", ChatMessageType.Broadcast));

                if (AllegianceXPCached != 0)
                {
                    Session.Network.EnqueueSend(new GameMessageSystemChat($"Your Vassals have produced experience points for you.\nTaking your skills as a leader into account, you gain {AllegianceXPCached:N0} xp.", ChatMessageType.Broadcast));
                    AddAllegianceXP();
                }
            });
            actionChain.EnqueueChain();

            if (Allegiance != null)
            {
                foreach (var member in Allegiance.OnlinePlayers)
                {
                    if (member.Guid != Guid && member.GetCharacterOption(CharacterOption.ShowAllegianceLogons))
                        member.Session.Network.EnqueueSend(new GameEventAllegianceLoginNotification(member.Session, Guid.Full, isLoggedIn: true));
                }
            }
        }

        public void HandleAllegianceOnLogout()
        {
            if (Allegiance != null)
            {
                foreach (var member in Allegiance.OnlinePlayers)
                {
                    if (member.Guid != Guid && member.GetCharacterOption(CharacterOption.ShowAllegianceLogons))
                        member.Session.Network.EnqueueSend(new GameEventAllegianceLoginNotification(member.Session, Guid.Full, isLoggedIn: false));
                }
            }
        }

        public string GetPrefix(Player allegianceMember)
        {
            var prefix = "";

            if (allegianceMember.Guid == AllegianceNode.Monarch.PlayerGuid)
                prefix = "Your monarch ";
            else if (AllegianceNode.Patron != null && allegianceMember.Guid == AllegianceNode.Patron.PlayerGuid)
                prefix = "Your patron ";
            else if (AllegianceNode.Vassals.ContainsKey(allegianceMember.Guid.Full))
                prefix = "Your vassal ";

            return prefix;
        }

        /// <summary>
        /// For an online patron, adds the pending allegiance XP stored in AllegianceXPCached
        /// to their total / unassigned xp
        /// </summary>
        public void AddAllegianceXP()
        {
            if (AllegianceXPCached == 0) return;

            // TODO: handle ulong -> long?
            GrantXP((long)AllegianceXPCached, XpType.Allegiance, ShareType.None);

            AllegianceXPReceived += AllegianceXPCached;

            AllegianceXPCached = 0;
        }

        public void HandleActionQueryMotd()
        {
            //Console.WriteLine($"{Name}.HandleActionQueryMotd()");

            // check if player is in an allegiance
            if (Allegiance == null)
            {
                Session.Network.EnqueueSend(new GameEventWeenieError(Session, WeenieError.YouAreNotInAllegiance));
                return;
            }

            if (Allegiance.AllegianceMotd == null)
            {
                Session.Network.EnqueueSend(new GameMessageSystemChat("Your allegiance has not set a message of the day.", ChatMessageType.Broadcast));
                return;
            }

            var msg = $"\"{Allegiance.AllegianceMotd}\" -- {Allegiance.AllegianceMotdSetBy}";

            Session.Network.EnqueueSend(new GameMessageSystemChat(msg, ChatMessageType.Broadcast));
        }

        public void HandleActionSetMotd(string motd)
        {
            //Console.WriteLine($"{Name}.HandleActionSetMotd({motd})");

            // check if player is in an allegiance
            if (Allegiance == null)
            {
                Session.Network.EnqueueSend(new GameEventWeenieError(Session, WeenieError.YouAreNotInAllegiance));
                return;
            }

            if (AllegiancePermissionLevel < AllegiancePermissionLevel.Speaker)
            {
                Session.Network.EnqueueSend(new GameEventWeenieError(Session, WeenieError.YouDoNotHaveAuthorityInAllegiance));
                return;
            }

            Allegiance.AllegianceMotd = motd;

            var rank = AllegianceTitle.GetTitle(HeritageGroup, (Gender)Gender, AllegianceNode.Rank);
            Allegiance.AllegianceMotdSetBy = $"{rank} {Name}";

            Allegiance.SaveBiotaToDatabase();

            Session.Network.EnqueueSend(new GameMessageSystemChat("Your message of the day has been set.", ChatMessageType.Broadcast));
        }

        public void HandleActionClearMotd()
        {
            //Console.WriteLine($"{Name}.HandleActionClearMotd()");

            // check if player is in an allegiance
            if (Allegiance == null)
            {
                Session.Network.EnqueueSend(new GameEventWeenieError(Session, WeenieError.YouAreNotInAllegiance));
                return;
            }

            if (AllegiancePermissionLevel < AllegiancePermissionLevel.Speaker)
            {
                Session.Network.EnqueueSend(new GameEventWeenieError(Session, WeenieError.YouDoNotHaveAuthorityInAllegiance));
                return;
            }

            Allegiance.AllegianceMotd = null;

            var rank = AllegianceTitle.GetTitle(HeritageGroup, (Gender)Gender, AllegianceNode.Rank);
            Allegiance.AllegianceMotdSetBy = $"{rank} {Name}";

            Allegiance.SaveBiotaToDatabase();

            Session.Network.EnqueueSend(new GameMessageSystemChat("Your message of the day has been cleared.", ChatMessageType.Broadcast));
        }

        public void HandleActionQueryAllegianceName()
        {
            //Console.WriteLine($"{Name}.HandleActionQueryAllegianceName()");

            // check if player is in an allegiance
            if (Allegiance == null)
            {
                Session.Network.EnqueueSend(new GameEventWeenieError(Session, WeenieError.YouAreNotInAllegiance));
                return;
            }

            if (Allegiance.AllegianceName == null)
            {
                Session.Network.EnqueueSend(new GameMessageSystemChat("Your allegiance has not set a name.", ChatMessageType.Broadcast));
                return;
            }

            Session.Network.EnqueueSend(new GameMessageSystemChat(Allegiance.AllegianceName, ChatMessageType.Broadcast));
        }

        public void HandleActionSetAllegianceName(string allegianceName)
        {
            //Console.WriteLine($"{Name}.HandleActionSetAllegianceName({allegianceName}");

            // check if player is in an allegiance
            if (Allegiance == null)
            {
                Session.Network.EnqueueSend(new GameEventWeenieError(Session, WeenieError.YouAreNotInAllegiance));
                return;
            }

            if (AllegiancePermissionLevel < AllegiancePermissionLevel.Castellan)
            {
                Session.Network.EnqueueSend(new GameEventWeenieError(Session, WeenieError.YouDoNotHaveAuthorityInAllegiance));
                return;
            }

            // TODO: name verifications
            // - same name as current allegiance name
            // - no empty names
            // - allegiance name too long (40 chars max.)
            // - allegiance name already in use
            // - bad chars (space, single quote, hyphen, A-Z, a-z)
            // - banned words from portal.dat
            // - name change timer (1 day?)

            Allegiance.AllegianceName = allegianceName;
            Allegiance.SaveBiotaToDatabase();

            Session.Network.EnqueueSend(new GameMessageSystemChat("Your allegiance name has been set.", ChatMessageType.Broadcast));
        }

        public void HandleActionClearAllegianceName()
        {
            //Console.WriteLine($"{Name}.HandleActionClearAllegianceName()");

            // check if player is in an allegiance
            if (Allegiance == null)
            {
                Session.Network.EnqueueSend(new GameEventWeenieError(Session, WeenieError.YouAreNotInAllegiance));
                return;
            }

            if (AllegiancePermissionLevel < AllegiancePermissionLevel.Castellan)
            {
                Session.Network.EnqueueSend(new GameEventWeenieError(Session, WeenieError.YouDoNotHaveAuthorityInAllegiance));
                return;
            }

            Allegiance.AllegianceName = null;
            Allegiance.SaveBiotaToDatabase();

            Session.Network.EnqueueSend(new GameMessageSystemChat("Your allegiance name has been cleared.", ChatMessageType.Broadcast));
        }

        public void HandleActionListAllegianceOfficers()
        {
            //Console.WriteLine($"{Name}.HandleActionListAllegianceOfficers()");

            // check if player is in an allegiance
            if (Allegiance == null)
            {
                Session.Network.EnqueueSend(new GameEventWeenieError(Session, WeenieError.YouAreNotInAllegiance));
                return;
            }

            var officerList = "Allegiance Officers:\n";
            officerList += Allegiance.Monarch.Player.Name + " (Monarch)\n";

            foreach (var officer in Allegiance.Officers.Values.Select(i => i.Player).OrderBy(i => i.Name))
            {
                var title = Allegiance.GetOfficerTitle((AllegianceOfficerLevel)officer.AllegianceOfficerRank);
                officerList += $"{officer.Name} ({title})\n";
            }

            Session.Network.EnqueueSend(new GameMessageSystemChat(officerList, ChatMessageType.Broadcast));
        }

        public void HandleActionListAllegianceOfficerTitles()
        {
            //Console.WriteLine($"{Name}.HandleActionListAllegianceOfficerTitles()");

            // check if player is in an allegiance
            if (Allegiance == null)
            {
                Session.Network.EnqueueSend(new GameEventWeenieError(Session, WeenieError.YouAreNotInAllegiance));
                return;
            }

            var speakerTitle = Allegiance.AllegianceSpeakerTitle ?? "Speaker";
            var seneschalTitle = Allegiance.AllegianceSeneschalTitle ?? "Seneschal";
            var castellanTitle = Allegiance.AllegianceCastellanTitle ?? "Castellan";

            var officerTitles = "Allegiance Officer Titles:\n";
            officerTitles += $"1. {speakerTitle}\n";
            officerTitles += $"2. {seneschalTitle}\n";
            officerTitles += $"3. {castellanTitle}\n";

            Session.Network.EnqueueSend(new GameMessageSystemChat(officerTitles, ChatMessageType.Broadcast));
        }

        public void HandleActionSetAllegianceOfficerTitle(uint rank, string title)
        {
            //Console.WriteLine($"{Name}.HandleActionSetAllegianceOfficerTitle({rank}, {title})");

            // check if player is in an allegiance
            if (Allegiance == null)
            {
                Session.Network.EnqueueSend(new GameEventWeenieError(Session, WeenieError.YouAreNotInAllegiance));
                return;
            }

            if (AllegiancePermissionLevel < AllegiancePermissionLevel.Castellan)
            {
                Session.Network.EnqueueSend(new GameEventWeenieError(Session, WeenieError.YouDoNotHaveAuthorityInAllegiance));
                return;
            }

            if (rank < 1 || rank > 3)
            {
                Session.Network.EnqueueSend(new GameMessageSystemChat("Please specify a valid officer level as a number between 1 and 3.", ChatMessageType.Broadcast));
                return;
            }

            switch (rank)
            {
                case 1:
                    Allegiance.AllegianceSpeakerTitle = title;
                    break;
                case 2:
                    Allegiance.AllegianceSeneschalTitle = title;
                    break;
                case 3:
                    Allegiance.AllegianceCastellanTitle = title;
                    break;
            }

            Session.Network.EnqueueSend(new GameMessageSystemChat($"Your allegiance {(AllegianceOfficerLevel)rank} title has been set.", ChatMessageType.Broadcast));

            Allegiance.SaveBiotaToDatabase();
        }

        public void HandleActionClearAllegianceOfficerTitles()
        {
            //Console.WriteLine($"{Name}.HandleActionClearAllegianceOfficerTitles()");

            // check if player is in an allegiance
            if (Allegiance == null)
            {
                Session.Network.EnqueueSend(new GameEventWeenieError(Session, WeenieError.YouAreNotInAllegiance));
                return;
            }

            // castellans can clear all titles?
            if (AllegiancePermissionLevel < AllegiancePermissionLevel.Castellan)
            {
                Session.Network.EnqueueSend(new GameEventWeenieError(Session, WeenieError.YouDoNotHaveAuthorityInAllegiance));
                return;
            }

            Allegiance.AllegianceSpeakerTitle = null;
            Allegiance.AllegianceSeneschalTitle = null;
            Allegiance.AllegianceCastellanTitle = null;

            Allegiance.SaveBiotaToDatabase();

            Session.Network.EnqueueSend(new GameMessageSystemChat($"Your allegiance officer titles have been cleared.", ChatMessageType.Broadcast));
        }

        public void HandleActionSetAllegianceOfficer(string playerName, uint officerLevel)
        {
            //Console.WriteLine($"{Name}.HandleActionSetAllegianceOfficer({playerName}, {officerLevel})");

            // check if player is in an allegiance
            if (Allegiance == null)
            {
                Session.Network.EnqueueSend(new GameEventWeenieError(Session, WeenieError.YouAreNotInAllegiance));
                return;
            }

            // TODO: also check officer permissions
            if (AllegiancePermissionLevel < AllegiancePermissionLevel.Seneschal)
            {
                Session.Network.EnqueueSend(new GameEventWeenieError(Session, WeenieError.YouDoNotHaveAuthorityInAllegiance));
                return;
            }

            var player = PlayerManager.FindByName(playerName);
            if (player == null)
            {
                Session.Network.EnqueueSend(new GameMessageSystemChat($"{playerName} not found", ChatMessageType.Broadcast));
                return;
            }

            if (!Allegiance.Members.ContainsKey(player.Guid))
            {
                Session.Network.EnqueueSend(new GameMessageSystemChat($"{playerName} not found in this allegiance", ChatMessageType.Broadcast));
                return;
            }

            if (player.Guid.Full == Allegiance.MonarchId)
            {
                Session.Network.EnqueueSend(new GameEventWeenieError(Session, WeenieError.YouDoNotHaveAuthorityInAllegiance));
                return;
            }

            // seneschals can only promote/demote speakers
            if (AllegiancePermissionLevel == AllegiancePermissionLevel.Seneschal)
            {
                if (officerLevel > 1 || (AllegianceOfficerLevel)(player.AllegianceOfficerRank ?? 0) > AllegianceOfficerLevel.Speaker)
                {
                    Session.Network.EnqueueSend(new GameEventWeenieError(Session, WeenieError.YouDoNotHaveAuthorityInAllegiance));
                    return;
                }
            }

            if (officerLevel < 1 || officerLevel > 3)
            {
                Session.Network.EnqueueSend(new GameMessageSystemChat("Please specify a valid officer level as a number between 1 and 3.", ChatMessageType.Broadcast));
                return;
            }

            player.AllegianceOfficerRank = (int)officerLevel;
            var title = Allegiance.GetOfficerTitle((AllegianceOfficerLevel)officerLevel);

            Allegiance.BuildOfficers();

            Session.Network.EnqueueSend(new GameMessageSystemChat($"{player.Name} is now {title}.", ChatMessageType.Broadcast));

            // send message to online target player?
        }

        public void HandleActionRemoveAllegianceOfficer(string officerName)
        {
            //Console.WriteLine($"{Name}.HandleActionRemoveAllegianceOfficer({officerName})");

            // check if player is in an allegiance
            if (Allegiance == null)
            {
                Session.Network.EnqueueSend(new GameEventWeenieError(Session, WeenieError.YouAreNotInAllegiance));
                return;
            }

            if (AllegiancePermissionLevel < AllegiancePermissionLevel.Seneschal)
            {
                Session.Network.EnqueueSend(new GameEventWeenieError(Session, WeenieError.YouDoNotHaveAuthorityInAllegiance));
                return;
            }

            var officer = PlayerManager.FindByName(officerName);
            if (officer == null)
            {
                Session.Network.EnqueueSend(new GameMessageSystemChat($"{officerName} not found", ChatMessageType.Broadcast));
                return;
            }

            if (!Allegiance.Members.ContainsKey(officer.Guid))
            {
                Session.Network.EnqueueSend(new GameMessageSystemChat($"{officerName} not found in this allegiance", ChatMessageType.Broadcast));
                return;
            }

            if (!Allegiance.Officers.ContainsKey(officer.Guid))
            {
                Session.Network.EnqueueSend(new GameMessageSystemChat($"{officer.Name} not found in allegiance officers", ChatMessageType.Broadcast));
                return;
            }

            // seneschals can only promote/demote speakers and non-officers
            if (AllegiancePermissionLevel == AllegiancePermissionLevel.Seneschal)
            {
                if ((AllegianceOfficerLevel)(officer.AllegianceOfficerRank ?? 0) > AllegianceOfficerLevel.Speaker)
                {
                    Session.Network.EnqueueSend(new GameEventWeenieError(Session, WeenieError.YouDoNotHaveAuthorityInAllegiance));
                    return;
                }
            }

            officer.AllegianceOfficerRank = null;
            Allegiance.BuildOfficers();

            Session.Network.EnqueueSend(new GameMessageSystemChat($"{officer.Name} has been removed from allegiance officers.", ChatMessageType.Broadcast));

            // send message to online target player?
        }

        public void HandleActionClearAllegianceOfficers()
        {
            //Console.WriteLine($"{Name}.HandleActionClearAllegianceOfficers()");

            // check if player is in an allegiance
            if (Allegiance == null)
            {
                Session.Network.EnqueueSend(new GameEventWeenieError(Session, WeenieError.YouAreNotInAllegiance));
                return;
            }

            // could castellans perform this action?
            if (AllegiancePermissionLevel < AllegiancePermissionLevel.Monarch)
            {
                Session.Network.EnqueueSend(new GameEventWeenieError(Session, WeenieError.YouDoNotHaveAuthorityInAllegiance));
                return;
            }

            foreach (var kvp in Allegiance.Officers)
            {
                var officer = PlayerManager.FindByGuid(kvp.Key);
                if (officer != null)
                    officer.AllegianceOfficerRank = null;
            }

            Allegiance.BuildOfficers();

            Session.Network.EnqueueSend(new GameMessageSystemChat($"The list of officers has been cleared.", ChatMessageType.Broadcast));
        }

        public void HandleActionAllegianceInfoRequest(string playerName)
        {
            //Console.WriteLine($"{Name}.HandleActionRemoveAllegianceOfficer({playerName})");

            // check if player is in an allegiance
            if (Allegiance == null)
            {
                Session.Network.EnqueueSend(new GameEventWeenieError(Session, WeenieError.YouAreNotInAllegiance));
                return;
            }

            if (AllegiancePermissionLevel < AllegiancePermissionLevel.Seneschal)
            {
                Session.Network.EnqueueSend(new GameEventWeenieError(Session, WeenieError.YouDoNotHaveAuthorityInAllegiance));
                return;
            }

            var player = PlayerManager.FindByName(playerName);
            if (player == null)
            {
                Session.Network.EnqueueSend(new GameMessageSystemChat($"{playerName} not found", ChatMessageType.Broadcast));
                return;
            }

            Allegiance.Members.TryGetValue(player.Guid, out var allegianceNode);
            if (allegianceNode == null)
            {
                Session.Network.EnqueueSend(new GameMessageSystemChat($"{playerName} not found in this allegiance", ChatMessageType.Broadcast));
                return;
            }
            var profile = new AllegianceProfile(Allegiance, allegianceNode);

            Session.Network.EnqueueSend(new GameEventAllegianceInfoResponse(Session, player.Guid.Full, profile));
        }

        public void HandleActionDoAllegianceLockAction(AllegianceLockAction action)
        {
            //Console.WriteLine($"{Name}.HandleActionDoAllegianceLockAction({action})");

            // check if player is in an allegiance
            if (Allegiance == null)
            {
                Session.Network.EnqueueSend(new GameEventWeenieError(Session, WeenieError.YouAreNotInAllegiance));
                return;
            }

            var lockStatus = Allegiance.IsLocked ? "locked" : "unlocked";

            // no permissions for checks?
            if (action == AllegianceLockAction.Check)
            {
                Session.Network.EnqueueSend(new GameMessageSystemChat($"The allegiance is currently {lockStatus}.", ChatMessageType.Broadcast));
                return;
            }

            if (action == AllegianceLockAction.CheckApproved)
            {
                if (Allegiance.ApprovedVassals.Count == 0)
                {
                    Session.Network.EnqueueSend(new GameMessageSystemChat($"The approved vassals list is currently empty.", ChatMessageType.Broadcast));
                    return;
                }

                var list = "Approved vassals:";
                foreach (var entity in Allegiance.ApprovedVassals)
                {
                    var approvedVassal = PlayerManager.FindByGuid(entity.Key);
                    if (approvedVassal == null)
                    {
                        // automatically remove?
                        log.Warn($"{Name}.HandleActionDoAllegianceLockAction({action}): couldn't find approved vassal {entity.Key:X8}");
                        continue;
                    }

                    list += $"\n{approvedVassal.Name}";
                }

                Session.Network.EnqueueSend(new GameMessageSystemChat(list, ChatMessageType.Broadcast));
                return;
            }

            if (AllegiancePermissionLevel < AllegiancePermissionLevel.Seneschal)
            {
                Session.Network.EnqueueSend(new GameEventWeenieError(Session, WeenieError.YouDoNotHaveAuthorityInAllegiance));
                return;
            }

            if (action == AllegianceLockAction.ClearApproved)
            {
                foreach (var entity in Allegiance.ApprovedVassals)
                    Allegiance.RemoveApprovedVassal(entity.Key);

                Session.Network.EnqueueSend(new GameMessageSystemChat($"The approved vassals list has been cleared.", ChatMessageType.Broadcast));
                return;
            }

            if (action == AllegianceLockAction.On && Allegiance.IsLocked || action== AllegianceLockAction.Off && !Allegiance.IsLocked)
            {
                Session.Network.EnqueueSend(new GameMessageSystemChat($"The allegiance is already {lockStatus}.", ChatMessageType.Broadcast));
                return;
            }

            if (action == AllegianceLockAction.On)
                Allegiance.IsLocked = true;
            else if (action == AllegianceLockAction.Off)
                Allegiance.IsLocked = false;
            else if (action == AllegianceLockAction.Toggle)
                Allegiance.IsLocked = !Allegiance.IsLocked;

            Allegiance.SaveBiotaToDatabase();

            lockStatus = Allegiance.IsLocked ? "locked" : "unlocked";

            Session.Network.EnqueueSend(new GameMessageSystemChat($"The allegiance is now {lockStatus}.", ChatMessageType.Broadcast));
        }

        public void HandleActionSetAllegianceApprovedVassal(string playerName)
        {
            //Console.WriteLine($"{Name}.HandleActionSetAllegianceApprovedVassal({playerName})");

            // check if player is in an allegiance
            if (Allegiance == null)
            {
                Session.Network.EnqueueSend(new GameEventWeenieError(Session, WeenieError.YouAreNotInAllegiance));
                return;
            }

            if (AllegiancePermissionLevel < AllegiancePermissionLevel.Castellan)
            {
                Session.Network.EnqueueSend(new GameEventWeenieError(Session, WeenieError.YouDoNotHaveAuthorityInAllegiance));
                return;
            }

            var player = PlayerManager.FindByName(playerName);

            if (player == null)
            {
                Session.Network.EnqueueSend(new GameMessageSystemChat($"{playerName} not found", ChatMessageType.Broadcast));
                return;
            }

            if (player.Allegiance != null)
            {
                Session.Network.EnqueueSend(new GameMessageSystemChat($"{player.Name} is already in an allegiance.", ChatMessageType.Broadcast));
                return;
            }

            if (Allegiance.Members.ContainsKey(player.Guid))
            {
                Session.Network.EnqueueSend(new GameMessageSystemChat($"{player.Name} is already in the allegiance.", ChatMessageType.Broadcast));
                return;
            }

            if (Allegiance.HasApprovedVassal(player.Guid.Full))
            {
                Session.Network.EnqueueSend(new GameMessageSystemChat($"{player.Name} is already an approved vassal.", ChatMessageType.Broadcast));
                return;
            }

            Allegiance.AddApprovedVassal(player.Guid.Full);

            Session.Network.EnqueueSend(new GameMessageSystemChat($"{player.Name} is now an approved vassal.", ChatMessageType.Broadcast));
        }

        public void HandleActionAllegianceChatBoot(string playerName, string reason)
        {
            //Console.WriteLine($"{Name}.HandleActionAllegianceChatBoot({playerName}, {reason})");

            // check if player is in an allegiance
            if (Allegiance == null)
            {
                Session.Network.EnqueueSend(new GameEventWeenieError(Session, WeenieError.YouAreNotInAllegiance));
                return;
            }

            if (AllegiancePermissionLevel < AllegiancePermissionLevel.Speaker)
            {
                Session.Network.EnqueueSend(new GameEventWeenieError(Session, WeenieError.YouDoNotHaveAuthorityInAllegiance));
                return;
            }

            var player = PlayerManager.FindByName(playerName);

            if (player == null)
            {
                Session.Network.EnqueueSend(new GameMessageSystemChat($"{playerName} not found", ChatMessageType.Broadcast));
                return;
            }

            if (!Allegiance.Members.ContainsKey(player.Guid))
            {
                Session.Network.EnqueueSend(new GameMessageSystemChat($"{player.Name} not found in allegiance", ChatMessageType.Broadcast));
                return;
            }

            if (player.Guid == Guid)
            {
                Session.Network.EnqueueSend(new GameMessageSystemChat($"You cannot boot yourself from allegiance chat.", ChatMessageType.Broadcast));
                return;
            }

            if (player.Guid.Full == Allegiance.MonarchId)
            {
                Session.Network.EnqueueSend(new GameMessageSystemChat($"You cannot boot the monarch from allegiance chat.", ChatMessageType.Broadcast));
                return;
            }

            if (Allegiance.ChatFilters.TryGetValue(player.Guid, out var existing))
            {
                if (existing == DateTime.MaxValue)
                {
                    Session.Network.EnqueueSend(new GameMessageSystemChat($"{player.Name} has already been booted from allegiance chat.", ChatMessageType.Broadcast));
                    return;
                }

                Allegiance.ChatFilters[player.Guid] = DateTime.MaxValue;
            }
            else
                Allegiance.ChatFilters.Add(player.Guid, DateTime.MaxValue);

            Session.Network.EnqueueSend(new GameMessageSystemChat($"{player.Name} has been booted from allegiance chat.", ChatMessageType.Broadcast));

            var onlinePlayer = PlayerManager.GetOnlinePlayer(player.Guid);

            if (onlinePlayer != null)
                onlinePlayer.Session.Network.EnqueueSend(new GameMessageSystemChat($"You have been booted from Allegiance chat ({reason})", ChatMessageType.Broadcast));
        }

        public static TimeSpan AllegianceChat_GagTime = TimeSpan.FromMinutes(5);

        public void HandleActionAllegianceChatGag(string playerName, bool enabled)
        {
            if (enabled)
                HandleActionAllegianceChatGag_Enabled(playerName);
            else
                HandleActionAllegianceChatGag_Disabled(playerName);
        }

        public void HandleActionAllegianceChatGag_Enabled(string playerName)
        {
            //Console.WriteLine($"{Name}.HandleActionAllegianceChatGag_Enabled({playerName})");

            // check if player is in an allegiance
            if (Allegiance == null)
            {
                Session.Network.EnqueueSend(new GameEventWeenieError(Session, WeenieError.YouAreNotInAllegiance));
                return;
            }

            if (AllegiancePermissionLevel < AllegiancePermissionLevel.Speaker)
            {
                Session.Network.EnqueueSend(new GameEventWeenieError(Session, WeenieError.YouDoNotHaveAuthorityInAllegiance));
                return;
            }

            var player = PlayerManager.FindByName(playerName);

            if (player == null)
            {
                Session.Network.EnqueueSend(new GameMessageSystemChat($"{playerName} not found", ChatMessageType.Broadcast));
                return;
            }

            if (!Allegiance.Members.ContainsKey(player.Guid))
            {
                Session.Network.EnqueueSend(new GameMessageSystemChat($"{player.Name} not found in allegiance", ChatMessageType.Broadcast));
                return;
            }

            if (player.Guid == Guid)
            {
                Session.Network.EnqueueSend(new GameMessageSystemChat($"You cannot gag yourself.", ChatMessageType.Broadcast));
                return;
            }

            if (player.Guid.Full == Allegiance.MonarchId)
            {
                Session.Network.EnqueueSend(new GameMessageSystemChat($"You cannot gag the monarch.", ChatMessageType.Broadcast));
                return;
            }

            if (Allegiance.ChatFilters.TryGetValue(player.Guid, out var existing))
            {
                if (existing == DateTime.MaxValue)
                {
                    Session.Network.EnqueueSend(new GameMessageSystemChat($"{player.Name} has already been booted from allegiance chat.", ChatMessageType.Broadcast));
                    return;
                }

                Allegiance.ChatFilters[player.Guid] = DateTime.UtcNow + AllegianceChat_GagTime;
            }
            else
                Allegiance.ChatFilters.Add(player.Guid, DateTime.UtcNow + AllegianceChat_GagTime);

            Session.Network.EnqueueSend(new GameMessageSystemChat($"{player.Name} has been gagged.", ChatMessageType.Broadcast));

            var onlinePlayer = PlayerManager.GetOnlinePlayer(player.Guid);

            if (onlinePlayer != null)
                onlinePlayer.Session.Network.EnqueueSend(new GameMessageSystemChat($"You have been gagged in allegiance chat.", ChatMessageType.Broadcast));
        }

        public void HandleActionAllegianceChatGag_Disabled(string playerName)
        {
            //Console.WriteLine($"{Name}.HandleActionAllegianceChatGag_Disabled({playerName})");

            // check if player is in an allegiance
            if (Allegiance == null)
            {
                Session.Network.EnqueueSend(new GameEventWeenieError(Session, WeenieError.YouAreNotInAllegiance));
                return;
            }

            if (AllegiancePermissionLevel < AllegiancePermissionLevel.Speaker)
            {
                Session.Network.EnqueueSend(new GameEventWeenieError(Session, WeenieError.YouDoNotHaveAuthorityInAllegiance));
                return;
            }

            var player = PlayerManager.FindByName(playerName);

            if (player == null)
            {
                Session.Network.EnqueueSend(new GameMessageSystemChat($"{playerName} not found", ChatMessageType.Broadcast));
                return;
            }

            if (!Allegiance.Members.ContainsKey(player.Guid))
            {
                Session.Network.EnqueueSend(new GameMessageSystemChat($"{player.Name} not found in allegiance", ChatMessageType.Broadcast));
                return;
            }

            if (player.Guid == Guid)
            {
                Session.Network.EnqueueSend(new GameMessageSystemChat($"You cannot ungag yourself.", ChatMessageType.Broadcast));
                return;
            }

            if (!Allegiance.ChatFilters.TryGetValue(player.Guid, out var existing))
            {
                Session.Network.EnqueueSend(new GameMessageSystemChat($"{player.Name} has not been gagged.", ChatMessageType.Broadcast));
                return;
            }

            if (existing == DateTime.MaxValue)
            {
                Session.Network.EnqueueSend(new GameMessageSystemChat($"{player.Name} has been booted from allegiance chat.", ChatMessageType.Broadcast));
                return;
            }

            Allegiance.ChatFilters.Remove(player.Guid);

            Session.Network.EnqueueSend(new GameMessageSystemChat($"{player.Name} has been ungagged.", ChatMessageType.Broadcast));

            var onlinePlayer = PlayerManager.GetOnlinePlayer(player.Guid);

            if (onlinePlayer != null)
                onlinePlayer.Session.Network.EnqueueSend(new GameMessageSystemChat($"You have been ungagged in allegiance chat.", ChatMessageType.Broadcast));
        }

        public void HandleActionListAllegianceBans()
        {
            //Console.WriteLine($"{Name}.HandleActionListAllegianceBans()");

            // check if player is in an allegiance
            if (Allegiance == null)
            {
                Session.Network.EnqueueSend(new GameEventWeenieError(Session, WeenieError.YouAreNotInAllegiance));
                return;
            }

            var banList = Allegiance.BanList;

            if (banList.Count == 0)
            {
                Session.Network.EnqueueSend(new GameMessageSystemChat($"The ban list is currently empty.", ChatMessageType.Broadcast));
                return;
            }

            var list = "Allegiance ban list:";
            foreach (var entity in banList)
            {
                var player = PlayerManager.FindByGuid(entity.Key);

                if (player == null)
                {
                    // automatically remove?
                    log.Warn($"{Name}.HandleActionListAllegianceBans(): couldn't find banned player {entity.Key:X8}");
                    continue;
                }

                list += $"\n{player.Name}";
            }

            Session.Network.EnqueueSend(new GameMessageSystemChat(list, ChatMessageType.Broadcast));
        }

        public void HandleActionAddAllegianceBan(string playerName)
        {
            //Console.WriteLine($"{Name}.HandleActionAddAllegianceBan({playerName})");

            // check if player is in an allegiance
            if (Allegiance == null)
            {
                Session.Network.EnqueueSend(new GameEventWeenieError(Session, WeenieError.YouAreNotInAllegiance));
                return;
            }

            if (AllegiancePermissionLevel < AllegiancePermissionLevel.Seneschal)
            {
                Session.Network.EnqueueSend(new GameEventWeenieError(Session, WeenieError.YouDoNotHaveAuthorityInAllegiance));
                return;
            }

            var player = PlayerManager.FindByName(playerName);

            if (player == null)
            {
                Session.Network.EnqueueSend(new GameMessageSystemChat($"{playerName} not found", ChatMessageType.Broadcast));
                return;
            }

            // any other restrictions?
            if (player.Guid.Full == Allegiance.MonarchId)
            {
                Session.Network.EnqueueSend(new GameMessageSystemChat($"{player.Name} cannot be banned from the allegiance!", ChatMessageType.Broadcast));
                return;
            }

            if (Allegiance.IsBanned(player.Guid.Full))
            {
                Session.Network.EnqueueSend(new GameMessageSystemChat($"{player.Name} is already banned from the allegiance.", ChatMessageType.Broadcast));
                return;
            }

            Allegiance.AddBan(player.Guid.Full);

            Session.Network.EnqueueSend(new GameMessageSystemChat($"{player.Name} has been banned from the allegiance.", ChatMessageType.Broadcast));

            // were they already a member? if so, boot them...
            if (Allegiance.Members.ContainsKey(player.Guid))
                HandleActionBreakAllegianceBoot(player.Name, false);
        }

        public void HandleActionRemoveAllegianceBan(string playerName)
        {
            //Console.WriteLine($"{Name}.HandleActionRemoveAllegianceBan({playerName})");

            // check if player is in an allegiance
            if (Allegiance == null)
            {
                Session.Network.EnqueueSend(new GameEventWeenieError(Session, WeenieError.YouAreNotInAllegiance));
                return;
            }

            if (AllegiancePermissionLevel < AllegiancePermissionLevel.Seneschal)
            {
                Session.Network.EnqueueSend(new GameEventWeenieError(Session, WeenieError.YouDoNotHaveAuthorityInAllegiance));
                return;
            }

            var player = PlayerManager.FindByName(playerName);

            if (player == null)
            {
                Session.Network.EnqueueSend(new GameMessageSystemChat($"{playerName} not found", ChatMessageType.Broadcast));
                return;
            }

            if (!Allegiance.IsBanned(player.Guid.Full))
            {
                Session.Network.EnqueueSend(new GameMessageSystemChat($"{player.Name} is not banned from the allegiance.", ChatMessageType.Broadcast));
                return;
            }

            Allegiance.RemoveBan(player.Guid.Full);

            Session.Network.EnqueueSend(new GameMessageSystemChat($"{player.Name} is no longer banned from the allegiance.", ChatMessageType.Broadcast));
        }

        public void HandleActionBreakAllegianceBoot(string playerName, bool accountBoot)
        {
            log.DebugFormat("[ALLEGIANCE] {0}.HandleActionBreakAllegianceBoot({1}, {2})", Name, playerName, accountBoot);

            // TODO: handle account boot

            // check if player is in an allegiance
            if (Allegiance == null)
            {
                Session.Network.EnqueueSend(new GameEventWeenieError(Session, WeenieError.YouAreNotInAllegiance));
                return;
            }

            if (AllegiancePermissionLevel < AllegiancePermissionLevel.Seneschal)
            {
                Session.Network.EnqueueSend(new GameEventWeenieError(Session, WeenieError.YouDoNotHaveAuthorityInAllegiance));
                return;
            }

            var player = PlayerManager.FindByName(playerName);

            if (player == null)
            {
                Session.Network.EnqueueSend(new GameMessageSystemChat($"{playerName} not found", ChatMessageType.Broadcast));
                return;
            }

            if (!Allegiance.Members.ContainsKey(player.Guid))
            {
                Session.Network.EnqueueSend(new GameMessageSystemChat($"{playerName} not found in allegiance", ChatMessageType.Broadcast));
                return;
            }

            if (player.Guid == Guid)
            {
                Session.Network.EnqueueSend(new GameMessageSystemChat($"You cannot boot yourself from the allegiance.", ChatMessageType.Broadcast));
                return;
            }

            if (player.Guid.Full == Allegiance.MonarchId)
            {
                Session.Network.EnqueueSend(new GameMessageSystemChat($"You cannot boot the monarch from the allegiance!", ChatMessageType.Broadcast));
                return;
            }

            var patron = PlayerManager.FindByGuid(new ObjectGuid(player.PatronId ?? 0));
            if (patron == null)
            {
                Console.WriteLine($"{Name}.HandleActionBreakAllegianceBoot({player.Name}, {accountBoot}): couldn't find patron id {player.PatronId}");
                return;
            }

            player.PatronId = null;
            player.UpdateProperty(PropertyInstanceId.Monarch, null, true);

            // Record the allegiance the booted player is leaving so they can re-swear back for free
            RecordReswearAnchorOnBreak(player, Allegiance.MonarchId ?? 0);
            player.SaveBiotaToDatabase();

            // walk the allegiance tree from this node, update monarch ids
            Allegiance.Members.TryGetValue(player.Guid, out var targetNode);

            targetNode.Walk((node) =>
            {
                node.Player.UpdateProperty(PropertyInstanceId.Monarch, player.Guid.Full, true);

                node.Player.SaveBiotaToDatabase();
            });

            // rebuild allegiance tree structures
            AllegianceManager.OnBreakAllegiance(player, patron);

            CheckAllegianceHouse(player.Guid);

            var newAllegiance = AllegianceManager.GetAllegiance(player);
            if (newAllegiance != null)
                newAllegiance.Monarch.Walk((node) => CheckAllegianceHouse(node.PlayerGuid), false);

            // update allegiance ui panels?

            Session.Network.EnqueueSend(new GameMessageSystemChat($"{player.Name} has been removed from the allegiance.", ChatMessageType.Broadcast));

            var onlinePlayer = PlayerManager.GetOnlinePlayer(player.Guid);
            if (onlinePlayer != null)
                onlinePlayer.Session.Network.EnqueueSend(new GameMessageSystemChat("You have been booted from the allegiance!", ChatMessageType.Broadcast));
        }

        /// <summary>
        /// After a voluntary break severs a subtree from its old monarch chain, checks whether any player
        /// in that subtree now has an account sibling still in the original allegiance.  If so, that player
        /// is force-broken from their patron (so the account cannot straddle two allegiances), and the break
        /// cascades downward: same-account patron–vassal bonds are preserved but their sub-vassals on other
        /// accounts are also severed.
        /// </summary>
        /// <param name="detachedRoot">Root node of the newly detached subtree.</param>
        /// <param name="oldMonarchId">Monarch GUID that the detached tree just left.</param>
        private static void ResolveAccountConflictsAfterBreak(AllegianceNode detachedRoot, uint oldMonarchId)
        {
            // Collect nodes that need to be force-broken (don't modify tree while walking)
            var conflicted = new List<AllegianceNode>();

            detachedRoot.Walk(node =>
            {
                var player = node.Player;
                if (player?.Account == null) return;

                var accountPlayers = PlayerManager.GetAccountPlayers(player.Account.AccountId);
                bool hasConflict = accountPlayers.Values.Any(ap =>
                    ap.Guid != player.Guid &&
                    ap.MonarchId.HasValue &&
                    ap.MonarchId.Value == oldMonarchId);

                if (hasConflict && node.Patron != null)
                    conflicted.Add(node);
            });

            foreach (var node in conflicted)
            {
                // Only process if the node is still in a state that needs breaking
                // (an earlier iteration may have already detached an ancestor)
                if (node.Patron == null) continue;

                var patronPlayer = node.Patron.Player;
                ForceBreakVassalCascade(node, patronPlayer, oldMonarchId);
            }
        }

        /// <summary>
        /// Force-breaks <paramref name="node"/> from <paramref name="directPatron"/>, stores the forced-break
        /// monarch so the player can re-swear into the original chain cooldown-free, then cascades downward:
        /// same-account bonds are preserved, different-account vassals are also severed.
        /// </summary>
        private static void ForceBreakVassalCascade(AllegianceNode node, IPlayer directPatron, uint priorMonarchId)
        {
            var player = node.Player;
            if (player == null) return;

            bool sameAccount = player.Account?.AccountId == directPatron.Account?.AccountId;

            if (!sameAccount)
            {
                // Sever this vassal from their patron
                player.PatronId = null;

                var newMonarchId = node.HasVassals ? (uint?)player.Guid.Full : null;
                player.UpdateProperty(PropertyInstanceId.Monarch, newMonarchId, true);

                // Record the break so they can re-swear into the original chain without cooldown
                player.SetProperty(PropertyFloat.AllegianceSwearTimestamp, Time.GetUnixTime());
                player.SetProperty(PropertyInstanceId.AllegianceForcedBreakMonarchId, priorMonarchId);
                player.SaveBiotaToDatabase();

                var onlinePlayer = PlayerManager.GetOnlinePlayer(player.Guid);
                onlinePlayer?.Session.Network.EnqueueSend(new GameMessageSystemChat(
                    "You have been released from your allegiance because a member of your patron's chain broke their oath. " +
                    "You may re-swear to your original allegiance without waiting.",
                    ChatMessageType.Broadcast));
            }

            // Cascade to vassals regardless of whether we broke this node
            foreach (var vassal in node.Vassals.Values.ToList())
                ForceBreakVassalCascade(vassal, player, priorMonarchId);
        }

        public AllegiancePermissionLevel AllegiancePermissionLevel
        {
            // https://asheron.fandom.com/wiki/Allegiance_Officers

            // Level 1: Speaker

            // - Allegiance chat kick.
            // - Allegiance chat gag.
            // - Allegiance broadcast.
            // - Set/clear the MOTD.

            // Level 2: Seneschal

            // - Promote/demote members under own rank (i.e. can promote/demote speakers)
            // - Allegiance boot.
            // - Allegiance ban.
            // - Access allegiance info.
            // - Lock/unlock the allegiance.

            // Level 3: Castellan

            // - Promote/demote members to any rank, including other Castellans.
            // - Change officer titles.
            // - Set/clear the allegiance name.
            // - Set allegiance bindstone.
            // - Change mansion allegiance access/storage permissions.
            // - Bypass allegiance lock with own vassals.
            // - Bypass allegiance lock by approving particular vassals.

            get
            {
                if (Allegiance == null)
                    return AllegiancePermissionLevel.None;

                if (Allegiance.MonarchId == Guid.Full)
                    return AllegiancePermissionLevel.Monarch;

                return (AllegiancePermissionLevel)(AllegianceOfficerRank ?? 0);
            }
        }
    }
}
