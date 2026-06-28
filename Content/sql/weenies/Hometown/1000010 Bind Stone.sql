-- ============================================================
-- Bind Stone Creature Proxy (WCID 1000010)
-- Used during Phase 2 of Allegiance Hometown Capture.
-- Spawned at the bindstone position when Phase 2 starts.
-- Uses the bindstone visual model so it looks identical.
-- Non-aggressive, stuck in place, no loot, no XP.
-- Death triggers attacker victory via BindstoneCreatureProxy.OnDeath override.
-- Low attributes and defense skills so attacks and spells always land.
-- Physical damage types (slash/pierce/bludgeon) are 95% resisted to keep melee/archer
-- damage in the same ballpark as magic damage.
-- HP is overridden at spawn by AllegianceHometownManager.ComputeBindstoneHp().
-- ============================================================

DELETE FROM `weenie` WHERE `class_Id` = 1000010;

INSERT INTO `weenie` (`class_Id`, `class_Name`, `type`, `last_Modified`)
VALUES (1000010, 'homestonephase2proxy', 10, '2026-06-25 00:00:00') /* WeenieType.Creature */;

INSERT INTO `weenie_properties_int` (`object_Id`, `type`, `value`)
VALUES (1000010,   1,         16) /* ItemType - Creature */
     , (1000010,   2,         47) /* CreatureType - Crystal */
     , (1000010,   6,         -1) /* ItemsCapacity */
     , (1000010,   7,         -1) /* ContainersCapacity */
     , (1000010,  16,          1) /* ItemUseable - No */
     , (1000010,  25,          1) /* Level */
     , (1000010,  27,          0) /* ArmorType - None */
     , (1000010,  40,          2) /* CombatMode - Melee */
     , (1000010,  67,          1) /* Tolerance - NoAttack */
     , (1000010,  93,       1032) /* PhysicsState - ReportCollisions, Gravity */
     , (1000010, 101,          1) /* AiAllowedCombatStyle - Unarmed */
     , (1000010, 133,          4) /* ShowableOnRadar - ShowAlways */;

INSERT INTO `weenie_properties_bool` (`object_Id`, `type`, `value`)
VALUES (1000010,   1, TRUE) /* Stuck */;

INSERT INTO `weenie_properties_float` (`object_Id`, `type`, `value`)
VALUES (1000010,   1,       5) /* HeartbeatInterval */
     , (1000010,  12,       0) /* Shade */
     , (1000010,  13,       1) /* ArmorModVsSlash */
     , (1000010,  14,       1) /* ArmorModVsPierce */
     , (1000010,  15,       1) /* ArmorModVsBludgeon */
     , (1000010,  16,       1) /* ArmorModVsCold */
     , (1000010,  17,       1) /* ArmorModVsFire */
     , (1000010,  18,       1) /* ArmorModVsAcid */
     , (1000010,  19,       1) /* ArmorModVsElectric */
     , (1000010,  39,       1) /* DefaultScale */
     , (1000010,  54,       3) /* UseRadius */
     , (1000010,  64,    0.05) /* ResistSlash       - 95% physical resist */
     , (1000010,  65,    0.05) /* ResistPierce      - 95% physical resist */
     , (1000010,  66,    0.05) /* ResistBludgeon    - 95% physical resist */
     , (1000010,  67,       1) /* ResistFire        - no resistance */
     , (1000010,  68,       1) /* ResistCold        - no resistance */
     , (1000010,  69,       1) /* ResistAcid        - no resistance */
     , (1000010,  70,       1) /* ResistElectric    - no resistance */
     , (1000010,  71,       1) /* ResistHealthBoost */
     , (1000010,  72,       0) /* ResistStaminaDrain */
     , (1000010,  73,       1) /* ResistStaminaBoost */
     , (1000010,  74,       0) /* ResistManaDrain */
     , (1000010,  75,       1) /* ResistManaBoost */
     , (1000010, 125,       0) /* ResistHealthDrain */;

INSERT INTO `weenie_properties_string` (`object_Id`, `type`, `value`)
VALUES (1000010, 1, 'Bind Stone') /* Name */;

-- Bindstone visual model (Setup 0x020010AC, MotionTable 0x09000160)
INSERT INTO `weenie_properties_d_i_d` (`object_Id`, `type`, `value`)
VALUES (1000010,   1, 0x020010AC) /* Setup - bindstone model */
     , (1000010,   2, 0x09000160) /* MotionTable */
     , (1000010,   8, 0x0600218C) /* Icon */
     , (1000010,  22, 0x3400009D) /* PhysicsEffectTable */;

-- Body parts: zero armor, no damage — proxy is purely a HP sponge
INSERT INTO `weenie_properties_body_part` (`object_Id`, `key`, `d_Type`, `d_Val`, `d_Var`, `base_Armor`, `armor_Vs_Slash`, `armor_Vs_Pierce`, `armor_Vs_Bludgeon`, `armor_Vs_Cold`, `armor_Vs_Fire`, `armor_Vs_Acid`, `armor_Vs_Electric`, `armor_Vs_Nether`, `b_h`, `h_l_f`, `m_l_f`, `l_l_f`, `h_r_f`, `m_r_f`, `l_r_f`, `h_l_b`, `m_l_b`, `l_l_b`, `h_r_b`, `m_r_b`, `l_r_b`)
VALUES (1000010,  0,  4, 0, 0,  0,  0,  0,  0,  0,  0,  0,  0,  0, 1,  0.5,  0.2,    0,  0.5,  0.2,    0,    0,    0,    0,    0,    0,    0) /* Head */
     , (1000010, 10,  4, 0, 0,  0,  0,  0,  0,  0,  0,  0,  0,  0, 2,  0.2,  0.4,  0.5,  0.2,  0.4,  0.5,    0,    0,    0,    0,    0,    0) /* FrontLeg */
     , (1000010, 12,  4, 0, 0,  0,  0,  0,  0,  0,  0,  0,  0,  0, 3,    0,    0, 0.25,    0,    0, 0.25,    0,    0,    0,    0,    0,    0) /* FrontFoot */
     , (1000010, 13,  4, 0, 0,  0,  0,  0,  0,  0,  0,  0,  0,  0, 2,    0,    0,    0,    0,    0,    0,  0.3,  0.4,  0.5,  0.3,  0.4,  0.5) /* RearLeg */
     , (1000010, 15,  4, 0, 0,  0,  0,  0,  0,  0,  0,  0,  0,  0, 3,    0,    0,    0,    0,    0,    0,    0,    0, 0.25,    0,    0, 0.25) /* RearFoot */
     , (1000010, 16,  4, 0, 0,  0,  0,  0,  0,  0,  0,  0,  0,  0, 2,  0.3,  0.4, 0.25,  0.3,  0.4, 0.25,  0.6,  0.5, 0.25,  0.6,  0.5, 0.25) /* Torso */
     , (1000010, 17,  4, 0, 0,  0,  0,  0,  0,  0,  0,  0,  0,  0, 2,    0,    0,    0,    0,    0,    0,  0.1,  0.1,    0,  0.1,  0.1,    0) /* Tail */;

-- Low attributes: Endurance=10 keeps the formula HP contribution near zero
-- (actual HP is set by code at spawn time)
INSERT INTO `weenie_properties_attribute` (`object_Id`, `type`, `init_Level`, `level_From_C_P`, `c_P_Spent`)
VALUES (1000010,   1,  10, 0, 0) /* Strength */
     , (1000010,   2,  10, 0, 0) /* Endurance */
     , (1000010,   3,  10, 0, 0) /* Quickness */
     , (1000010,   4,  10, 0, 0) /* Coordination */
     , (1000010,   5,  10, 0, 0) /* Focus */
     , (1000010,   6,  10, 0, 0) /* Self */;

-- MaxHealth placeholder; overridden at spawn by ComputeBindstoneHp()
INSERT INTO `weenie_properties_attribute_2nd` (`object_Id`, `type`, `init_Level`, `level_From_C_P`, `c_P_Spent`, `current_Level`)
VALUES (1000010,   1, 10000, 0, 0, 10000) /* MaxHealth */
     , (1000010,   3,     0, 0, 0,     0) /* MaxStamina */
     , (1000010,   5,     0, 0, 0,     0) /* MaxMana */;

-- Minimal defense skills (trained, init_Level=1) so attacks and spells always land
INSERT INTO `weenie_properties_skill` (`object_Id`, `type`, `level_From_P_P`, `s_a_c`, `p_p`, `init_Level`, `resistance_At_Last_Check`, `last_Used_Time`)
VALUES (1000010,  6, 0, 2, 0,  1, 0, 0) /* MeleeDefense   Trained */
     , (1000010,  7, 0, 2, 0,  1, 0, 0) /* MissileDefense Trained */
     , (1000010, 15, 0, 2, 0,  1, 0, 0) /* MagicDefense   Trained */;

-- Generation emote: play On animation when spawned
INSERT INTO `weenie_properties_emote` (`object_Id`, `category`, `probability`, `weenie_Class_Id`, `style`, `substyle`, `quest`, `vendor_Type`, `min_Health`, `max_Health`)
VALUES (1000010, 9 /* Generation */, 1, NULL, NULL, NULL, NULL, NULL, NULL, NULL);

SET @parent_id = LAST_INSERT_ID();

INSERT INTO `weenie_properties_emote_action` (`emote_Id`, `order`, `type`, `delay`, `extent`, `motion`, `message`, `test_String`, `min`, `max`, `min_64`, `max_64`, `min_Dbl`, `max_Dbl`, `stat`, `display`, `amount`, `amount_64`, `hero_X_P_64`, `percent`, `spell_Id`, `wealth_Rating`, `treasure_Class`, `treasure_Type`, `p_Script`, `sound`, `destination_Type`, `weenie_Class_Id`, `stack_Size`, `palette`, `shade`, `try_To_Bond`, `obj_Cell_Id`, `origin_X`, `origin_Y`, `origin_Z`, `angles_W`, `angles_X`, `angles_Y`, `angles_Z`)
VALUES (@parent_id, 0, 5 /* Motion */, 0, 1, 0x4000000B /* On */, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
