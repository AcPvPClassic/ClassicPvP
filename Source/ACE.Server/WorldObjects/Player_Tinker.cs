using System.Collections.Generic;

using ACE.DatLoader;
using ACE.Entity.Enum;
using ACE.Server.Factories;
using ACE.Server.Network.GameMessages.Messages;

namespace ACE.Server.WorldObjects
{
    partial class Player
    {
        // -------------------------------------------------------------------------
        // Tinker designation
        // -------------------------------------------------------------------------

        /// <summary>
        /// Crafting skills that a Tinker character has auto-specialized and maxed.
        /// </summary>
        public static readonly List<Skill> TinkerSkills = new List<Skill>
        {
            Skill.ItemTinkering,
            Skill.WeaponTinkering,
            Skill.ArmorTinkering,
            Skill.MagicItemTinkering,
            Skill.Alchemy,
            Skill.Lockpick,
            Skill.Fletching,
            Skill.Cooking,
        };

        /// <summary>
        /// Offensive / combat skills that are untrained on a Tinker character.
        /// </summary>
        public static readonly List<Skill> TinkerOffensiveSkills = new List<Skill>
        {
            Skill.HeavyWeapons,
            Skill.LightWeapons,
            Skill.FinesseWeapons,
            Skill.MissileWeapons,
            Skill.TwoHandedCombat,
            Skill.DualWield,
            Skill.Recklessness,
            Skill.SneakAttack,
            Skill.DirtyFighting,
            Skill.WarMagic,
            Skill.VoidMagic,
            Skill.LifeMagic,
            Skill.CreatureEnchantment,
            Skill.ItemEnchantment,
            Skill.Shield,
            Skill.Summoning,
        };

        /// <summary>
        /// Flags the character as a Tinker, specializing and maxing all crafting skills,
        /// untraining all offensive combat skills, and maxing all attributes.
        /// This is irreversible.
        /// </summary>
        public void FlagAsTinker()
        {
            // Guard: cannot flag as Tinker more than once
            if (IsTinker)
                return;

            GameplayMode = GameplayModes.Tinker;

            var specXpTable = DatManager.PortalDat.XpTable.SpecializedSkillXpList;
            var maxSpecXp   = specXpTable[specXpTable.Count - 1];

            var attrXpTable = DatManager.PortalDat.XpTable.AttributeXpList;
            var maxAttrXp   = attrXpTable[attrXpTable.Count - 1];

            // --- Specialize and fully rank all Tinker crafting skills ---
            foreach (var skill in TinkerSkills)
            {
                var cs = GetCreatureSkill(skill);

                if (cs.AdvancementClass == SkillAdvancementClass.Untrained)
                {
                    cs.AdvancementClass = SkillAdvancementClass.Trained;
                    cs.InitLevel        = 0;
                    cs.ExperienceSpent  = 0;
                    cs.Ranks            = 0;
                }

                if (cs.AdvancementClass == SkillAdvancementClass.Trained)
                {
                    cs.AdvancementClass = SkillAdvancementClass.Specialized;
                    cs.InitLevel        = 10;
                }

                cs.ExperienceSpent = maxSpecXp;
                cs.Ranks           = (ushort)CalcSkillRank(SkillAdvancementClass.Specialized, maxSpecXp);

                Session.Network.EnqueueSend(new GameMessagePrivateUpdateSkill(this, cs));
            }

            // --- Untrain all offensive / combat skills ---
            foreach (var skill in TinkerOffensiveSkills)
            {
                var cs = GetCreatureSkill(skill);

                if (cs.AdvancementClass >= SkillAdvancementClass.Trained)
                {
                    cs.AdvancementClass = SkillAdvancementClass.Untrained;
                    cs.InitLevel        = 0;
                    cs.Ranks            = 0;
                    cs.ExperienceSpent  = 0;

                    Session.Network.EnqueueSend(new GameMessagePrivateUpdateSkill(this, cs));
                }
            }

            // --- Max all base attributes ---
            foreach (var attr in Attributes.Values)
            {
                attr.ExperienceSpent = maxAttrXp;
                attr.Ranks           = (ushort)CalcAttributeRank(maxAttrXp);

                Session.Network.EnqueueSend(new GameMessagePrivateUpdateAttribute(this, attr));
            }

            // Refresh vitals now that Endurance and Self are maxed
            SetMaxVitals();

            Session.Network.EnqueueSend(
                new GameMessagePrivateUpdateVital(this, Health),
                new GameMessagePrivateUpdateVital(this, Stamina),
                new GameMessagePrivateUpdateVital(this, Mana));

            // --- Grant Tinkering Trinket ---
            var trinket = WorldObjectFactory.CreateNewWorldObject(8142017u);
            if (trinket != null)
                TryCreateInInventoryWithNetworking(trinket);

            Session.Network.EnqueueSend(new GameMessageSystemChat(
                "You have been designated as a Tinker. Your crafting skills have been fully specialized and all combat skills have been removed. " +
                "This character may not train or specialize new skills, and will not suffer a vitae penalty on death.",
                ChatMessageType.Broadcast));
        }
    }
}
