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

            // Phase 2 in progress — cannot use while proxy is alive
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

            // Phase 1 already in progress
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
