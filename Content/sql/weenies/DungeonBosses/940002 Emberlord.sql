/* Random Dungeon Boss: Vaeth'ren the Emberlord (fire war-magic glass cannon)
   Model frame: Bael'Zharon (shadow caster), recoloured red. Combat stats authored at
   reference level 275, scaled to the season level cap at spawn
   (DungeonBossManager.ScaleBossToCap). Radar hidden. Rewards (scattered currency + XP)
   handled in code. For normal generated loot, set a valid DeathTreasureType DID
   (type 35) on the commented line below. */

DELETE FROM `weenie` WHERE `class_Id` = 940002;

INSERT INTO `weenie` (`class_Id`, `class_Name`, `type`, `last_Modified`)
VALUES (940002, 'dungeonbossemberlord', 10, '2026-08-07 00:00:00') /* Creature */;

INSERT INTO `weenie_properties_int` (`object_Id`, `type`, `value`)
VALUES (940002,   1,         16) /* ItemType - Creature */
     , (940002,   2,         52) /* CreatureType - Shadow */
     , (940002,   3,         14) /* PaletteTemplate - Red */
     , (940002,   6,         -1) /* ItemsCapacity */
     , (940002,   7,         -1) /* ContainersCapacity */
     , (940002,  16,          1) /* ItemUseable - No */
     , (940002,  25,        275) /* Level (reference; overwritten to level cap at spawn) */
     , (940002,  27,          0) /* ArmorType - None */
     , (940002,  40,          1) /* CombatMode - NonCombat */
     , (940002,  68,         13) /* TargetingTactic - Random, LastDamager, TopDamager */
     , (940002,  93,       1032) /* PhysicsState - ReportCollisions, Gravity */
     , (940002, 101,        183) /* AiAllowedCombatStyle - Unarmed, OneHanded, OneHandedAndShield, Bow, Crossbow, ThrownWeapon */
     , (940002, 133,          1) /* ShowableOnRadar - ShowNever (hunt for it) */;

INSERT INTO `weenie_properties_bool` (`object_Id`, `type`, `value`)
VALUES (940002,   1, False) /* Stuck */
     , (940002,   6, False) /* AiUsesMana */
     , (940002,  11, False) /* IgnoreCollisions */
     , (940002,  12, True ) /* ReportCollisions */
     , (940002,  13, False) /* Ethereal */
     , (940002,  19, True ) /* Attackable */
     , (940002,  52, False) /* AiImmobile */;

INSERT INTO `weenie_properties_float` (`object_Id`, `type`, `value`)
VALUES (940002,   1,      5) /* HeartbeatInterval */
     , (940002,   2,      0) /* HeartbeatTimestamp */
     , (940002,   3,     10) /* HealthRate */
     , (940002,   4,    100) /* StaminaRate */
     , (940002,   5,     50) /* ManaRate */
     , (940002,  31,     45) /* VisualAwarenessRange */
     , (940002,  39,      1) /* DefaultScale (natural - do not increase) */
     , (940002,  54,      5) /* UseRadius */
     , (940002,  55,     70) /* HomeRadius */
     , (940002,  80,      2) /* AiUseMagicDelay */
     , (940002, 104,     45) /* ObviousRadarRange */;

INSERT INTO `weenie_properties_string` (`object_Id`, `type`, `value`)
VALUES (940002,   1, 'Vaeth''ren the Emberlord') /* Name */
     , (940002,   5, 'Dungeon Boss') /* Template */;

INSERT INTO `weenie_properties_d_i_d` (`object_Id`, `type`, `value`)
VALUES (940002,   1, 0x02000D41) /* Setup - Martine caster frame (BZ setup absent in Infiltration dat) */
     , (940002,   2, 0x090000FE) /* MotionTable */
     , (940002,   3, 0x20000090) /* SoundTable */
     , (940002,   4, 0x3000002F) /* CombatTable */
     , (940002,   6, 0x0400007E) /* PaletteBase */
     , (940002,   7, 0x100003AD) /* ClothingBase */
     , (940002,   8, 0x06001036) /* Icon */
     , (940002,  22, 0x34000004) /* PhysicsEffectTable */
     , (940002,  35,     940000) /* DeathTreasureType - Dungeon Boss Loot Profile */;

INSERT INTO `weenie_properties_attribute` (`object_Id`, `type`, `init_Level`, `level_From_C_P`, `c_P_Spent`)
VALUES (940002,   1, 340, 0, 0) /* Strength */
     , (940002,   2, 360, 0, 0) /* Endurance */
     , (940002,   3, 380, 0, 0) /* Quickness */
     , (940002,   4, 380, 0, 0) /* Coordination */
     , (940002,   5, 520, 0, 0) /* Focus */
     , (940002,   6, 540, 0, 0) /* Self */;

INSERT INTO `weenie_properties_attribute_2nd` (`object_Id`, `type`, `init_Level`, `level_From_C_P`, `c_P_Spent`, `current_Level`)
VALUES (940002,   1, 40000, 0, 0, 40000) /* MaxHealth */
     , (940002,   3, 50000, 0, 0, 50000) /* MaxStamina */
     , (940002,   5, 60000, 0, 0, 60000) /* MaxMana */;

INSERT INTO `weenie_properties_skill` (`object_Id`, `type`, `level_From_P_P`, `s_a_c`, `p_p`, `init_Level`, `resistance_At_Last_Check`, `last_Used_Time`)
VALUES (940002,  6, 0, 3, 0, 300, 0, 0) /* MeleeDefense    Specialized */
     , (940002,  7, 0, 3, 0, 300, 0, 0) /* MissileDefense  Specialized */
     , (940002, 15, 0, 3, 0, 300, 0, 0) /* MagicDefense    Specialized */
     , (940002, 33, 0, 3, 0, 300, 0, 0) /* LifeMagic       Specialized */
     , (940002, 34, 0, 3, 0, 300, 0, 0) /* WarMagic        Specialized */;

INSERT INTO `weenie_properties_spell_book` (`object_Id`, `spell`, `probability`)
VALUES (940002, 4096, 2.10)  /* Flame Chain */
     , (940002, 3878, 2.05)  /* Incendiary Strike */
     , (940002, 4188, 2.05)  /* Shadowed Flame */
     , (940002, 4481, 2.10)  /* Incantation of Fire Vulnerability Other */;

INSERT INTO `weenie_properties_body_part` (`object_Id`, `key`, `d_Type`, `d_Val`, `d_Var`, `base_Armor`, `armor_Vs_Slash`, `armor_Vs_Pierce`, `armor_Vs_Bludgeon`, `armor_Vs_Cold`, `armor_Vs_Fire`, `armor_Vs_Acid`, `armor_Vs_Electric`, `armor_Vs_Nether`, `b_h`, `h_l_f`, `m_l_f`, `l_l_f`, `h_r_f`, `m_r_f`, `l_r_f`, `h_l_b`, `m_l_b`, `l_l_b`, `h_r_b`, `m_r_b`, `l_r_b`)
VALUES (940002,  0,  4,   0,    0,  600, 700, 700, 700, 700, 700, 700, 700, 700, 1, 0.33,    0,    0, 0.33,    0,    0, 0.33,    0,    0, 0.33,    0,    0) /* Head */
     , (940002,  1,  4,   0,    0,  600, 700, 700, 700, 700, 700, 700, 700, 700, 2, 0.44, 0.17,    0, 0.44, 0.17,    0, 0.44, 0.17,    0, 0.44, 0.17,    0) /* Chest */
     , (940002,  2,  4,   0,    0,  600, 700, 700, 700, 700, 700, 700, 700, 700, 3,    0, 0.17,    0,    0, 0.17,    0,    0, 0.17,    0,    0, 0.17,    0) /* Abdomen */
     , (940002,  3,  4,   0,    0,  600, 700, 700, 700, 700, 700, 700, 700, 700, 1, 0.23, 0.03,    0, 0.23, 0.03,    0, 0.23, 0.03,    0, 0.23, 0.03,    0) /* UpperArm */
     , (940002,  4,  4,   0,    0,  600, 700, 700, 700, 700, 700, 700, 700, 700, 2,    0,  0.3,    0,    0,  0.3,    0,    0,  0.3,    0,    0,  0.3,    0) /* LowerArm */
     , (940002,  5,  4,  90, 0.75,  600, 700, 700, 700, 700, 700, 700, 700, 700, 2,    0,  0.2,    0,    0,  0.2,    0,    0,  0.2,    0,    0,  0.2,    0) /* Hand */
     , (940002,  6,  4,   0,    0,  600, 700, 700, 700, 700, 700, 700, 700, 700, 3,    0, 0.13, 0.18,    0, 0.13, 0.18,    0, 0.13, 0.18,    0, 0.13, 0.18) /* UpperLeg */
     , (940002,  7,  4,   0,    0,  600, 700, 700, 700, 700, 700, 700, 700, 700, 3,    0,    0,  0.6,    0,    0,  0.6,    0,    0,  0.6,    0,    0,  0.6) /* LowerLeg */
     , (940002,  8,  4,  90, 0.75,  600, 700, 700, 700, 700, 700, 700, 700, 700, 3,    0,    0, 0.22,    0,    0, 0.22,    0,    0, 0.22,    0,    0, 0.22) /* Foot */;
