using ACE.Common.Extensions;

namespace ACE.Server.Network.GameAction.Actions
{
    public static class GameActionSetMotd
    {
        [GameAction(GameActionType.SetMotd)]
        public static void Handle(ClientMessage message, Session session)
        {
            if (!Command.Handlers.PlayerCommands.CheckPlayerCommandRateLimit(session, 1))
            {
                return;
            }

            var motd = message.Payload.ReadString16L();

            session.Player.HandleActionSetMotd(motd);
        }
    }
}
