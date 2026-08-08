/* Dungeon Boss death-loot profile (treasure_Type 940000).
   A copy of Darkbeat's Storage Locker profile (treasure_Type 10000, tier 6) with a
   slightly higher loot_Quality_Mod (0.425 -> 0.50). Referenced by every Dungeon Boss
   weenie via DeathTreasureType (DID type 35) so the boss corpse generates normal loot
   in addition to the scattered A Box and the trophies/phials awarded on kill. */

DELETE FROM ace_world.treasure_death WHERE treasure_Type = 940000;

INSERT INTO ace_world.treasure_death
(
  treasure_Type,
  tier,
  loot_Quality_Mod,
  unknown_Chances,
  item_Chance,
  item_Min_Amount,
  item_Max_Amount,
  item_Treasure_Type_Selection_Chances,
  magic_Item_Chance,
  magic_Item_Min_Amount,
  magic_Item_Max_Amount,
  magic_Item_Treasure_Type_Selection_Chances,
  mundane_Item_Chance,
  mundane_Item_Min_Amount,
  mundane_Item_Max_Amount,
  mundane_Item_Type_Selection_Chances
)
VALUES
(
  940000,
  6,
  0.50,
  19,
  0,
  0,
  1,
  11,
  100,
  12,
  18,
  22,
  0,
  0,
  1,
  7
);
