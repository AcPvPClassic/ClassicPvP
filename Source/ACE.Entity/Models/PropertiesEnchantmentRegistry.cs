using System;

using ACE.Entity.Enum;

namespace ACE.Entity.Models
{
    public class PropertiesEnchantmentRegistry
    {
        public uint EnchantmentCategory { get; set; }
        public int SpellId { get; set; }
        public ushort LayerId { get; set; }
        public bool HasSpellSetId { get; set; }
        public SpellCategory SpellCategory { get; set; }
        public uint PowerLevel { get; set; }
        public double StartTime { get; set; }
        public double Duration { get; set; }
        public uint CasterObjectId { get; set; }
        public float DegradeModifier { get; set; }
        public float DegradeLimit { get; set; }
        public double LastTimeDegraded { get; set; }
        public EnchantmentTypeFlags StatModType { get; set; }
        public uint StatModKey { get; set; }
        public float StatModValue { get; set; }
        public EquipmentSet SpellSetId { get; set; }

        /// <summary>
        /// True when this enchantment was cast by a PK on a PK (used for pvp_dmg_mod_void_dot_rating_reduction).
        /// Not persisted to the database; set at runtime when the enchantment is created.
        /// </summary>
        public bool IsPvP { get; set; }

        public float? CachedModifiedStatModValue = null;
        public object CachedCasterObject = null;
    }
}
