using System;

namespace ACE.Database.Models.Log
{
    public class AllegianceHometownEvent
    {
        public uint   EventId                  { get; set; }
        public byte   TownId                   { get; set; }
        public uint   AttackerMonarchId        { get; set; }
        public string AttackerAllegianceName   { get; set; }
        public uint?  DefenderMonarchId        { get; set; }
        public string DefenderAllegianceName   { get; set; }
        public DateTime EventStartTime         { get; set; }
        public DateTime? Phase2StartTime       { get; set; }
        public DateTime? EventEndTime          { get; set; }
        /// <summary>0 = attacker win, 1 = defender win, 2 = phase 1 timeout</summary>
        public byte?  Outcome                  { get; set; }
    }
}
