/* -----------------------------------------------------------------------
   Custom WS10 Salvage Bag Weenies (510036-510041) - Extras

   Adds full WS10 salvage bags for the six materials that feed the Tinkering
   Lotto minor-cantrip / missile-damage handlers but had no custom WS10
   variant yet (only Bloodstone / 510028 did):

     WCID    Material        Base    Mat#  Tinker Lotto effect
     510036  Agate           21034    10   Minor Focus imbue upgrade
     510037  Carnelian       21043    18   Minor Strength imbue upgrade
     510038  Lapis Lazuli    21057    28   Minor Willpower imbue upgrade
     510039  Rose Quartz     21071    37   Minor Quickness imbue upgrade
     510040  Smoky Quartz    21078    40   Minor Coordination imbue upgrade
     510041  Mahogany        20988    74   +4% Missile Damage Mod

   Each new weenie is CLONED from its base salvage weenie via INSERT ... SELECT
   (same technique the companion CookBook script uses), so material, palette,
   icon, colour underlay, name ("Salvaged X") and imbue Use text are copied
   verbatim from the retail-correct base - no hand-transcribed cosmetics.
   We then bake in the three WS10 properties the base weenies lack
   (Structure=100, ItemWorkmanship=100, NumItemsInMaterial=10) plus
   MaxStructure=100, so a bag spawned from a loot box / generator is a proper
   full WS10 bag instead of an empty 0-workmanship bag. This mirrors exactly
   what 510028 (Bloodstone) does relative to base 21041.

   Idempotent: every weenie + its property rows are deleted before re-insert.
   No stored procedures / DELIMITER - plain statements only, matching the rest
   of the Content SQL so any semicolon-splitting loader applies it cleanly.
   ----------------------------------------------------------------------- */

/* ---- 510036: Salvaged Agate (WS10) - clone of base 21034 - Minor Focus imbue upgrade ---- */
DELETE FROM `weenie_properties_int`    WHERE `object_Id` = 510036;
DELETE FROM `weenie_properties_string` WHERE `object_Id` = 510036;
DELETE FROM `weenie_properties_d_i_d`  WHERE `object_Id` = 510036;
DELETE FROM `weenie_properties_float`  WHERE `object_Id` = 510036;
DELETE FROM `weenie_properties_bool`   WHERE `object_Id` = 510036;
DELETE FROM `weenie`                   WHERE `class_Id`  = 510036;

INSERT INTO `weenie` (`class_Id`, `class_Name`, `type`, `last_Modified`)
SELECT 510036, 'ace510036-salvageagate', `type`, '2026-08-24 00:00:00' FROM `weenie` WHERE `class_Id` = 21034;

INSERT INTO `weenie_properties_int`    (`object_Id`, `type`, `value`) SELECT 510036, `type`, `value` FROM `weenie_properties_int`    WHERE `object_Id` = 21034;
INSERT INTO `weenie_properties_string` (`object_Id`, `type`, `value`) SELECT 510036, `type`, `value` FROM `weenie_properties_string` WHERE `object_Id` = 21034;
INSERT INTO `weenie_properties_d_i_d`  (`object_Id`, `type`, `value`) SELECT 510036, `type`, `value` FROM `weenie_properties_d_i_d`  WHERE `object_Id` = 21034;
INSERT INTO `weenie_properties_float`  (`object_Id`, `type`, `value`) SELECT 510036, `type`, `value` FROM `weenie_properties_float`  WHERE `object_Id` = 21034;
INSERT INTO `weenie_properties_bool`   (`object_Id`, `type`, `value`) SELECT 510036, `type`, `value` FROM `weenie_properties_bool`   WHERE `object_Id` = 21034;

/* Bake in the WS10 full-bag properties the base weenies do not set */
DELETE FROM `weenie_properties_int` WHERE `object_Id` = 510036 AND `type` IN (91, 92, 105, 170);
INSERT INTO `weenie_properties_int` (`object_Id`, `type`, `value`) VALUES
   (510036,  91, 100)   /* MaxStructure */
 , (510036,  92, 100)   /* Structure */
 , (510036, 105, 100)   /* ItemWorkmanship */
 , (510036, 170,  10);  /* NumItemsInMaterial */

/* ---- 510037: Salvaged Carnelian (WS10) - clone of base 21043 - Minor Strength imbue upgrade ---- */
DELETE FROM `weenie_properties_int`    WHERE `object_Id` = 510037;
DELETE FROM `weenie_properties_string` WHERE `object_Id` = 510037;
DELETE FROM `weenie_properties_d_i_d`  WHERE `object_Id` = 510037;
DELETE FROM `weenie_properties_float`  WHERE `object_Id` = 510037;
DELETE FROM `weenie_properties_bool`   WHERE `object_Id` = 510037;
DELETE FROM `weenie`                   WHERE `class_Id`  = 510037;

INSERT INTO `weenie` (`class_Id`, `class_Name`, `type`, `last_Modified`)
SELECT 510037, 'ace510037-salvagecarnelian', `type`, '2026-08-24 00:00:00' FROM `weenie` WHERE `class_Id` = 21043;

INSERT INTO `weenie_properties_int`    (`object_Id`, `type`, `value`) SELECT 510037, `type`, `value` FROM `weenie_properties_int`    WHERE `object_Id` = 21043;
INSERT INTO `weenie_properties_string` (`object_Id`, `type`, `value`) SELECT 510037, `type`, `value` FROM `weenie_properties_string` WHERE `object_Id` = 21043;
INSERT INTO `weenie_properties_d_i_d`  (`object_Id`, `type`, `value`) SELECT 510037, `type`, `value` FROM `weenie_properties_d_i_d`  WHERE `object_Id` = 21043;
INSERT INTO `weenie_properties_float`  (`object_Id`, `type`, `value`) SELECT 510037, `type`, `value` FROM `weenie_properties_float`  WHERE `object_Id` = 21043;
INSERT INTO `weenie_properties_bool`   (`object_Id`, `type`, `value`) SELECT 510037, `type`, `value` FROM `weenie_properties_bool`   WHERE `object_Id` = 21043;

/* Bake in the WS10 full-bag properties the base weenies do not set */
DELETE FROM `weenie_properties_int` WHERE `object_Id` = 510037 AND `type` IN (91, 92, 105, 170);
INSERT INTO `weenie_properties_int` (`object_Id`, `type`, `value`) VALUES
   (510037,  91, 100)   /* MaxStructure */
 , (510037,  92, 100)   /* Structure */
 , (510037, 105, 100)   /* ItemWorkmanship */
 , (510037, 170,  10);  /* NumItemsInMaterial */

/* ---- 510038: Salvaged Lapis Lazuli (WS10) - clone of base 21057 - Minor Willpower imbue upgrade ---- */
DELETE FROM `weenie_properties_int`    WHERE `object_Id` = 510038;
DELETE FROM `weenie_properties_string` WHERE `object_Id` = 510038;
DELETE FROM `weenie_properties_d_i_d`  WHERE `object_Id` = 510038;
DELETE FROM `weenie_properties_float`  WHERE `object_Id` = 510038;
DELETE FROM `weenie_properties_bool`   WHERE `object_Id` = 510038;
DELETE FROM `weenie`                   WHERE `class_Id`  = 510038;

INSERT INTO `weenie` (`class_Id`, `class_Name`, `type`, `last_Modified`)
SELECT 510038, 'ace510038-salvagelapislazuli', `type`, '2026-08-24 00:00:00' FROM `weenie` WHERE `class_Id` = 21057;

INSERT INTO `weenie_properties_int`    (`object_Id`, `type`, `value`) SELECT 510038, `type`, `value` FROM `weenie_properties_int`    WHERE `object_Id` = 21057;
INSERT INTO `weenie_properties_string` (`object_Id`, `type`, `value`) SELECT 510038, `type`, `value` FROM `weenie_properties_string` WHERE `object_Id` = 21057;
INSERT INTO `weenie_properties_d_i_d`  (`object_Id`, `type`, `value`) SELECT 510038, `type`, `value` FROM `weenie_properties_d_i_d`  WHERE `object_Id` = 21057;
INSERT INTO `weenie_properties_float`  (`object_Id`, `type`, `value`) SELECT 510038, `type`, `value` FROM `weenie_properties_float`  WHERE `object_Id` = 21057;
INSERT INTO `weenie_properties_bool`   (`object_Id`, `type`, `value`) SELECT 510038, `type`, `value` FROM `weenie_properties_bool`   WHERE `object_Id` = 21057;

/* Bake in the WS10 full-bag properties the base weenies do not set */
DELETE FROM `weenie_properties_int` WHERE `object_Id` = 510038 AND `type` IN (91, 92, 105, 170);
INSERT INTO `weenie_properties_int` (`object_Id`, `type`, `value`) VALUES
   (510038,  91, 100)   /* MaxStructure */
 , (510038,  92, 100)   /* Structure */
 , (510038, 105, 100)   /* ItemWorkmanship */
 , (510038, 170,  10);  /* NumItemsInMaterial */

/* ---- 510039: Salvaged Rose Quartz (WS10) - clone of base 21071 - Minor Quickness imbue upgrade ---- */
DELETE FROM `weenie_properties_int`    WHERE `object_Id` = 510039;
DELETE FROM `weenie_properties_string` WHERE `object_Id` = 510039;
DELETE FROM `weenie_properties_d_i_d`  WHERE `object_Id` = 510039;
DELETE FROM `weenie_properties_float`  WHERE `object_Id` = 510039;
DELETE FROM `weenie_properties_bool`   WHERE `object_Id` = 510039;
DELETE FROM `weenie`                   WHERE `class_Id`  = 510039;

INSERT INTO `weenie` (`class_Id`, `class_Name`, `type`, `last_Modified`)
SELECT 510039, 'ace510039-salvagerosequartz', `type`, '2026-08-24 00:00:00' FROM `weenie` WHERE `class_Id` = 21071;

INSERT INTO `weenie_properties_int`    (`object_Id`, `type`, `value`) SELECT 510039, `type`, `value` FROM `weenie_properties_int`    WHERE `object_Id` = 21071;
INSERT INTO `weenie_properties_string` (`object_Id`, `type`, `value`) SELECT 510039, `type`, `value` FROM `weenie_properties_string` WHERE `object_Id` = 21071;
INSERT INTO `weenie_properties_d_i_d`  (`object_Id`, `type`, `value`) SELECT 510039, `type`, `value` FROM `weenie_properties_d_i_d`  WHERE `object_Id` = 21071;
INSERT INTO `weenie_properties_float`  (`object_Id`, `type`, `value`) SELECT 510039, `type`, `value` FROM `weenie_properties_float`  WHERE `object_Id` = 21071;
INSERT INTO `weenie_properties_bool`   (`object_Id`, `type`, `value`) SELECT 510039, `type`, `value` FROM `weenie_properties_bool`   WHERE `object_Id` = 21071;

/* Bake in the WS10 full-bag properties the base weenies do not set */
DELETE FROM `weenie_properties_int` WHERE `object_Id` = 510039 AND `type` IN (91, 92, 105, 170);
INSERT INTO `weenie_properties_int` (`object_Id`, `type`, `value`) VALUES
   (510039,  91, 100)   /* MaxStructure */
 , (510039,  92, 100)   /* Structure */
 , (510039, 105, 100)   /* ItemWorkmanship */
 , (510039, 170,  10);  /* NumItemsInMaterial */

/* ---- 510040: Salvaged Smoky Quartz (WS10) - clone of base 21078 - Minor Coordination imbue upgrade ---- */
DELETE FROM `weenie_properties_int`    WHERE `object_Id` = 510040;
DELETE FROM `weenie_properties_string` WHERE `object_Id` = 510040;
DELETE FROM `weenie_properties_d_i_d`  WHERE `object_Id` = 510040;
DELETE FROM `weenie_properties_float`  WHERE `object_Id` = 510040;
DELETE FROM `weenie_properties_bool`   WHERE `object_Id` = 510040;
DELETE FROM `weenie`                   WHERE `class_Id`  = 510040;

INSERT INTO `weenie` (`class_Id`, `class_Name`, `type`, `last_Modified`)
SELECT 510040, 'ace510040-salvagesmokyquartz', `type`, '2026-08-24 00:00:00' FROM `weenie` WHERE `class_Id` = 21078;

INSERT INTO `weenie_properties_int`    (`object_Id`, `type`, `value`) SELECT 510040, `type`, `value` FROM `weenie_properties_int`    WHERE `object_Id` = 21078;
INSERT INTO `weenie_properties_string` (`object_Id`, `type`, `value`) SELECT 510040, `type`, `value` FROM `weenie_properties_string` WHERE `object_Id` = 21078;
INSERT INTO `weenie_properties_d_i_d`  (`object_Id`, `type`, `value`) SELECT 510040, `type`, `value` FROM `weenie_properties_d_i_d`  WHERE `object_Id` = 21078;
INSERT INTO `weenie_properties_float`  (`object_Id`, `type`, `value`) SELECT 510040, `type`, `value` FROM `weenie_properties_float`  WHERE `object_Id` = 21078;
INSERT INTO `weenie_properties_bool`   (`object_Id`, `type`, `value`) SELECT 510040, `type`, `value` FROM `weenie_properties_bool`   WHERE `object_Id` = 21078;

/* Bake in the WS10 full-bag properties the base weenies do not set */
DELETE FROM `weenie_properties_int` WHERE `object_Id` = 510040 AND `type` IN (91, 92, 105, 170);
INSERT INTO `weenie_properties_int` (`object_Id`, `type`, `value`) VALUES
   (510040,  91, 100)   /* MaxStructure */
 , (510040,  92, 100)   /* Structure */
 , (510040, 105, 100)   /* ItemWorkmanship */
 , (510040, 170,  10);  /* NumItemsInMaterial */

/* ---- 510041: Salvaged Mahogany (WS10) - clone of base 20988 - +4% Missile Damage Mod ---- */
DELETE FROM `weenie_properties_int`    WHERE `object_Id` = 510041;
DELETE FROM `weenie_properties_string` WHERE `object_Id` = 510041;
DELETE FROM `weenie_properties_d_i_d`  WHERE `object_Id` = 510041;
DELETE FROM `weenie_properties_float`  WHERE `object_Id` = 510041;
DELETE FROM `weenie_properties_bool`   WHERE `object_Id` = 510041;
DELETE FROM `weenie`                   WHERE `class_Id`  = 510041;

INSERT INTO `weenie` (`class_Id`, `class_Name`, `type`, `last_Modified`)
SELECT 510041, 'ace510041-salvagemahogany', `type`, '2026-08-24 00:00:00' FROM `weenie` WHERE `class_Id` = 20988;

INSERT INTO `weenie_properties_int`    (`object_Id`, `type`, `value`) SELECT 510041, `type`, `value` FROM `weenie_properties_int`    WHERE `object_Id` = 20988;
INSERT INTO `weenie_properties_string` (`object_Id`, `type`, `value`) SELECT 510041, `type`, `value` FROM `weenie_properties_string` WHERE `object_Id` = 20988;
INSERT INTO `weenie_properties_d_i_d`  (`object_Id`, `type`, `value`) SELECT 510041, `type`, `value` FROM `weenie_properties_d_i_d`  WHERE `object_Id` = 20988;
INSERT INTO `weenie_properties_float`  (`object_Id`, `type`, `value`) SELECT 510041, `type`, `value` FROM `weenie_properties_float`  WHERE `object_Id` = 20988;
INSERT INTO `weenie_properties_bool`   (`object_Id`, `type`, `value`) SELECT 510041, `type`, `value` FROM `weenie_properties_bool`   WHERE `object_Id` = 20988;

/* Bake in the WS10 full-bag properties the base weenies do not set */
DELETE FROM `weenie_properties_int` WHERE `object_Id` = 510041 AND `type` IN (91, 92, 105, 170);
INSERT INTO `weenie_properties_int` (`object_Id`, `type`, `value`) VALUES
   (510041,  91, 100)   /* MaxStructure */
 , (510041,  92, 100)   /* Structure */
 , (510041, 105, 100)   /* ItemWorkmanship */
 , (510041, 170,  10);  /* NumItemsInMaterial */
