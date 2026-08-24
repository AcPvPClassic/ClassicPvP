/* -----------------------------------------------------------------------
   Cook-book entries for custom WS10 salvage bag weenies (510036–510041)

   The ACE crafting system looks up recipes by source weenie class ID in
   cook_book.  Our custom weenies are distinct WCIDs from their base salvage
   counterparts, so without these rows the craft system finds no recipe and
   rejects the bag on any target with "X cannot be used on Y."

   This copies every cook_book entry from each base salvage weenie to its
   custom WS10 counterpart — same technique as the 510020–510035 CookBook.

     510036  Agate         (base 21034) — Minor Focus jewelry imbue
     510037  Carnelian     (base 21043) — Minor Strength jewelry imbue
     510038  Lapis Lazuli  (base 21057) — Minor Willpower jewelry imbue
     510039  Rose Quartz   (base 21071) — Minor Quickness jewelry imbue
     510040  Smoky Quartz  (base 21078) — Minor Coordination jewelry imbue
     510041  Mahogany      (base 20988) — missile weapon tinker

   Idempotent: DELETE before INSERT so re-running is safe.
   ----------------------------------------------------------------------- */

-- 510036: Agate  (base 21034)
DELETE FROM `cook_book` WHERE `source_W_C_I_D` = 510036;
INSERT INTO `cook_book` (`recipe_Id`, `source_W_C_I_D`, `target_W_C_I_D`, `last_Modified`)
SELECT `recipe_Id`, 510036, `target_W_C_I_D`, `last_Modified`
FROM `cook_book` WHERE `source_W_C_I_D` = 21034;

-- 510037: Carnelian  (base 21043)
DELETE FROM `cook_book` WHERE `source_W_C_I_D` = 510037;
INSERT INTO `cook_book` (`recipe_Id`, `source_W_C_I_D`, `target_W_C_I_D`, `last_Modified`)
SELECT `recipe_Id`, 510037, `target_W_C_I_D`, `last_Modified`
FROM `cook_book` WHERE `source_W_C_I_D` = 21043;

-- 510038: Lapis Lazuli  (base 21057)
DELETE FROM `cook_book` WHERE `source_W_C_I_D` = 510038;
INSERT INTO `cook_book` (`recipe_Id`, `source_W_C_I_D`, `target_W_C_I_D`, `last_Modified`)
SELECT `recipe_Id`, 510038, `target_W_C_I_D`, `last_Modified`
FROM `cook_book` WHERE `source_W_C_I_D` = 21057;

-- 510039: Rose Quartz  (base 21071)
DELETE FROM `cook_book` WHERE `source_W_C_I_D` = 510039;
INSERT INTO `cook_book` (`recipe_Id`, `source_W_C_I_D`, `target_W_C_I_D`, `last_Modified`)
SELECT `recipe_Id`, 510039, `target_W_C_I_D`, `last_Modified`
FROM `cook_book` WHERE `source_W_C_I_D` = 21071;

-- 510040: Smoky Quartz  (base 21078)
DELETE FROM `cook_book` WHERE `source_W_C_I_D` = 510040;
INSERT INTO `cook_book` (`recipe_Id`, `source_W_C_I_D`, `target_W_C_I_D`, `last_Modified`)
SELECT `recipe_Id`, 510040, `target_W_C_I_D`, `last_Modified`
FROM `cook_book` WHERE `source_W_C_I_D` = 21078;

-- 510041: Mahogany  (base 20988)
DELETE FROM `cook_book` WHERE `source_W_C_I_D` = 510041;
INSERT INTO `cook_book` (`recipe_Id`, `source_W_C_I_D`, `target_W_C_I_D`, `last_Modified`)
SELECT `recipe_Id`, 510041, `target_W_C_I_D`, `last_Modified`
FROM `cook_book` WHERE `source_W_C_I_D` = 20988;
