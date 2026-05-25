using System;

namespace ACE.Database.Models.Log
{
    /// <summary>
    /// Represents a single movement speed violation event recorded to the ace_log database.
    /// Used as long-term evidence for manual ban reviews.
    /// </summary>
    public partial class MovementViolationLog
    {
        public uint   Id              { get; set; }
        public uint   CharacterId     { get; set; }
        public string CharacterName   { get; set; }
        public string AccountName     { get; set; }
        public float  ObservedSpeed   { get; set; }
        public float  AllowedSpeed    { get; set; }
        /// <summary>Running suspicion score at the time this violation was recorded.</summary>
        public float  SuspicionScore  { get; set; }
        public string Location        { get; set; }
        public DateTime ViolationDateTime { get; set; }
    }
}
