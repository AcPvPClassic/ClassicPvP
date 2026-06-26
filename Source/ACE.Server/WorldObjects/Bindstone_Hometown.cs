using ACE.Entity.Enum;
using ACE.Server.Entity.AllegianceHometown;
using ACE.Server.Managers;
using ACE.Server.Network.GameEvent.Events;
using ACE.Server.Network.GameMessages.Messages;

namespace ACE.Server.WorldObjects
{
    /// <summary>
    /// Allegiance Hometown Phase 1 behaviour for Bindstone world objects.
    /// Handles USE interaction to claim unowned towns or start Phase 1 assaults.
    /// Phase 2 (combat) is handled by BindstoneCreatureProxy spawned at Phase 2 start.
    /// </summary>
    public partial class Bindstone
    {
        private void HandleHometownUse(Player player)
        {
            var entry = AllegianceHometownRegistry.GetByLandblock(CurrentLandblock?.Id.Landblock ?? 0);
            if (entry == null)
            {
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

            var monarchId     = player.Allegiance.MonarchId ?? player.Guid.Full;
            var allegianceName = player.Allegiance.AllegianceName ?? player.Name;
            var town          = AllegianceHometownManager.GetTown(entry.TownId);

            if (town == null)
            {
                player.SendTransientError("Hometown data is unavailable. Please try again later.");
                return;
            }

            // Unowned town — free claim via USE
            if (!town.OwnerMonarchId.HasValue)
            {
                if (town.ConflictPhase != 0)
                {
                    player.SendTransientError("This town cannot be claimed right now.");
                    return;
                }
                AllegianceHometownManager.ClaimTown(entry.TownId, monarchId, allegianceName);
                player.Session.Network.EnqueueSend(new GameMessageSystemChat(
                    $"Your allegiance has claimed {entry.TownName}!", ChatMessageType.Magic));
                return;
            }

            // Owned town — attacking is automatic; USE only shows status
            if (town.OwnerMonarchId == monarchId)
            {
                player.Session.Network.EnqueueSend(new GameMessageSystemChat(
                    $"Your allegiance owns {entry.TownName}. Defend the bind stone against attackers.", ChatMessageType.Magic));
                return;
            }

            if (town.ConflictPhase == 2)
            {
                player.Session.Network.EnqueueSend(new GameMessageSystemChat(
                    $"{entry.TownName} is under siege! Attack the Bind Stone creature to capture it.", ChatMessageType.Magic));
                return;
            }

            if (town.ConflictPhase == 1)
            {
                player.Session.Network.EnqueueSend(new GameMessageSystemChat(
                    $"{entry.TownName} is already under Phase 1 assault. Hold the bind stone with 2 allies to advance.", ChatMessageType.Magic));
                return;
            }

            player.Session.Network.EnqueueSend(new GameMessageSystemChat(
                $"{entry.TownName} is owned by {town.OwnerAllegianceName}. Gather 2 allies within 5m of the bind stone with no enemies on the landblock to start an assault.",
                ChatMessageType.Magic));
        }

        private void HandleSanctuaryUse(Player player)
        {
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

            var actionChain = new ACE.Server.Entity.Actions.ActionChain();
            if (player.CombatMode != CombatMode.NonCombat)
            {
                var stanceTime = player.SetCombatMode(CombatMode.NonCombat);
                actionChain.AddDelaySeconds(stanceTime);
                player.LastUseTime += stanceTime;
            }

            actionChain.AddAction(this, () => EnqueueBroadcastMotion(new ACE.Server.Entity.Motion(MotionStance.NonCombat, MotionCommand.Twitch1)));
            player.LastUseTime += player.EnqueueMotion(actionChain, MotionCommand.Sanctuary);

            actionChain.AddAction(this, () =>
            {
                if (player.IsWithinUseRadiusOf(this))
                {
                    player.Allegiance.Sanctuary = new ACE.Entity.Position(player.Location);
                    player.Allegiance.SaveBiotaToDatabase();
                    player.Session.Network.EnqueueSend(new GameMessageSystemChat(
                        GetProperty(ACE.Entity.Enum.Properties.PropertyString.UseMessage), ChatMessageType.Magic));
                }
                else
                    player.Session.Network.EnqueueSend(new GameEventWeenieError(player.Session, WeenieError.YouHaveMovedTooFar));
            });

            actionChain.EnqueueChain();
        }
    }
}
