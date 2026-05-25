using System;

namespace ACE.Database.Models.Log
{
    /// <summary>
    /// Represents a single movement anti-cheat violation event recorded to the ace_log database.
    /// Used as long-term evidence for manual ban reviews.
    /// </summary>
    public partial class MovementViolationLog
    {
        public uint   Id              { get; set; }
        public uint   CharacterId     { get; set; }
        public string CharacterName   { get; set; }
        public string AccountName     { get; set; }

        /// <summary>
        /// Short identifier for which anti-cheat check fired.
        /// Known values: speed_packet, speed_avg_3s, speed_avg_15s, geometry,
        /// jump_height, door_ghost, spawn_ghost, script_timing, script_packet_rate,
        /// script_reversal.
        /// </summary>
        public string ViolationType   { get; set; }

        /// <summary>
        /// The measured value that exceeded the limit (units depend on violation type:
        /// distance/s for speed checks, CV for timing, packets/s for packet rate, radians for reversal).
        /// </summary>
        public float  ObservedSpeed   { get; set; }

        /// <summary>The configured or computed limit corresponding to ObservedSpeed.</summary>
        public float  AllowedSpeed    { get; set; }

        /// <summary>Running suspicion score at the time this violation was recorded.</summary>
        public float  SuspicionScore  { get; set; }
        public string Location        { get; set; }
        public DateTime ViolationDateTime { get; set; }
    }
}
