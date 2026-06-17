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
VALUES (0x7018B03C, 4200156, 0x018B0194, 49.9298, -26.7753, 0, 0.021508, 0, 0, -0.999769, False, '2026-06-16 20:25:04'); /* Death Zone Melee(4200156/ace4200156-DeathZoneMelee) - Level: 999 */
/* @teleloc 0x018B0194 [49.929798 -26.775299 0.000000] 0.021508 0.000000 0.000000 -0.999769 */

INSERT INTO `landblock_instance` (`guid`, `weenie_Class_Id`, `obj_Cell_Id`, `origin_X`, `origin_Y`, `origin_Z`, `angles_W`, `angles_X`, `angles_Y`, `angles_Z`, `is_Link_Child`, `last_Modified`)
VALUES (0x7018B03D, 4200156, 0x018B018B, 41.3601, -30.3008, 0, -0.790481, 0, 0, 0.612486, False, '2026-06-16 20:38:45'); /* Death Zone Melee(4200156/ace4200156-DeathZoneMelee) - Level: 999 */
/* @teleloc 0x018B018B [41.360100 -30.300800 0.000000] -0.790481 0.000000 0.000000 0.612486 */

INSERT INTO `landblock_instance` (`guid`, `weenie_Class_Id`, `obj_Cell_Id`, `origin_X`, `origin_Y`, `origin_Z`, `angles_W`, `angles_X`, `angles_Y`, `angles_Z`, `is_Link_Child`, `last_Modified`)
VALUES (0x7018B03E, 4200156, 0x018B01A8, 60.4584, -30.5744, 0, 0.761311, 0, 0, 0.648388, False, '2026-06-16 20:38:50'); /* Death Zone Melee(4200156/ace4200156-DeathZoneMelee) - Level: 999 */
/* @teleloc 0x018B01A8 [60.458401 -30.574400 0.000000] 0.761311 0.000000 0.000000 0.648388 */

INSERT INTO `landblock_instance` (`guid`, `weenie_Class_Id`, `obj_Cell_Id`, `origin_X`, `origin_Y`, `origin_Z`, `angles_W`, `angles_X`, `angles_Y`, `angles_Z`, `is_Link_Child`, `last_Modified`)
VALUES (0x7018B03F, 4200156, 0x018B0199, 49.4224, -38.8015, 0, 0.991913, 0, 0, -0.126918, False, '2026-06-16 20:38:55'); /* Death Zone Melee(4200156/ace4200156-DeathZoneMelee) - Level: 999 */
/* @teleloc 0x018B0199 [49.422401 -38.801498 0.000000] 0.991913 0.000000 0.000000 -0.126918 */

INSERT INTO `landblock_instance` (`guid`, `weenie_Class_Id`, `obj_Cell_Id`, `origin_X`, `origin_Y`, `origin_Z`, `angles_W`, `angles_X`, `angles_Y`, `angles_Z`, `is_Link_Child`, `last_Modified`)
VALUES (0x7018B040, 4200155, 0x018B0195, 50.3081, -34.56, 0, 0.999773, 0, 0, 0.02129, False, '2026-06-16 20:39:02'); /* Death Zone(4200155/ace4200155-DeathZoneMagic) - Level: 999 */
/* @teleloc 0x018B0195 [50.308102 -34.560001 0.000000] 0.999773 0.000000 0.000000 0.021290 */
