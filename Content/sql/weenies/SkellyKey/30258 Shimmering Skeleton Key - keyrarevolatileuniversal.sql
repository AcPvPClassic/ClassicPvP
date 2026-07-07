DELETE FROM `weenie` WHERE `class_Id` = 30258;

INSERT INTO `weenie` (`class_Id`, `class_Name`, `type`, `last_Modified`)
VALUES (30258, 'keyrarevolatileuniversal', 22, '2019-02-04 06:52:23') /* Key */;

INSERT INTO `weenie_properties_int` (`object_Id`, `type`, `value`)
VALUES (30258,   1,      16384) /* ItemType - Key */
     , (30258,   5,          5) /* EncumbranceVal */
     , (30258,   8,          5) /* Mass */
     , (30258,  16,    2097160) /* ItemUseable - SourceContainedTargetRemote */
     , (30258,  19,          0) /* Value */
     , (30258,  33,         -1) /* Bonded - Slippery (always drops on death) */
     , (30258,  91,          1) /* MaxStructure - single use */
     , (30258,  92,          1) /* Structure - single use */
     , (30258,  93,       1044) /* PhysicsState - Ethereal, IgnoreCollisions, Gravity */
     , (30258,  94,        640) /* TargetType - LockableMagicTarget */;

INSERT INTO `weenie_properties_bool` (`object_Id`, `type`, `value`)
VALUES (30258,  22, True ) /* Inscribable */
     , (30258,  62, True ) /* OpensAnyLock - unlocks any door or chest */;

INSERT INTO `weenie_properties_string` (`object_Id`, `type`, `value`)
VALUES (30258,   1, 'Shimmering Skeleton Key') /* Name */
     , (30258,  14, 'Use this key on any locked door or chest to unlock it, no matter the lock. The key crumbles to dust after a single use. It is slippery and will slip from your grasp if you are slain.') /* Use */
     , (30258,  16, 'A rare skeleton key that opens any lock. Single use, and drops on death.') /* LongDesc */;

INSERT INTO `weenie_properties_d_i_d` (`object_Id`, `type`, `value`)
VALUES (30258,   1, 0x02000160) /* Setup */
     , (30258,   3, 0x20000014) /* SoundTable */
     , (30258,   8, 0x0600105D) /* Icon */
     , (30258,  22, 0x3400002B) /* PhysicsEffectTable */;
