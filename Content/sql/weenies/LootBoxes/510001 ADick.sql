DELETE FROM `weenie` WHERE `class_Id` = 510001;

INSERT INTO `weenie` (`class_Id`, `class_Name`, `type`, `last_Modified`)
VALUES (510001, 'ace510001-adick', 18, '2005-02-09 10:00:00') /* Food */;

INSERT INTO `weenie_properties_int` (`object_Id`, `type`, `value`)
VALUES (510001,   1,         32) /* ItemType - Food */
     , (510001,   5,        200) /* EncumbranceVal */
     , (510001,   8,         50) /* Mass */
     , (510001,   9,          0) /* ValidLocations - None */
     , (510001,  11,          1) /* MaxStackSize - not stackable */
     , (510001,  12,          1) /* StackSize */
     , (510001,  13,        200) /* StackUnitEncumbrance */
     , (510001,  14,         50) /* StackUnitMass */
     , (510001,  15,          1) /* StackUnitValue */
     , (510001,  16,          8) /* ItemUseable - Contained */
     , (510001,  19,          1) /* Value - 1 (costs 1 PK Trophy at Anti Parazi) */
     , (510001,  93,       1044) /* PhysicsState - Ethereal, IgnoreCollisions, Gravity */
	 , (510001,  280,        110) /* SharedCooldown */;

INSERT INTO `weenie_properties_string` (`object_Id`, `type`, `value`)
VALUES (510001,   1, 'A Dick') /* Name */
     , (510001,  16, 'It''s a dick! Eat it to burn away your Vitae penalty. It''ll also put hair on your chest.') /* LongDesc */
	 , (510001,  14, 'Eat this to remove your Vitae penalty.') /* Use */
     , (510001,  15, 'A plump meaty dick.') /* ShortDesc */
     , (510001,  20, 'A Stack of Dicks') /* PluralName */;

INSERT INTO `weenie_properties_float` (`object_Id`, `type`, `value`)
VALUES (510001, 167,      1) /* CooldownDuration */;

INSERT INTO `weenie_properties_d_i_d` (`object_Id`, `type`, `value`)
VALUES (510001,   1, 0x020008CA) /* Setup */
     , (510001,   3, 0x20000014) /* SoundTable */
     , (510001,   8, 0x06001D98) /* Icon */
     , (510001,  22, 0x3400002B) /* PhysicsEffectTable */
	 /*, (510001,  52, 0x06007575)  IconUnderlay */;

/* On use, this item removes the player's Vitae penalty. The effect is handled
   server-side in Food.ApplyConsumable (keyed on WeenieClassId 510001), which also
   consumes the item — no on-use emote is required. */
