/* Steel Chest (8999) and Sturdy Steel Chest (24476)
   Make these instantly re-lock and reroll their contents when closed,
   the same way Darkbeat's Storage Locker (480607) does.

   Chest.Close() checks ChestRegenOnClose (PropertyBool 86) and, if set,
   immediately calls Reset() instead of waiting for ResetInterval to
   elapse. Reset() re-locks the chest (when DefaultLocked is set) and
   regenerates its loot generator. See Source/ACE.Server/WorldObjects/Chest.cs.

   This only sets the new property on the existing base-game weenies —
   it does not redefine them, since their full definitions (loot
   generator, physics, etc.) live in the base world database, not this
   repo.
*/

INSERT INTO `weenie_properties_bool` (`object_Id`, `type`, `value`)
VALUES (8999,  86, True) /* Steel Chest - ChestRegenOnClose */
     , (24476, 86, True) /* Sturdy Steel Chest - ChestRegenOnClose */
ON DUPLICATE KEY UPDATE `value` = VALUES(`value`);
