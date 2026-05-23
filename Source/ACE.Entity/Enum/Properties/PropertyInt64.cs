using System.ComponentModel;

namespace ACE.Entity.Enum.Properties
{
    // No properties are sent to the client unless they featured an attribute.
    // SendOnLogin gets sent to players in the PlayerDescription event
    // AssessmentProperty gets sent in successful appraisal
    public enum PropertyInt64 : ushort
    {
        Undef                 = 0,
        [SendOnLogin]
        TotalExperience       = 1,
        [SendOnLogin]
        AvailableExperience   = 2,
        [AssessmentProperty]
        AugmentationCost      = 3,
        [AssessmentProperty]
        ItemTotalXp           = 4,
        [AssessmentProperty]
        ItemBaseXp            = 5,
        [SendOnLogin]
        AvailableLuminance    = 6,
        [SendOnLogin]
        MaximumLuminance      = 7,
        InteractionReqs       = 8,

        /* Custom Properties */
        AllegianceXPCached    = 9000,
        AllegianceXPGenerated = 9001,
        AllegianceXPReceived  = 9002,
        VerifyXp              = 9003,

        // CustomDM
        XpTrackerTotalXp            = 10000,
        XpTrackerStartTimestamp     = 10001,
        GameplayModeExtraIdentifier = 10002,
        TradeNoteValue              = 10003,

        // Rolling XP Cap — per-category XP buckets (Infiltration)
        CapQuestXp        = 10010,  // XP accumulated in the quest category since last cap advance
        CapMonsterXp      = 10011,  // XP accumulated in the monster/kill category since last cap advance
        CapPreviousXpCap  = 10012,  // Last rolling_xp_cap value seen; used to detect cap advancement and reset buckets
        CapDailyMaxPerCat = 10013,  // Per-category ceiling locked at the most recent cap advance
    }
}
