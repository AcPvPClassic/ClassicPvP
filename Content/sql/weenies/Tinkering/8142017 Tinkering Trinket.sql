DELETE FROM `weenie` WHERE `class_Id` = 8142017;

INSERT INTO `weenie` (`class_Id`, `class_Name`, `type`, `last_Modified`)
VALUES (8142017, 'tinkeringtrinket', 1, '2021-11-17 16:56:08') /* Generic */;

INSERT INTO `weenie_properties_int` (`object_Id`, `type`, `value`)
VALUES (8142017,   1,          8) /* ItemType - Jewelry */
     , (8142017,   5,         60) /* EncumbranceVal */
     , (8142017,   9,   67108864) /* ValidLocations - TrinketOne */
     , (8142017,  16,          1) /* ItemUseable - No */
     , (8142017,  18,          1) /* UI Effects Magical */
     , (8142017,  19,         10) /* Value */
     , (8142017,  93,       1044) /* PhysicsState - Ethereal, IgnoreCollisions, Gravity */
     , (8142017, 106,         50) /* ItemSpellcraft */
     , (8142017, 107,       120000) /* ItemCurMana */
     , (8142017, 108,       120000) /* ItemMaxMana */
     , (8142017,  33,          1) /* Bonded */
     , (8142017, 114,          1) /* Attuned */
     , (8142017, 109,         15) /* ItemDifficulty */;

INSERT INTO `weenie_properties_bool` (`object_Id`, `type`, `value`)
VALUES (8142017,  11, True ) /* IgnoreCollisions */
     , (8142017,  13, True ) /* Ethereal */
     , (8142017,  14, True ) /* GravityStatus */
     , (8142017,  19, True ) /* Attackable */
     , (8142017,  22, True ) /* Inscribable */
     , (8142017,  91, False) /* Retained */;

INSERT INTO `weenie_properties_float` (`object_Id`, `type`, `value`)
VALUES (8142017,   5,  -0.049) /* ManaRate */
     , (8142017,  39,    0.67) /* DefaultScale */;

INSERT INTO `weenie_properties_string` (`object_Id`, `type`, `value`)
VALUES (8142017,   1, 'Trinket of Tinkering') /* Name */
     , (8142017,  16, 'A trinket made for enhanced tinkering and trade crafts.') /* LongDesc */;

INSERT INTO `weenie_properties_d_i_d` (`object_Id`, `type`, `value`)
VALUES (8142017,   1, 0x02000179) /* Setup */
     , (8142017,   3, 0x20000014) /* SoundTable */
     , (8142017,   8, 100668277) /* Icon */
     , (8142017,  52, 100673920) /* IconUnderlay */
     , (8142017,  22, 0x3400002B) /* PhysicsEffectTable */;

INSERT INTO `weenie_properties_spell_book` (`object_Id`, `spell`, `probability`)
VALUES (8142017,  689,      2)  /* Arcane Enlightenment Other VI */
, (8142017, 713, 2) /*	Armor Tinkering Expertise Other VI */
, (8142017, 737, 2) /*	Item Tinkering Expertise Other VI */
, (8142017, 761, 2) /*	Magic Item Tinkering Expertise Other VI */
, (8142017, 785, 2) /*	Weapon Tinkering Expertise Other VI */
, (8142017, 933, 2) /*	Lockpick Mastery Other VI */
, (8142017, 1005, 2) /*	Leaden Feet Other VI */
, (8142017, 1017, 2) /*	Jumping Ineptitude Other VI */
, (8142017, 1053, 2) /*	Bludgeoning Vulnerability Other VI */
, (8142017, 1065, 2) /*	Cold Vulnerability Other VI */
, (8142017, 1089, 2) /*	Lightning Vulnerability Other VI */
, (8142017, 1108, 2) /*	Fire Vulnerability Other VI */
, (8142017, 1132, 2) /*	Blade Vulnerability Other VI */
, (8142017, 1156, 2) /*	Piercing Vulnerability Other VI */
, (8142017, 1327, 2) /*	Imperil Other VI */
, (8142017, 1337, 2) /*	Strength Other VI */
, (8142017, 1360, 2) /*	Endurance Other VI */
, (8142017, 1384, 2) /*	Coordination Other VI */
, (8142017, 1408, 2) /*	Quickness Other VI */
, (8142017, 1432, 2) /*	Focus Other VI */
, (8142017, 1456, 2) /*	Willpower Other VI */
, (8142017, 1714, 2) /*	Cooking Mastery Other VI */
, (8142017, 1738, 2) /*	Fletching Mastery Other VI */
, (8142017, 1762, 2) /*	Alchemy Mastery Other VI */
, (8142017, 1774, 2) /*	Alchemy Ineptitude Other VI */;
