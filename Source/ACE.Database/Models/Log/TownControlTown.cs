using System;

namespace ACE.Database.Models.Log
{
    public class TownControlTown
    {
        public ushort TownId { get; set; }
        public string TownName { get; set; }
        public uint? OwnerId { get; set; }
        public bool IsInConflict { get; set; }
        public DateTime? LastConflictStartTime { get; set; }
        public uint ConflictLength { get; set; }
        public uint? ConflictRespiteLength { get; set; }
        public ushort AttackerAwardsIndividual { get; set; }
        public ushort AttackerAwardsTotal { get; set; }
        public ushort DefenderAwardsIndividual { get; set; }
        public ushort DefenderAwardsTotal { get; set; }
    }
}
