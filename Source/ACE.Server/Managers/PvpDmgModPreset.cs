using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ACE.Server.Managers
{
    /// <summary>
    /// Root object deserialized from pvp_dmg_mod_presets.json.
    /// </summary>
    public class PvpDmgModPresetConfig
    {
        /// <summary>
        /// Ordered list of presets. The highest preset whose LevelThreshold is &lt;= the current
        /// global level cap is the "active" preset and will be applied automatically.
        /// </summary>
        [JsonPropertyName("presets")]
        public List<PvpDmgModPreset> Presets { get; set; } = new List<PvpDmgModPreset>();
    }

    /// <summary>
    /// One named level-threshold preset containing a partial set of pvp_dmg_mod overrides.
    /// </summary>
    public class PvpDmgModPreset
    {
        /// <summary>
        /// The global level cap at which this preset becomes active.
        /// The preset fires the first time the rolling level cap reaches or exceeds this value.
        /// </summary>
        [JsonPropertyName("levelThreshold")]
        public int LevelThreshold { get; set; }

        /// <summary>
        /// Human-readable label shown in admin commands and server logs.
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Map of PropertyManager key → double value.
        /// Only keys listed here are modified; all other pvp_dmg_mod properties are unchanged.
        /// Every key must already exist in DefaultPropertyManager (i.e. be a valid double property).
        /// </summary>
        [JsonPropertyName("properties")]
        public Dictionary<string, double> Properties { get; set; } = new Dictionary<string, double>();
    }
}
