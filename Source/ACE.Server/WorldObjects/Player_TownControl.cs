using ACE.Server.Entity.TownControl;
using ACE.Server.Managers;

namespace ACE.Server.WorldObjects
{
    partial class Player
    {
        /// <summary>
        /// Called on player login.  Validates all Town Control ownership quest stamps
        /// against the current TownControlManager state.  If the player has a stamp for
        /// a town whose owning allegiance no longer matches theirs, the stamp is erased.
        /// If the player's allegiance owns a town but they lack the stamp, it is granted.
        /// </summary>
        public void ValidateTownControlQuestStamps()
        {
            if (!TownControlManager.IsInitialized) return;

            var playerAllegiance  = AllegianceManager.GetAllegiance(this);
            var playerMonarchId   = playerAllegiance?.MonarchId;

            foreach (var town in TownControlManager.GetAllTowns())
            {
                var questName = town.TownName.Trim().Replace(" ", "") + "TownControlOwner";

                bool hasStamp    = QuestManager.HasQuest(questName);
                bool ownerMatch  = playerMonarchId.HasValue && town.OwnerId.HasValue
                                   && playerMonarchId.Value == town.OwnerId.Value;

                if (hasStamp && !ownerMatch)
                {
                    // Player has the stamp but is no longer part of the owning allegiance
                    QuestManager.Erase(questName);
                }
                else if (!hasStamp && ownerMatch)
                {
                    // Player belongs to the owning allegiance but lacks the stamp
                    QuestManager.Update(questName);
                }
            }
        }
    }
}
