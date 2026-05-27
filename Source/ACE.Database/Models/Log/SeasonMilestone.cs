using System;

namespace ACE.Database.Models.Log
{
    /// <summary>
    /// One row per weekly Sunday milestone snapshot taken by SeasonManager.
    /// </summary>
    public partial class SeasonMilestone
    {
        public ushort   Id               { get; set; }
        public ushort   WeekNumber       { get; set; }
        public DateTime SnapshotDatetime { get; set; }
    }
}
