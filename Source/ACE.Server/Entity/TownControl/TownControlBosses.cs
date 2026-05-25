using System.Collections.Generic;

namespace ACE.Server.Entity.TownControl
{
    public enum TownControlBossType
    {
        InitiationBoss = 0,
        ConflictBoss   = 1
    }

    public class TownControlBoss
    {
        public uint   WeenieId  { get; set; }
        public ushort TownId    { get; set; }
        public string TownName  { get; set; }
        public TownControlBossType BossType { get; set; }
    }

    public static class TownControlBosses
    {
        private static readonly Dictionary<uint, TownControlBoss> _map = new Dictionary<uint, TownControlBoss>
        {
            // Shoushi — Init boss
            [42153365] = new TownControlBoss { WeenieId = 42153365, TownId = 91,  TownName = "Shoushi", BossType = TownControlBossType.InitiationBoss },
            // Shoushi — Conflict boss
            [42132032] = new TownControlBoss { WeenieId = 42132032, TownId = 91,  TownName = "Shoushi", BossType = TownControlBossType.ConflictBoss },

            // Holtburg — Init boss
            [4200001]  = new TownControlBoss { WeenieId = 4200001,  TownId = 72,  TownName = "Holtburg", BossType = TownControlBossType.InitiationBoss },
            // Holtburg — Conflict boss
            [4200007]  = new TownControlBoss { WeenieId = 4200007,  TownId = 72,  TownName = "Holtburg", BossType = TownControlBossType.ConflictBoss },

            // Yaraq — Init boss
            [4200003]  = new TownControlBoss { WeenieId = 4200003,  TownId = 102, TownName = "Yaraq", BossType = TownControlBossType.InitiationBoss },
            // Yaraq — Conflict boss
            [4200008]  = new TownControlBoss { WeenieId = 4200008,  TownId = 102, TownName = "Yaraq", BossType = TownControlBossType.ConflictBoss },
        };

        public static IReadOnlyDictionary<uint, TownControlBoss> BossMap => _map;

        public static bool IsTownControlBoss(uint weenieId)         => _map.ContainsKey(weenieId);
        public static bool IsTownControlInitBoss(uint weenieId)     => _map.TryGetValue(weenieId, out var b) && b.BossType == TownControlBossType.InitiationBoss;
        public static bool IsTownControlConflictBoss(uint weenieId) => _map.TryGetValue(weenieId, out var b) && b.BossType == TownControlBossType.ConflictBoss;

        public static TownControlBoss Get(uint weenieId) => _map.TryGetValue(weenieId, out var b) ? b : null;
    }
}
