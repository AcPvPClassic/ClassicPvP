using System;

namespace ACE.Database.Models.Log
{
    public class AllegianceHometownTown
    {
        public byte   TownId                    { get; set; }
        public string TownName                  { get; set; }
        public uint?  OwnerMonarchId            { get; set; }
        public string OwnerAllegianceName       { get; set; }
        public DateTime? CapturedAt             { get; set; }
        /// <summary>0 = none, 1 = Phase 1, 2 = Phase 2</summary>
        public byte   ConflictPhase             { get; set; }
        public uint?  ConflictAttackerMonarchId { get; set; }
        public string ConflictAttackerName      { get; set; }
        public DateTime? ConflictStartTime      { get; set; }
        public DateTime? Phase2StartTime        { get; set; }
    }
}
