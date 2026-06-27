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
   MaxGeneratedObjects = 3: 1 always-spawn slot (-1 loot profile) +
   2 optional rolls from this table per chest open.

   -1 entry always spawns (tier 6 loot profile).
   Probability 1.0 entry (Drawing) uses when_Create=1 (regen on destroy)
   and acts as the fallback if no other band is matched.

   Band summary (per optional roll):
     Salvage bags     0.0000 – 0.1100   (1% each × 11 = 11%)
     Foolproof gems   0.1100 – 0.1800   (0.5% each × 14 = 7%)
     Essence of Power 0.1800 – 0.2100   (3%)
     Gauntlet Coin    0.2100 – 0.2300   (2%)
     BM Gem Dispel    0.2300 – 0.2600   (3%)
     A Box            0.2600 – 0.2800   (2%)
     Trade Note ×25   0.2800 – 0.3600   (8%)
     PK Trophy ×20    0.3600 – 0.5600   (20%)
     Phial ×2         0.5600 – 0.6600   (10%)
     Healing Kit      0.6600 – 0.6850   (2.5%)
     Salted Meat ×20  0.6850 – 0.7100   (2.5%)
     Mana Philtre ×20 0.7100 – 0.7350   (2.5%)
     Stam Philtre ×20 0.7350 – 0.7600   (2.5%)
     Drawing fallback 0.7600 – 1.0000   (24%)
   ----------------------------------------------------------------------- */

INSERT INTO `weenie_properties_generator` (`object_Id`, `probability`, `weenie_Class_Id`, `delay`, `init_Create`, `max_Create`, `when_Create`, `where_Create`, `stack_Size`, `palette_Id`, `shade`, `obj_Cell_Id`, `origin_X`, `origin_Y`, `origin_Z`, `angles_W`, `angles_X`, `angles_Y`, `angles_Z`)
VALUES (480607,    -1, 10000, 1, 1,   1, 2, 72,   -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Always: Tier 6 loot profile */
     , (480607, 0.0100, 510034, 1, 1, 1, 2,  8,   -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Sunstone Salvage WS10 - Armor Rend */
     , (480607, 0.0200, 510033, 1, 1, 1, 2,  8,   -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Red Garnet Salvage WS10 - Fire Rend */
     , (480607, 0.0300, 510026, 1, 1, 1, 2,  8,   -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Black Garnet Salvage WS10 - Pierce Rend/Imbue */
     , (480607, 0.0400, 510031, 1, 1, 1, 2,  8,   -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Imperial Topaz Salvage WS10 - Slash Rend/Imbue */
     , (480607, 0.0500, 510032, 1, 1, 1, 2,  8,   -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Jet Salvage WS10 - Lightning Rend/Imbue */
     , (480607, 0.0600, 510025, 1, 1, 1, 2,  8,   -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Aquamarine Salvage WS10 - Cold Rend/Imbue */
     , (480607, 0.0700, 510035, 1, 1, 1, 2,  8,   -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* White Sapphire Salvage WS10 - Bludgeon Rend */
     , (480607, 0.0800, 510029, 1, 1, 1, 2,  8,   -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Emerald Salvage WS10 - Acid Rend/Imbue */
     , (480607, 0.0900, 510030, 1, 1, 1, 2,  8,   -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Fire Opal Salvage WS10 - Crippling Blow */
     , (480607, 0.1000, 510027, 1, 1, 1, 2,  8,   -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Black Opal Salvage WS10 - Critical Strike */
     , (480607, 0.1100, 510028, 1, 1, 1, 2,  8,   -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Bloodstone Salvage WS10 - Minor Endurance Imbue */
     , (480607, 0.1150, 36619, 1, 1,  1, 2,  8,   -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Aquamarine Foolproof */
     , (480607, 0.1200, 36620, 1, 1,  1, 2,  8,   -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Black Garnet Foolproof */
     , (480607, 0.1250, 36622, 1, 1,  1, 2,  8,   -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Emerald Foolproof */
     , (480607, 0.1300, 36624, 1, 1,  1, 2,  8,   -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Imperial Topaz Foolproof */
     , (480607, 0.1350, 36625, 1, 1,  1, 2,  8,   -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Jet Foolproof */
     , (480607, 0.1400, 36634, 1, 1,  1, 2,  8,   -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Peridot Foolproof */
     , (480607, 0.1450, 36626, 1, 1,  1, 2,  8,   -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Red Garnet Foolproof */
     , (480607, 0.1500, 36628, 1, 1,  1, 2,  8,   -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* White Sapphire Foolproof */
     , (480607, 0.1550, 36635, 1, 1,  1, 2,  8,   -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Yellow Topaz Foolproof */
     , (480607, 0.1600, 36634, 1, 1,  1, 2,  8,   -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Peridot Foolproof */
     , (480607, 0.1650, 36636, 1, 1,  1, 2,  8,   -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Zircon Foolproof */
     , (480607, 0.1700, 36621, 1, 1,  1, 2,  8,   -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Black Opal Foolproof */
     , (480607, 0.1750, 36623, 1, 1,  1, 2,  8,   -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Fire Opal Foolproof */
     , (480607, 0.1800, 36627, 1, 1,  1, 2,  8,   -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Sunstone Foolproof */
     , (480607, 0.2100, 490326, 1, 1, 2, 2,  8,    2, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Essence of Power x2 */
     , (480607, 0.2300,  52797, 1, 1, 1, 2,  8,    1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Gauntlet Coin x1 */
     , (480607, 0.2600,  38726, 1, 1, 5, 2,  8,    5, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Black Market Gem of Dispelling x5 */
     , (480607, 0.2800, 510000, 1, 1, 1, 2,  8,   -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* A Box */
     , (480607, 0.3600,  20630, 1, 1, 25, 2, 8,   25, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Trade Note (250,000) x25 */
     , (480607, 0.5600, 1000002, 1, 1, 20, 2, 8,  20, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* PK Trophy x20 */
     , (480607, 0.6600, 1000003, 1, 1, 2, 2,  8,   2, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Phial of Bloody Tears x2 */
     , (480607, 0.6850,   9229, 1, 1, 1, 2,  8,   -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Treated Healing Kit */
     , (480607, 0.7100,  27669, 1, 1, 20, 2, 8,   20, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Tumerok Salted Meat x20 */
     , (480607, 0.7350,  27321, 1, 1, 20, 2, 8,   20, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Mana Philtre x20 */
     , (480607, 0.7600,  27325, 1, 1, 20, 2, 8,   20, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Stamina Philtre x20 */
     , (480607,      1, 480612, 1, 1, 1, 1,  8,   -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Darkbeat's Golem Drawing - always present */;
