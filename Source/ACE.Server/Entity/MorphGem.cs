using ACE.Common;
using ACE.Entity.Enum;
using ACE.Entity.Enum.Properties;
using ACE.Entity.Models;
using ACE.Server.Entity;
using ACE.Server.Factories.Tables;
using ACE.Server.Managers;
using ACE.Server.Network.GameMessages.Messages;
using ACE.Server.WorldObjects;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ACE.Server.Entity
{
    public class MorphGem
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Morph Gem Weenie IDs
        public const uint MorphGemValue            = 4200023;
        public const uint MorphGemRandomWorkmanship = 490027;
        public const uint MorphGemArcane           = 4200026;
        public const uint MorphGemRemoveMissileDReq = 480484;
        public const uint MorphGemRemoveMeleeDReq  = 480483;
        public const uint MorphGemRandomizeWeaponImbue = 480486;
        public const uint MorphGemRemovePlayerReq  = 480485;
        public const uint MorphGemRemoveLevelReq   = 480609;
        public const uint MorphGemSlayerUpgrade    = 480639;
        public const uint MorphGemBurningCoal      = 480638;
        public const uint MorphGemImpen            = 490025;
        public const uint MorphGemBanditHilt       = 490026;
        public const uint MorphGemRareUpgrade      = 490040;
        public const uint MorphGemRareReduction    = 490270;
        public const uint MorphGemJewelersSawblade = 490271;
        public const uint MorphGemAddSlayer        = 490304;
        public const uint MorphGemHematite         = 490284;
        public const uint MorphGemStrengthbeer     = 490327;
        public const uint MorphGemEndurancebeer    = 490328;
        public const uint MorphGemCoordinationbeer = 490329;
        public const uint MorphGemQuicknessbeer    = 490330;
        public const uint MorphGemFocusbeer        = 490331;
        public const uint MorphGemWillpowerbeer    = 490332;
        public const uint MorphGemHeroicMaster     = 1548800;
        public const uint MorphGemRandomCantrip    = 1548803;
        public const uint MorphGemBurden           = 1548804;
        public const uint MorphGemRareDmgBoost     = 1548805;
        public const uint MorphGemRareDmgReduction = 1548806;
        public const uint MorphGemMeleeCleave      = 490512;
        public const uint MorphGemMinValue         = 20000;
        #endregion Morph Gem Weenie IDs

        #region Rare/Cantrip Morph Gem Weenie IDs
        public const uint MorphGemRuneofAcidBane                         = 30112;
        public const uint MorphGemIdeographofAcidProtection              = 30113;
        public const uint MorphGemHieroglyphofAlchemyMastery             = 30114;
        public const uint MorphGemHieroglyphofArcaneEnlightenment        = 30115;
        public const uint MorphGemIdeographofArmor                       = 30116;
        public const uint MorphGemHieroglyphofArmorTinkeringExpertise    = 30117;
        public const uint MorphGemHieroglyphofMonsterAttunement          = 30118;
        public const uint MorphGemHieroglyphofPersonAttunement           = 30119;
        public const uint MorphGemHieroglyphofLightWeaponMastery         = 30120;
        public const uint MorphGemRuneofBladeBane                        = 30121;
        public const uint MorphGemRuneofBloodDrinker                     = 30122;
        public const uint MorphGemRuneofBludgeonBane                     = 30123;
        public const uint MorphGemIdeographofBludgeoningProtection       = 30124;
        public const uint MorphGemHieroglyphofMissileWeaponMastery       = 30125;
        public const uint MorphGemHieroglyphofCookingMastery             = 30126;
        public const uint MorphGemPictographofCoordination               = 30127;
        public const uint MorphGemHieroglyphofCreatureEnchantmentMastery = 30128;
        public const uint MorphGemHieroglyphofFinesseWeaponMastery       = 30130;
        public const uint MorphGemHieroglyphofDeceptionMastery           = 30131;
        public const uint MorphGemRuneofDefender                         = 30132;
        public const uint MorphGemPictographofEndurance                  = 30134;
        public const uint MorphGemIdeographofFireProtection              = 30135;
        public const uint MorphGemRuneofFlameBane                        = 30136;
        public const uint MorphGemHieroglyphofFletchingMastery           = 30137;
        public const uint MorphGemPictographofFocus                      = 30138;
        public const uint MorphGemRuneofFrostBane                        = 30139;
        public const uint MorphGemIdeographofFrostProtection             = 30140;
        public const uint MorphGemHieroglyphofHealingMastery             = 30141;
        public const uint MorphGemIdeographofRegeneration                = 30142;
        public const uint MorphGemRuneofHeartSeeker                      = 30143;
        public const uint MorphGemRuneofHermeticLink                     = 30144;
        public const uint MorphGemRuneofImpenetrability                  = 30145;
        public const uint MorphGemHieroglyphofItemEnchantmentMastery     = 30146;
        public const uint MorphGemHieroglyphofItemTinkeringExpertise     = 30147;
        public const uint MorphGemHieroglyphofJumpingMastery             = 30148;
        public const uint MorphGemHieroglyphofLeadershipMastery          = 30149;
        public const uint MorphGemHieroglyphofLifeMagicMastery           = 30150;
        public const uint MorphGemRuneofLightningBane                    = 30152;
        public const uint MorphGemIdeographofLightningProtection         = 30153;
        public const uint MorphGemHieroglyphofLockpickMastery            = 30154;
        public const uint MorphGemHieroglyphofFealtyMastery              = 30155;
        public const uint MorphGemHieroglyphofMagicResistance            = 30157;
        public const uint MorphGemHieroglyphofMagicItemTinkeringExpertise = 30158;
        public const uint MorphGemHieroglyphofManaConversionMastery      = 30159;
        public const uint MorphGemIdeographofBattlemagesBlessing         = 30160;
        public const uint MorphGemHieroglyphofInvulnerability            = 30161;
        public const uint MorphGemHieroglyphofImpregnability             = 30162;
        public const uint MorphGemRuneofPierceBane                       = 30163;
        public const uint MorphGemIdeographofPiercingProtection          = 30164;
        public const uint MorphGemPictographofQuickness                  = 30166;
        public const uint MorphGemHieroglyphofSprint                     = 30167;
        public const uint MorphGemPictographofWillpower                  = 30168;
        public const uint MorphGemIdeographofBladeProtection             = 30169;
        public const uint MorphGemRuneofSpiritDrinker                    = 30171;
        public const uint MorphGemIdeographofRevitalization              = 30173;
        public const uint MorphGemPictographofStrength                   = 30174;
        public const uint MorphGemRuneofSwiftKiller                      = 30175;
        public const uint MorphGemHieroglyphofHeavyWeaponMastery         = 30176;
        public const uint MorphGemHieroglyphofWarMagicMastery            = 30179;
        public const uint MorphGemHieroglyphofWeaponTinkeringExpertise   = 30180;
        public const uint MorphGemHieroglyphofDirtyFightingMastery       = 45361;
        public const uint MorphGemHieroglyphofDualWieldMastery           = 45362;
        public const uint MorphGemHieroglyphofRecklessnessMastery        = 45363;
        public const uint MorphGemHieroglyphofShieldMastery              = 45364;
        public const uint MorphGemHieroglyphofSneakAttackMastery         = 45365;
        public const uint MorphGemHieroglyphofVoidMagicMastery           = 70001;
        public const uint MorphGemHieroglyphofTwoHandedWeaponsMastery    = 70002;
        public const uint MorphGemHieroglyphofSummoningMastery           = 70003;
        #endregion Rare/Cantrip Morph Gem IDs

        public static HashSet<uint> MorphGems = new HashSet<uint>()
        {
            MorphGemValue,
            MorphGemArcane,
            MorphGemRemoveMissileDReq,
            MorphGemRemoveMeleeDReq,
            MorphGemRandomizeWeaponImbue,
            MorphGemRemovePlayerReq,
            MorphGemRemoveLevelReq,
            MorphGemSlayerUpgrade,
            MorphGemBurningCoal,
            MorphGemImpen,
            MorphGemRandomWorkmanship,
            MorphGemBanditHilt,
            MorphGemRareUpgrade,
            MorphGemJewelersSawblade,
            MorphGemRareReduction,
            MorphGemAddSlayer,
            MorphGemHematite,
            MorphGemStrengthbeer,
            MorphGemEndurancebeer,
            MorphGemCoordinationbeer,
            MorphGemQuicknessbeer,
            MorphGemFocusbeer,
            MorphGemWillpowerbeer,
            MorphGemHeroicMaster,
            MorphGemRandomCantrip,
            MorphGemBurden,
            MorphGemRareDmgBoost,
            MorphGemRareDmgReduction,
            MorphGemMeleeCleave,
            MorphGemRuneofAcidBane,
            MorphGemIdeographofAcidProtection,
            MorphGemHieroglyphofAlchemyMastery,
            MorphGemHieroglyphofArcaneEnlightenment,
            MorphGemIdeographofArmor,
            MorphGemHieroglyphofArmorTinkeringExpertise,
            MorphGemHieroglyphofMonsterAttunement,
            MorphGemHieroglyphofPersonAttunement,
            MorphGemHieroglyphofLightWeaponMastery,
            MorphGemRuneofBladeBane,
            MorphGemRuneofBloodDrinker,
            MorphGemRuneofBludgeonBane,
            MorphGemIdeographofBludgeoningProtection,
            MorphGemHieroglyphofMissileWeaponMastery,
            MorphGemHieroglyphofCookingMastery,
            MorphGemPictographofCoordination,
            MorphGemHieroglyphofCreatureEnchantmentMastery,
            MorphGemHieroglyphofFinesseWeaponMastery,
            MorphGemHieroglyphofDeceptionMastery,
            MorphGemRuneofDefender,
            MorphGemPictographofEndurance,
            MorphGemIdeographofFireProtection,
            MorphGemRuneofFlameBane,
            MorphGemHieroglyphofFletchingMastery,
            MorphGemPictographofFocus,
            MorphGemRuneofFrostBane,
            MorphGemIdeographofFrostProtection,
            MorphGemHieroglyphofHealingMastery,
            MorphGemIdeographofRegeneration,
            MorphGemRuneofHeartSeeker,
            MorphGemRuneofHermeticLink,
            MorphGemRuneofImpenetrability,
            MorphGemHieroglyphofItemEnchantmentMastery,
            MorphGemHieroglyphofItemTinkeringExpertise,
            MorphGemHieroglyphofJumpingMastery,
            MorphGemHieroglyphofLeadershipMastery,
            MorphGemHieroglyphofLifeMagicMastery,
            MorphGemRuneofLightningBane,
            MorphGemIdeographofLightningProtection,
            MorphGemHieroglyphofLockpickMastery,
            MorphGemHieroglyphofFealtyMastery,
            MorphGemHieroglyphofMagicResistance,
            MorphGemHieroglyphofMagicItemTinkeringExpertise,
            MorphGemHieroglyphofManaConversionMastery,
            MorphGemIdeographofBattlemagesBlessing,
            MorphGemHieroglyphofInvulnerability,
            MorphGemHieroglyphofImpregnability,
            MorphGemRuneofPierceBane,
            MorphGemIdeographofPiercingProtection,
            MorphGemPictographofQuickness,
            MorphGemHieroglyphofSprint,
            MorphGemPictographofWillpower,
            MorphGemIdeographofBladeProtection,
            MorphGemRuneofSpiritDrinker,
            MorphGemIdeographofRevitalization,
            MorphGemPictographofStrength,
            MorphGemRuneofSwiftKiller,
            MorphGemHieroglyphofHeavyWeaponMastery,
            MorphGemHieroglyphofWarMagicMastery,
            MorphGemHieroglyphofWeaponTinkeringExpertise,
            MorphGemHieroglyphofDirtyFightingMastery,
            MorphGemHieroglyphofDualWieldMastery,
            MorphGemHieroglyphofRecklessnessMastery,
            MorphGemHieroglyphofShieldMastery,
            MorphGemHieroglyphofSneakAttackMastery,
            MorphGemHieroglyphofVoidMagicMastery,
            MorphGemHieroglyphofTwoHandedWeaponsMastery,
            MorphGemHieroglyphofSummoningMastery,
        };

        public static bool IsMorphGem(uint weenieId)
        {
            return MorphGems.Contains(weenieId);
        }

        #region readonly references

        public static readonly List<int> HeroicMasterSpells =
            new List<int>()
            {
                4733,    //Master Duelist's Coordination
                4737,    //Master Hero's Endurance
                4741,    //Master Sage's Focus
                4745,    //Master Rover's Quickness
                4749,    //Master Brute's Strength
                4753,    //Master Adherent's Willpower
                4755,    //Journeyman Survivor's Health
                4757,    //Journeyman Clairvoyant's Mana
                4759,    //Journeyman Tracker's Stamina
                4906,    //Apprentice Challenger's Rejuvenation
                6333,    //Gauntlet Damage Reduction II
                6335,    //Gauntlet Critical Damage Reduction II
                6340,    //Gauntlet Vitality III
                6337,    //Gauntlet Healing Boost II
                6331,    //Gauntlet Damage Boost II
                6329,    //Gauntlet Critical Damage Boost II
            };

        private static readonly HashSet<uint> morphGemsAllowedNonLootGen = new HashSet<uint>()
        {
            MorphGemRemoveLevelReq,
            MorphGemRemovePlayerReq,
            MorphGemRareUpgrade,
            MorphGemRareReduction,
            MorphGemBurden,
            MorphGemValue,
            MorphGemJewelersSawblade,
            MorphGemImpen,
        };

        #endregion readonly references

        public static void ApplyMorphGem(Player player, WorldObject source, WorldObject target)
        {
            try
            {
                //Only allow loot gen items to be morphed, except for gems that are allowed to be applied to quest / rare items
                if ((target.ItemWorkmanship == null ||
                    target.IsAttunedOrContainsAttuned ||
                    (target.ResistMagic == 9999 && !target.IsShield && !(target.ValidLocations?.HasFlag(EquipMask.Cloak) ?? false)))
                    && !morphGemsAllowedNonLootGen.Contains(source.WeenieClassId))
                {
                    player.SendUseDoneEvent(WeenieError.YouDoNotPassCraftingRequirements);
                    return;
                }

                string playerMsg = string.Empty;

                var targetItemSpells = target.Biota.GetKnownSpellsIds(target.BiotaDatabaseLock);

                switch (source.WeenieClassId)
                {
                    #region MorphGemValue
                    case MorphGemValue:

                        var currentItemValue = target.GetProperty(PropertyInt.Value);

                        if (!currentItemValue.HasValue)
                        {
                            player.SendUseDoneEvent(WeenieError.YouDoNotPassCraftingRequirements);
                            return;
                        }

                        if (currentItemValue.Value <= 20000)
                        {
                            player.Session.Network.EnqueueSend(new GameMessageSystemChat("Morph gems do not allow an item's Value to be reduced below 20k", ChatMessageType.Broadcast));
                            player.SendUseDoneEvent(WeenieError.YouDoNotPassCraftingRequirements);
                            return;
                        }

                        if (target.GetProperty(PropertyInt.RareId).HasValue)
                        {
                            player.Session.Network.EnqueueSend(new GameMessageSystemChat("This gem cannot be used on Rare armor or weapons.", ChatMessageType.Broadcast));
                            player.SendUseDoneEvent(WeenieError.YouDoNotPassCraftingRequirements);
                            return;
                        }

                        var valRandom = new Random();
                        bool valueGain = valRandom.Next(0, 99) < 10;
                        var percentChange = valRandom.Next(5, 16) / 100f;
                        var valueChange = (int)Math.Round(currentItemValue.Value * percentChange * (valueGain ? 1 : -1));
                        var newValue = currentItemValue.Value + valueChange;

                        if (newValue < 20000)
                        {
                            valueChange = 20000 - currentItemValue.Value;
                            newValue = 20000;
                        }

                        player.UpdateProperty(target, PropertyInt.Value, newValue);
                        AddMorphGemLog(target, MorphGemValue);

                        if (valueChange > 0)
                            playerMsg = $"Bad luck. The Morph Gem backfired. Your item's value has increased by {valueChange}";
                        else if (valueChange == 0)
                            playerMsg = $"The Morph Gem shatters against your item and leaves it unchanged. Could be worse.";
                        else
                            playerMsg = $"You apply the Morph Gem skillfully and have reduced the value of your item by {-1 * valueChange}";

                        player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                        break;

                    #endregion MorphGemValue

                    #region MorphGemRandomWorkmanship
                    case MorphGemRandomWorkmanship:

                        var currentItemWork = target.GetProperty(PropertyInt.ItemWorkmanship);

                        if (!currentItemWork.HasValue)
                        {
                            player.SendUseDoneEvent(WeenieError.YouDoNotPassCraftingRequirements);
                            return;
                        }

                        var workRandom = ThreadSafeRandom.Next(1, 10);
                        var workChange = (currentItemWork.Value - workRandom);

                        player.UpdateProperty(target, PropertyInt.ItemWorkmanship, workRandom);
                        AddMorphGemLog(target, MorphGemRandomWorkmanship);

                        if (workChange < 0)
                            playerMsg = $"The Morph Gem backfired. Your item's workmanship has increased by {-workChange}";
                        else if (workChange == 0)
                            playerMsg = $"The Morph Gem shatters against your {target.NameWithMaterial} and leaves it unchanged. Could be worse.";
                        else
                            playerMsg = $"You apply the Morph Gem skillfully and have reduced the workmanship of your {target.NameWithMaterial} by {workChange}";

                        player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                        break;

                    #endregion MorphGemRandomWorkmanship

                    #region MorphGemArcane
                    case MorphGemArcane:

                        var currentItemArcane = target.GetProperty(PropertyInt.ItemDifficulty);

                        if (!currentItemArcane.HasValue)
                        {
                            player.SendUseDoneEvent(WeenieError.YouDoNotPassCraftingRequirements);
                            return;
                        }

                        var arcaneRoll = ThreadSafeRandom.Next(0, 99);
                        var arcaneChange = arcaneRoll > 9 ? -25 : 50;
                        var newArcane = currentItemArcane.Value + arcaneChange;

                        if (newArcane < 1)
                        {
                            newArcane = 1;
                            arcaneChange = currentItemArcane.Value < 1 ? 0 : 1 - currentItemArcane.Value;
                        }

                        player.UpdateProperty(target, PropertyInt.ItemDifficulty, newArcane);
                        AddMorphGemLog(target, MorphGemArcane);

                        if (arcaneChange > 0)
                            playerMsg = $"The Morph Gem shatters against your {target.NameWithMaterial} and its arcane requirement has increased by {arcaneChange}";
                        else if (arcaneChange == 0)
                            playerMsg = $"The Morph Gem shatters against your {target.NameWithMaterial} and leaves it unchanged. Could be worse.";
                        else
                            playerMsg = $"You apply the Morph Gem skillfully and have reduced the arcane requirement of your item by {-1 * arcaneChange}";

                        player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                        break;

                    #endregion MorphGemArcane

                    #region MorphGemRemoveMissileDReq
                    case MorphGemRemoveMissileDReq:

                        if (target.ItemSkillLimit != Skill.MissileDefense || target.ItemSkillLevelLimit == null)
                        {
                            player.SendUseDoneEvent(WeenieError.YouDoNotPassCraftingRequirements);
                            return;
                        }

                        target.ItemSkillLimit = null;
                        target.ItemSkillLevelLimit = null;

                        playerMsg = $"You apply the Morph Gem skillfully and have removed the Missile Defense activation requirement of your item.";
                        AddMorphGemLog(target, MorphGemRemoveMissileDReq);

                        player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                        break;

                    #endregion MorphGemRemoveMissileDReq

                    #region MorphGemRemoveMeleeDReq
                    case MorphGemRemoveMeleeDReq:

                        if (target.ItemSkillLimit != Skill.MeleeDefense || target.ItemSkillLevelLimit == null)
                        {
                            player.SendUseDoneEvent(WeenieError.YouDoNotPassCraftingRequirements);
                            return;
                        }

                        target.ItemSkillLimit = null;
                        target.ItemSkillLevelLimit = null;

                        playerMsg = $"You apply the Morph Gem skillfully and have removed the Melee Defense activation requirement of your item.";
                        AddMorphGemLog(target, MorphGemRemoveMeleeDReq);

                        player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                        break;

                    #endregion MorphGemRemoveMeleeDReq

                    #region MorphGemRandomizeWeaponImbue
                    case MorphGemRandomizeWeaponImbue:

                        var isValid = false;
                        var hasFetish = target.HasImbuedEffect(ImbuedEffectType.IgnoreSomeMagicProjectileDamage);

                        if (target.HasImbuedEffect(ImbuedEffectType.CripplingBlow) ||
                            target.HasImbuedEffect(ImbuedEffectType.ArmorRending) ||
                            target.HasImbuedEffect(ImbuedEffectType.CriticalStrike))
                        {
                            isValid = true;
                        }

                        if (!isValid)
                        {
                            player.SendUseDoneEvent(WeenieError.YouDoNotPassCraftingRequirements);
                            return;
                        }

                        var origImbueEffect = target.ImbuedEffect;
                        var roll = ThreadSafeRandom.Next(0, 1);

                        if (target.HasImbuedEffect(ImbuedEffectType.CripplingBlow))
                            target.ImbuedEffect = roll == 0 ? ImbuedEffectType.ArmorRending : ImbuedEffectType.CriticalStrike;
                        else if (target.HasImbuedEffect(ImbuedEffectType.ArmorRending))
                            target.ImbuedEffect = roll == 0 ? ImbuedEffectType.CripplingBlow : ImbuedEffectType.CriticalStrike;
                        else if (target.HasImbuedEffect(ImbuedEffectType.CriticalStrike))
                            target.ImbuedEffect = roll == 0 ? ImbuedEffectType.ArmorRending : ImbuedEffectType.CripplingBlow;

                        target.IconUnderlayId = RecipeManager.IconUnderlay[target.ImbuedEffect];

                        if (hasFetish)
                            target.ImbuedEffect |= ImbuedEffectType.IgnoreSomeMagicProjectileDamage;

                        playerMsg = $"You apply the Morph Gem skillfully and have changed your weapon's imbue from {origImbueEffect} to {target.ImbuedEffect}";
                        AddMorphGemLog(target, MorphGemRandomizeWeaponImbue);

                        player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                        break;

                    #endregion MorphGemRandomizeWeaponImbue

                    #region MorphGemRemovePlayerReq
                    case MorphGemRemovePlayerReq:

                        if (!target.GetProperty(PropertyInstanceId.AllowedWielder).HasValue)
                        {
                            player.SendUseDoneEvent(WeenieError.YouDoNotPassCraftingRequirements);
                            return;
                        }

                        var origWielder = target.GetProperty(PropertyString.CraftsmanName);

                        target.RemoveProperty(PropertyInstanceId.AllowedWielder);
                        target.RemoveProperty(PropertyString.CraftsmanName);

                        playerMsg = $"You apply the Morph Gem skillfully and have altered your item so it is no longer wield restricted to {origWielder}";
                        AddMorphGemLog(target, MorphGemRemovePlayerReq);

                        player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                        break;

                    #endregion MorphGemRemovePlayerReq

                    #region MorphGemRemoveLevelReq
                    case MorphGemRemoveLevelReq:

                        if (!target.GetProperty(PropertyInt.WieldDifficulty).HasValue ||
                            !target.GetProperty(PropertyInt.WieldRequirements).HasValue ||
                            target.GetProperty(PropertyInt.WieldRequirements) != 7)
                        {
                            playerMsg = "The gem can only be applied to items that have a Level requirement";
                            player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                            player.SendUseDoneEvent(WeenieError.YouDoNotPassCraftingRequirements);
                            return;
                        }

                        if (target is MeleeWeapon || target is Caster || target is MissileLauncher)
                        {
                            playerMsg = "The gem can not be applied to weapons";
                            player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                            player.SendUseDoneEvent(WeenieError.YouDoNotPassCraftingRequirements);
                            return;
                        }

                        var origLevelReq = target.GetProperty(PropertyInt.WieldDifficulty);
                        target.RemoveProperty(PropertyInt.WieldDifficulty);

                        playerMsg = $"You apply the Morph Gem skillfully and have altered your item so it no longer requires level {origLevelReq} to wield";

                        player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                        AddMorphGemLog(target, MorphGemRemoveLevelReq);
                        break;

                    #endregion MorphGemRemoveLevelReq

                    #region MorphGemSlayerUpgrade
                    case MorphGemSlayerUpgrade:

                        var tinkerLottoLog2 = target.GetProperty(PropertyString.TinkerLottoLog);
                        if (!String.IsNullOrEmpty(tinkerLottoLog2) && tinkerLottoLog2.Contains("Slayer") && target.SlayerCreatureType != null)
                        {
                            if (target.SlayerDamageBonus < 1.8)
                            {
                                playerMsg = $"The Morph Gem alters your weapon's slayer damage bonus to 1.8";
                                target.SlayerDamageBonus = 1.8;
                                player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                            }
                            else
                            {
                                playerMsg = $"Your weapon's slayer damage bonus is already >= 1.8";
                                player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                                player.SendUseDoneEvent(WeenieError.YouDoNotPassCraftingRequirements);
                                return;
                            }
                        }
                        else
                        {
                            playerMsg = "The gem can only be applied to weapons that hit the tinkering lottery to add a slayer";
                            player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                            player.SendUseDoneEvent(WeenieError.YouDoNotPassCraftingRequirements);
                            return;
                        }

                        AddMorphGemLog(target, MorphGemSlayerUpgrade);
                        break;

                    #endregion MorphGemSlayerUpgrade

                    #region MorphGemBurningCoal
                    case MorphGemBurningCoal:

                        if (!(target.ItemType == ItemType.Armor || target.ItemType == ItemType.Jewelry || target.ItemType == ItemType.Clothing))
                        {
                            playerMsg = "The gem can only be applied to armor, clothing or jewelry";
                            player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                            player.SendUseDoneEvent(WeenieError.YouDoNotPassCraftingRequirements);
                            return;
                        }

                        if (targetItemSpells == null || targetItemSpells.Count < 1)
                        {
                            playerMsg = "The gem can only be applied to magical items";
                            player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                            player.SendUseDoneEvent(WeenieError.YouDoNotPassCraftingRequirements);
                            return;
                        }
                        else if (targetItemSpells.Contains(3204))
                        {
                            playerMsg = "Your target item already has Blazing Heart on it, you cannot add it twice";
                            player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                            player.SendUseDoneEvent(WeenieError.YouDoNotPassCraftingRequirements);
                            return;
                        }

                        target.Biota.GetOrAddKnownSpell(3204, target.BiotaDatabaseLock, out _);
                        playerMsg = $"With a steady hand and pure heart, you skillfully apply the morph gem to your {target.NameWithMaterial} and have successfully added the spell Blazing Heart";
                        player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                        AddMorphGemLog(target, MorphGemBurningCoal);
                        break;

                    #endregion MorphGemBurningCoal

                    #region MorphGemImpen
                    case MorphGemImpen:

                        if (target.WeenieType != WeenieType.Clothing)
                        {
                            playerMsg = "The gem can only be applied to armor and underclothes";
                            player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                            player.SendUseDoneEvent(WeenieError.YouDoNotPassCraftingRequirements);
                            return;
                        }

                        if (target.ArmorLevel > 0 && target.ItemWorkmanship == null && !target.GetProperty(PropertyInt.RareId).HasValue)
                        {
                            playerMsg = "The gem cannot be applied quest armor, only loot gen or rare armor";
                            player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                            player.SendUseDoneEvent(WeenieError.YouDoNotPassCraftingRequirements);
                            return;
                        }

                        if (!target.ItemMaxMana.HasValue || targetItemSpells == null || targetItemSpells.Count == 0)
                        {
                            playerMsg = "The gem can only be applied to magical items";
                            player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                            player.SendUseDoneEvent(WeenieError.YouDoNotPassCraftingRequirements);
                            return;
                        }

                        if (targetItemSpells.Contains(2604) ||
                            targetItemSpells.Contains(2592) ||
                            targetItemSpells.Contains(4667) ||
                            targetItemSpells.Contains(6095) ||
                            targetItemSpells.Contains(3710))
                        {
                            playerMsg = "The gem cannot be used on an item that already has an Impenetrability cantrip";
                            player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                            player.SendUseDoneEvent(WeenieError.YouDoNotPassCraftingRequirements);
                            return;
                        }

                        var success = true;

                        if (success)
                        {
                            playerMsg = "You successfully apply the morph gem and have added {0} Impenetrability cantrip to your {1}";

                            var spellId = 0;
                            var impenLevel = ThreadSafeRandom.Next(0, 99);
                            if (impenLevel < 40)
                            {
                                spellId = 2604;
                                playerMsg = String.Format(playerMsg, "a Minor", target.Name);
                            }
                            else if (impenLevel < 70)
                            {
                                spellId = 2592;
                                playerMsg = String.Format(playerMsg, "a Major", target.Name);
                            }
                            else if (impenLevel < 97)
                            {
                                spellId = 4667;
                                playerMsg = String.Format(playerMsg, "an Epic", target.Name);
                            }
                            else
                            {
                                spellId = 6095;
                                playerMsg = String.Format(playerMsg, "a Legendary", target.Name);
                            }

                            player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                            target.Biota.GetOrAddKnownSpell(spellId, target.BiotaDatabaseLock, out _);
                            AddMorphGemLog(target, MorphGemImpen);
                        }
                        else
                        {
                            if (target.OwnerId == player.Guid.Full || player.GetInventoryItem(target.Guid) != null)
                            {
                                if (!player.TryConsumeFromInventoryWithNetworking(target))
                                    log.Warn($"MorphGem.ApplyMorphGem failed to consume target item for Impen. Player = {player.Name}, Target = {target.Name}");
                            }
                            else if (target.WielderId == player.Guid.Full)
                            {
                                if (!player.TryDequipObjectWithNetworking(target.Guid, out _, Player.DequipObjectAction.ConsumeItem))
                                    log.Warn($"MorphGem.ApplyMorphGem failed to consume target item for Impen. Player = {player.Name}, Target = {target.Name}");
                            }
                            else
                            {
                                target.Destroy();
                            }

                            var destroyMessage = new GameMessageSystemChat($"The morph gem fails to apply and has destroyed your {target.Name}", ChatMessageType.Craft);
                            player.Session.Network.EnqueueSend(destroyMessage);
                        }
                        break;

                    #endregion MorphGemImpen

                    #region MorphGemBanditHilt
                    case MorphGemBanditHilt:

                        if (target.WeenieType != WeenieType.MeleeWeapon ||
                            target.WeaponSkill != Skill.LightWeapons ||
                            (!target.W_AttackType.HasFlag(AttackType.DoubleSlash) && !target.W_AttackType.HasFlag(AttackType.DoubleThrust)))
                        {
                            playerMsg = "This gem can only be used on Light Weapon melee weapons with the Multi-Strike property";
                            player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                            player.SendUseDoneEvent(WeenieError.YouDoNotPassCraftingRequirements);
                            return;
                        }

                        target.W_AttackType = AttackType.TripleStrike;
                        playerMsg = $"The morph gem alters your {target.NameWithMaterial} into a Triple-Strike weapon";
                        player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                        AddMorphGemLog(target, MorphGemBanditHilt);
                        break;

                    #endregion MorphGemBanditHilt

                    #region MorphGemRareUpgrade
                    case MorphGemRareUpgrade:

                        if (!target.GetProperty(PropertyInt.RareId).HasValue)
                        {
                            playerMsg = "This gem can only be used on rare armor, jewelry and weapons";
                            player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                            player.SendUseDoneEvent(WeenieError.YouDoNotPassCraftingRequirements);
                            return;
                        }

                        if (target.WeenieType != WeenieType.Clothing &&
                            target.WeenieType != WeenieType.Caster &&
                            target.WeenieType != WeenieType.MeleeWeapon &&
                            target.WeenieType != WeenieType.MissileLauncher &&
                            target.ItemType != ItemType.Jewelry &&
                            !target.IsShield)
                        {
                            playerMsg = "This gem can only be used on rare armor, jewelry and weapons";
                            player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                            player.SendUseDoneEvent(WeenieError.YouDoNotPassCraftingRequirements);
                            return;
                        }

                        var itemEpicList = target.EpicCantrips.Keys;
                        if (itemEpicList == null || itemEpicList.Count < 1)
                        {
                            player.SendUseDoneEvent(WeenieError.YouDoNotPassCraftingRequirements);
                            player.Session.Network.EnqueueSend(new GameMessageSystemChat("The target item has no epic cantrips to upgrade", ChatMessageType.Broadcast));
                            return;
                        }

                        foreach (var epicSpellId in itemEpicList)
                        {
                            var level1SpellId = SpellLevelProgression.GetLevel1SpellId((SpellId)epicSpellId);
                            var progression = SpellLevelProgression.GetSpellLevels(level1SpellId);
                            if (progression != null && progression.Count >= 4)
                            {
                                var legendarySpellId = progression[3];
                                target.Biota.TryRemoveKnownSpell(epicSpellId, target.BiotaDatabaseLock);
                                target.Biota.GetOrAddKnownSpell((int)legendarySpellId, target.BiotaDatabaseLock, out _);
                            }
                        }

                        player.Session.Network.EnqueueSend(new GameMessageSystemChat($"Your {target.NameWithMaterial} has had its epic armor cantrips upgraded to legendaries", ChatMessageType.Broadcast));
                        AddMorphGemLog(target, MorphGemRareUpgrade);
                        break;

                    #endregion MorphGemRareUpgrade

                    #region MorphGemRareReduction
                    case MorphGemRareReduction:

                        if (!target.ArmorLevel.HasValue || target.ArmorLevel.Value < 1 || !target.GetProperty(PropertyInt.RareId).HasValue)
                        {
                            playerMsg = "This gem can only be used on multi-slot rare armor";
                            player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                            player.SendUseDoneEvent(WeenieError.YouDoNotPassCraftingRequirements);
                            return;
                        }

                        EquipMask targetValidLocations = target.ValidLocations ?? EquipMask.None;

                        if (targetValidLocations.HasFlag(EquipMask.ChestArmor))
                        {
                            playerMsg = $"You successfully apply the {source.Name} to reduce your {target.NameWithMaterial} to cover only your chest.";
                            player.UpdateProperty(target, PropertyInt.ValidLocations, (int)EquipMask.ChestArmor);
                            player.UpdateProperty(target, PropertyInt.ClothingPriority, (int)CoverageMask.OuterwearChest);
                        }
                        else if (targetValidLocations.HasFlag(EquipMask.UpperLegArmor))
                        {
                            playerMsg = $"You successfully apply the {source.Name} to reduce your {target.NameWithMaterial} to cover only your upper legs.";
                            player.UpdateProperty(target, PropertyInt.ValidLocations, (int)EquipMask.UpperLegArmor);
                            player.UpdateProperty(target, PropertyInt.ClothingPriority, (int)CoverageMask.OuterwearUpperLegs);
                        }
                        else
                        {
                            playerMsg = "This gem can only be used on multi-slot rare armor";
                            player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                            player.SendUseDoneEvent(WeenieError.YouDoNotPassCraftingRequirements);
                            return;
                        }

                        player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                        AddMorphGemLog(target, MorphGemRareReduction);
                        break;

                    #endregion MorphGemRareReduction

                    #region MorphGemJewelersSawblade
                    case MorphGemJewelersSawblade:

                        EquipMask validLocations = target.ValidLocations ?? EquipMask.None;
                        int newLocRoll = ThreadSafeRandom.Next(0, 1);

                        if (validLocations.HasFlag(EquipMask.NeckWear))
                        {
                            if (newLocRoll == 0)
                            {
                                player.UpdateProperty(target, PropertyInt.ValidLocations, (int)EquipMask.WristWear);
                                playerMsg = $"You have successfully used the {source.Name} to alter your {target.NameWithMaterial} to be wearable on your wrists!";
                                player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                            }
                            else
                            {
                                player.UpdateProperty(target, PropertyInt.ValidLocations, (int)EquipMask.FingerWear);
                                playerMsg = $"You have successfully used the {source.Name} to alter your {target.NameWithMaterial} to be wearable on your fingers!";
                                player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                            }
                        }
                        else if (validLocations.HasFlag(EquipMask.FingerWearLeft) || validLocations.HasFlag(EquipMask.FingerWearRight))
                        {
                            if (newLocRoll == 0)
                            {
                                player.UpdateProperty(target, PropertyInt.ValidLocations, (int)EquipMask.WristWear);
                                playerMsg = $"You have successfully used the {source.Name} to alter your {target.NameWithMaterial} to be wearable on your wrists!";
                                player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                            }
                            else
                            {
                                player.UpdateProperty(target, PropertyInt.ValidLocations, (int)EquipMask.NeckWear);
                                playerMsg = $"You have successfully used the {source.Name} to alter your {target.NameWithMaterial} to be wearable on your neck!";
                                player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                            }
                        }
                        else if (validLocations.HasFlag(EquipMask.WristWearLeft) || validLocations.HasFlag(EquipMask.WristWearRight))
                        {
                            if (newLocRoll == 0)
                            {
                                player.UpdateProperty(target, PropertyInt.ValidLocations, (int)EquipMask.FingerWear);
                                playerMsg = $"You have successfully used the {source.Name} to alter your {target.NameWithMaterial} to be wearable on your fingers!";
                                player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                            }
                            else
                            {
                                player.UpdateProperty(target, PropertyInt.ValidLocations, (int)EquipMask.NeckWear);
                                playerMsg = $"You have successfully used the {source.Name} to alter your {target.NameWithMaterial} to be wearable on your neck!";
                                player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                            }
                        }
                        else
                        {
                            playerMsg = "This gem can only be used on necklaces, rings and bracelets";
                            player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                            player.SendUseDoneEvent(WeenieError.YouDoNotPassCraftingRequirements);
                            return;
                        }

                        AddMorphGemLog(target, MorphGemJewelersSawblade);
                        break;

                    #endregion MorphGemJewelersSawblade

                    #region MorphGemAddSlayer
                    case MorphGemAddSlayer:

                        if (target as MeleeWeapon == null &&
                            !target.IsCaster &&
                            !target.IsBow &&
                            !target.IsThrownWeapon)
                        {
                            playerMsg = "This gem can only be used on weapons or magic casters";
                            player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                            player.SendUseDoneEvent(WeenieError.YouDoNotPassCraftingRequirements);
                            return;
                        }

                        if (target.SlayerCreatureType != null &&
                            target.SlayerCreatureType > 0 &&
                            target.SlayerDamageBonus > 1)
                        {
                            playerMsg = "This gem cant be used on a weapon or magic caster that already has a slayer on it";
                            player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                            player.SendUseDoneEvent(WeenieError.YouDoNotPassCraftingRequirements);
                            return;
                        }

                        target.ApplyRandomSlayer(1.8);
                        target.HandleTinkerLottoLog($"MorphGemSlayer");
                        playerMsg = $"You have successfully used the {source.Name} to add {target.SlayerCreatureType?.ToString() ?? "Unknown"} Slayer to your {target.NameWithMaterial}!";
                        player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                        AddMorphGemLog(target, MorphGemAddSlayer);
                        break;

                    #endregion MorphGemAddSlayer

                    #region MorphGemHematite
                    case MorphGemHematite:

                        if (!(target.ItemType == ItemType.Armor || target.ItemType == ItemType.Jewelry || target.ItemType == ItemType.Clothing))
                        {
                            playerMsg = "The gem can only be applied to armor, clothing or jewelry";
                            player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                            player.SendUseDoneEvent(WeenieError.YouDoNotPassCraftingRequirements);
                            return;
                        }

                        if (targetItemSpells == null || targetItemSpells.Count < 1)
                        {
                            playerMsg = "The gem can only be applied to magical items";
                            player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                            player.SendUseDoneEvent(WeenieError.YouDoNotPassCraftingRequirements);
                            return;
                        }
                        else if (targetItemSpells.Contains(2004))
                        {
                            playerMsg = "Your target item already has Warrior's Vitality on it, you cannot add it twice";
                            player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                            player.SendUseDoneEvent(WeenieError.YouDoNotPassCraftingRequirements);
                            return;
                        }

                        target.Biota.GetOrAddKnownSpell(2004, target.BiotaDatabaseLock, out _);
                        playerMsg = $"With a steady hand you skillfully apply the morph gem to your {target.NameWithMaterial} and have successfully added the spell Warrior's Vitality";
                        player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                        AddMorphGemLog(target, MorphGemHematite);
                        break;

                    #endregion MorphGemHematite

                    #region MorphGemStrengthbeer
                    case MorphGemStrengthbeer:

                        if (!(target.ItemType == ItemType.Armor || target.ItemType == ItemType.Jewelry || target.ItemType == ItemType.Clothing))
                        {
                            playerMsg = "The gem can only be applied to armor, clothing or jewelry";
                            player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                            player.SendUseDoneEvent(WeenieError.YouDoNotPassCraftingRequirements);
                            return;
                        }

                        if (targetItemSpells == null || targetItemSpells.Count < 1)
                        {
                            playerMsg = "The gem can only be applied to magical items";
                            player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                            player.SendUseDoneEvent(WeenieError.YouDoNotPassCraftingRequirements);
                            return;
                        }
                        else if (targetItemSpells.Contains(3864))
                        {
                            playerMsg = "Your target item already has Zongo's Fist on it, you cannot add it twice";
                            player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                            player.SendUseDoneEvent(WeenieError.YouDoNotPassCraftingRequirements);
                            return;
                        }

                        target.Biota.GetOrAddKnownSpell(3864, target.BiotaDatabaseLock, out _);
                        playerMsg = $"With a steady hand you skillfully apply the morph gem to your {target.NameWithMaterial} and have successfully added the spell Zongo's Fist";
                        player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                        AddMorphGemLog(target, MorphGemStrengthbeer);
                        break;

                    #endregion MorphGemStrengthbeer

                    #region MorphGemEndurancebeer
                    case MorphGemEndurancebeer:

                        if (!(target.ItemType == ItemType.Armor || target.ItemType == ItemType.Jewelry || target.ItemType == ItemType.Clothing))
                        {
                            playerMsg = "The gem can only be applied to armor, clothing or jewelry";
                            player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                            player.SendUseDoneEvent(WeenieError.YouDoNotPassCraftingRequirements);
                            return;
                        }

                        if (targetItemSpells == null || targetItemSpells.Count < 1)
                        {
                            playerMsg = "The gem can only be applied to magical items";
                            player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                            player.SendUseDoneEvent(WeenieError.YouDoNotPassCraftingRequirements);
                            return;
                        }
                        else if (targetItemSpells.Contains(3863))
                        {
                            playerMsg = "Your target item already has Hunter's Hardiness on it, you cannot add it twice";
                            player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                            player.SendUseDoneEvent(WeenieError.YouDoNotPassCraftingRequirements);
                            return;
                        }

                        target.Biota.GetOrAddKnownSpell(3863, target.BiotaDatabaseLock, out _);
                        playerMsg = $"With a steady hand you skillfully apply the morph gem to your {target.NameWithMaterial} and have successfully added the spell Hunter's Hardiness";
                        player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                        AddMorphGemLog(target, MorphGemEndurancebeer);
                        break;

                    #endregion MorphGemEndurancebeer

                    #region MorphGemCoordinationbeer
                    case MorphGemCoordinationbeer:

                        if (!(target.ItemType == ItemType.Armor || target.ItemType == ItemType.Jewelry || target.ItemType == ItemType.Clothing))
                        {
                            playerMsg = "The gem can only be applied to armor, clothing or jewelry";
                            player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                            player.SendUseDoneEvent(WeenieError.YouDoNotPassCraftingRequirements);
                            return;
                        }

                        if (targetItemSpells == null || targetItemSpells.Count < 1)
                        {
                            playerMsg = "The gem can only be applied to magical items";
                            player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                            player.SendUseDoneEvent(WeenieError.YouDoNotPassCraftingRequirements);
                            return;
                        }
                        else if (targetItemSpells.Contains(3533))
                        {
                            playerMsg = "Your target item already has Brighteyes' Favor on it, you cannot add it twice";
                            player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                            player.SendUseDoneEvent(WeenieError.YouDoNotPassCraftingRequirements);
                            return;
                        }

                        target.Biota.GetOrAddKnownSpell(3533, target.BiotaDatabaseLock, out _);
                        playerMsg = $"With a steady hand you skillfully apply the morph gem to your {target.NameWithMaterial} and have successfully added the spell Brighteyes' Favor";
                        player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                        AddMorphGemLog(target, MorphGemCoordinationbeer);
                        break;

                    #endregion MorphGemCoordinationbeer

                    #region MorphGemQuicknessbeer
                    case MorphGemQuicknessbeer:

                        if (!(target.ItemType == ItemType.Armor || target.ItemType == ItemType.Jewelry || target.ItemType == ItemType.Clothing))
                        {
                            playerMsg = "The gem can only be applied to armor, clothing or jewelry";
                            player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                            player.SendUseDoneEvent(WeenieError.YouDoNotPassCraftingRequirements);
                            return;
                        }

                        if (targetItemSpells == null || targetItemSpells.Count < 1)
                        {
                            playerMsg = "The gem can only be applied to magical items";
                            player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                            player.SendUseDoneEvent(WeenieError.YouDoNotPassCraftingRequirements);
                            return;
                        }
                        else if (targetItemSpells.Contains(3531))
                        {
                            playerMsg = "Your target item already has Bobo's Quickening on it, you cannot add it twice";
                            player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                            player.SendUseDoneEvent(WeenieError.YouDoNotPassCraftingRequirements);
                            return;
                        }

                        target.Biota.GetOrAddKnownSpell(3531, target.BiotaDatabaseLock, out _);
                        playerMsg = $"With a steady hand you skillfully apply the morph gem to your {target.NameWithMaterial} and have successfully added the spell Bobo's Quickening";
                        player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                        AddMorphGemLog(target, MorphGemQuicknessbeer);
                        break;

                    #endregion MorphGemQuicknessbeer

                    #region MorphGemFocusbeer
                    case MorphGemFocusbeer:

                        if (!(target.ItemType == ItemType.Armor || target.ItemType == ItemType.Jewelry || target.ItemType == ItemType.Clothing))
                        {
                            playerMsg = "The gem can only be applied to armor, clothing or jewelry";
                            player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                            player.SendUseDoneEvent(WeenieError.YouDoNotPassCraftingRequirements);
                            return;
                        }

                        if (targetItemSpells == null || targetItemSpells.Count < 1)
                        {
                            playerMsg = "The gem can only be applied to magical items";
                            player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                            player.SendUseDoneEvent(WeenieError.YouDoNotPassCraftingRequirements);
                            return;
                        }
                        else if (targetItemSpells.Contains(3530))
                        {
                            playerMsg = "Your target item already has Ketnan's Eye on it, you cannot add it twice";
                            player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                            player.SendUseDoneEvent(WeenieError.YouDoNotPassCraftingRequirements);
                            return;
                        }

                        target.Biota.GetOrAddKnownSpell(3530, target.BiotaDatabaseLock, out _);
                        playerMsg = $"With a steady hand you skillfully apply the morph gem to your {target.NameWithMaterial} and have successfully added the spell Ketnan's Eye";
                        player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                        AddMorphGemLog(target, MorphGemFocusbeer);
                        break;

                    #endregion MorphGemFocusbeer

                    #region MorphGemWillpowerbeer
                    case MorphGemWillpowerbeer:

                        if (!(target.ItemType == ItemType.Armor || target.ItemType == ItemType.Jewelry || target.ItemType == ItemType.Clothing))
                        {
                            playerMsg = "The gem can only be applied to armor, clothing or jewelry";
                            player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                            player.SendUseDoneEvent(WeenieError.YouDoNotPassCraftingRequirements);
                            return;
                        }

                        if (targetItemSpells == null || targetItemSpells.Count < 1)
                        {
                            playerMsg = "The gem can only be applied to magical items";
                            player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                            player.SendUseDoneEvent(WeenieError.YouDoNotPassCraftingRequirements);
                            return;
                        }
                        else if (targetItemSpells.Contains(3862))
                        {
                            playerMsg = "Your target item already has Duke Raoul's Pride on it, you cannot add it twice";
                            player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                            player.SendUseDoneEvent(WeenieError.YouDoNotPassCraftingRequirements);
                            return;
                        }

                        target.Biota.GetOrAddKnownSpell(3862, target.BiotaDatabaseLock, out _);
                        playerMsg = $"With a steady hand you skillfully apply the morph gem to your {target.NameWithMaterial} and have successfully added the spell Duke Raoul's Pride";
                        player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                        AddMorphGemLog(target, MorphGemWillpowerbeer);
                        break;

                    #endregion MorphGemWillpowerbeer

                    #region MorphGemHeroicMaster
                    case MorphGemHeroicMaster:

                        if (target.ItemType != ItemType.Jewelry)
                        {
                            playerMsg = $"{source.Name} can only be applied to jewelry.";
                            player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                            player.SendUseDoneEvent(WeenieError.YouDoNotPassCraftingRequirements);
                            return;
                        }

                        if (GetMorphGemLogCount(target, MorphGemHeroicMaster) > 0)
                        {
                            playerMsg = $"{source.Name} can only be applied once and has already been applied to your target item.";
                            player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                            player.SendUseDoneEvent(WeenieError.YouDoNotPassCraftingRequirements);
                            return;
                        }

                        int spellRoll = ThreadSafeRandom.Next(0, 100);
                        int spellCount = 1;
                        if (spellRoll > 66) spellCount = 2;
                        if (spellRoll > 96) spellCount = 3;

                        var spellList = HeroicMasterSpells.OrderBy(x => Guid.NewGuid()).Take(spellCount);

                        var spellNames = new List<string>();
                        foreach (var heroicSpellId in spellList)
                        {
                            target.Biota.GetOrAddKnownSpell(heroicSpellId, target.BiotaDatabaseLock, out _);
                            spellNames.Add(new Spell(heroicSpellId).Name);
                        }

                        playerMsg = $"With a steady hand you skillfully apply the {source.Name} to your {target.NameWithMaterial} and have successfully added the following spells\n{String.Join('\n', spellNames)}";
                        player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                        AddMorphGemLog(target, MorphGemHeroicMaster);
                        break;

                    #endregion MorphGemHeroicMaster

                    #region MorphGemRandomCantrip
                    case MorphGemRandomCantrip:

                        if ((target.ItemType != ItemType.Jewelry &&
                            target.ItemType != ItemType.Armor &&
                            target.ItemType != ItemType.Clothing &&
                            !target.IsShield)
                            || (target.ValidLocations?.HasFlag(EquipMask.Cloak) ?? false))
                        {
                            player.SendUseDoneEvent(WeenieError.YouDoNotPassCraftingRequirements);
                            player.Session.Network.EnqueueSend(new GameMessageSystemChat($"The {source.Name} can only be applied to armor, jewelry or underclothes", ChatMessageType.Broadcast));
                            return;
                        }

                        if (GetMorphGemLogCount(target, MorphGemRandomCantrip) > 0)
                        {
                            playerMsg = $"{source.Name} can only be applied once and has already been applied to your target item.";
                            player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                            player.SendUseDoneEvent(WeenieError.YouDoNotPassCraftingRequirements);
                            return;
                        }

                        var itemLegendaries = target.LegendaryCantrips;
                        if (itemLegendaries == null || itemLegendaries.Count < 1)
                        {
                            player.SendUseDoneEvent(WeenieError.YouDoNotPassCraftingRequirements);
                            player.Session.Network.EnqueueSend(new GameMessageSystemChat("The target item has no legendary cantrips to randomize", ChatMessageType.Broadcast));
                            return;
                        }

                        List<int> newLegendaryList = new List<int>();
                        foreach (var currLegendary in itemLegendaries)
                        {
                            var counter = 0;
                            while (counter < 20)
                            {
                                SpellId newCantrip = ArmorCantrips.Roll(target.IsShield);

                                if (target.ItemType == ItemType.Jewelry)
                                    newCantrip = JewelryCantrips.Roll();

                                List<SpellId> progression = SpellLevelProgression.GetSpellLevels(newCantrip);

                                if (progression != null && progression.Count >= 4)
                                {
                                    int newLegendarySpellId = (int)progression[3];
                                    if (newLegendarySpellId != currLegendary.Key && !newLegendaryList.Contains(newLegendarySpellId))
                                    {
                                        newLegendaryList.Add(newLegendarySpellId);
                                        break;
                                    }
                                }
                                counter++;
                            }
                        }

                        if (newLegendaryList.Count > 1)
                        {
                            var legRandom = new Random();
                            var legRandomRoll = legRandom.Next(0, int.MaxValue);
                            if (legRandomRoll % 15 == 0 && newLegendaryList.Count > 0)
                                newLegendaryList.RemoveAt(0);
                        }

                        if (newLegendaryList.Count < 4)
                        {
                            var legRandom = new Random();
                            var legRandomRoll = legRandom.Next(0, int.MaxValue);
                            if (legRandomRoll % 10 == 0 && newLegendaryList.Count > 0)
                            {
                                while (true)
                                {
                                    SpellId newCantrip = ArmorCantrips.Roll(target.IsShield);
                                    List<SpellId> progression = SpellLevelProgression.GetSpellLevels(newCantrip);
                                    if (progression != null && progression.Count >= 4)
                                    {
                                        int newLegendarySpellId = (int)progression[3];
                                        if (!newLegendaryList.Contains(newLegendarySpellId))
                                        {
                                            newLegendaryList.Add(newLegendarySpellId);
                                            break;
                                        }
                                    }
                                }
                            }
                        }

                        bool cantripImpenSuccess = false;
                        if (target.ItemType != ItemType.Jewelry)
                        {
                            var impenRandom = new Random();
                            var impenRoll = impenRandom.Next(0, int.MaxValue);
                            if (impenRoll % 7 == 0 && !newLegendaryList.Contains(6095))
                            {
                                if (newLegendaryList.Count < 4)
                                    newLegendaryList.Add(6095);
                                else
                                    newLegendaryList[0] = 6095;

                                cantripImpenSuccess = true;
                            }
                        }

                        string removedSpellList = "";
                        int removedLegNum = 0;
                        foreach (var spell in itemLegendaries)
                        {
                            target.Biota.TryRemoveKnownSpell(spell.Key, target.BiotaDatabaseLock);
                            removedLegNum++;
                            if (removedLegNum == 1)
                                removedSpellList = $"{new Spell(spell.Key, true).Name}";
                            else if (removedLegNum == itemLegendaries.Count)
                                removedSpellList += $" and {new Spell(spell.Key, true).Name}";
                            else
                                removedSpellList += $", {new Spell(spell.Key, true).Name}";
                        }

                        string addedSpellList = "";
                        int addedLegNum = 0;
                        foreach (var cantripSpellId in newLegendaryList)
                        {
                            target.Biota.GetOrAddKnownSpell(cantripSpellId, target.BiotaDatabaseLock, out _);
                            addedLegNum++;
                            if (addedLegNum == 1)
                                addedSpellList = $"{new Spell(cantripSpellId, true).Name}";
                            else if (addedLegNum == newLegendaryList.Count)
                                addedSpellList += $" and {new Spell(cantripSpellId, true).Name}";
                            else
                                addedSpellList += $", {new Spell(cantripSpellId, true).Name}";
                        }

                        string cantripImpenMessage = cantripImpenSuccess ? "\n\nYour armor also somehow looks tougher, like it might have once been worn by some kind of tough guy and his tough guy essence sort of rubbed off on it and now it's more tough than it was before." : "";

                        string randomizeResultMsg = $"Staring into the morph gem intently, your head swims at the chaos within it.  As you slump to the ground you scream in silence at the realization that eternity is boundless and upon you; upon us all.  You smash the morph gem hard against your armor and it explodes into everything and nothing.  Washed away are the legendary enchantments that once took hold.\n\nThe spells {removedSpellList} are no longer.\n\nIn their place, the spells {addedSpellList} have been cast upon your armor.{cantripImpenMessage}";
                        player.Session.Network.EnqueueSend(new GameMessageSystemChat(randomizeResultMsg, ChatMessageType.Broadcast));
                        AddMorphGemLog(target, MorphGemRandomCantrip);
                        break;

                    #endregion MorphGemRandomCantrip

                    #region MorphGemBurden
                    case MorphGemBurden:

                        if (!target.EncumbranceVal.HasValue)
                        {
                            playerMsg = $"{source.Name} can only be applied to items that have an encumbrance.";
                            player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                            player.SendUseDoneEvent(WeenieError.YouDoNotPassCraftingRequirements);
                            return;
                        }

                        if (target.EncumbranceVal.Value < -999)
                        {
                            playerMsg = $"Your {target.NameWithMaterial} has already reached the minimum amount of encumbrance.";
                            player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                            player.SendUseDoneEvent(WeenieError.YouDoNotPassCraftingRequirements);
                            return;
                        }

                        if (GetMorphGemLogCount(target, MorphGemBurden) > 2)
                        {
                            playerMsg = $"{source.Name} can only be applied to an item three times and your target item has reached this maximum.";
                            player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                            player.SendUseDoneEvent(WeenieError.YouDoNotPassCraftingRequirements);
                            return;
                        }

                        int encumbranceRoll;
                        if (target.EncumbranceVal >= 1000)
                            encumbranceRoll = ThreadSafeRandom.Next(100, 650);
                        else if (target.EncumbranceVal >= 500)
                            encumbranceRoll = ThreadSafeRandom.Next(75, 420);
                        else if (target.EncumbranceVal > 0)
                            encumbranceRoll = ThreadSafeRandom.Next(50, 333);
                        else
                            encumbranceRoll = ThreadSafeRandom.Next(10, 333);

                        if (target.EncumbranceVal.Value - encumbranceRoll < -1000)
                            encumbranceRoll = 1000 + target.EncumbranceVal.Value;

                        target.EncumbranceVal = target.EncumbranceVal - encumbranceRoll;

                        playerMsg = $"With a steady hand you skillfully apply the {source.Name} to your {target.NameWithMaterial} and have successfully reduced its encumbrance by {encumbranceRoll}";
                        player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                        AddMorphGemLog(target, MorphGemBurden);
                        break;

                    #endregion MorphGemBurden

                    #region MorphGemRareDmgBoost
                    case MorphGemRareDmgBoost:

                        if (!(target.ItemType == ItemType.Armor || target.ItemType == ItemType.Jewelry || target.ItemType == ItemType.Clothing))
                        {
                            playerMsg = $"The {source.Name} can only be applied to armor, clothing or jewelry";
                            player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                            player.SendUseDoneEvent(WeenieError.YouDoNotPassCraftingRequirements);
                            return;
                        }

                        if (targetItemSpells == null || targetItemSpells.Count < 1)
                        {
                            playerMsg = "The gem can only be applied to magical items";
                            player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                            player.SendUseDoneEvent(WeenieError.YouDoNotPassCraftingRequirements);
                            return;
                        }
                        else if (targetItemSpells.Contains(5978))
                        {
                            playerMsg = "Your target item already has Rare Damage Boost V on it, you cannot add it twice";
                            player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                            player.SendUseDoneEvent(WeenieError.YouDoNotPassCraftingRequirements);
                            return;
                        }

                        target.Biota.GetOrAddKnownSpell(5978, target.BiotaDatabaseLock, out _);
                        playerMsg = $"With a steady hand you skillfully apply the {source.Name} to your {target.NameWithMaterial} and have successfully added the spell Rare Damage Boost V";
                        player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                        AddMorphGemLog(target, MorphGemRareDmgBoost);
                        break;

                    #endregion MorphGemRareDmgBoost

                    #region MorphGemRareDmgReduction
                    case MorphGemRareDmgReduction:

                        if (!(target.ItemType == ItemType.Armor || target.ItemType == ItemType.Jewelry || target.ItemType == ItemType.Clothing))
                        {
                            playerMsg = $"The {source.Name} can only be applied to armor, clothing or jewelry";
                            player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                            player.SendUseDoneEvent(WeenieError.YouDoNotPassCraftingRequirements);
                            return;
                        }

                        if (targetItemSpells == null || targetItemSpells.Count < 1)
                        {
                            playerMsg = "The gem can only be applied to magical items";
                            player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                            player.SendUseDoneEvent(WeenieError.YouDoNotPassCraftingRequirements);
                            return;
                        }
                        else if (targetItemSpells.Contains(5192))
                        {
                            playerMsg = "Your target item already has Rare Damage Reduction V on it, you cannot add it twice";
                            player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                            player.SendUseDoneEvent(WeenieError.YouDoNotPassCraftingRequirements);
                            return;
                        }

                        target.Biota.GetOrAddKnownSpell(5192, target.BiotaDatabaseLock, out _);
                        playerMsg = $"With a steady hand you skillfully apply the {source.Name} to your {target.NameWithMaterial} and have successfully added the spell Rare Damage Reduction V";
                        player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                        AddMorphGemLog(target, MorphGemRareDmgReduction);
                        break;

                    #endregion MorphGemRareDmgReduction

                    #region MorphGemMeleeCleave
                    case MorphGemMeleeCleave:

                        if (target.ItemType != ItemType.MeleeWeapon)
                        {
                            playerMsg = "This gem can only be used on melee weapons";
                            player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                            player.SendUseDoneEvent(WeenieError.YouDoNotPassCraftingRequirements);
                            return;
                        }

                        int currCleave = target.GetProperty(PropertyInt.Cleaving) ?? 0;

                        if (currCleave >= 3)
                        {
                            playerMsg = $"Your {target.NameWithMaterial} already has the maximum number of cleave targets and thus the gem would have no effect";
                            player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                            player.SendUseDoneEvent(WeenieError.YouDoNotPassCraftingRequirements);
                            return;
                        }

                        if (currCleave < 2)
                            target.SetProperty(PropertyInt.Cleaving, 2);
                        else
                            target.SetProperty(PropertyInt.Cleaving, 3);

                        playerMsg = $"You have successfully used the {source.Name} on your {target.NameWithMaterial} to increase its melee cleaving targets to {target.CleaveTargets + 1}!";
                        player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                        AddMorphGemLog(target, MorphGemMeleeCleave);
                        break;

                    #endregion MorphGemMeleeCleave

                    #region Rare Cantrip Morph Gems

                    case MorphGemRuneofAcidBane:
                        if (!ApplyMorphGem_RareLegendaryCantrip(player, source, target, 6088, targetItemSpells)) return;
                        break;

                    case MorphGemIdeographofAcidProtection:
                        if (!ApplyMorphGem_RareLegendaryCantrip(player, source, target, 6080, targetItemSpells)) return;
                        break;

                    case MorphGemHieroglyphofAlchemyMastery:
                        if (!ApplyMorphGem_RareLegendaryCantrip(player, source, target, 6040, targetItemSpells)) return;
                        break;

                    case MorphGemHieroglyphofArcaneEnlightenment:
                        if (!ApplyMorphGem_RareLegendaryCantrip(player, source, target, 6041, targetItemSpells)) return;
                        break;

                    case MorphGemIdeographofArmor:
                        if (!ApplyMorphGem_RareLegendaryCantrip(player, source, target, 6102, targetItemSpells)) return;
                        break;

                    case MorphGemHieroglyphofArmorTinkeringExpertise:
                        if (!ApplyMorphGem_RareLegendaryCantrip(player, source, target, 6042, targetItemSpells)) return;
                        break;

                    case MorphGemHieroglyphofMonsterAttunement:
                        if (!ApplyMorphGem_RareLegendaryCantrip(player, source, target, 6065, targetItemSpells)) return;
                        break;

                    case MorphGemHieroglyphofPersonAttunement:
                        if (!ApplyMorphGem_RareLegendaryCantrip(player, source, target, 6066, targetItemSpells)) return;
                        break;

                    case MorphGemHieroglyphofLightWeaponMastery:
                        if (!ApplyMorphGem_RareLegendaryCantrip(player, source, target, 6043, targetItemSpells)) return;
                        break;

                    case MorphGemRuneofBladeBane:
                        if (!ApplyMorphGem_RareLegendaryCantrip(player, source, target, 6097, targetItemSpells)) return;
                        break;

                    case MorphGemRuneofBloodDrinker:

                        if (target as MeleeWeapon == null && !target.IsRanged)
                        {
                            playerMsg = "This gem can only be used on melee or missile weapons";
                            player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                            player.SendUseDoneEvent(WeenieError.YouDoNotPassCraftingRequirements);
                            return;
                        }

                        if (targetItemSpells == null || targetItemSpells.Count < 1)
                        {
                            playerMsg = "The gem can only be applied to magical items";
                            player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                            player.SendUseDoneEvent(WeenieError.YouDoNotPassCraftingRequirements);
                            return;
                        }
                        else if (targetItemSpells.Contains(6089))
                        {
                            playerMsg = "Your target item already has Legendary Blood Thirst on it, you cannot add it twice";
                            player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                            player.SendUseDoneEvent(WeenieError.YouDoNotPassCraftingRequirements);
                            return;
                        }

                        RemoveAllCantripsInProgression(target, 6089);
                        target.Biota.GetOrAddKnownSpell(6089, target.BiotaDatabaseLock, out _);
                        playerMsg = $"With a steady hand you skillfully apply the morph gem to your {target.NameWithMaterial} and have successfully added the spell Legendary Blood Thirst";
                        player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                        AddMorphGemLog(target, MorphGemRuneofBloodDrinker);
                        break;

                    case MorphGemRuneofBludgeonBane:
                        if (!ApplyMorphGem_RareLegendaryCantrip(player, source, target, 6090, targetItemSpells)) return;
                        break;

                    case MorphGemIdeographofBludgeoningProtection:
                        if (!ApplyMorphGem_RareLegendaryCantrip(player, source, target, 6081, targetItemSpells)) return;
                        break;

                    case MorphGemHieroglyphofMissileWeaponMastery:
                        if (!ApplyMorphGem_RareLegendaryCantrip(player, source, target, 6044, targetItemSpells)) return;
                        break;

                    case MorphGemHieroglyphofCookingMastery:
                        if (!ApplyMorphGem_RareLegendaryCantrip(player, source, target, 6045, targetItemSpells)) return;
                        break;

                    case MorphGemPictographofCoordination:
                        if (!ApplyMorphGem_RareLegendaryCantrip(player, source, target, 6103, targetItemSpells)) return;
                        break;

                    case MorphGemHieroglyphofCreatureEnchantmentMastery:
                        if (!ApplyMorphGem_RareLegendaryCantrip(player, source, target, 6046, targetItemSpells)) return;
                        break;

                    case MorphGemHieroglyphofFinesseWeaponMastery:
                        if (!ApplyMorphGem_RareLegendaryCantrip(player, source, target, 6047, targetItemSpells)) return;
                        break;

                    case MorphGemHieroglyphofDeceptionMastery:
                        if (!ApplyMorphGem_RareLegendaryCantrip(player, source, target, 6048, targetItemSpells)) return;
                        break;

                    case MorphGemRuneofDefender:

                        if (target as MeleeWeapon == null && !target.IsCaster && !target.IsRanged)
                        {
                            playerMsg = "This gem can only be used on weapons or magic casters";
                            player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                            player.SendUseDoneEvent(WeenieError.YouDoNotPassCraftingRequirements);
                            return;
                        }

                        if (targetItemSpells == null || targetItemSpells.Count < 1)
                        {
                            playerMsg = "The gem can only be applied to magical items";
                            player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                            player.SendUseDoneEvent(WeenieError.YouDoNotPassCraftingRequirements);
                            return;
                        }
                        else if (targetItemSpells.Contains(6091))
                        {
                            playerMsg = "Your target item already has Legendary Defender on it, you cannot add it twice";
                            player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                            player.SendUseDoneEvent(WeenieError.YouDoNotPassCraftingRequirements);
                            return;
                        }

                        RemoveAllCantripsInProgression(target, 6091);
                        target.Biota.GetOrAddKnownSpell(6091, target.BiotaDatabaseLock, out _);
                        playerMsg = $"With a steady hand you skillfully apply the morph gem to your {target.NameWithMaterial} and have successfully added the spell Legendary Defender";
                        player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                        AddMorphGemLog(target, MorphGemRuneofDefender);
                        break;

                    case MorphGemPictographofEndurance:
                        if (!ApplyMorphGem_RareLegendaryCantrip(player, source, target, 6104, targetItemSpells)) return;
                        break;

                    case MorphGemIdeographofFireProtection:
                        if (!ApplyMorphGem_RareLegendaryCantrip(player, source, target, 6082, targetItemSpells)) return;
                        break;

                    case MorphGemRuneofFlameBane:
                        if (!ApplyMorphGem_RareLegendaryCantrip(player, source, target, 6092, targetItemSpells)) return;
                        break;

                    case MorphGemHieroglyphofFletchingMastery:
                        if (!ApplyMorphGem_RareLegendaryCantrip(player, source, target, 6052, targetItemSpells)) return;
                        break;

                    case MorphGemPictographofFocus:
                        if (!ApplyMorphGem_RareLegendaryCantrip(player, source, target, 6105, targetItemSpells)) return;
                        break;

                    case MorphGemRuneofFrostBane:
                        if (!ApplyMorphGem_RareLegendaryCantrip(player, source, target, 6093, targetItemSpells)) return;
                        break;

                    case MorphGemIdeographofFrostProtection:
                        if (!ApplyMorphGem_RareLegendaryCantrip(player, source, target, 6083, targetItemSpells)) return;
                        break;

                    case MorphGemHieroglyphofHealingMastery:
                        if (!ApplyMorphGem_RareLegendaryCantrip(player, source, target, 6053, targetItemSpells)) return;
                        break;

                    case MorphGemIdeographofRegeneration:
                        if (!ApplyMorphGem_RareLegendaryCantrip(player, source, target, 6077, targetItemSpells)) return;
                        break;

                    case MorphGemRuneofHeartSeeker:

                        if (target as MeleeWeapon == null)
                        {
                            playerMsg = "This gem can only be used on melee weapons";
                            player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                            player.SendUseDoneEvent(WeenieError.YouDoNotPassCraftingRequirements);
                            return;
                        }

                        if (targetItemSpells == null || targetItemSpells.Count < 1)
                        {
                            playerMsg = "The gem can only be applied to magical items";
                            player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                            player.SendUseDoneEvent(WeenieError.YouDoNotPassCraftingRequirements);
                            return;
                        }
                        else if (targetItemSpells.Contains(6094))
                        {
                            playerMsg = "Your target item already has Legendary Heart Thirst on it, you cannot add it twice";
                            player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                            player.SendUseDoneEvent(WeenieError.YouDoNotPassCraftingRequirements);
                            return;
                        }

                        RemoveAllCantripsInProgression(target, 6094);
                        target.Biota.GetOrAddKnownSpell(6094, target.BiotaDatabaseLock, out _);
                        playerMsg = $"With a steady hand you skillfully apply the morph gem to your {target.NameWithMaterial} and have successfully added the spell Legendary Heart Thirst";
                        player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                        AddMorphGemLog(target, MorphGemRuneofHeartSeeker);
                        break;

                    case MorphGemRuneofHermeticLink:

                        if (target as Caster == null)
                        {
                            playerMsg = "This gem can only be used on magic casters";
                            player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                            player.SendUseDoneEvent(WeenieError.YouDoNotPassCraftingRequirements);
                            return;
                        }

                        if (targetItemSpells == null || targetItemSpells.Count < 1)
                        {
                            playerMsg = "The gem can only be applied to magical items";
                            player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                            player.SendUseDoneEvent(WeenieError.YouDoNotPassCraftingRequirements);
                            return;
                        }
                        else if (targetItemSpells.Contains(6087))
                        {
                            playerMsg = "Your target item already has Legendary Hermetic Link on it, you cannot add it twice";
                            player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                            player.SendUseDoneEvent(WeenieError.YouDoNotPassCraftingRequirements);
                            return;
                        }

                        RemoveAllCantripsInProgression(target, 6087);
                        target.Biota.GetOrAddKnownSpell(6087, target.BiotaDatabaseLock, out _);
                        playerMsg = $"With a steady hand you skillfully apply the morph gem to your {target.NameWithMaterial} and have successfully added the spell Legendary Hermetic Link";
                        player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                        AddMorphGemLog(target, MorphGemRuneofHermeticLink);
                        break;

                    case MorphGemRuneofImpenetrability:
                        if (!ApplyMorphGem_RareLegendaryCantrip(player, source, target, 6095, targetItemSpells)) return;
                        break;

                    case MorphGemHieroglyphofItemEnchantmentMastery:
                        if (!ApplyMorphGem_RareLegendaryCantrip(player, source, target, 6056, targetItemSpells)) return;
                        break;

                    case MorphGemHieroglyphofItemTinkeringExpertise:
                        if (!ApplyMorphGem_RareLegendaryCantrip(player, source, target, 6057, targetItemSpells)) return;
                        break;

                    case MorphGemHieroglyphofJumpingMastery:
                        if (!ApplyMorphGem_RareLegendaryCantrip(player, source, target, 6058, targetItemSpells)) return;
                        break;

                    case MorphGemHieroglyphofLeadershipMastery:
                        if (!ApplyMorphGem_RareLegendaryCantrip(player, source, target, 6059, targetItemSpells)) return;
                        break;

                    case MorphGemHieroglyphofLifeMagicMastery:
                        if (!ApplyMorphGem_RareLegendaryCantrip(player, source, target, 6060, targetItemSpells)) return;
                        break;

                    case MorphGemRuneofLightningBane:
                        if (!ApplyMorphGem_RareLegendaryCantrip(player, source, target, 6099, targetItemSpells)) return;
                        break;

                    case MorphGemIdeographofLightningProtection:
                        if (!ApplyMorphGem_RareLegendaryCantrip(player, source, target, 6079, targetItemSpells)) return;
                        break;

                    case MorphGemHieroglyphofLockpickMastery:
                        if (!ApplyMorphGem_RareLegendaryCantrip(player, source, target, 6061, targetItemSpells)) return;
                        break;

                    case MorphGemHieroglyphofFealtyMastery:
                        if (!ApplyMorphGem_RareLegendaryCantrip(player, source, target, 6051, targetItemSpells)) return;
                        break;

                    case MorphGemHieroglyphofMagicResistance:
                        if (!ApplyMorphGem_RareLegendaryCantrip(player, source, target, 6063, targetItemSpells)) return;
                        break;

                    case MorphGemHieroglyphofMagicItemTinkeringExpertise:
                        if (!ApplyMorphGem_RareLegendaryCantrip(player, source, target, 6062, targetItemSpells)) return;
                        break;

                    case MorphGemHieroglyphofManaConversionMastery:
                        if (!ApplyMorphGem_RareLegendaryCantrip(player, source, target, 6064, targetItemSpells)) return;
                        break;

                    case MorphGemIdeographofBattlemagesBlessing:
                        if (!ApplyMorphGem_RareLegendaryCantrip(player, source, target, 6078, targetItemSpells)) return;
                        break;

                    case MorphGemHieroglyphofInvulnerability:
                        if (!ApplyMorphGem_RareLegendaryCantrip(player, source, target, 6055, targetItemSpells)) return;
                        break;

                    case MorphGemHieroglyphofImpregnability:
                        if (!ApplyMorphGem_RareLegendaryCantrip(player, source, target, 6054, targetItemSpells)) return;
                        break;

                    case MorphGemRuneofPierceBane:
                        if (!ApplyMorphGem_RareLegendaryCantrip(player, source, target, 6096, targetItemSpells)) return;
                        break;

                    case MorphGemIdeographofPiercingProtection:
                        if (!ApplyMorphGem_RareLegendaryCantrip(player, source, target, 6084, targetItemSpells)) return;
                        break;

                    case MorphGemPictographofQuickness:
                        if (!ApplyMorphGem_RareLegendaryCantrip(player, source, target, 6106, targetItemSpells)) return;
                        break;

                    case MorphGemHieroglyphofSprint:
                        if (!ApplyMorphGem_RareLegendaryCantrip(player, source, target, 6071, targetItemSpells)) return;
                        break;

                    case MorphGemPictographofWillpower:
                        if (!ApplyMorphGem_RareLegendaryCantrip(player, source, target, 6101, targetItemSpells)) return;
                        break;

                    case MorphGemIdeographofBladeProtection:
                        if (!ApplyMorphGem_RareLegendaryCantrip(player, source, target, 6085, targetItemSpells)) return;
                        break;

                    case MorphGemRuneofSpiritDrinker:

                        if (target as Caster == null)
                        {
                            playerMsg = "This gem can only be used on magic casters";
                            player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                            player.SendUseDoneEvent(WeenieError.YouDoNotPassCraftingRequirements);
                            return;
                        }

                        if (targetItemSpells == null || targetItemSpells.Count < 1)
                        {
                            playerMsg = "The gem can only be applied to magical items";
                            player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                            player.SendUseDoneEvent(WeenieError.YouDoNotPassCraftingRequirements);
                            return;
                        }
                        else if (targetItemSpells.Contains(6098))
                        {
                            playerMsg = "Your target item already has Legendary Spirit Thirst on it, you cannot add it twice";
                            player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                            player.SendUseDoneEvent(WeenieError.YouDoNotPassCraftingRequirements);
                            return;
                        }

                        RemoveAllCantripsInProgression(target, 6098);
                        target.Biota.GetOrAddKnownSpell(6098, target.BiotaDatabaseLock, out _);
                        playerMsg = $"With a steady hand you skillfully apply the morph gem to your {target.NameWithMaterial} and have successfully added the spell Legendary Spirit Thirst";
                        player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                        AddMorphGemLog(target, MorphGemRuneofSpiritDrinker);
                        break;

                    case MorphGemIdeographofRevitalization:
                        if (!ApplyMorphGem_RareLegendaryCantrip(player, source, target, 6076, targetItemSpells)) return;
                        break;

                    case MorphGemPictographofStrength:
                        if (!ApplyMorphGem_RareLegendaryCantrip(player, source, target, 6107, targetItemSpells)) return;
                        break;

                    case MorphGemRuneofSwiftKiller:

                        if (target as MeleeWeapon == null && !target.IsBow && !target.IsThrownWeapon)
                        {
                            playerMsg = "This gem can only be used on melee or missile weapons";
                            player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                            player.SendUseDoneEvent(WeenieError.YouDoNotPassCraftingRequirements);
                            return;
                        }

                        if (targetItemSpells == null || targetItemSpells.Count < 1)
                        {
                            playerMsg = "The gem can only be applied to magical items";
                            player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                            player.SendUseDoneEvent(WeenieError.YouDoNotPassCraftingRequirements);
                            return;
                        }
                        else if (targetItemSpells.Contains(6100))
                        {
                            playerMsg = "Your target item already has Legendary Swift Hunter on it, you cannot add it twice";
                            player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                            player.SendUseDoneEvent(WeenieError.YouDoNotPassCraftingRequirements);
                            return;
                        }

                        RemoveAllCantripsInProgression(target, 6100);
                        target.Biota.GetOrAddKnownSpell(6100, target.BiotaDatabaseLock, out _);
                        playerMsg = $"With a steady hand you skillfully apply the morph gem to your {target.NameWithMaterial} and have successfully added the spell Legendary Swift Hunter";
                        player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                        AddMorphGemLog(target, MorphGemRuneofSwiftKiller);
                        break;

                    case MorphGemHieroglyphofHeavyWeaponMastery:
                        if (!ApplyMorphGem_RareLegendaryCantrip(player, source, target, 6072, targetItemSpells)) return;
                        break;

                    case MorphGemHieroglyphofWarMagicMastery:
                        if (!ApplyMorphGem_RareLegendaryCantrip(player, source, target, 6075, targetItemSpells)) return;
                        break;

                    case MorphGemHieroglyphofWeaponTinkeringExpertise:
                        if (!ApplyMorphGem_RareLegendaryCantrip(player, source, target, 6039, targetItemSpells)) return;
                        break;

                    case MorphGemHieroglyphofDirtyFightingMastery:
                        if (!ApplyMorphGem_RareLegendaryCantrip(player, source, target, 6049, targetItemSpells)) return;
                        break;

                    case MorphGemHieroglyphofDualWieldMastery:
                        if (!ApplyMorphGem_RareLegendaryCantrip(player, source, target, 6050, targetItemSpells)) return;
                        break;

                    case MorphGemHieroglyphofRecklessnessMastery:
                        if (!ApplyMorphGem_RareLegendaryCantrip(player, source, target, 6067, targetItemSpells)) return;
                        break;

                    case MorphGemHieroglyphofShieldMastery:
                        if (!ApplyMorphGem_RareLegendaryCantrip(player, source, target, 6069, targetItemSpells)) return;
                        break;

                    case MorphGemHieroglyphofSneakAttackMastery:
                        if (!ApplyMorphGem_RareLegendaryCantrip(player, source, target, 6070, targetItemSpells)) return;
                        break;

                    case MorphGemHieroglyphofVoidMagicMastery:
                        if (!ApplyMorphGem_RareLegendaryCantrip(player, source, target, 6074, targetItemSpells)) return;
                        break;

                    case MorphGemHieroglyphofTwoHandedWeaponsMastery:
                        if (!ApplyMorphGem_RareLegendaryCantrip(player, source, target, 6073, targetItemSpells)) return;
                        break;

                    case MorphGemHieroglyphofSummoningMastery:
                        if (!ApplyMorphGem_RareLegendaryCantrip(player, source, target, 6125, targetItemSpells)) return;
                        break;

                    #endregion Rare Cantrip Morph Gems

                    default:
                        player.SendUseDoneEvent(WeenieError.YouDoNotPassCraftingRequirements);
                        return;
                }

                player.TryConsumeFromInventoryWithNetworking(source, 1);
                target.SaveBiotaToDatabase();
                player.SendUseDoneEvent();
            }
            catch (Exception ex)
            {
                log.ErrorFormat("Exception in MorphGem.ApplyMorphGem. Ex: {0}", ex);
            }
        }

        private static bool ApplyMorphGem_RareLegendaryCantrip(Player player, WorldObject source, WorldObject target, int spellId, List<int> targetItemSpells)
        {
            string playerMsg = "";

            var spell = new Spell(spellId);
            if (spell == null)
                return false;

            if (target.ItemType == ItemType.Jewelry && (spell.Name.Contains(" Bane", StringComparison.OrdinalIgnoreCase) || spell.Name.Contains(" Impenitrability")))
            {
                playerMsg = "The gem can only be applied to armor or clothing";
                player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                player.SendUseDoneEvent(WeenieError.YouDoNotPassCraftingRequirements);
                return false;
            }
            else if (!(target.ItemType == ItemType.Armor || target.ItemType == ItemType.Jewelry || target.ItemType == ItemType.Clothing))
            {
                playerMsg = "The gem can only be applied to armor, clothing or jewelry";
                player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                player.SendUseDoneEvent(WeenieError.YouDoNotPassCraftingRequirements);
                return false;
            }

            if (targetItemSpells == null || targetItemSpells.Count < 1)
            {
                playerMsg = "The gem can only be applied to magical items";
                player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                player.SendUseDoneEvent(WeenieError.YouDoNotPassCraftingRequirements);
                return false;
            }
            else if (targetItemSpells.Contains(spellId))
            {
                playerMsg = $"Your target item already has {spell.Name} on it, you cannot add it twice";
                player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
                player.SendUseDoneEvent(WeenieError.YouDoNotPassCraftingRequirements);
                return false;
            }

            RemoveAllCantripsInProgression(target, spellId);
            target.Biota.GetOrAddKnownSpell(spellId, target.BiotaDatabaseLock, out _);
            playerMsg = $"With a steady hand you skillfully apply the morph gem to your {target.NameWithMaterial} and have successfully added the spell {spell.Name}";
            player.Session.Network.EnqueueSend(new GameMessageSystemChat(playerMsg, ChatMessageType.Broadcast));
            AddMorphGemLog(target, source.WeenieClassId);
            return true;
        }

        private static void RemoveAllCantripsInProgression(WorldObject target, int spellId)
        {
            var progression = SpellLevelProgression.GetSpellLevels((SpellId)spellId);
            if (progression != null)
            {
                foreach (var progressionSpellId in progression)
                    target.Biota.TryRemoveKnownSpell((int)progressionSpellId, target.BiotaDatabaseLock);
            }
        }

        #region Morph Gem Log
        public static void AddMorphGemLog(WorldObject target, uint gemWeenieId)
        {
            if (!string.IsNullOrEmpty(target.MorphGemLog))
                target.MorphGemLog += ",";

            target.MorphGemLog += gemWeenieId;
        }

        public static int GetMorphGemLogCount(WorldObject target, uint gemWeenieId)
        {
            if (string.IsNullOrEmpty(target.MorphGemLog))
                return 0;

            var logEntries = target.MorphGemLog.Split(',');
            var matchingLogEntries = logEntries.Where(x => x.Equals(gemWeenieId.ToString()));
            return matchingLogEntries?.Count() ?? 0;
        }
        #endregion Morph Gem Log
    }
}
