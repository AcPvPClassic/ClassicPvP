/* Tusker Tusk and Olthoi Pincer turn-in quests
   Lower the repeat timer (min_Delta) from 1814400 (21 days) to 72000 (20 hours).

   - Tusker Tusk turn-ins: the 14 Tusk*PickUp quests stamped when a tusk is picked up.
   - Olthoi Pincer turn-ins: the 8 OlthoiHunting quests. Each is stamped by picking up
     the matching pincer (PropertyString.Quest = 33) that Behdo Yii accepts:
         Harvester Pincer (10845) -> OlthoiHunting1
         Gardener Pincer  (10844) -> OlthoiHunting2
         Soldier Pincer   (10847) -> OlthoiHunting3
         Legionary Pincer (10846) -> OlthoiHunting4
         Eviscerator Pincer (10843) -> OlthoiHunting5
         Worker Pincer    (27591) -> OlthoiHunting6
         Warrior Pincer   (27590) -> OlthoiHunting7
         Mutilator Pincer (27589) -> OlthoiHunting8
*/

UPDATE `quest`
SET `min_Delta` = 72000
WHERE `name` IN (
    -- Tusker Tusk turn-ins
    'TuskArmoredPickUp',
    'TuskAssailerPickUp',
    'TuskCrimsonbackPickUp',
    'TuskDevastatorPickUp',
    'TuskFemalePickUp',
    'TuskGoldenbackPickUp',
    'TuskGuardPickUp',
    'TuskLiberatorPickUp',
    'TuskMalePickUp',
    'TuskPlatedPickUp',
    'TuskRampagerPickUp',
    'TuskRedeemerPickUp',
    'TuskSilverPickUp',
    'TuskSlavePickUp',
    -- Olthoi Pincer turn-ins
    'OlthoiHunting1',
    'OlthoiHunting2',
    'OlthoiHunting3',
    'OlthoiHunting4',
    'OlthoiHunting5',
    'OlthoiHunting6',
    'OlthoiHunting7',
    'OlthoiHunting8'
);
