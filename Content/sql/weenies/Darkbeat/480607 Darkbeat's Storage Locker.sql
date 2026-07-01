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
     , (480607,  81,          4) /* MaxGeneratedObjects */
     , (480607,  82,          4) /* InitGeneratedObjects */
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
   MaxGeneratedObjects = 4: 1 always-spawn slot (-1 loot profile) +
   3 optional rolls from this table per chest open.

   -1 entry always spawns (tier 6 loot profile).
   Every optional roll is guaranteed to produce an item (no fallback entry).

   NOTE on the -1 loot profile: while the -1 slot is unspawned,
   GetTotalProbability() returns 1.0 (WorldObject_Generators.cs), so the
   FIRST RNG roll of a fresh/reset chest is locked to the [0,1.0] existing-item
   space and can never select the Sturdy Iron Key group below. The key is only
   eligible on the 2 remaining rolls — see the Sturdy Iron Key group note.

   Main group (0.0000 – 0.5000, 50% of total probability space):
     Salvage bags     0.0000 – 0.0550   (0.5% each × 11 = 5.5%)
     Foolproof gems   0.0550 – 0.0900   (0.25% each × 14 = 3.5%)
     A Box            0.0900 – 0.1100   (2%)
     Trade Note ×25   0.1100 – 0.1700   (6%)
     PK Trophy ×20    0.1700 – 0.3200   (15%)
     Phial ×2         0.3200 – 0.4000   (8%)
     Healing Kit      0.4000 – 0.4250   (2.5%)
     Salted Meat ×20  0.4250 – 0.4500   (2.5%)
     Mana Philtre ×20 0.4500 – 0.4750   (2.5%)
     Stam Philtre ×20 0.4750 – 0.5000   (2.5%)

   Massive Mana Stone group — probability reset (each 10% of total):
     Mana Stone #1    0.5000 – 0.6000   (10%)  ← reset triggers (0.10 < 0.50)
     Mana Stone #2    0.6000 – 0.7000   (10%)
     Mana Stone #3    0.7000 – 0.8000   (10%)
     Mana Stone #4    0.8000 – 0.9000   (10%)
     Mana Stone #5    0.9000 – 1.0000   (10%)

   Sturdy Iron Key group — probability reset (independent ~20% key roll):
     Sturdy Iron Key  1.0000 – 1.1180   (raw prob 0.1180; reset triggers, 0.1180 < 0.5000)

   GetTotalProbability() = 0.5000 + 0.5000 + 0.1180 = 1.1180
   Per RNG roll the key occupies band [1.0000, 1.1180] of a [0,1.1180] roll
   = 0.1180 / 1.1180 = 10.55%. Because the first RNG roll is locked to [0,1.0]
   (see note above), the key is eligible on 2 rolls per chest:
   1 - (1 - 0.1055)^2 ≈ 20% chance of a Sturdy Iron Key per chest open.
   max_Create = 1 caps it at one key per chest.
   ----------------------------------------------------------------------- */

INSERT INTO `weenie_properties_generator` (`object_Id`, `probability`, `weenie_Class_Id`, `delay`, `init_Create`, `max_Create`, `when_Create`, `where_Create`, `stack_Size`, `palette_Id`, `shade`, `obj_Cell_Id`, `origin_X`, `origin_Y`, `origin_Z`, `angles_W`, `angles_X`, `angles_Y`, `angles_Z`)
VALUES (480607,    -1, 10000, 1, 1,   1, 2, 72,   -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Always: Tier 6 loot profile */
     , (480607, 0.0050, 510034, 1, 1, 1, 2,  8,   -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Sunstone Salvage WS10 - Armor Rend */
     , (480607, 0.0100, 510033, 1, 1, 1, 2,  8,   -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Red Garnet Salvage WS10 - Fire Rend */
     , (480607, 0.0150, 510026, 1, 1, 1, 2,  8,   -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Black Garnet Salvage WS10 - Pierce Rend/Imbue */
     , (480607, 0.0200, 510031, 1, 1, 1, 2,  8,   -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Imperial Topaz Salvage WS10 - Slash Rend/Imbue */
     , (480607, 0.0250, 510032, 1, 1, 1, 2,  8,   -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Jet Salvage WS10 - Lightning Rend/Imbue */
     , (480607, 0.0300, 510025, 1, 1, 1, 2,  8,   -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Aquamarine Salvage WS10 - Cold Rend/Imbue */
     , (480607, 0.0350, 510035, 1, 1, 1, 2,  8,   -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* White Sapphire Salvage WS10 - Bludgeon Rend */
     , (480607, 0.0400, 510029, 1, 1, 1, 2,  8,   -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Emerald Salvage WS10 - Acid Rend/Imbue */
     , (480607, 0.0450, 510030, 1, 1, 1, 2,  8,   -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Fire Opal Salvage WS10 - Crippling Blow */
     , (480607, 0.0500, 510027, 1, 1, 1, 2,  8,   -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Black Opal Salvage WS10 - Critical Strike */
     , (480607, 0.0550, 510028, 1, 1, 1, 2,  8,   -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Bloodstone Salvage WS10 - Minor Endurance Imbue */
     , (480607, 0.0575, 36619, 1, 1,  1, 2,  8,   -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Aquamarine Foolproof */
     , (480607, 0.0600, 36620, 1, 1,  1, 2,  8,   -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Black Garnet Foolproof */
     , (480607, 0.0625, 36622, 1, 1,  1, 2,  8,   -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Emerald Foolproof */
     , (480607, 0.0650, 36624, 1, 1,  1, 2,  8,   -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Imperial Topaz Foolproof */
     , (480607, 0.0675, 36625, 1, 1,  1, 2,  8,   -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Jet Foolproof */
     , (480607, 0.0700, 36634, 1, 1,  1, 2,  8,   -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Peridot Foolproof */
     , (480607, 0.0725, 36626, 1, 1,  1, 2,  8,   -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Red Garnet Foolproof */
     , (480607, 0.0750, 36628, 1, 1,  1, 2,  8,   -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* White Sapphire Foolproof */
     , (480607, 0.0775, 36635, 1, 1,  1, 2,  8,   -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Yellow Topaz Foolproof */
     , (480607, 0.0800, 36634, 1, 1,  1, 2,  8,   -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Peridot Foolproof */
     , (480607, 0.0825, 36636, 1, 1,  1, 2,  8,   -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Zircon Foolproof */
     , (480607, 0.0850, 36621, 1, 1,  1, 2,  8,   -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Black Opal Foolproof */
     , (480607, 0.0875, 36623, 1, 1,  1, 2,  8,   -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Fire Opal Foolproof */
     , (480607, 0.0900, 36627, 1, 1,  1, 2,  8,   -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Sunstone Foolproof */
     , (480607, 0.1100, 510000, 1, 1, 1, 2,  8,   -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* A Box */
     , (480607, 0.1700,  20630, 1, 1, 25, 2, 8,   25, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Trade Note (250,000) x25 */
     , (480607, 0.3200, 1000002, 1, 1, 20, 2, 8,  20, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* PK Trophy x20 */
     , (480607, 0.4000, 1000003, 1, 1, 2, 2,  8,   2, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Phial of Bloody Tears x2 */
     , (480607, 0.4250,   9229, 1, 1, 1, 2,  8,   -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Treated Healing Kit */
     , (480607, 0.4500,  27669, 1, 1, 20, 2, 8,   20, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Tumerok Salted Meat x20 */
     , (480607, 0.4750,  27321, 1, 1, 20, 2, 8,   20, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Mana Philtre x20 */
     , (480607, 0.5000,  27325, 1, 1, 20, 2, 8,   20, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Stamina Philtre x20 */
     , (480607, 0.1000,  27329, 1, 1, 1, 2,  8,   -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Massive Mana Stone #1 — reset group start */
     , (480607, 0.2000,  27329, 1, 1, 1, 2,  8,   -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Massive Mana Stone #2 */
     , (480607, 0.3000,  27329, 1, 1, 1, 2,  8,   -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Massive Mana Stone #3 */
     , (480607, 0.4000,  27329, 1, 1, 1, 2,  8,   -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Massive Mana Stone #4 */
     , (480607, 0.5000,  27329, 1, 1, 1, 2,  8,   -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Massive Mana Stone #5 */
     , (480607, 0.1180,   6876, 1, 1, 1, 2,  8,   -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0) /* Sturdy Iron Key — reset group, ~20% independent key roll */;
