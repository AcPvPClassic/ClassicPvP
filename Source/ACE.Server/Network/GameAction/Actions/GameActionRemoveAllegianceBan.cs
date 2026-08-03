using ACE.Common.Extensions;

namespace ACE.Server.Network.GameAction.Actions
{
    public static class GameActionRemoveAllegianceBan
    {
        [GameAction(GameActionType.RemoveAllegianceBan)]
        public static void Handle(ClientMessage message, Session session)
        {
            if (!Command.Handlers.PlayerCommands.CheckPlayerCommandRateLimit(session, 1))
            {
                return;
            }

            var playerName = message.Payload.ReadString16L();

            session.Player.HandleActionRemoveAllegianceBan(playerName);
        }
    }
}
