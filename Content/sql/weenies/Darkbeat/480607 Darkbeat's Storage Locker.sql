DELETE FROM `weenie` WHERE `class_Id` = 480607;

INSERT INTO `weenie` (`class_Id`, `class_Name`, `type`, `last_Modified`)
VALUES (480607, 'ace480607-arenarewardchest', 20, '2021-11-01 00:00:00') /* Chest */;

INSERT INTO `weenie_properties_int` (`object_Id`, `type`, `value`)
VALUES (480607,   1,        512) /* ItemType - Container */
     , (480607,   5,       9000) /* EncumbranceVal */
     , (480607,   6,         -1) /* ItemsCapacity */
     , (480607,   7,         -1) /* ContainersCapacity */
     , (480607,   8,       3000) /* Mass */
     , (480607,  16,         48) /* ItemUseable - ViewedRemote */
     , (480607,  19,       2500) /* Value */
     , (480607,  38,       9999) /* ResistLockpick */
     , (480607,  81,          3) /* MaxGeneratedObjects */
     , (480607,  82,          3) /* InitGeneratedObjects */
     , (480607,  93,       1048) /* PhysicsState - ReportCollisions, IgnoreCollisions, Gravity */
     , (480607,  96,        500) /* EncumbranceCapacity */
     , (480607, 100,          1) /* GeneratorType - Relative */;

INSERT INTO `weenie_properties_bool` (`object_Id`, `type`, `value`)
VALUES (480607,   1, True ) /* Stuck */
     , (480607,   2, False) /* Open */
     , (480607,   3, True ) /* Locked */
     , (480607,  12, True ) /* ReportCollisions */
     , (480607,  13, False) /* Ethereal */
     , (480607,  33, False) /* ResetMessagePending */
     , (480607,  34, False) /* DefaultOpen */
     , (480607,  35, True ) /* DefaultLocked */
     , (480607,  86, True ) /* ChestRegenOnClose */;

INSERT INTO `weenie_properties_float` (`object_Id`, `type`, `value`)
VALUES (480607,  39,       2) /* DefaultScale */
     , (480607,  41,      60) /* RegenerationInterval */
     , (480607,  43,       1) /* GeneratorRadius */
     , (480607,  54,       1) /* UseRadius */;

INSERT INTO `weenie_properties_string` (`object_Id`, `type`, `value`)
VALUES (480607,   1, 'Darkbeat''s Storage Locker') /* Name */
     , (480607,  12, 'darkbeatkey') /* LockCode */;

INSERT INTO `weenie_properties_d_i_d` (`object_Id`, `type`, `value`)
VALUES (480607,   1, 0x02000F7A) /* Setup */
     , (480607,   2, 0x09000004) /* MotionTable */
     , (480607,   3, 0x20000021) /* SoundTable */
     , (480607,   7, 0x10000567) /* ClothingBase */
     , (480607,   8, 0x0600344A) /* Icon */
     , (480607,  22, 0x3400002B) /* PhysicsEffectTable */;

/* -----------------------------------------------------------------------
   Generator table — cumulative probability bands
   -1 entry always spawns (tier 6 loot profile).
   Probability 1.0 entry (Drawing) uses when_Create=1 (regen on destroy)
   and is always present in the chest.

   Salvage bags occupy the first ~8.95% of the cumulative range (~0.81% each).
   Consumables occupy the last 10% of the range (4 entries × 2.5% each).
   All other entries are scaled by 0.8953 to preserve relative weighting.

   Salvage bands (11 entries × ~0.0081 = ~8.95% total):
     Sunstone          0.0000 – 0.0081   (0.81%)
     Red Garnet        0.0081 – 0.0163   (0.82%)
     Black Garnet      0.0163 – 0.0244   (0.81%)
     Imperial Topaz    0.0244 – 0.0326   (0.82%)
     Jet               0.0326 – 0.0407   (0.81%)
     Aquamarine        0.0407 – 0.0488   (0.81%)
     White Sapphire    0.0488 – 0.0569   (0.81%)
     Emerald           0.0569 – 0.0651   (0.82%)
     Fire Opal         0.0651 – 0.0732   (0.81%)
     Black Opal        0.0732 – 0.0814   (0.82%)
     Bloodstone        0.0814 – 0.0895   (0.81%)

   Consumable bands (4 entries × 0.025 = 10% total):
     Treated Healing Kit   0.8550 – 0.8800   (2.5%)
     Tumerok Salted Meat   0.8800 – 0.9050   (2.5%)
     Mana Philtre          0.9050 – 0.9300   (2.5%)
     Stamina Philtre       0.9300 – 0.9550   (2.5%)
   ----------------------------------------------------------------------- */

INSERT INTO `weenie_properties_generator` (`object_Id`, `probability`, `weenie_Class_Id`, `delay`, `init_Create`, `max_Create`, `when_Create`, `where_Create`, `stack_Size`, `palette_Id`, `shade`, `obj_Cell_Id`, `origin_X`, `origin_Y`, `origin_Z`, `angles_W`, `angles_X`, `angles_Y`, `angles_Z`)
VALUES (480607,    -1, 10000, 1, 1,   1, 2, 72,   -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Always: Tier 6 loot profile */
     , (480607, 0.0081, 510034, 1, 1, 1, 2,  8,   -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Sunstone Salvage WS10 - Armor Rend */
     , (480607, 0.0163, 510033, 1, 1, 1, 2,  8,   -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Red Garnet Salvage WS10 - Fire Rend */
     , (480607, 0.0244, 510026, 1, 1, 1, 2,  8,   -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Black Garnet Salvage WS10 - Pierce Rend/Imbue */
     , (480607, 0.0326, 510031, 1, 1, 1, 2,  8,   -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Imperial Topaz Salvage WS10 - Slash Rend/Imbue */
     , (480607, 0.0407, 510032, 1, 1, 1, 2,  8,   -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Jet Salvage WS10 - Lightning Rend/Imbue */
     , (480607, 0.0488, 510025, 1, 1, 1, 2,  8,   -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Aquamarine Salvage WS10 - Cold Rend/Imbue */
     , (480607, 0.0569, 510035, 1, 1, 1, 2,  8,   -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* White Sapphire Salvage WS10 - Bludgeon Rend */
     , (480607, 0.0651, 510029, 1, 1, 1, 2,  8,   -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Emerald Salvage WS10 - Acid Rend/Imbue */
     , (480607, 0.0732, 510030, 1, 1, 1, 2,  8,   -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Fire Opal Salvage WS10 - Crippling Blow */
     , (480607, 0.0814, 510027, 1, 1, 1, 2,  8,   -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Black Opal Salvage WS10 - Critical Strike */
     , (480607, 0.0895, 510028, 1, 1, 1, 2,  8,   -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Bloodstone Salvage WS10 - Minor Endurance Imbue */
     , (480607, 0.0916, 36619, 1, 1,  1, 2,  8,   -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Aquamarine Foolproof */
     , (480607, 0.0936, 36620, 1, 1,  1, 2,  8,   -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Black Garnet Foolproof */
     , (480607, 0.0957, 36622, 1, 1,  1, 2,  8,   -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Emerald Foolproof */
     , (480607, 0.0976, 36624, 1, 1,  1, 2,  8,   -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Imperial Topaz Foolproof */
     , (480607, 0.0997, 36625, 1, 1,  1, 2,  8,   -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Jet Foolproof */
     , (480607, 0.1016, 36634, 1, 1,  1, 2,  8,   -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Peridot Foolproof */
     , (480607, 0.1037, 36626, 1, 1,  1, 2,  8,   -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Red Garnet Foolproof */
     , (480607, 0.1056, 36628, 1, 1,  1, 2,  8,   -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* White Sapphire Foolproof */
     , (480607, 0.1218, 36635, 1, 1,  1, 2,  8,   -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Yellow Topaz Foolproof */
     , (480607, 0.1379, 36634, 1, 1,  1, 2,  8,   -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Peridot Foolproof */
     , (480607, 0.1540, 36636, 1, 1,  1, 2,  8,   -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Zircon Foolproof */
     , (480607, 0.1701, 36621, 1, 1,  1, 2,  8,   -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Black Opal Foolproof */
     , (480607, 0.1862, 36623, 1, 1,  1, 2,  8,   -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Fire Opal Foolproof */
     , (480607, 0.2023, 36627, 1, 1,  1, 2,  8,   -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Sunstone Foolproof */
     , (480607, 0.2265, 490326, 1, 1, 2, 2,  8,    2, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Essence of Power x2 */
     , (480607, 0.2507,  52797, 1, 1, 1, 2,  8,    1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Gauntlet Coin x1 */
     , (480607, 0.2829,  38726, 1, 1, 5, 2,  8,    5, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Black Market Gem of Dispelling x5 */
     , (480607, 0.2910, 510000, 1, 1, 1, 2,  8,   -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* A Box */
     , (480607, 0.3313,  52968, 1, 1, 100, 2, 8, 100, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Infused Amber Shard x100 */
     , (480607, 0.3515,  52969, 1, 1, 20, 2, 8,   20, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Corrupted Amber Shard x20 */
     , (480607, 0.3917,  20630, 1, 1, 25, 2, 8,   25, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Trade Note (250,000) x25 */
     , (480607, 0.4119, 490321, 1, 1, 1, 2,  8,   -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Luminance Exchange Token */
     , (480607, 0.4320, 480634, 1, 1, 1, 2,  8,   -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Gem of Greater Luminance */
     , (480607, 0.4521,  53450, 1, 1, 5, 2,  8,    5, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Viridian Rise Deru Portal Gem x5 */
     , (480607, 0.4683, 490364, 1, 1, 1, 2,  8,   -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Hera Vault Key */
     , (480607, 0.5730, 1000002, 1, 1, 20, 2, 8,  20, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* PK Trophy x20 */
     , (480607, 0.7342,  43901, 1, 1, 50, 2, 8,   50, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Promissory Note x50 */
     , (480607, 0.7745, 1000003, 1, 1, 2, 2,  8,   2, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Phial of Bloody Tears x2 */
     , (480607, 0.8147, 480611, 1, 1, 1, 2,  8,   -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Experience Certificate */
     , (480607, 0.8550, 490070, 1, 1, 2, 2,  8,    2, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Water of Enlightenment x2 */
     , (480607, 0.8800,   9229, 1, 1, 1, 2,  8,   -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Treated Healing Kit */
     , (480607, 0.9050,  27669, 1, 1, 20, 2, 8,   20, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Tumerok Salted Meat x20 */
     , (480607, 0.9300,  27321, 1, 1, 20, 2, 8,   20, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Mana Philtre x20 */
     , (480607, 0.9550,  27325, 1, 1, 20, 2, 8,   20, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Stamina Philtre x20 */
     , (480607,      1, 480612, 1, 1, 1, 1,  8,   -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Darkbeat's Golem Drawing - always present */;
