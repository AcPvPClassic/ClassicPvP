using ACE.Common.Extensions;

namespace ACE.Server.Network.GameAction.Actions
{
    public static class GameActionRemoveAllegianceOfficer
    {
        [GameAction(GameActionType.RemoveAllegianceOfficer)]
        public static void Handle(ClientMessage message, Session session)
        {
            if (!Command.Handlers.PlayerCommands.CheckPlayerCommandRateLimit(session, 1))
            {
                return;
            }

            var officerName = message.Payload.ReadString16L();

            session.Player.HandleActionRemoveAllegianceOfficer(officerName);
        }
    }
}
