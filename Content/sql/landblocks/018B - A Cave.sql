DELETE FROM `landblock_instance` WHERE `landblock` = 0x018B;

INSERT INTO `landblock_instance` (`guid`, `weenie_Class_Id`, `obj_Cell_Id`, `origin_X`, `origin_Y`, `origin_Z`, `angles_W`, `angles_X`, `angles_Y`, `angles_Z`, `is_Link_Child`, `last_Modified`)
VALUES (0x7018B006,  1912, 0x018B0106, 60.4138, -107.043, -17.9611, -0.707107, 0, 0, -0.707107, False, '2005-02-09 10:00:00'); /* Chest(1912/chestfoodhigh) - Content - DeathTreasureType: T4_Warrior(T4) */
/* @teleloc 0x018B0106 [60.413799 -107.042999 -17.961100] -0.707107 0.000000 0.000000 -0.707107 */

INSERT INTO `landblock_instance` (`guid`, `weenie_Class_Id`, `obj_Cell_Id`, `origin_X`, `origin_Y`, `origin_Z`, `angles_W`, `angles_X`, `angles_Y`, `angles_Z`, `is_Link_Child`, `last_Modified`)
VALUES (0x7018B007,  1912, 0x018B0106, 60.3001, -105.784, -17.9611, -0.707107, 0, 0, -0.707107, False, '2005-02-09 10:00:00'); /* Chest(1912/chestfoodhigh) - Content - DeathTreasureType: T4_Warrior(T4) */
/* @teleloc 0x018B0106 [60.300098 -105.783997 -17.961100] -0.707107 0.000000 0.000000 -0.707107 */

INSERT INTO `landblock_instance` (`guid`, `weenie_Class_Id`, `obj_Cell_Id`, `origin_X`, `origin_Y`, `origin_Z`, `angles_W`, `angles_X`, `angles_Y`, `angles_Z`, `is_Link_Child`, `last_Modified`)
VALUES (0x7018B00C,  3645, 0x018B0109, 67.6217, -109.169, -17.8339, 0, 0, 0, -1,  True, '2005-02-09 10:00:00'); /* Tibri's Fire Spear(3645/tibrisfirespear) */
/* @teleloc 0x018B0109 [67.621696 -109.168999 -17.833900] 0.000000 0.000000 0.000000 -1.000000 */

INSERT INTO `landblock_instance` (`guid`, `weenie_Class_Id`, `obj_Cell_Id`, `origin_X`, `origin_Y`, `origin_Z`, `angles_W`, `angles_X`, `angles_Y`, `angles_Z`, `is_Link_Child`, `last_Modified`)
VALUES (0x7018B01F,  1929, 0x018B0135, 118.809, -124.099, -12, 0, 0, 0, -1, False, '2005-02-09 10:00:00'); /* Chest(1929/chestmoneyhigh) - Content - DeathTreasureType: T4_Chest_Money(T4) */
/* @teleloc 0x018B0135 [118.808998 -124.098999 -12.000000] 0.000000 0.000000 0.000000 -1.000000 */

INSERT INTO `landblock_instance` (`guid`, `weenie_Class_Id`, `obj_Cell_Id`, `origin_X`, `origin_Y`, `origin_Z`, `angles_W`, `angles_X`, `angles_Y`, `angles_Z`, `is_Link_Child`, `last_Modified`)
VALUES (0x7018B026,  1946, 0x018B013C, 125.697, -118.828, -12, -0.707107, 0, 0, -0.707107, False, '2005-02-09 10:00:00'); /* Chest(1946/chestwarriorlow) - Content - DeathTreasureType: T2_Chest_Warrior(T2) */
/* @teleloc 0x018B013C [125.696999 -118.828003 -12.000000] -0.707107 0.000000 0.000000 -0.707107 */

INSERT INTO `landblock_instance` (`guid`, `weenie_Class_Id`, `obj_Cell_Id`, `origin_X`, `origin_Y`, `origin_Z`, `angles_W`, `angles_X`, `angles_Y`, `angles_Z`, `is_Link_Child`, `last_Modified`)
VALUES (0x7018B036,  3607, 0x018B01C7, 80.6764, -60.4128, 0.005, 0.89668, 0, 0, -0.44268,  True, '2005-02-09 10:00:00'); /* Tibri the Cavedweller(3607/tibrithecavedweller) - Level: 40 - Generates - Smock(2589/smock) / Breeches(2602/breechesloose) / Boots(2606/boots) */
/* @teleloc 0x018B01C7 [80.676399 -60.412800 0.005000] 0.896680 0.000000 0.000000 -0.442680 */

INSERT INTO `landblock_instance` (`guid`, `weenie_Class_Id`, `obj_Cell_Id`, `origin_X`, `origin_Y`, `origin_Z`, `angles_W`, `angles_X`, `angles_Y`, `angles_Z`, `is_Link_Child`, `last_Modified`)
VALUES (0x7018B038,  5085, 0x018B0109, 68.6784, -108.938, -17.9878, 0, 0, 0, -1, False, '2005-02-09 10:00:00'); /* Linkable Item Gen - 25 seconds(5085/linkitemgen25seconds) - Generates - Place Holder Object(3666/placeholder) */
/* @teleloc 0x018B0109 [68.678398 -108.938004 -17.987801] 0.000000 0.000000 0.000000 -1.000000 */

INSERT INTO `landblock_instance_link` (`parent_GUID`, `child_GUID`, `last_Modified`)
VALUES (0x7018B038, 0x7018B00C, '2005-02-09 10:00:00')/* Tibri's Fire Spear (3645/tibrisfirespear) */;

INSERT INTO `landblock_instance` (`guid`, `weenie_Class_Id`, `obj_Cell_Id`, `origin_X`, `origin_Y`, `origin_Z`, `angles_W`, `angles_X`, `angles_Y`, `angles_Z`, `is_Link_Child`, `last_Modified`)
VALUES (0x7018B03B,  3951, 0x018B01C7, 80, -60, 0, 0.707107, 0, 0, -0.707107, False, '2005-02-09 10:00:00'); /* Linkable Monster Gen (1 hour)(3951/linkmonstergen1hour) - Generates - Place Holder Object(3666/placeholder) */
/* @teleloc 0x018B01C7 [80.000000 -60.000000 0.000000] 0.707107 0.000000 0.000000 -0.707107 */

INSERT INTO `landblock_instance_link` (`parent_GUID`, `child_GUID`, `last_Modified`)
VALUES (0x7018B03B, 0x7018B036, '2005-02-09 10:00:00')/* Tibri the Cavedweller (3607/tibrithecavedweller) - Level: 40 - Generates - Smock(2589/smock) / Breeches(2602/breechesloose) / Boots(2606/boots) */;

INSERT INTO `landblock_instance` (`guid`, `weenie_Class_Id`, `obj_Cell_Id`, `origin_X`, `origin_Y`, `origin_Z`, `angles_W`, `angles_X`, `angles_Y`, `angles_Z`, `is_Link_Child`, `last_Modified`)
VALUES (0x7018B053, 42025456, 0x018B0197, 49.8959, -25.4499, -0.068, 0.043729, 0, 0, -0.999044, False, '2026-07-09 14:24:49'); /* Reinforced Door(42025456/unopenabledoor) - Locked(999999) */
/* @teleloc 0x018B0197 [49.895901 -25.449900 -0.068000] 0.043729 0.000000 0.000000 -0.999044 */

INSERT INTO `landblock_instance` (`guid`, `weenie_Class_Id`, `obj_Cell_Id`, `origin_X`, `origin_Y`, `origin_Z`, `angles_W`, `angles_X`, `angles_Y`, `angles_Z`, `is_Link_Child`, `last_Modified`)
VALUES (0x7018B054, 4200155, 0x018B0194, 48.5161, -27.9876, 0, 0.731606, 0, 0, -0.681727, False, '2026-07-09 15:34:14'); /* Death Zone(4200155/ace4200155-DeathZoneHotspot) */
/* @teleloc 0x018B0194 [48.516102 -27.987600 0.000000] 0.731606 0.000000 0.000000 -0.681727 */

INSERT INTO `landblock_instance` (`guid`, `weenie_Class_Id`, `obj_Cell_Id`, `origin_X`, `origin_Y`, `origin_Z`, `angles_W`, `angles_X`, `angles_Y`, `angles_Z`, `is_Link_Child`, `last_Modified`)
VALUES (0x7018B055, 4200155, 0x018B0194, 52.4881, -28.1331, 0, -0.863327, 0, 0, -0.504646, False, '2026-07-09 15:34:22'); /* Death Zone(4200155/ace4200155-DeathZoneHotspot) */
/* @teleloc 0x018B0194 [52.488098 -28.133101 0.000000] -0.863327 0.000000 0.000000 -0.504646 */

INSERT INTO `landblock_instance` (`guid`, `weenie_Class_Id`, `obj_Cell_Id`, `origin_X`, `origin_Y`, `origin_Z`, `angles_W`, `angles_X`, `angles_Y`, `angles_Z`, `is_Link_Child`, `last_Modified`)
VALUES (0x7018B056, 4200155, 0x018B0194, 46.3557, -29.6281, 0, -0.963496, 0, 0, 0.267722, False, '2026-07-09 15:34:29'); /* Death Zone(4200155/ace4200155-DeathZoneHotspot) */
/* @teleloc 0x018B0194 [46.355701 -29.628099 0.000000] -0.963496 0.000000 0.000000 0.267722 */

INSERT INTO `landblock_instance` (`guid`, `weenie_Class_Id`, `obj_Cell_Id`, `origin_X`, `origin_Y`, `origin_Z`, `angles_W`, `angles_X`, `angles_Y`, `angles_Z`, `is_Link_Child`, `last_Modified`)
VALUES (0x7018B057, 4200155, 0x018B0194, 53.7903, -30.0492, 0, -0.942263, 0, 0, -0.334874, False, '2026-07-09 15:34:32'); /* Death Zone(4200155/ace4200155-DeathZoneHotspot) */
/* @teleloc 0x018B0194 [53.790298 -30.049200 0.000000] -0.942263 0.000000 0.000000 -0.334874 */

INSERT INTO `landblock_instance` (`guid`, `weenie_Class_Id`, `obj_Cell_Id`, `origin_X`, `origin_Y`, `origin_Z`, `angles_W`, `angles_X`, `angles_Y`, `angles_Z`, `is_Link_Child`, `last_Modified`)
VALUES (0x7018B058, 4200155, 0x018B0194, 50.4917, -27.4995, 0, 0.029971, 0, 0, 0.999551, False, '2026-07-09 19:06:00'); /* Death Zone(4200155/ace4200155-DeathZoneHotspot) */
/* @teleloc 0x018B0194 [50.491699 -27.499500 0.000000] 0.029971 0.000000 0.000000 0.999551 */
