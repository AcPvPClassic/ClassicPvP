namespace ACE.Database
{
    /// <summary>
    /// Weenie IDs for custom ClassicPvP reward items.
    /// All features that spawn or check these items should reference this class
    /// rather than embedding raw integer literals.
    /// </summary>
    public static class CustomWeenieId
    {
        /// <summary>Darkbeat's Lost Storage Key (arena and PK quest reward)</summary>
        public const uint DarkbeatKey         = 480608;

        /// <summary>A Box (Hot Dungeon drop, Town Control and PK quest reward)</summary>
        public const uint ABox                = 510000;

        /// <summary>PK Trophy (arena, Town Control, PK quest reward and rename currency)</summary>
        public const uint PkTrophy            = 1000002;

        /// <summary>Phial of Bloody Tears (arena, Hot Dungeon and PK quest reward)</summary>
        public const uint PhialOfBloodyTears  = 1000003;

        public const uint XpBottle = 490071;

        public const uint TinkeringTool = 490298;

        public const uint SkillAttrResetGem = 49090101;


    }
}
