DELETE FROM `weenie` WHERE `class_Id` = 42025456;

INSERT INTO `weenie` (`class_Id`, `class_Name`, `type`, `last_Modified`)
VALUES (42025456, 'unopenabledoor', 19, '2005-02-09 10:00:00') /* Door */;

INSERT INTO `weenie_properties_int` (`object_Id`, `type`, `value`)
VALUES (42025456,   1,        128) /* ItemType - Misc */
     , (42025456,   8,        500) /* Mass */
     , (42025456,  16,          1) /* ItemUseable - No */
     , (42025456,  19,          0) /* Value */
     , (42025456,  38,     999999) /* ResistLockpick - >= 9999 = unpickable (CannotBePicked) */
     , (42025456,  93,          8) /* PhysicsState - ReportCollisions */
     , (42025456, 119,          0) /* Active - 0 = cannot be activated/used, so it can never be opened */;

INSERT INTO `weenie_properties_bool` (`object_Id`, `type`, `value`)
VALUES (42025456,   1, True ) /* Stuck */
     , (42025456,   2, False) /* Open */
     , (42025456,   3, True ) /* Locked */
     , (42025456,  11, False) /* IgnoreCollisions */
     , (42025456,  12, True ) /* ReportCollisions */
     , (42025456,  13, False) /* Ethereal */
     , (42025456,  14, False) /* GravityStatus */
     , (42025456,  24, True ) /* UiHidden */
     , (42025456,  33, False) /* ResetMessagePending */
     , (42025456,  34, False) /* DefaultOpen */
     , (42025456,  35, True ) /* DefaultLocked */;

INSERT INTO `weenie_properties_float` (`object_Id`, `type`, `value`)
VALUES (42025456,  11,     300) /* ResetInterval */
     , (42025456,  39,       1) /* DefaultScale */;

INSERT INTO `weenie_properties_string` (`object_Id`, `type`, `value`)
VALUES (42025456,   1, 'Reinforced Door') /* Name */
     , (42025456,  15, 'A locked door, impossible to pick.') /* ShortDesc */;

INSERT INTO `weenie_properties_d_i_d` (`object_Id`, `type`, `value`)
VALUES (42025456,   1, 0x02000FB5) /* Setup */
     , (42025456,   2, 0x09000115) /* MotionTable */
     , (42025456,   3, 0x20000059) /* SoundTable */
     , (42025456,   8, 0x060027C8) /* Icon */
     , (42025456,  22, 0x3400006B) /* PhysicsEffectTable */;
