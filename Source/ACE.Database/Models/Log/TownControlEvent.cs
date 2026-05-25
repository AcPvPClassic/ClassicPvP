using System;

namespace ACE.Database.Models.Log
{
    public class TownControlEvent
    {
        public uint EventId { get; set; }
        public ushort TownId { get; set; }
        public DateTime? EventStartTime { get; set; }
        public DateTime? EventEndTime { get; set; }
        public uint AttackerId { get; set; }
        public string AttackerClanName { get; set; }
        public uint? DefenderId { get; set; }
        public string DefenderClanName { get; set; }
        public bool? IsAttackSuccess { get; set; }
    }
}
