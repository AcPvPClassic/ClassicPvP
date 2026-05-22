using ACE.Server.Managers;

namespace ACE.Server.Entity
{
    public static class IPlayerExtensions
    {
        /// <summary>
        /// Returns true if this player belongs to an allegiance whose MonarchId is in
        /// the whitelisted_allegiances server config list.
        /// </summary>
        public static bool IsAllegianceWhitelisted(this IPlayer player)
        {
            var allegiance = AllegianceManager.GetAllegiance(player);
            return allegiance?.MonarchId.HasValue == true &&
                   WhitelistedAllegiances.IsAllowedAllegiance((int)allegiance.MonarchId!.Value);
        }

        /// <summary>
        /// Returns true if playerA and playerB share the same monarch
        /// (treating solo players as their own monarch).
        /// </summary>
        public static bool IsSameAllegiance(this IPlayer playerA, IPlayer playerB)
        {
            var monarchA = playerA.MonarchId ?? playerA.Guid.Full;
            var monarchB = playerB.MonarchId ?? playerB.Guid.Full;
            return monarchA == monarchB;
        }
    }
}
