using System;
using System.Collections.Generic;
using System.Linq;
using ACE.Common;
using ACE.Entity.Enum;

using ECreatureType = ACE.Entity.Enum.CreatureType;

namespace ACE.Server.WorldObjects
{
    partial class WorldObject
    {
        public static HashSet<CreatureType> SupportedGearCreatureTypes = new HashSet<CreatureType>()
        {
            ECreatureType.Banderling,
            ECreatureType.Drudge,
            ECreatureType.Gromnie,
            ECreatureType.Lugian,
            ECreatureType.Grievver,
            ECreatureType.Mattekar,
            ECreatureType.Mosswart,
            ECreatureType.Monouga,
            ECreatureType.Olthoi,
            ECreatureType.PhyntosWasp,
            ECreatureType.Shadow,
            ECreatureType.Shreth,
            ECreatureType.Skeleton,
            ECreatureType.Tumerok,
            ECreatureType.Tusker,
            ECreatureType.Virindi,
            ECreatureType.Wisp,
            ECreatureType.Zefir,
            ECreatureType.Golem,
            ECreatureType.Burun,
            ECreatureType.Remoran,
            ECreatureType.Reedshark,
            ECreatureType.Sclavus,
            ECreatureType.GotrokLugian,
            ECreatureType.Rat,
            ECreatureType.Moar,
            ECreatureType.Niffis,
            ECreatureType.Mite,
            ECreatureType.Undead,
            ECreatureType.Elemental,
            ECreatureType.Doll,
            ECreatureType.Simulacrum,
            ECreatureType.Siraluun,
            ECreatureType.Knathtead,
            ECreatureType.Margul,
            ECreatureType.HollowMinion
        };

        public CreatureType GetRandomCreatureType(CreatureType currentCreatureType = default)
        {
            var filteredCreatureTypes = SupportedGearCreatureTypes.Where(x => !x.Equals(currentCreatureType));
            return filteredCreatureTypes.ElementAt(new Random().Next(0, filteredCreatureTypes.Count()));
        }

        public void ApplyRandomSlayer(double slayerDamageBonus = 1.2f, CreatureType currentCreatureType = default)
        {
            this.SlayerDamageBonus = slayerDamageBonus;
            this.SlayerCreatureType = GetRandomCreatureType(currentCreatureType);
        }

        public void ApplyRandomGearCreatureSlayerRating(int? rating = default, CreatureType currentCreatureType = default)
        {
            this.GearCreatureSlayerRating = rating ?? ThreadSafeRandom.Next(1, 3);
            this.GearCreatureSlayerType = GetRandomCreatureType(currentCreatureType);
        }

        public void ApplyRandomGearCreatureResistRating(int? rating = default, CreatureType currentCreatureType = default)
        {
            this.GearCreatureResistRating = rating ?? ThreadSafeRandom.Next(1, 3);
            this.GearCreatureResistType = GetRandomCreatureType(currentCreatureType);
        }
    }
}
