namespace ACE.Server.Network.GameAction.Actions
{
    public static class GameActionListAllegianceBans
    {
        [GameAction(GameActionType.ListAllegianceBans)]
        public static void Handle(ClientMessage message, Session session)
        {
            if (!Command.Handlers.PlayerCommands.CheckPlayerCommandRateLimit(session))
            {
                return;
            }

            session.Player.HandleActionListAllegianceBans();
        }
    }
}
