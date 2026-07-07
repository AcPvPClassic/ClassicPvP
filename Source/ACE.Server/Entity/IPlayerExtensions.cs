using System.Linq;

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
            // When the whitelist is globally disabled, treat everyone as whitelisted.
            if (PropertyManager.GetBool("disable_allegiance_whitelist").Item)
                return true;

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

        /// <summary>
        /// Returns true if a DIFFERENT character on this player's account belongs to the
        /// allegiance led by <paramref name="monarchId"/>. The player's own character is
        /// excluded; the allegiance's monarch (whose own MonarchId may be null/self) is
        /// matched by guid. Uses the same Monarch-id semantics as the account-lock code.
        /// </summary>
        public static bool AccountHasAllegianceMember(this IPlayer player, uint monarchId)
        {
            if (monarchId == 0 || player?.Account == null)
                return false;

            var accountPlayers = PlayerManager.GetAccountPlayers(player.Account.AccountId);
            if (accountPlayers == null)
                return false;

            return accountPlayers.Values.Any(p =>
                p.Guid != player.Guid &&
                ((p.MonarchId ?? p.Guid.Full) == monarchId));
        }

        /// <summary>
        /// Anti-alt-farming rule: returns true when the victim is a throwaway parked on an
        /// allegiance-mate's account — i.e. the victim's account holds another character
        /// sworn into the KILLER's allegiance. Such kills earn no PK rewards (season
        /// leaderboard, arena, PK quests/bounties). Solo killers (no allegiance) never match.
        /// </summary>
        public static bool VictimIsAllegianceMateAlt(this IPlayer killer, IPlayer victim)
        {
            var killerMonarch = AllegianceManager.GetAllegiance(killer)?.MonarchId;
            if (!killerMonarch.HasValue || victim == null)
                return false;

            return victim.AccountHasAllegianceMember(killerMonarch.Value);
        }
    }
}
