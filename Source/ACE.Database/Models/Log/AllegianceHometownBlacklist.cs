using System;

namespace ACE.Database.Models.Log
{
    public class AllegianceHometownBlacklist
    {
        public uint     MonarchId      { get; set; }
        public string   AllegianceName { get; set; }
        public string   Reason         { get; set; }
        public string   AddedBy        { get; set; }
        public DateTime AddedAt        { get; set; }
    }
}
