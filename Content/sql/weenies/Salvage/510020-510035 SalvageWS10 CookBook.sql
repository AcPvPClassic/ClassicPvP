/* -----------------------------------------------------------------------
   Cook-book entries for custom WS10 salvage bag weenies (510020–510035)

   The ACE crafting system looks up recipes by source weenie class ID in
   cook_book.  Our custom weenies (510020–510035) are distinct WCIDs from
   their base salvage counterparts (20985–21086), so without these rows the
   craft system finds no recipe and rejects the bag on any target with the
   message "X cannot be used on Y."

   This script copies every cook_book entry from each base weenie to its
   custom counterpart.  Green Garnet (510023 / base 21050) is omitted
   because it has no cook_book entries in the base data — it has no
   crafting use by design.

   Idempotent: DELETE before INSERT so re-running is safe.
   ----------------------------------------------------------------------- */

-- 510020: Granite  (base 20985)
DELETE FROM `cook_book` WHERE `source_W_C_I_D` = 510020;
INSERT INTO `cook_book` (`recipe_Id`, `source_W_C_I_D`, `target_W_C_I_D`, `last_Modified`)
SELECT `recipe_Id`, 510020, `target_W_C_I_D`, `last_Modified`
FROM `cook_book` WHERE `source_W_C_I_D` = 20985;

-- 510021: Iron  (base 20986)
DELETE FROM `cook_book` WHERE `source_W_C_I_D` = 510021;
INSERT INTO `cook_book` (`recipe_Id`, `source_W_C_I_D`, `target_W_C_I_D`, `last_Modified`)
SELECT `recipe_Id`, 510021, `target_W_C_I_D`, `last_Modified`
FROM `cook_book` WHERE `source_W_C_I_D` = 20986;

-- 510022: Steel  (base 20993)
DELETE FROM `cook_book` WHERE `source_W_C_I_D` = 510022;
INSERT INTO `cook_book` (`recipe_Id`, `source_W_C_I_D`, `target_W_C_I_D`, `last_Modified`)
SELECT `recipe_Id`, 510022, `target_W_C_I_D`, `last_Modified`
FROM `cook_book` WHERE `source_W_C_I_D` = 20993;

-- 510023: Green Garnet  (base 21050) — no cook_book entries in base data; omitted

-- 510024: Opal  (base 21065)
DELETE FROM `cook_book` WHERE `source_W_C_I_D` = 510024;
INSERT INTO `cook_book` (`recipe_Id`, `source_W_C_I_D`, `target_W_C_I_D`, `last_Modified`)
SELECT `recipe_Id`, 510024, `target_W_C_I_D`, `last_Modified`
FROM `cook_book` WHERE `source_W_C_I_D` = 21065;

-- 510025: Aquamarine  (base 21037)
DELETE FROM `cook_book` WHERE `source_W_C_I_D` = 510025;
INSERT INTO `cook_book` (`recipe_Id`, `source_W_C_I_D`, `target_W_C_I_D`, `last_Modified`)
SELECT `recipe_Id`, 510025, `target_W_C_I_D`, `last_Modified`
FROM `cook_book` WHERE `source_W_C_I_D` = 21037;

-- 510026: Black Garnet  (base 21039)
DELETE FROM `cook_book` WHERE `source_W_C_I_D` = 510026;
INSERT INTO `cook_book` (`recipe_Id`, `source_W_C_I_D`, `target_W_C_I_D`, `last_Modified`)
SELECT `recipe_Id`, 510026, `target_W_C_I_D`, `last_Modified`
FROM `cook_book` WHERE `source_W_C_I_D` = 21039;

-- 510027: Black Opal  (base 21040)
DELETE FROM `cook_book` WHERE `source_W_C_I_D` = 510027;
INSERT INTO `cook_book` (`recipe_Id`, `source_W_C_I_D`, `target_W_C_I_D`, `last_Modified`)
SELECT `recipe_Id`, 510027, `target_W_C_I_D`, `last_Modified`
FROM `cook_book` WHERE `source_W_C_I_D` = 21040;

-- 510028: Bloodstone  (base 21041)
DELETE FROM `cook_book` WHERE `source_W_C_I_D` = 510028;
INSERT INTO `cook_book` (`recipe_Id`, `source_W_C_I_D`, `target_W_C_I_D`, `last_Modified`)
SELECT `recipe_Id`, 510028, `target_W_C_I_D`, `last_Modified`
FROM `cook_book` WHERE `source_W_C_I_D` = 21041;

-- 510029: Emerald  (base 21048)
DELETE FROM `cook_book` WHERE `source_W_C_I_D` = 510029;
INSERT INTO `cook_book` (`recipe_Id`, `source_W_C_I_D`, `target_W_C_I_D`, `last_Modified`)
SELECT `recipe_Id`, 510029, `target_W_C_I_D`, `last_Modified`
FROM `cook_book` WHERE `source_W_C_I_D` = 21048;

-- 510030: Fire Opal  (base 21049)
DELETE FROM `cook_book` WHERE `source_W_C_I_D` = 510030;
INSERT INTO `cook_book` (`recipe_Id`, `source_W_C_I_D`, `target_W_C_I_D`, `last_Modified`)
SELECT `recipe_Id`, 510030, `target_W_C_I_D`, `last_Modified`
FROM `cook_book` WHERE `source_W_C_I_D` = 21049;

-- 510031: Imperial Topaz  (base 21054)
DELETE FROM `cook_book` WHERE `source_W_C_I_D` = 510031;
INSERT INTO `cook_book` (`recipe_Id`, `source_W_C_I_D`, `target_W_C_I_D`, `last_Modified`)
SELECT `recipe_Id`, 510031, `target_W_C_I_D`, `last_Modified`
FROM `cook_book` WHERE `source_W_C_I_D` = 21054;

-- 510032: Jet  (base 21056)
DELETE FROM `cook_book` WHERE `source_W_C_I_D` = 510032;
INSERT INTO `cook_book` (`recipe_Id`, `source_W_C_I_D`, `target_W_C_I_D`, `last_Modified`)
SELECT `recipe_Id`, 510032, `target_W_C_I_D`, `last_Modified`
FROM `cook_book` WHERE `source_W_C_I_D` = 21056;

-- 510033: Red Garnet  (base 21069)
DELETE FROM `cook_book` WHERE `source_W_C_I_D` = 510033;
INSERT INTO `cook_book` (`recipe_Id`, `source_W_C_I_D`, `target_W_C_I_D`, `last_Modified`)
SELECT `recipe_Id`, 510033, `target_W_C_I_D`, `last_Modified`
FROM `cook_book` WHERE `source_W_C_I_D` = 21069;

-- 510034: Sunstone  (base 21079)
DELETE FROM `cook_book` WHERE `source_W_C_I_D` = 510034;
INSERT INTO `cook_book` (`recipe_Id`, `source_W_C_I_D`, `target_W_C_I_D`, `last_Modified`)
SELECT `recipe_Id`, 510034, `target_W_C_I_D`, `last_Modified`
FROM `cook_book` WHERE `source_W_C_I_D` = 21079;

-- 510035: White Sapphire  (base 21086)
DELETE FROM `cook_book` WHERE `source_W_C_I_D` = 510035;
INSERT INTO `cook_book` (`recipe_Id`, `source_W_C_I_D`, `target_W_C_I_D`, `last_Modified`)
SELECT `recipe_Id`, 510035, `target_W_C_I_D`, `last_Modified`
FROM `cook_book` WHERE `source_W_C_I_D` = 21086;
