/* -----------------------------------------------------------------------
   Custom WS10 Salvage Bag Weenies (510020–510036)

   These weenies are copies of the base salvage bag weenies with
   Structure (92) = 100, ItemWorkmanship (105) = 100, and
   NumItemsInMaterial (170) = 10 baked in.

   Base salvage bag weenies (20985–21086, 20988) do NOT have these three
   properties set, so spawning them from a loot box emote or generator
   yields an empty bag with 0 workmanship.  These custom variants fix
   that so loot boxes and Darkbeat's Storage Locker award proper full
   WS10 bags.

   WCID → Material mapping:
     510020  Granite          (base 20985)
     510021  Iron             (base 20986)
     510022  Steel            (base 20993)
     510023  Green Garnet     (base 21050)
     510024  Opal             (base 21065)
     510025  Aquamarine       (base 21037)
     510026  Black Garnet     (base 21039)
     510027  Black Opal       (base 21040)
     510028  Bloodstone       (base 21041)
     510029  Emerald          (base 21048)
     510030  Fire Opal        (base 21049)
     510031  Imperial Topaz   (base 21054)
     510032  Jet              (base 21056)
     510033  Red Garnet       (base 21069)
     510034  Sunstone         (base 21079)
     510035  White Sapphire   (base 21086)
     510036  Mahogany         (base 20988)

   Matching cook_book entries required for each of these WCIDs — see
   510020-510036 SalvageWS10 CookBook.sql. Without those rows the craft
   system rejects the bag on any target with "X cannot be used on Y."

   510036 (Mahogany) added 2026-08-08.
   ----------------------------------------------------------------------- */

/* ---- 510020: Salvaged Granite (WS10) ---- */
DELETE FROM `weenie` WHERE `class_Id` = 510020;
INSERT INTO `weenie` (`class_Id`, `class_Name`, `type`, `last_Modified`)
VALUES (510020, 'ace510020-salvagegranite', 44, '2026-06-27 00:00:00') /* CraftTool */;

INSERT INTO `weenie_properties_int` (`object_Id`, `type`, `value`)
VALUES (510020,   1, 1073741824) /* ItemType - Salvage */
     , (510020,   3,         14) /* PaletteTemplate */
     , (510020,   5,        100) /* EncumbranceVal */
     , (510020,   8,        100) /* Mass */
     , (510020,   9,          0) /* ValidLocations */
     , (510020,  11,          1) /* MaxStackSize */
     , (510020,  12,          1) /* StackSize */
     , (510020,  13,        100) /* StackUnitEncumbrance */
     , (510020,  14,        100) /* StackUnitMass */
     , (510020,  15,         10) /* StackUnitValue */
     , (510020,  16,     524296) /* ItemUseable */
     , (510020,  19,         10) /* Value */
     , (510020,  33,          1)
     , (510020,  91,        100) /* MaxStructure */
     , (510020,  92,        100) /* Structure */
     , (510020,  93,       1044) /* PhysicsState */
     , (510020,  94,        257)
     , (510020, 105,        100) /* ItemWorkmanship */
     , (510020, 131,         67) /* MaterialType - Granite */
     , (510020, 150,        103)
     , (510020, 151,          9) /* HookType */
     , (510020, 170,         10) /* NumItemsInMaterial */;

INSERT INTO `weenie_properties_string` (`object_Id`, `type`, `value`)
VALUES (510020,   1, 'Salvaged Granite') /* Name */
     , (510020,  14, 'Apply this material to a treasure-generated weapon to improve the weapon''s variance by 20%.') /* Use */
     , (510020,  15, 'A brick of granite material salvaged from old items.') /* LongDesc */
     , (510020,  22, '')
     , (510020,  23, '');

INSERT INTO `weenie_properties_d_i_d` (`object_Id`, `type`, `value`)
VALUES (510020,   1, 0x02000181) /* Setup */
     , (510020,   3, 0x20000014) /* SoundTable */
     , (510020,   6, 0x04000BEF)
     , (510020,   7, 0x100003CE) /* ClothingBase */
     , (510020,   8, 0x0600102C) /* Icon */
     , (510020,  22, 0x3400002B) /* PhysicsEffectTable */
     , (510020,  50, 0x060026CD) /* IconUnderlay */;

/* ---- 510021: Salvaged Iron (WS10) ---- */
DELETE FROM `weenie` WHERE `class_Id` = 510021;
INSERT INTO `weenie` (`class_Id`, `class_Name`, `type`, `last_Modified`)
VALUES (510021, 'ace510021-salvageiron', 44, '2026-06-27 00:00:00') /* CraftTool */;

INSERT INTO `weenie_properties_int` (`object_Id`, `type`, `value`)
VALUES (510021,   1, 1073741824) /* ItemType - Salvage */
     , (510021,   3,         14) /* PaletteTemplate */
     , (510021,   5,        100) /* EncumbranceVal */
     , (510021,   8,        100) /* Mass */
     , (510021,   9,          0) /* ValidLocations */
     , (510021,  11,          1) /* MaxStackSize */
     , (510021,  12,          1) /* StackSize */
     , (510021,  13,        100) /* StackUnitEncumbrance */
     , (510021,  14,        100) /* StackUnitMass */
     , (510021,  15,         10) /* StackUnitValue */
     , (510021,  16,     524296) /* ItemUseable */
     , (510021,  19,         10) /* Value */
     , (510021,  33,          1)
     , (510021,  91,        100) /* MaxStructure */
     , (510021,  92,        100) /* Structure */
     , (510021,  93,       1044) /* PhysicsState */
     , (510021,  94,        257)
     , (510021, 105,        100) /* ItemWorkmanship */
     , (510021, 131,         61) /* MaterialType - Iron */
     , (510021, 150,        103)
     , (510021, 151,          9) /* HookType */
     , (510021, 170,         10) /* NumItemsInMaterial */;

INSERT INTO `weenie_properties_string` (`object_Id`, `type`, `value`)
VALUES (510021,   1, 'Salvaged Iron') /* Name */
     , (510021,  14, 'Apply this material to a treasure-generated weapon to increase the weapon''s damage by 1.') /* Use */
     , (510021,  15, 'A bar of iron material salvaged from old items.') /* LongDesc */
     , (510021,  22, '')
     , (510021,  23, '');

INSERT INTO `weenie_properties_d_i_d` (`object_Id`, `type`, `value`)
VALUES (510021,   1, 0x02000181) /* Setup */
     , (510021,   3, 0x20000014) /* SoundTable */
     , (510021,   6, 0x04000BEF)
     , (510021,   7, 0x100003CE) /* ClothingBase */
     , (510021,   8, 0x0600102C) /* Icon */
     , (510021,  22, 0x3400002B) /* PhysicsEffectTable */
     , (510021,  50, 0x060026CE) /* IconUnderlay */;

/* ---- 510022: Salvaged Steel (WS10) ---- */
DELETE FROM `weenie` WHERE `class_Id` = 510022;
INSERT INTO `weenie` (`class_Id`, `class_Name`, `type`, `last_Modified`)
VALUES (510022, 'ace510022-salvagesteel', 44, '2026-06-27 00:00:00') /* CraftTool */;

INSERT INTO `weenie_properties_int` (`object_Id`, `type`, `value`)
VALUES (510022,   1, 1073741824) /* ItemType - Salvage */
     , (510022,   3,         13) /* PaletteTemplate */
     , (510022,   5,        100) /* EncumbranceVal */
     , (510022,   8,        100) /* Mass */
     , (510022,   9,          0) /* ValidLocations */
     , (510022,  11,          1) /* MaxStackSize */
     , (510022,  12,          1) /* StackSize */
     , (510022,  13,        100) /* StackUnitEncumbrance */
     , (510022,  14,        100) /* StackUnitMass */
     , (510022,  15,         10) /* StackUnitValue */
     , (510022,  16,     524296) /* ItemUseable */
     , (510022,  19,         10) /* Value */
     , (510022,  33,          1)
     , (510022,  91,        100) /* MaxStructure */
     , (510022,  92,        100) /* Structure */
     , (510022,  93,       1044) /* PhysicsState */
     , (510022,  94,          2)
     , (510022, 105,        100) /* ItemWorkmanship */
     , (510022, 131,         64) /* MaterialType - Steel */
     , (510022, 150,        103)
     , (510022, 151,          9) /* HookType */
     , (510022, 170,         10) /* NumItemsInMaterial */;

INSERT INTO `weenie_properties_string` (`object_Id`, `type`, `value`)
VALUES (510022,   1, 'Salvaged Steel') /* Name */
     , (510022,  14, 'Apply this material to treasure-generated armor to increase the armor''s armor level by 20. This material cannot be used on Covenant Armor.') /* Use */
     , (510022,  15, 'A bar of steel material salvaged from old items.') /* LongDesc */
     , (510022,  22, '')
     , (510022,  23, '');

INSERT INTO `weenie_properties_d_i_d` (`object_Id`, `type`, `value`)
VALUES (510022,   1, 0x02000181) /* Setup */
     , (510022,   3, 0x20000014) /* SoundTable */
     , (510022,   6, 0x04000BEF)
     , (510022,   7, 0x100003CE) /* ClothingBase */
     , (510022,   8, 0x0600102C) /* Icon */
     , (510022,  22, 0x3400002B) /* PhysicsEffectTable */
     , (510022,  50, 0x060026D5) /* IconUnderlay */;

/* ---- 510023: Salvaged Green Garnet (WS10) ---- */
/* Note: green garnet has ItemUseable=1 and no type-94 in the base weenie */
DELETE FROM `weenie` WHERE `class_Id` = 510023;
INSERT INTO `weenie` (`class_Id`, `class_Name`, `type`, `last_Modified`)
VALUES (510023, 'ace510023-salvaggreengarnet', 44, '2026-06-27 00:00:00') /* CraftTool */;

INSERT INTO `weenie_properties_int` (`object_Id`, `type`, `value`)
VALUES (510023,   1, 1073741824) /* ItemType - Salvage */
     , (510023,   3,          4) /* PaletteTemplate */
     , (510023,   5,        100) /* EncumbranceVal */
     , (510023,   8,        100) /* Mass */
     , (510023,   9,          0) /* ValidLocations */
     , (510023,  11,          1) /* MaxStackSize */
     , (510023,  12,          1) /* StackSize */
     , (510023,  13,        100) /* StackUnitEncumbrance */
     , (510023,  14,        100) /* StackUnitMass */
     , (510023,  15,         10) /* StackUnitValue */
     , (510023,  16,          1) /* ItemUseable */
     , (510023,  19,         10) /* Value */
     , (510023,  33,          1)
     , (510023,  91,        100) /* MaxStructure */
     , (510023,  92,        100) /* Structure */
     , (510023,  93,       1044) /* PhysicsState */
     , (510023, 105,        100) /* ItemWorkmanship */
     , (510023, 131,         23) /* MaterialType - Green Garnet */
     , (510023, 150,        103)
     , (510023, 151,          9) /* HookType */
     , (510023, 170,         10) /* NumItemsInMaterial */;

INSERT INTO `weenie_properties_string` (`object_Id`, `type`, `value`)
VALUES (510023,   1, 'Salvaged Green Garnet') /* Name */
     , (510023,  14, 'This item has no apparent use.') /* Use */
     , (510023,  15, 'Chips of green garnet material salvaged from old items.') /* LongDesc */
     , (510023,  22, '')
     , (510023,  23, '');

INSERT INTO `weenie_properties_d_i_d` (`object_Id`, `type`, `value`)
VALUES (510023,   1, 0x02000181) /* Setup */
     , (510023,   3, 0x20000014) /* SoundTable */
     , (510023,   6, 0x04000BEF)
     , (510023,   7, 0x100003CE) /* ClothingBase */
     , (510023,   8, 0x0600102C) /* Icon */
     , (510023,  22, 0x3400002B) /* PhysicsEffectTable */
     , (510023,  50, 0x060026FA) /* IconUnderlay */;

/* ---- 510024: Salvaged Opal (WS10) ---- */
DELETE FROM `weenie` WHERE `class_Id` = 510024;
INSERT INTO `weenie` (`class_Id`, `class_Name`, `type`, `last_Modified`)
VALUES (510024, 'ace510024-salvageopal', 44, '2026-06-27 00:00:00') /* CraftTool */;

INSERT INTO `weenie_properties_int` (`object_Id`, `type`, `value`)
VALUES (510024,   1, 1073741824) /* ItemType - Salvage */
     , (510024,   3,          2) /* PaletteTemplate */
     , (510024,   5,        100) /* EncumbranceVal */
     , (510024,   8,        100) /* Mass */
     , (510024,   9,          0) /* ValidLocations */
     , (510024,  11,          1) /* MaxStackSize */
     , (510024,  12,          1) /* StackSize */
     , (510024,  13,        100) /* StackUnitEncumbrance */
     , (510024,  14,        100) /* StackUnitMass */
     , (510024,  15,         10) /* StackUnitValue */
     , (510024,  16,     524296) /* ItemUseable */
     , (510024,  19,         10) /* Value */
     , (510024,  33,          1)
     , (510024,  91,        100) /* MaxStructure */
     , (510024,  92,        100) /* Structure */
     , (510024,  93,       1044) /* PhysicsState */
     , (510024,  94,      32768)
     , (510024, 105,        100) /* ItemWorkmanship */
     , (510024, 131,         33) /* MaterialType - Opal */
     , (510024, 150,        103)
     , (510024, 151,          9) /* HookType */
     , (510024, 170,         10) /* NumItemsInMaterial */;

INSERT INTO `weenie_properties_string` (`object_Id`, `type`, `value`)
VALUES (510024,   1, 'Salvaged Opal') /* Name */
     , (510024,  14, 'Apply this material to a treasure-generated magic caster to increase the its mana conversion bonus by 1%.') /* Use */
     , (510024,  15, 'Chips of opal material salvaged from old items.') /* LongDesc */
     , (510024,  22, '')
     , (510024,  23, '');

INSERT INTO `weenie_properties_d_i_d` (`object_Id`, `type`, `value`)
VALUES (510024,   1, 0x02000181) /* Setup */
     , (510024,   3, 0x20000014) /* SoundTable */
     , (510024,   6, 0x04000BEF)
     , (510024,   7, 0x100003CE) /* ClothingBase */
     , (510024,   8, 0x0600102C) /* Icon */
     , (510024,  22, 0x3400002B) /* PhysicsEffectTable */
     , (510024,  50, 0x06002708) /* IconUnderlay */;

/* ---- 510025: Salvaged Aquamarine (WS10) ---- */
DELETE FROM `weenie` WHERE `class_Id` = 510025;
INSERT INTO `weenie` (`class_Id`, `class_Name`, `type`, `last_Modified`)
VALUES (510025, 'ace510025-salvageaquamarine', 44, '2026-06-27 00:00:00') /* CraftTool */;

INSERT INTO `weenie_properties_int` (`object_Id`, `type`, `value`)
VALUES (510025,   1, 1073741824) /* ItemType - Salvage */
     , (510025,   3,         14) /* PaletteTemplate */
     , (510025,   5,        100) /* EncumbranceVal */
     , (510025,   8,        100) /* Mass */
     , (510025,   9,          0) /* ValidLocations */
     , (510025,  11,          1) /* MaxStackSize */
     , (510025,  12,          1) /* StackSize */
     , (510025,  13,        100) /* StackUnitEncumbrance */
     , (510025,  14,        100) /* StackUnitMass */
     , (510025,  15,         10) /* StackUnitValue */
     , (510025,  16,     524296) /* ItemUseable */
     , (510025,  19,         10) /* Value */
     , (510025,  33,          1)
     , (510025,  91,        100) /* MaxStructure */
     , (510025,  92,        100) /* Structure */
     , (510025,  93,       1044) /* PhysicsState */
     , (510025,  94,      33025)
     , (510025, 105,        100) /* ItemWorkmanship */
     , (510025, 131,         13) /* MaterialType - Aquamarine */
     , (510025, 150,        103)
     , (510025, 151,          9) /* HookType */
     , (510025, 170,         10) /* NumItemsInMaterial */;

INSERT INTO `weenie_properties_string` (`object_Id`, `type`, `value`)
VALUES (510025,   1, 'Salvaged Aquamarine') /* Name */
     , (510025,  14, 'Apply this material to a treasure-generated weapon or magic-casting implement to imbue the target with Cold Rending. Cold Rending gives the weapon the ability to make its opponent vulnerable to cold attacks. The amount of vulnerability depends on the attack skill of the wielder. This effect does not stack with Cold Vulnerability spells. ') /* Use */
     , (510025,  15, 'Chips of aquamarine material salvaged from old items.') /* LongDesc */
     , (510025,  22, '')
     , (510025,  23, '');

INSERT INTO `weenie_properties_d_i_d` (`object_Id`, `type`, `value`)
VALUES (510025,   1, 0x02000181) /* Setup */
     , (510025,   3, 0x20000014) /* SoundTable */
     , (510025,   6, 0x04000BEF)
     , (510025,   7, 0x100003CE) /* ClothingBase */
     , (510025,   8, 0x0600102C) /* Icon */
     , (510025,  22, 0x3400002B) /* PhysicsEffectTable */
     , (510025,  50, 0x060026EE) /* IconUnderlay */;

/* ---- 510026: Salvaged Black Garnet (WS10) ---- */
DELETE FROM `weenie` WHERE `class_Id` = 510026;
INSERT INTO `weenie` (`class_Id`, `class_Name`, `type`, `last_Modified`)
VALUES (510026, 'ace510026-salvageblackgarnet', 44, '2026-06-27 00:00:00') /* CraftTool */;

INSERT INTO `weenie_properties_int` (`object_Id`, `type`, `value`)
VALUES (510026,   1, 1073741824) /* ItemType - Salvage */
     , (510026,   3,         14) /* PaletteTemplate */
     , (510026,   5,        100) /* EncumbranceVal */
     , (510026,   8,        100) /* Mass */
     , (510026,   9,          0) /* ValidLocations */
     , (510026,  11,          1) /* MaxStackSize */
     , (510026,  12,          1) /* StackSize */
     , (510026,  13,        100) /* StackUnitEncumbrance */
     , (510026,  14,        100) /* StackUnitMass */
     , (510026,  15,         10) /* StackUnitValue */
     , (510026,  16,     524296) /* ItemUseable */
     , (510026,  19,         10) /* Value */
     , (510026,  33,          1)
     , (510026,  91,        100) /* MaxStructure */
     , (510026,  92,        100) /* Structure */
     , (510026,  93,       1044) /* PhysicsState */
     , (510026,  94,      33025)
     , (510026, 105,        100) /* ItemWorkmanship */
     , (510026, 131,         15) /* MaterialType - Black Garnet */
     , (510026, 150,        103)
     , (510026, 151,          9) /* HookType */
     , (510026, 170,         10) /* NumItemsInMaterial */;

INSERT INTO `weenie_properties_string` (`object_Id`, `type`, `value`)
VALUES (510026,   1, 'Salvaged Black Garnet') /* Name */
     , (510026,  14, 'Apply this material to a treasure-generated weapon or magic-casting implement to imbue the target with Pierce Rending. Pierce Rending gives the weapon the ability to make its opponent vulnerable to piercing attacks. The amount of vulnerability depends on the attack skill of the wielder. This effect does not stack with Piercing Vulnerability spells. ') /* Use */
     , (510026,  15, 'Chips of black garnet material salvaged from old items.') /* LongDesc */
     , (510026,  22, '')
     , (510026,  23, '');

INSERT INTO `weenie_properties_d_i_d` (`object_Id`, `type`, `value`)
VALUES (510026,   1, 0x02000181) /* Setup */
     , (510026,   3, 0x20000014) /* SoundTable */
     , (510026,   6, 0x04000BEF)
     , (510026,   7, 0x100003CE) /* ClothingBase */
     , (510026,   8, 0x0600102C) /* Icon */
     , (510026,  22, 0x3400002B) /* PhysicsEffectTable */
     , (510026,  50, 0x060026F0) /* IconUnderlay */;

/* ---- 510027: Salvaged Black Opal (WS10) ---- */
DELETE FROM `weenie` WHERE `class_Id` = 510027;
INSERT INTO `weenie` (`class_Id`, `class_Name`, `type`, `last_Modified`)
VALUES (510027, 'ace510027-salvageblackopal', 44, '2026-06-27 00:00:00') /* CraftTool */;

INSERT INTO `weenie_properties_int` (`object_Id`, `type`, `value`)
VALUES (510027,   1, 1073741824) /* ItemType - Salvage */
     , (510027,   3,          2) /* PaletteTemplate */
     , (510027,   5,        100) /* EncumbranceVal */
     , (510027,   8,        100) /* Mass */
     , (510027,   9,          0) /* ValidLocations */
     , (510027,  11,          1) /* MaxStackSize */
     , (510027,  12,          1) /* StackSize */
     , (510027,  13,        100) /* StackUnitEncumbrance */
     , (510027,  14,        100) /* StackUnitMass */
     , (510027,  15,         10) /* StackUnitValue */
     , (510027,  16,     524296) /* ItemUseable */
     , (510027,  19,         10) /* Value */
     , (510027,  33,          1)
     , (510027,  91,        100) /* MaxStructure */
     , (510027,  92,        100) /* Structure */
     , (510027,  93,       1044) /* PhysicsState */
     , (510027,  94,      33025)
     , (510027, 105,        100) /* ItemWorkmanship */
     , (510027, 131,         16) /* MaterialType - Black Opal */
     , (510027, 150,        103)
     , (510027, 151,          9) /* HookType */
     , (510027, 170,         10) /* NumItemsInMaterial */;

INSERT INTO `weenie_properties_string` (`object_Id`, `type`, `value`)
VALUES (510027,   1, 'Salvaged Black Opal') /* Name */
     , (510027,  14, 'Apply this material to a treasure-generated weapon or magic-casting implement to imbue the target with Critical Strike. Critical Strike increases the chance that the item critically hits its opponent. The increase in chance depends on the attack skill of the wielder.') /* Use */
     , (510027,  15, 'Chips of black opal material salvaged from old items.') /* LongDesc */
     , (510027,  22, '')
     , (510027,  23, '');

INSERT INTO `weenie_properties_d_i_d` (`object_Id`, `type`, `value`)
VALUES (510027,   1, 0x02000181) /* Setup */
     , (510027,   3, 0x20000014) /* SoundTable */
     , (510027,   6, 0x04000BEF)
     , (510027,   7, 0x100003CE) /* ClothingBase */
     , (510027,   8, 0x0600102C) /* Icon */
     , (510027,  22, 0x3400002B) /* PhysicsEffectTable */
     , (510027,  50, 0x060026F1) /* IconUnderlay */;

/* ---- 510028: Salvaged Bloodstone (WS10) ---- */
DELETE FROM `weenie` WHERE `class_Id` = 510028;
INSERT INTO `weenie` (`class_Id`, `class_Name`, `type`, `last_Modified`)
VALUES (510028, 'ace510028-salvagebloodstone', 44, '2026-06-27 00:00:00') /* CraftTool */;

INSERT INTO `weenie_properties_int` (`object_Id`, `type`, `value`)
VALUES (510028,   1, 1073741824) /* ItemType - Salvage */
     , (510028,   3,          2) /* PaletteTemplate */
     , (510028,   5,        100) /* EncumbranceVal */
     , (510028,   8,        100) /* Mass */
     , (510028,   9,          0) /* ValidLocations */
     , (510028,  11,          1) /* MaxStackSize */
     , (510028,  12,          1) /* StackSize */
     , (510028,  13,        100) /* StackUnitEncumbrance */
     , (510028,  14,        100) /* StackUnitMass */
     , (510028,  15,         10) /* StackUnitValue */
     , (510028,  16,     524296) /* ItemUseable */
     , (510028,  19,         10) /* Value */
     , (510028,  33,          1)
     , (510028,  91,        100) /* MaxStructure */
     , (510028,  92,        100) /* Structure */
     , (510028,  93,       1044) /* PhysicsState */
     , (510028,  94,          8)
     , (510028, 105,        100) /* ItemWorkmanship */
     , (510028, 131,         17) /* MaterialType - Bloodstone */
     , (510028, 150,        103)
     , (510028, 151,          9) /* HookType */
     , (510028, 170,         10) /* NumItemsInMaterial */;

INSERT INTO `weenie_properties_string` (`object_Id`, `type`, `value`)
VALUES (510028,   1, 'Salvaged Bloodstone') /* Name */
     , (510028,  14, 'Apply this material to a piece of magical treasure-generated jewelry to imbue the target with Minor Endurance. This will also raise the Arcane Lore difficulty and the Spellcraft of the piece by 25.') /* Use */
     , (510028,  15, 'Chips of bloodstone material salvaged from old items.') /* LongDesc */
     , (510028,  22, '')
     , (510028,  23, '');

INSERT INTO `weenie_properties_d_i_d` (`object_Id`, `type`, `value`)
VALUES (510028,   1, 0x02000181) /* Setup */
     , (510028,   3, 0x20000014) /* SoundTable */
     , (510028,   6, 0x04000BEF)
     , (510028,   7, 0x100003CE) /* ClothingBase */
     , (510028,   8, 0x0600102C) /* Icon */
     , (510028,  22, 0x3400002B) /* PhysicsEffectTable */
     , (510028,  50, 0x060026F2) /* IconUnderlay */;

/* ---- 510029: Salvaged Emerald (WS10) ---- */
DELETE FROM `weenie` WHERE `class_Id` = 510029;
INSERT INTO `weenie` (`class_Id`, `class_Name`, `type`, `last_Modified`)
VALUES (510029, 'ace510029-salvageemerald', 44, '2026-06-27 00:00:00') /* CraftTool */;

INSERT INTO `weenie_properties_int` (`object_Id`, `type`, `value`)
VALUES (510029,   1, 1073741824) /* ItemType - Salvage */
     , (510029,   3,         14) /* PaletteTemplate */
     , (510029,   5,        100) /* EncumbranceVal */
     , (510029,   8,        100) /* Mass */
     , (510029,   9,          0) /* ValidLocations */
     , (510029,  11,          1) /* MaxStackSize */
     , (510029,  12,          1) /* StackSize */
     , (510029,  13,        100) /* StackUnitEncumbrance */
     , (510029,  14,        100) /* StackUnitMass */
     , (510029,  15,         10) /* StackUnitValue */
     , (510029,  16,     524296) /* ItemUseable */
     , (510029,  19,         10) /* Value */
     , (510029,  33,          1)
     , (510029,  91,        100) /* MaxStructure */
     , (510029,  92,        100) /* Structure */
     , (510029,  93,       1044) /* PhysicsState */
     , (510029,  94,      33025)
     , (510029, 105,        100) /* ItemWorkmanship */
     , (510029, 131,         21) /* MaterialType - Emerald */
     , (510029, 150,        103)
     , (510029, 151,          9) /* HookType */
     , (510029, 170,         10) /* NumItemsInMaterial */;

INSERT INTO `weenie_properties_string` (`object_Id`, `type`, `value`)
VALUES (510029,   1, 'Salvaged Emerald') /* Name */
     , (510029,  14, 'Apply this material to a treasure-generated weapon or magic-casting implement to imbue the target with Acid Rending. Acid Rending gives the weapon the ability to make its opponent vulnerable to acid attacks. The amount of vulnerability depends on the attack skill of the wielder. This effect does not stack with Acid Vulnerability spells. ') /* Use */
     , (510029,  15, 'Chips of emerald material salvaged from old items.') /* LongDesc */
     , (510029,  22, '')
     , (510029,  23, '');

INSERT INTO `weenie_properties_d_i_d` (`object_Id`, `type`, `value`)
VALUES (510029,   1, 0x02000181) /* Setup */
     , (510029,   3, 0x20000014) /* SoundTable */
     , (510029,   6, 0x04000BEF)
     , (510029,   7, 0x100003CE) /* ClothingBase */
     , (510029,   8, 0x0600102C) /* Icon */
     , (510029,  22, 0x3400002B) /* PhysicsEffectTable */
     , (510029,  50, 0x060026F8) /* IconUnderlay */;

/* ---- 510030: Salvaged Fire Opal (WS10) ---- */
DELETE FROM `weenie` WHERE `class_Id` = 510030;
INSERT INTO `weenie` (`class_Id`, `class_Name`, `type`, `last_Modified`)
VALUES (510030, 'ace510030-salvagefireopal', 44, '2026-06-27 00:00:00') /* CraftTool */;

INSERT INTO `weenie_properties_int` (`object_Id`, `type`, `value`)
VALUES (510030,   1, 1073741824) /* ItemType - Salvage */
     , (510030,   3,          2) /* PaletteTemplate */
     , (510030,   5,        100) /* EncumbranceVal */
     , (510030,   8,        100) /* Mass */
     , (510030,   9,          0) /* ValidLocations */
     , (510030,  11,          1) /* MaxStackSize */
     , (510030,  12,          1) /* StackSize */
     , (510030,  13,        100) /* StackUnitEncumbrance */
     , (510030,  14,        100) /* StackUnitMass */
     , (510030,  15,         10) /* StackUnitValue */
     , (510030,  16,     524296) /* ItemUseable */
     , (510030,  19,         10) /* Value */
     , (510030,  33,          1)
     , (510030,  91,        100) /* MaxStructure */
     , (510030,  92,        100) /* Structure */
     , (510030,  93,       1044) /* PhysicsState */
     , (510030,  94,      33025)
     , (510030, 105,        100) /* ItemWorkmanship */
     , (510030, 131,         22) /* MaterialType - Fire Opal */
     , (510030, 150,        103)
     , (510030, 151,          9) /* HookType */
     , (510030, 170,         10) /* NumItemsInMaterial */;

INSERT INTO `weenie_properties_string` (`object_Id`, `type`, `value`)
VALUES (510030,   1, 'Salvaged Fire Opal') /* Name */
     , (510030,  14, 'Apply this material to a treasure-generated weapon or magic-casting implement to imbue the target with Crippling Blow. Crippling Blow increases the amount of damage that the item does when it critically hits its opponent. The amount of extra damage depends on the attack skill of the wielder.') /* Use */
     , (510030,  15, 'Chips of fire opal material salvaged from old items.') /* LongDesc */
     , (510030,  22, '')
     , (510030,  23, '');

INSERT INTO `weenie_properties_d_i_d` (`object_Id`, `type`, `value`)
VALUES (510030,   1, 0x02000181) /* Setup */
     , (510030,   3, 0x20000014) /* SoundTable */
     , (510030,   6, 0x04000BEF)
     , (510030,   7, 0x100003CE) /* ClothingBase */
     , (510030,   8, 0x0600102C) /* Icon */
     , (510030,  22, 0x3400002B) /* PhysicsEffectTable */
     , (510030,  50, 0x060026F9) /* IconUnderlay */;

/* ---- 510031: Salvaged Imperial Topaz (WS10) ---- */
DELETE FROM `weenie` WHERE `class_Id` = 510031;
INSERT INTO `weenie` (`class_Id`, `class_Name`, `type`, `last_Modified`)
VALUES (510031, 'ace510031-salvageimperialtopaz', 44, '2026-06-27 00:00:00') /* CraftTool */;

INSERT INTO `weenie_properties_int` (`object_Id`, `type`, `value`)
VALUES (510031,   1, 1073741824) /* ItemType - Salvage */
     , (510031,   3,         14) /* PaletteTemplate */
     , (510031,   5,        100) /* EncumbranceVal */
     , (510031,   8,        100) /* Mass */
     , (510031,   9,          0) /* ValidLocations */
     , (510031,  11,          1) /* MaxStackSize */
     , (510031,  12,          1) /* StackSize */
     , (510031,  13,        100) /* StackUnitEncumbrance */
     , (510031,  14,        100) /* StackUnitMass */
     , (510031,  15,         10) /* StackUnitValue */
     , (510031,  16,     524296) /* ItemUseable */
     , (510031,  19,         10) /* Value */
     , (510031,  33,          1)
     , (510031,  91,        100) /* MaxStructure */
     , (510031,  92,        100) /* Structure */
     , (510031,  93,       1044) /* PhysicsState */
     , (510031,  94,      33025)
     , (510031, 105,        100) /* ItemWorkmanship */
     , (510031, 131,         26) /* MaterialType - Imperial Topaz */
     , (510031, 150,        103)
     , (510031, 151,          9) /* HookType */
     , (510031, 170,         10) /* NumItemsInMaterial */;

INSERT INTO `weenie_properties_string` (`object_Id`, `type`, `value`)
VALUES (510031,   1, 'Salvaged Imperial Topaz') /* Name */
     , (510031,  14, 'Apply this material to a treasure-generated weapon or magic-casting implement to imbue the target with Slash Rending. Slash Rending gives the weapon the ability to make its opponent vulnerable to slashing attacks. The amount of vulnerability depends on the attack skill of the wielder. This effect does not stack with Slashing Vulnerability spells. ') /* Use */
     , (510031,  15, 'Chips of imperial topaz material salvaged from old items.') /* LongDesc */
     , (510031,  22, '')
     , (510031,  23, '');

INSERT INTO `weenie_properties_d_i_d` (`object_Id`, `type`, `value`)
VALUES (510031,   1, 0x02000181) /* Setup */
     , (510031,   3, 0x20000014) /* SoundTable */
     , (510031,   6, 0x04000BEF)
     , (510031,   7, 0x100003CE) /* ClothingBase */
     , (510031,   8, 0x0600102C) /* Icon */
     , (510031,  22, 0x3400002B) /* PhysicsEffectTable */
     , (510031,  50, 0x060026FE) /* IconUnderlay */;

/* ---- 510032: Salvaged Jet (WS10) ---- */
DELETE FROM `weenie` WHERE `class_Id` = 510032;
INSERT INTO `weenie` (`class_Id`, `class_Name`, `type`, `last_Modified`)
VALUES (510032, 'ace510032-salvagejet', 44, '2026-06-27 00:00:00') /* CraftTool */;

INSERT INTO `weenie_properties_int` (`object_Id`, `type`, `value`)
VALUES (510032,   1, 1073741824) /* ItemType - Salvage */
     , (510032,   3,         14) /* PaletteTemplate */
     , (510032,   5,        100) /* EncumbranceVal */
     , (510032,   8,        100) /* Mass */
     , (510032,   9,          0) /* ValidLocations */
     , (510032,  11,          1) /* MaxStackSize */
     , (510032,  12,          1) /* StackSize */
     , (510032,  13,        100) /* StackUnitEncumbrance */
     , (510032,  14,        100) /* StackUnitMass */
     , (510032,  15,         10) /* StackUnitValue */
     , (510032,  16,     524296) /* ItemUseable */
     , (510032,  19,         10) /* Value */
     , (510032,  33,          1)
     , (510032,  91,        100) /* MaxStructure */
     , (510032,  92,        100) /* Structure */
     , (510032,  93,       1044) /* PhysicsState */
     , (510032,  94,      33025)
     , (510032, 105,        100) /* ItemWorkmanship */
     , (510032, 131,         27) /* MaterialType - Jet */
     , (510032, 150,        103)
     , (510032, 151,          9) /* HookType */
     , (510032, 170,         10) /* NumItemsInMaterial */;

INSERT INTO `weenie_properties_string` (`object_Id`, `type`, `value`)
VALUES (510032,   1, 'Salvaged Jet') /* Name */
     , (510032,  14, 'Apply this material to a treasure-generated weapon or magic-casting implement to imbue the target with Lightning Rending. Lightning Rending gives the weapon the ability to make its opponent vulnerable to lightning attacks. The amount of vulnerability depends on the attack skill of the wielder. This effect does not stack with Lightning Vulnerability spells. ') /* Use */
     , (510032,  15, 'Chips of jet material salvaged from old items.') /* LongDesc */
     , (510032,  22, '')
     , (510032,  23, '');

INSERT INTO `weenie_properties_d_i_d` (`object_Id`, `type`, `value`)
VALUES (510032,   1, 0x02000181) /* Setup */
     , (510032,   3, 0x20000014) /* SoundTable */
     , (510032,   6, 0x04000BEF)
     , (510032,   7, 0x100003CE) /* ClothingBase */
     , (510032,   8, 0x0600102C) /* Icon */
     , (510032,  22, 0x3400002B) /* PhysicsEffectTable */
     , (510032,  50, 0x06002700) /* IconUnderlay */;

/* ---- 510033: Salvaged Red Garnet (WS10) ---- */
DELETE FROM `weenie` WHERE `class_Id` = 510033;
INSERT INTO `weenie` (`class_Id`, `class_Name`, `type`, `last_Modified`)
VALUES (510033, 'ace510033-salvageredgarnet', 44, '2026-06-27 00:00:00') /* CraftTool */;

INSERT INTO `weenie_properties_int` (`object_Id`, `type`, `value`)
VALUES (510033,   1, 1073741824) /* ItemType - Salvage */
     , (510033,   3,         14) /* PaletteTemplate */
     , (510033,   5,        100) /* EncumbranceVal */
     , (510033,   8,        100) /* Mass */
     , (510033,   9,          0) /* ValidLocations */
     , (510033,  11,          1) /* MaxStackSize */
     , (510033,  12,          1) /* StackSize */
     , (510033,  13,        100) /* StackUnitEncumbrance */
     , (510033,  14,        100) /* StackUnitMass */
     , (510033,  15,         10) /* StackUnitValue */
     , (510033,  16,     524296) /* ItemUseable */
     , (510033,  19,         10) /* Value */
     , (510033,  33,          1)
     , (510033,  91,        100) /* MaxStructure */
     , (510033,  92,        100) /* Structure */
     , (510033,  93,       1044) /* PhysicsState */
     , (510033,  94,      33025)
     , (510033, 105,        100) /* ItemWorkmanship */
     , (510033, 131,         35) /* MaterialType - Red Garnet */
     , (510033, 150,        103)
     , (510033, 151,          9) /* HookType */
     , (510033, 170,         10) /* NumItemsInMaterial */;

INSERT INTO `weenie_properties_string` (`object_Id`, `type`, `value`)
VALUES (510033,   1, 'Salvaged Red Garnet') /* Name */
     , (510033,  14, 'Apply this material to a treasure-generated weapon or magic-casting implement to imbue the target with Fire Rending. Fire Rending gives the weapon the ability to make its opponent vulnerable to fire attacks. The amount of vulnerability depends on the attack skill of the wielder. This effect does not stack with Fire Vulnerability spells. ') /* Use */
     , (510033,  15, 'Chips of red garnet material salvaged from old items.') /* LongDesc */
     , (510033,  22, '')
     , (510033,  23, '');

INSERT INTO `weenie_properties_d_i_d` (`object_Id`, `type`, `value`)
VALUES (510033,   1, 0x02000181) /* Setup */
     , (510033,   3, 0x20000014) /* SoundTable */
     , (510033,   6, 0x04000BEF)
     , (510033,   7, 0x100003CE) /* ClothingBase */
     , (510033,   8, 0x0600102C) /* Icon */
     , (510033,  22, 0x3400002B) /* PhysicsEffectTable */
     , (510033,  50, 0x0600270C) /* IconUnderlay */;

/* ---- 510034: Salvaged Sunstone (WS10) ---- */
DELETE FROM `weenie` WHERE `class_Id` = 510034;
INSERT INTO `weenie` (`class_Id`, `class_Name`, `type`, `last_Modified`)
VALUES (510034, 'ace510034-salvagesunstone', 44, '2026-06-27 00:00:00') /* CraftTool */;

INSERT INTO `weenie_properties_int` (`object_Id`, `type`, `value`)
VALUES (510034,   1, 1073741824) /* ItemType - Salvage */
     , (510034,   3,          2) /* PaletteTemplate */
     , (510034,   5,        100) /* EncumbranceVal */
     , (510034,   8,        100) /* Mass */
     , (510034,   9,          0) /* ValidLocations */
     , (510034,  11,          1) /* MaxStackSize */
     , (510034,  12,          1) /* StackSize */
     , (510034,  13,        100) /* StackUnitEncumbrance */
     , (510034,  14,        100) /* StackUnitMass */
     , (510034,  15,         10) /* StackUnitValue */
     , (510034,  16,     524296) /* ItemUseable */
     , (510034,  19,         10) /* Value */
     , (510034,  33,          1)
     , (510034,  91,        100) /* MaxStructure */
     , (510034,  92,        100) /* Structure */
     , (510034,  93,       1044) /* PhysicsState */
     , (510034,  94,        257)
     , (510034, 105,        100) /* ItemWorkmanship */
     , (510034, 131,         41) /* MaterialType - Sunstone */
     , (510034, 150,        103)
     , (510034, 151,          9) /* HookType */
     , (510034, 170,         10) /* NumItemsInMaterial */;

INSERT INTO `weenie_properties_string` (`object_Id`, `type`, `value`)
VALUES (510034,   1, 'Salvaged Sunstone') /* Name */
     , (510034,  14, 'Apply this material to a treasure-generated weapon to imbue the target with Armor Rending. Armor Rending gives the item the ability to ignore some of its opponent''s armor. The amount of armor it ignores depends on the attack skill of the wielder.') /* Use */
     , (510034,  15, 'Chips of sunstone material salvaged from old items.') /* LongDesc */
     , (510034,  22, '')
     , (510034,  23, '');

INSERT INTO `weenie_properties_d_i_d` (`object_Id`, `type`, `value`)
VALUES (510034,   1, 0x02000181) /* Setup */
     , (510034,   3, 0x20000014) /* SoundTable */
     , (510034,   6, 0x04000BEF)
     , (510034,   7, 0x100003CE) /* ClothingBase */
     , (510034,   8, 0x0600102C) /* Icon */
     , (510034,  22, 0x3400002B) /* PhysicsEffectTable */
     , (510034,  50, 0x06002717) /* IconUnderlay */;

/* ---- 510035: Salvaged White Sapphire (WS10) ---- */
DELETE FROM `weenie` WHERE `class_Id` = 510035;
INSERT INTO `weenie` (`class_Id`, `class_Name`, `type`, `last_Modified`)
VALUES (510035, 'ace510035-salvagewhitesapphire', 44, '2026-06-27 00:00:00') /* CraftTool */;

INSERT INTO `weenie_properties_int` (`object_Id`, `type`, `value`)
VALUES (510035,   1, 1073741824) /* ItemType - Salvage */
     , (510035,   3,         14) /* PaletteTemplate */
     , (510035,   5,        100) /* EncumbranceVal */
     , (510035,   8,        100) /* Mass */
     , (510035,   9,          0) /* ValidLocations */
     , (510035,  11,          1) /* MaxStackSize */
     , (510035,  12,          1) /* StackSize */
     , (510035,  13,        100) /* StackUnitEncumbrance */
     , (510035,  14,        100) /* StackUnitMass */
     , (510035,  15,         10) /* StackUnitValue */
     , (510035,  16,     524296) /* ItemUseable */
     , (510035,  19,         10) /* Value */
     , (510035,  33,          1)
     , (510035,  91,        100) /* MaxStructure */
     , (510035,  92,        100) /* Structure */
     , (510035,  93,       1044) /* PhysicsState */
     , (510035,  94,      33025)
     , (510035, 105,        100) /* ItemWorkmanship */
     , (510035, 131,         47) /* MaterialType - White Sapphire */
     , (510035, 150,        103)
     , (510035, 151,          9) /* HookType */
     , (510035, 170,         10) /* NumItemsInMaterial */;

INSERT INTO `weenie_properties_string` (`object_Id`, `type`, `value`)
VALUES (510035,   1, 'Salvaged White Sapphire') /* Name */
     , (510035,  14, 'Apply this material to a treasure-generated weapon or magic-casting implement to imbue the target with Bludgeon Rending. Bludgeon Rending gives the weapon the ability to make its opponent vulnerable to bludgeoning attacks. The amount of vulnerability depends on the attack skill of the wielder. This effect does not stack with Bludgeoning Vulnerability spells. ') /* Use */
     , (510035,  15, 'Chips of white sapphire material salvaged from old items.') /* LongDesc */
     , (510035,  22, '')
     , (510035,  23, '');

INSERT INTO `weenie_properties_d_i_d` (`object_Id`, `type`, `value`)
VALUES (510035,   1, 0x02000181) /* Setup */
     , (510035,   3, 0x20000014) /* SoundTable */
     , (510035,   6, 0x04000BEF)
     , (510035,   7, 0x100003CE) /* ClothingBase */
     , (510035,   8, 0x0600102C) /* Icon */
     , (510035,  22, 0x3400002B) /* PhysicsEffectTable */
     , (510035,  50, 0x0600271E) /* IconUnderlay */;

/* ---- 510036: Salvaged Mahogany (WS10) ---- */
DELETE FROM `weenie` WHERE `class_Id` = 510036;
INSERT INTO `weenie` (`class_Id`, `class_Name`, `type`, `last_Modified`)
VALUES (510036, 'ace510036-salvagemahogany', 44, '2026-08-08 00:00:00') /* CraftTool */;

INSERT INTO `weenie_properties_int` (`object_Id`, `type`, `value`)
VALUES (510036,   1, 1073741824) /* ItemType - Salvage */
     , (510036,   3,         14) /* PaletteTemplate */
     , (510036,   5,        100) /* EncumbranceVal */
     , (510036,   8,        100) /* Mass */
     , (510036,   9,          0) /* ValidLocations */
     , (510036,  11,          1) /* MaxStackSize */
     , (510036,  12,          1) /* StackSize */
     , (510036,  13,        100) /* StackUnitEncumbrance */
     , (510036,  14,        100) /* StackUnitMass */
     , (510036,  15,         10) /* StackUnitValue */
     , (510036,  16,     524296) /* ItemUseable */
     , (510036,  19,         10) /* Value */
     , (510036,  33,          1) /* Bonded */
     , (510036,  91,        100) /* MaxStructure */
     , (510036,  92,        100) /* Structure */
     , (510036,  93,       1044) /* PhysicsState */
     , (510036,  94,        256) /* TargetType - MissileWeapon */
     , (510036, 105,        100) /* ItemWorkmanship */
     , (510036, 131,         74) /* MaterialType - Mahogany */
     , (510036, 150,        103)
     , (510036, 151,          9) /* HookType */
     , (510036, 170,         10) /* NumItemsInMaterial */;

INSERT INTO `weenie_properties_string` (`object_Id`, `type`, `value`)
VALUES (510036,   1, 'Salvaged Mahogany') /* Name */
     , (510036,  14, 'Apply this material to a treasure-generated missile weapon to increase the weapon''s damage modifier by 4%.') /* Use */
     , (510036,  15, 'A bundle of mahogany material salvaged from old items.') /* LongDesc */
     , (510036,  22, '')
     , (510036,  23, '');

INSERT INTO `weenie_properties_d_i_d` (`object_Id`, `type`, `value`)
VALUES (510036,   1, 0x02000181) /* Setup */
     , (510036,   3, 0x20000014) /* SoundTable */
     , (510036,   6, 0x04000BEF) /* PaletteBase */
     , (510036,   7, 0x100003CE) /* ClothingBase */
     , (510036,   8, 0x060026C4) /* Icon */
     , (510036,  22, 0x3400002B) /* PhysicsEffectTable */
     , (510036,  50, 0x060026D0) /* IconOverlay */
     , (510036,  52, 0x06020017) /* IconUnderlay */;
