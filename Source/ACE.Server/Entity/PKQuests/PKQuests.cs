using System;
using System.Collections.Generic;
using System.Linq;

namespace ACE.Server.Entity.PKQuests
{
    public static class PKQuests
    {
        private static List<PKQuest> _pkQuestList = null;

        public static List<PKQuest> PkQuestList
        {
            get
            {
                if (_pkQuestList == null)
                {
                    _pkQuestList = new List<PKQuest>();

                    //- Participate in 5 arena matches
                    var arena_any_5 = new PKQuest();
                    arena_any_5.QuestCode = "ARENA_ANY_5";
                    arena_any_5.Description = "Participate in 5 Arena matches";
                    arena_any_5.RewardDescription = "25% XP to next level, 1 Darkbeat Key, 1 Phial of Bloody Tears, 5 PK Trophies";
                    arena_any_5.Rewards = new List<string>() { "XP%,25", "DBKEY,1", "PHIAL,1", "PKTROPHY,5" };
                    arena_any_5.TaskCount = 5;
                    _pkQuestList.Add(arena_any_5);

                    //- Participate in 15 arena matches
                    var arena_any_15 = new PKQuest();
                    arena_any_15.QuestCode = "ARENA_ANY_15";
                    arena_any_15.Description = "Participate in 15 Arena matches";
                    arena_any_15.RewardDescription = "35% XP to next level, 3 Darkbeat Keys, 2 Phials of Bloody Tears, 25 PK Trophies, 1 Box";
                    arena_any_15.Rewards = new List<string>() { "XP%,35", "DBKEY,3", "PHIAL,2", "PKTROPHY,25", "BOX,1" };
                    arena_any_15.TaskCount = 15;
                    _pkQuestList.Add(arena_any_15);

                    //- Participate in 30 arena matches
                    var arena_any_30 = new PKQuest();
                    arena_any_30.QuestCode = "ARENA_ANY_30";
                    arena_any_30.Description = "Participate in 30 Arena matches";
                    arena_any_30.RewardDescription = "50% XP to next level, 3 Darkbeat Keys, 3 Phials of Bloody Tears, 50 PK Trophies, 2 Boxes.";
                    arena_any_30.Rewards = new List<string>() { "XP%,50", "DBKEY,3", "PHIAL,3", "PKTROPHY,50", "BOX,2" };
                    arena_any_30.TaskCount = 30;
                    _pkQuestList.Add(arena_any_30);

                    //- Participate in 50 arena matches
                    var arena_any_50 = new PKQuest();
                    arena_any_50.QuestCode = "ARENA_ANY_50";
                    arena_any_50.Description = "Participate in 50 Arena matches";
                    arena_any_50.RewardDescription = "100% XP to next level, 5 Darkbeat Keys, 5 Phials of Bloody Tears, 100 PK Trophies, 5 Boxes.";
                    arena_any_50.Rewards = new List<string>() { "XP%,100", "DBKEY,5", "PHIAL,5", "PKTROPHY,100", "BOX,5" };
                    arena_any_50.TaskCount = 50;
                    _pkQuestList.Add(arena_any_50);

                    //- Win 10 arena matches
                    var arena_any_win_10 = new PKQuest();
                    arena_any_win_10.QuestCode = "ARENA_ANY_WIN_10";
                    arena_any_win_10.Description = "Win 10 Arena matches";
                    arena_any_win_10.RewardDescription = "100% XP to next level, 3 Darkbeat Keys, 2 Phials of Bloody Tears, 40 PK Trophies, 1 Box";
                    arena_any_win_10.Rewards = new List<string>() { "XP%,100", "DBKEY,3", "PHIAL,2", "PKTROPHY,40", "BOX,1" };
                    arena_any_win_10.TaskCount = 10;
                    _pkQuestList.Add(arena_any_win_10);

                    //- Win 20 arena matches
                    var arena_any_win_20 = new PKQuest();
                    arena_any_win_20.QuestCode = "ARENA_ANY_WIN_20";
                    arena_any_win_20.Description = "Win 20 Arena matches";
                    arena_any_win_20.RewardDescription = "100% XP to next level, 3 Darkbeat Keys, 3 Phials of Bloody Tears, 100 PK Trophies, 3 Boxes";
                    arena_any_win_20.Rewards = new List<string>() { "XP%,100", "DBKEY,3", "PHIAL,3", "PKTROPHY,100", "BOX,3" };
                    arena_any_win_20.TaskCount = 20;
                    _pkQuestList.Add(arena_any_win_20);

                    //- Win 30 arena matches
                    var arena_any_win_30 = new PKQuest();
                    arena_any_win_30.QuestCode = "ARENA_ANY_WIN_30";
                    arena_any_win_30.Description = "Win 30 Arena matches";
                    arena_any_win_30.RewardDescription = "100% XP to next level, 5 Darkbeat Keys, 5 Phials of Bloody Tears, 200 PK Trophies, 5 Boxes";
                    arena_any_win_30.Rewards = new List<string>() { "XP%,100", "DBKEY,5", "PHIAL,5", "PKTROPHY,200", "BOX,5" };
                    arena_any_win_30.TaskCount = 30;
                    _pkQuestList.Add(arena_any_win_30);

                    //- Kill 10 players from an opposing whitelisted allegiance (open world or arena)
                    var kill_any_10 = new PKQuest();
                    kill_any_10.QuestCode = "KILL_ANY_10";
                    kill_any_10.Description = "Kill any 10 players from an opposing whitelisted allegiance";
                    kill_any_10.RewardDescription = "50% XP to next level, 1 Phial of Bloody Tears, 20 PK Trophies, 1 Box";
                    kill_any_10.Rewards = new List<string>() { "XP%,50", "PHIAL,1", "PKTROPHY,20", "BOX,1" };
                    kill_any_10.TaskCount = 10;
                    _pkQuestList.Add(kill_any_10);

                    //- Kill 30 players from an opposing whitelisted allegiance (open world or arena)
                    var kill_any_30 = new PKQuest();
                    kill_any_30.QuestCode = "KILL_ANY_30";
                    kill_any_30.Description = "Kill any 30 players from an opposing whitelisted allegiance";
                    kill_any_30.RewardDescription = "75% XP to next level, 3 Phials of Bloody Tears, 100 PK Trophies, 5 Boxes";
                    kill_any_30.Rewards = new List<string>() { "XP%,75", "PHIAL,3", "PKTROPHY,100", "BOX,5" };
                    kill_any_30.TaskCount = 30;
                    _pkQuestList.Add(kill_any_30);

                    //- Participate in 10 1v1 arena matches
                    var arena_1v1_10 = new PKQuest();
                    arena_1v1_10.QuestCode = "ARENA_1v1_10";
                    arena_1v1_10.Description = "Participate in 10 Arena 1v1 matches";
                    arena_1v1_10.RewardDescription = "25% XP to next level, 1 Darkbeat Key, 1 Phial of Bloody Tears, 5 PK Trophies";
                    arena_1v1_10.Rewards = new List<string>() { "XP%,25", "DBKEY,1", "PHIAL,1", "PKTROPHY,5" };
                    arena_1v1_10.TaskCount = 10;
                    _pkQuestList.Add(arena_1v1_10);

                    //- Participate in 20 1v1 arena matches
                    var arena_1v1_20 = new PKQuest();
                    arena_1v1_20.QuestCode = "ARENA_1v1_20";
                    arena_1v1_20.Description = "Participate in 20 Arena 1v1 matches";
                    arena_1v1_20.RewardDescription = "35% XP to next level, 2 Darkbeat Keys, 2 Phials of Bloody Tears, 25 PK Trophies";
                    arena_1v1_20.Rewards = new List<string>() { "XP%,35", "DBKEY,2", "PHIAL,2", "PKTROPHY,25" };
                    arena_1v1_20.TaskCount = 20;
                    _pkQuestList.Add(arena_1v1_20);

                    //- Participate in 3 2v2 arena matches
                    var arena_2v2_3 = new PKQuest();
                    arena_2v2_3.QuestCode = "ARENA_2v2_3";
                    arena_2v2_3.Description = "Participate in 3 Arena 2v2 matches";
                    arena_2v2_3.RewardDescription = "50% XP to next level, 1 Darkbeat Key, 1 Phial of Bloody Tears, 25 PK Trophies";
                    arena_2v2_3.Rewards = new List<string>() { "XP%,50", "DBKEY,1", "PHIAL,1", "PKTROPHY,25" };
                    arena_2v2_3.TaskCount = 3;
                    _pkQuestList.Add(arena_2v2_3);

                    //- Participate in 10 2v2 arena matches
                    var arena_2v2_10 = new PKQuest();
                    arena_2v2_10.QuestCode = "ARENA_2v2_10";
                    arena_2v2_10.Description = "Participate in 10 Arena 2v2 matches";
                    arena_2v2_10.RewardDescription = "75% XP to next level, 3 Darkbeat Keys, 3 Phials of Bloody Tears, 50 PK Trophies";
                    arena_2v2_10.Rewards = new List<string>() { "XP%,75", "DBKEY,3", "PHIAL,3", "PKTROPHY,50" };
                    arena_2v2_10.TaskCount = 10;
                    _pkQuestList.Add(arena_2v2_10);

                    //- Participate in 2 FFA arena matches
                    var arena_ffa_2 = new PKQuest();
                    arena_ffa_2.QuestCode = "ARENA_FFA_2";
                    arena_ffa_2.Description = "Participate in 2 Arena FFA matches";
                    arena_ffa_2.RewardDescription = "25% XP to next level, 1 Darkbeat Key, 1 Phial of Bloody Tears, 15 PK Trophies";
                    arena_ffa_2.Rewards = new List<string>() { "XP%,25", "DBKEY,1", "PHIAL,1", "PKTROPHY,15" };
                    arena_ffa_2.TaskCount = 2;
                    _pkQuestList.Add(arena_ffa_2);

                    //- Participate in 2 Tugak arena matches
                    var arena_tugak_2 = new PKQuest();
                    arena_tugak_2.QuestCode = "ARENA_TUGAK_2";
                    arena_tugak_2.Description = "Participate in 2 Arena Tugak War matches";
                    arena_tugak_2.RewardDescription = "25% XP to next level, 1 Darkbeat Key, 1 Phial of Bloody Tears, 15 PK Trophies";
                    arena_tugak_2.Rewards = new List<string>() { "XP%,25", "DBKEY,1", "PHIAL,1", "PKTROPHY,15" };
                    arena_tugak_2.TaskCount = 2;
                    _pkQuestList.Add(arena_tugak_2);

                    //- Participate in 25 Tugak arena matches
                    var arena_tugak_25 = new PKQuest();
                    arena_tugak_25.QuestCode = "ARENA_TUGAK_25";
                    arena_tugak_25.Description = "Participate in 25 Arena Tugak War matches";
                    arena_tugak_25.RewardDescription = "100% XP to next level, 4 Darkbeat Keys, 15 Phials of Bloody Tears, 250 PK Trophies, 5 Boxes";
                    arena_tugak_25.Rewards = new List<string>() { "XP%,100", "DBKEY,4", "PHIAL,15", "PKTROPHY,250", "BOX,5" };
                    arena_tugak_25.TaskCount = 25;
                    _pkQuestList.Add(arena_tugak_25);

                    //- Participate in 1 group arena match
                    var arena_group_1 = new PKQuest();
                    arena_group_1.QuestCode = "ARENA_GROUP_1";
                    arena_group_1.Description = "Participate in 1 Arena Group match";
                    arena_group_1.RewardDescription = "50% XP to next level, 2 Darkbeat Keys, 1 Phial of Bloody Tears, 25 PK Trophies";
                    arena_group_1.Rewards = new List<string>() { "XP%,50", "DBKEY,2", "PHIAL,1", "PKTROPHY,25" };
                    arena_group_1.TaskCount = 1;
                    _pkQuestList.Add(arena_group_1);

                    //- Participate in 3 group arena matches
                    var arena_group_3 = new PKQuest();
                    arena_group_3.QuestCode = "ARENA_GROUP_3";
                    arena_group_3.Description = "Participate in 3 Arena Group matches";
                    arena_group_3.RewardDescription = "75% XP to next level, 3 Darkbeat Keys, 2 Phials of Bloody Tears, 50 PK Trophies";
                    arena_group_3.Rewards = new List<string>() { "XP%,75", "DBKEY,3", "PHIAL,2", "PKTROPHY,50" };
                    arena_group_3.TaskCount = 3;
                    _pkQuestList.Add(arena_group_3);

                    //- Participate in 10 group arena matches
                    var arena_group_10 = new PKQuest();
                    arena_group_10.QuestCode = "ARENA_GROUP_10";
                    arena_group_10.Description = "Participate in 10 Arena Group matches";
                    arena_group_10.RewardDescription = "100% XP to next level, 5 Darkbeat Keys, 3 Phials of Bloody Tears, 75 PK Trophies";
                    arena_group_10.Rewards = new List<string>() { "XP%,100", "DBKEY,5", "PHIAL,3", "PKTROPHY,75" };
                    arena_group_10.TaskCount = 10;
                    _pkQuestList.Add(arena_group_10);

                    //- Win 5 1v1 arena matches
                    var arena_1v1_win_5 = new PKQuest();
                    arena_1v1_win_5.QuestCode = "ARENA_1v1_WIN_5";
                    arena_1v1_win_5.Description = "Win 5 Arena 1v1 matches";
                    arena_1v1_win_5.RewardDescription = "25% XP to next level, 3 Darkbeat Keys, 2 Phials of Bloody Tears, 15 PK Trophies";
                    arena_1v1_win_5.Rewards = new List<string>() { "XP%,25", "DBKEY,3", "PHIAL,2", "PKTROPHY,15" };
                    arena_1v1_win_5.TaskCount = 5;
                    _pkQuestList.Add(arena_1v1_win_5);

                    //- Win 15 1v1 arena matches
                    var arena_1v1_win_15 = new PKQuest();
                    arena_1v1_win_15.QuestCode = "ARENA_1v1_WIN_15";
                    arena_1v1_win_15.Description = "Win 15 Arena 1v1 matches";
                    arena_1v1_win_15.RewardDescription = "75% XP to next level, 5 Darkbeat Keys, 3 Phials of Bloody Tears, 50 PK Trophies";
                    arena_1v1_win_15.Rewards = new List<string>() { "XP%,75", "DBKEY,5", "PHIAL,3", "PKTROPHY,50" };
                    arena_1v1_win_15.TaskCount = 15;
                    _pkQuestList.Add(arena_1v1_win_15);

                    //- Win 2 2v2 arena matches
                    var arena_2v2_win_2 = new PKQuest();
                    arena_2v2_win_2.QuestCode = "ARENA_2v2_WIN_2";
                    arena_2v2_win_2.Description = "Win 2 Arena 2v2 matches";
                    arena_2v2_win_2.RewardDescription = "50% XP to next level, 2 Darkbeat Keys, 2 Phial of Bloody Tears, 15 PK Trophies";
                    arena_2v2_win_2.Rewards = new List<string>() { "XP%,50", "DBKEY,2", "PHIAL,2", "PKTROPHY,15" };
                    arena_2v2_win_2.TaskCount = 2;
                    _pkQuestList.Add(arena_2v2_win_2);

                    //- Place 1st in an FFA arena match
                    var ARENA_FFA_WIN_1 = new PKQuest();
                    ARENA_FFA_WIN_1.QuestCode = "ARENA_FFA_WIN_1";
                    ARENA_FFA_WIN_1.Description = "Win 1 Arena FFA match";
                    ARENA_FFA_WIN_1.RewardDescription = "50% XP to next level, 2 Darkbeat Keys, 2 Phials of Bloody Tears, 15 PK Trophies";
                    ARENA_FFA_WIN_1.Rewards = new List<string>() { "XP%,50", "DBKEY,2", "PHIAL,2", "PKTROPHY,15" };
                    ARENA_FFA_WIN_1.TaskCount = 1;
                    _pkQuestList.Add(ARENA_FFA_WIN_1);

                    //- Place in the top 3 in an FFA arena match
                    var arena_ffa_top3 = new PKQuest();
                    arena_ffa_top3.QuestCode = "ARENA_FFA_TOP3";
                    arena_ffa_top3.Description = "Place in the top 3 in an Arena FFA match";
                    arena_ffa_top3.RewardDescription = "35% XP to next level, 1 Darkbeat Key, 1 Phial of Bloody Tears, 5 PK Trophies";
                    arena_ffa_top3.Rewards = new List<string>() { "XP%,35", "DBKEY,1", "PHIAL,1", "PKTROPHY,5" };
                    arena_ffa_top3.TaskCount = 1;
                    _pkQuestList.Add(arena_ffa_top3);

                    //- Place 1st in a Tugak War arena match
                    var ARENA_TUGAK_WIN_1 = new PKQuest();
                    ARENA_TUGAK_WIN_1.QuestCode = "ARENA_TUGAK_WIN_1";
                    ARENA_TUGAK_WIN_1.Description = "Win 1 Arena Tugak War match";
                    ARENA_TUGAK_WIN_1.RewardDescription = "35% XP to next level, 1 Darkbeat Keys, 1 Phials of Bloody Tears, 5 PK Trophies";
                    ARENA_TUGAK_WIN_1.Rewards = new List<string>() { "XP%,35", "DBKEY,1", "PHIAL,1", "PKTROPHY,5" };
                    ARENA_TUGAK_WIN_1.TaskCount = 1;
                    _pkQuestList.Add(ARENA_TUGAK_WIN_1);

                    //- Place 1st in 20 Tugak War arena matches
                    var ARENA_TUGAK_WIN_20 = new PKQuest();
                    ARENA_TUGAK_WIN_20.QuestCode = "ARENA_TUGAK_WIN_20";
                    ARENA_TUGAK_WIN_20.Description = "Win 20 Arena Tugak War matches";
                    ARENA_TUGAK_WIN_20.RewardDescription = "200% XP to next level, 6 Darkbeat Keys, 10 Phials of Bloody Tears, 250 PK Trophies, 5 Boxes.";
                    ARENA_TUGAK_WIN_20.Rewards = new List<string>() { "XP%,200", "DBKEY,6", "PHIAL,10", "PKTROPHY,250", "BOX,5" };
                    ARENA_TUGAK_WIN_20.TaskCount = 20;
                    _pkQuestList.Add(ARENA_TUGAK_WIN_20);

                    //- Place in the top 3 in a Tugak War arena match
                    var arena_tugak_top3 = new PKQuest();
                    arena_tugak_top3.QuestCode = "ARENA_TUGAK_TOP3";
                    arena_tugak_top3.Description = "Place in the top 3 in an Arena Tugak War match";
                    arena_tugak_top3.RewardDescription = "35% XP to next level, 1 Darkbeat Key, 1 Phial of Bloody Tears, 5 PK Trophies";
                    arena_tugak_top3.Rewards = new List<string>() { "XP%,35", "DBKEY,1", "PHIAL,1", "PKTROPHY,5" };
                    arena_tugak_top3.TaskCount = 1;
                    _pkQuestList.Add(arena_tugak_top3);

                    //- Win 1 group arena match
                    var arena_group_win_1 = new PKQuest();
                    arena_group_win_1.QuestCode = "ARENA_GROUP_WIN_1";
                    arena_group_win_1.Description = "Win 1 Arena Group match";
                    arena_group_win_1.RewardDescription = "25% XP to next level, 1 Darkbeat Key, 1 Phial of Bloody Tears, 15 PK Trophies, 2 Boxes";
                    arena_group_win_1.Rewards = new List<string>() { "XP%,25", "DBKEY,1", "PHIAL,1", "PKTROPHY,15", "BOX,2" };
                    arena_group_win_1.TaskCount = 1;
                    _pkQuestList.Add(arena_group_win_1);

                    //- Win 5 group arena matches
                    var arena_group_win_5 = new PKQuest();
                    arena_group_win_5.QuestCode = "ARENA_GROUP_WIN_5";
                    arena_group_win_5.Description = "Win 5 Arena Group matches";
                    arena_group_win_5.RewardDescription = "50% XP to next level, 2 Darkbeat Keys, 3 Phials of Bloody Tears, 50 PK Trophies, 3 Boxes";
                    arena_group_win_5.Rewards = new List<string>() { "XP%,50", "DBKEY,2", "PHIAL,3", "PKTROPHY,50", "BOX,3" };
                    arena_group_win_5.TaskCount = 5;
                    _pkQuestList.Add(arena_group_win_5);

                    //- Win 10 group arena matches
                    var arena_group_win_10 = new PKQuest();
                    arena_group_win_10.QuestCode = "ARENA_GROUP_WIN_10";
                    arena_group_win_10.Description = "Win 10 Arena Group matches";
                    arena_group_win_10.RewardDescription = "75% XP to next level, 5 Darkbeat Keys, 5 Phials of Bloody Tears, 75 PK Trophies, 5 Boxes";
                    arena_group_win_10.Rewards = new List<string>() { "XP%,75", "DBKEY,5", "PHIAL,5", "PKTROPHY,75", "BOX,5" };
                    arena_group_win_10.TaskCount = 10;
                    _pkQuestList.Add(arena_group_win_10);

                    //- Do 20k dmg in any type of arena matches
                    var arenaDmg20k = new PKQuest();
                    arenaDmg20k.QuestCode = "ARENA_DMG20K";
                    arenaDmg20k.Description = "Deal 20k PK damage during arena matches";
                    arenaDmg20k.RewardDescription = "Reward = 25% XP to next level, 1 Darkbeat Key";
                    arenaDmg20k.TaskCount = 20000;
                    arenaDmg20k.Rewards = new List<string>() { "XP%,25", "DBKEY,1" };
                    _pkQuestList.Add(arenaDmg20k);

                    //- Receive less than 800 damage as the winner of a single arena match
                    var arena_recdmg_800 = new PKQuest();
                    arena_recdmg_800.QuestCode = "ARENA_RECDMG800";
                    arena_recdmg_800.Description = "Win an arena match while receiving less than 800 damage";
                    arena_recdmg_800.RewardDescription = "25% XP to next level, 1 Darkbeat Key";
                    arena_recdmg_800.TaskCount = 1;
                    arena_recdmg_800.Rewards = new List<string>() { "XP%,25", "DBKEY,1" };
                    _pkQuestList.Add(arena_recdmg_800);

                    ////- Kill 3 players from an opposing whitelisted allegiance in Town Network
                    //var pkkill_tn_3 = new PKQuest();
                    //pkkill_tn_3.QuestCode = "PKKILL_TN_3";
                    //pkkill_tn_3.Description = "Kill 3 members of an opposing whitelisted allegiance in Town Network";
                    //pkkill_tn_3.RewardDescription = "25% XP to next level, 2 Darkbeat Keys, 1 Phial of Bloody Tears, 25 PK Trophies.";
                    //pkkill_tn_3.TaskCount = 3;
                    //pkkill_tn_3.Rewards = new List<string>() { "XP%,25", "DBKEY,2", "PHIAL,1", "PKTROPHY,25" };
                    //_pkQuestList.Add(pkkill_tn_3);

                    //- Kill 3 players from an opposing whitelisted allegiance in Subway
                    var pkkill_sub_3 = new PKQuest();
                    pkkill_sub_3.QuestCode = "PKKILL_SUB_3";
                    pkkill_sub_3.Description = "Kill 3 members of an opposing whitelisted allegiance in Subway";
                    pkkill_sub_3.RewardDescription = "25% XP to next level, 2 Darkbeat Keys, 1 Phial of Bloody Tears, 25 PK Trophies";
                    pkkill_sub_3.TaskCount = 3;
                    pkkill_sub_3.Rewards = new List<string>() { "XP%,25", "DBKEY,2", "PHIAL,1", "PKTROPHY,25" };
                    _pkQuestList.Add(pkkill_sub_3);

                    ////- Kill 3 players from an opposing whitelisted allegiance at the Island LS
                    //var pkkill_islandls_3 = new PKQuest();
                    //pkkill_islandls_3.QuestCode = "PKKILL_ISLANDLS_3";
                    //pkkill_islandls_3.Description = "Kill 3 members of an opposing whitelisted allegiance at the Peddler's Outpost Lifestone";
                    //pkkill_islandls_3.RewardDescription = "25% XP to next level, 2 Darkbeat Keys, 1 Phial of Bloody Tears, 25 PK Trophies";
                    //pkkill_islandls_3.TaskCount = 3;
                    //pkkill_islandls_3.Rewards = new List<string>() { "XP%,25", "DBKEY,2", "PHIAL,1", "PKTROPHY,25" };
                    //_pkQuestList.Add(pkkill_islandls_3);

                    ////- Kill 3 players from an opposing whitelisted allegiance in VR Roots
                    //var pkkill_vrroots_3 = new PKQuest();
                    //pkkill_vrroots_3.QuestCode = "PKKILL_VRROOTS_3";
                    //pkkill_vrroots_3.Description = "Kill 3 members of an opposing whitelisted allegiance in the Viridian Rise Roots dungeon";
                    //pkkill_vrroots_3.RewardDescription = "25% XP to next level, 2 Darkbeat Keys, 1 Phial of Bloody Tears, 25 PK Trophies";
                    //pkkill_vrroots_3.TaskCount = 3;
                    //pkkill_vrroots_3.Rewards = new List<string>() { "XP%,25", "DBKEY,2", "PHIAL,1", "PKTROPHY,25" };
                    //_pkQuestList.Add(pkkill_vrroots_3);

                    ////- Kill 3 players from an opposing whitelisted allegiance in Shreths
                    //var pkkill_islandshreths_3 = new PKQuest();
                    //pkkill_islandshreths_3.QuestCode = "PKKILL_ISLANDSHRETHS_3";
                    //pkkill_islandshreths_3.Description = "Kill 3 members of an opposing whitelisted allegiance in Shreth Caverns.";
                    //pkkill_islandshreths_3.RewardDescription = "25% XP to next level, 2 Darkbeat Keys, 1 Phial of Bloody Tears, 25 PK Trophies";
                    //pkkill_islandshreths_3.TaskCount = 3;
                    //pkkill_islandshreths_3.Rewards = new List<string>() { "XP%,25", "DBKEY,2", "PHIAL,1", "PKTROPHY,25" };
                    //_pkQuestList.Add(pkkill_islandshreths_3);

                    ////- Kill 3 players from an opposing whitelisted allegiance in Mites
                    //var pkkill_islandmites_3 = new PKQuest();
                    //pkkill_islandmites_3.QuestCode = "PKKILL_ISLANDMITES_3";
                    //pkkill_islandmites_3.Description = "Kill 3 members of an opposing whitelisted allegiance in Mite Hole.";
                    //pkkill_islandmites_3.RewardDescription = "25% XP to next level, 2 Darkbeat Keys, 1 Phial of Bloody Tears, 25 PK Trophies";
                    //pkkill_islandmites_3.TaskCount = 3;
                    //pkkill_islandmites_3.Rewards = new List<string>() { "XP%,25", "DBKEY,2", "PHIAL,1", "PKTROPHY,25" };
                    //_pkQuestList.Add(pkkill_islandmites_3);

                    ////- Kill 3 players from an opposing whitelisted allegiance in Dragons
                    //var pkkill_islanddragons_3 = new PKQuest();
                    //pkkill_islanddragons_3.QuestCode = "PKKILL_ISLANDDRAGONS_3";
                    //pkkill_islanddragons_3.Description = "Kill 3 members of an opposing whitelisted allegiance in Dragon's Den.";
                    //pkkill_islanddragons_3.RewardDescription = "25% XP to next level, 2 Darkbeat Keys, 1 Phial of Bloody Tears, 25 PK Trophies";
                    //pkkill_islanddragons_3.TaskCount = 3;
                    //pkkill_islanddragons_3.Rewards = new List<string>() { "XP%,25", "DBKEY,2", "PHIAL,1", "PKTROPHY,25" };
                    //_pkQuestList.Add(pkkill_islanddragons_3);

                    ////- Kill 3 players from an opposing whitelisted allegiance in Golems
                    //var pkkill_islandgolems_3 = new PKQuest();
                    //pkkill_islandgolems_3.QuestCode = "PKKILL_ISLANDGOLEMS_3";
                    //pkkill_islandgolems_3.Description = "Kill 3 members of an opposing whitelisted allegiance in Ancient Temple.";
                    //pkkill_islandgolems_3.RewardDescription = "25% XP to next level, 2 Darkbeat Keys, 1 Phial of Bloody Tears, 25 PK Trophies";
                    //pkkill_islandgolems_3.TaskCount = 3;
                    //pkkill_islandgolems_3.Rewards = new List<string>() { "XP%,25", "DBKEY,2", "PHIAL,1", "PKTROPHY,25" };
                    //_pkQuestList.Add(pkkill_islandgolems_3);

                    ////- Kill 3 players from an opposing whitelisted allegiance in Wasps
                    //var pkkill_islandwasps_3 = new PKQuest();
                    //pkkill_islandwasps_3.QuestCode = "PKKILL_ISLANDWASPS_3";
                    //pkkill_islandwasps_3.Description = "Kill 3 members of an opposing whitelisted allegiance in Swarm Hive.";
                    //pkkill_islandwasps_3.RewardDescription = "25% XP to next level, 2 Darkbeat Keys, 1 Phial of Bloody Tears, 25 PK Trophies";
                    //pkkill_islandwasps_3.TaskCount = 3;
                    //pkkill_islandwasps_3.Rewards = new List<string>() { "XP%,25", "DBKEY,2", "PHIAL,1", "PKTROPHY,25" };
                    //_pkQuestList.Add(pkkill_islandwasps_3);

                    ////- Kill 3 players from an opposing whitelisted allegiance in Rats
                    //var pkkill_islandrats_3 = new PKQuest();
                    //pkkill_islandrats_3.QuestCode = "PKKILL_ISLANDRATS_3";
                    //pkkill_islandrats_3.Description = "Kill 3 members of an opposing whitelisted allegiance in Rat Nest.";
                    //pkkill_islandrats_3.RewardDescription = "25% XP to next level, 2 Darkbeat Keys, 1 Phial of Bloody Tears, 25 PK Trophies";
                    //pkkill_islandrats_3.TaskCount = 3;
                    //pkkill_islandrats_3.Rewards = new List<string>() { "XP%,25", "DBKEY,2", "PHIAL,1", "PKTROPHY,25" };
                    //_pkQuestList.Add(pkkill_islandrats_3);

                    ////- Kill 3 players from an opposing whitelisted allegiance in Drudges
                    //var pkkill_islanddrudges_3 = new PKQuest();
                    //pkkill_islanddrudges_3.QuestCode = "PKKILL_ISLANDDRUDGES_3";
                    //pkkill_islanddrudges_3.Description = "Kill 3 members of an opposing whitelisted allegiance in Drudge Stronghold.";
                    //pkkill_islanddrudges_3.RewardDescription = "25% XP to next level, 2 Darkbeat Keys, 1 Phial of Bloody Tears, 25 PK Trophies";
                    //pkkill_islanddrudges_3.TaskCount = 3;
                    //pkkill_islanddrudges_3.Rewards = new List<string>() { "XP%,25", "DBKEY,2", "PHIAL,1", "PKTROPHY,25" };
                    //_pkQuestList.Add(pkkill_islanddrudges_3);

                    ////- Kill 3 players from an opposing whitelisted allegiance in any of the T9 dungeons
                    //var pkkill_T9_3 = new PKQuest();
                    //pkkill_T9_3.QuestCode = "PKKILL_T9_3";
                    //pkkill_T9_3.Description = "Kill 3 members of an opposing whitelisted allegiance in any of the Tier 9 Halls of Metos, Black Spawn Den or Mountain Citadel dungeons.";
                    //pkkill_T9_3.RewardDescription = "50% XP to next level, 5 Darkbeat Keys, 3 Phials of Bloody Tears, 100 PK Trophies, 2 Boxes";
                    //pkkill_T9_3.TaskCount = 3;
                    //pkkill_T9_3.Rewards = new List<string>() { "XP%,50", "DBKEY,5", "PHIAL,3", "PKTROPHY,100", "BOX,2" };
                    //_pkQuestList.Add(pkkill_T9_3);

                    ////- Kill 15 players from an opposing whitelisted allegiance in any of the T9 dungeons
                    //var pkkill_T9_15 = new PKQuest();
                    //pkkill_T9_15.QuestCode = "PKKILL_T9_15";
                    //pkkill_T9_15.Description = "Kill 15 members of an opposing whitelisted allegiance in any of the Tier 9 Halls of Metos, Black Spawn Den or Mountain Citadel dungeons.";
                    //pkkill_T9_15.RewardDescription = "75% XP to next level, 5 Darkbeat Keys, 15 Phials of Bloody Tears, 250 PK Trophies, 5 Boxes";
                    //pkkill_T9_15.TaskCount = 15;
                    //pkkill_T9_15.Rewards = new List<string>() { "XP%,75", "DBKEY,5", "PHIAL,15", "PKTROPHY,250", "BOX,5" };
                    //_pkQuestList.Add(pkkill_T9_15);

                    ////- Kill 100 players from an opposing whitelisted allegiance in any of the T9 dungeons
                    //var pkkill_T9_100 = new PKQuest();
                    //pkkill_T9_100.QuestCode = "PKKILL_T9_100";
                    //pkkill_T9_100.Description = "Kill 100 members of an opposing whitelisted allegiance in any of the Tier 9 Halls of Metos, Black Spawn Den or Mountain Citadel dungeons.";
                    //pkkill_T9_100.RewardDescription = "250% XP to next level, 5 Darkbeat Keys, 50 Phials of Bloody Tears, 500 PK Trophies, 10 Boxes";
                    //pkkill_T9_100.TaskCount = 100;
                    //pkkill_T9_100.Rewards = new List<string>() { "XP%,250", "DBKEY,5", "PHIAL,50", "PKTROPHY,500", "BOX,10" };
                    //_pkQuestList.Add(pkkill_T9_100);

                    // Bounties
                    // Complete 1 bounty contract
                    var bounty_any_1 = new PKQuest();
                    bounty_any_1.QuestCode = "BOUNTY_ANY_1";
                    bounty_any_1.Description = "Complete 1 Bounty Contract";
                    bounty_any_1.RewardDescription = "20% XP to next level, 2 Darkbeat Keys, 1 Phial of Bloody Tears, 25 PK Trophies, 1 Box";
                    bounty_any_1.Rewards = new List<string>() { "XP%,20", "DBKEY,2", "PHIAL,1", "PKTROPHY,25", "BOX,1" };
                    bounty_any_1.TaskCount = 1;
                    _pkQuestList.Add(bounty_any_1);

                    // Complete 5 bounty contracts
                    var bounty_any_5 = new PKQuest();
                    bounty_any_5.QuestCode = "BOUNTY_ANY_5";
                    bounty_any_5.Description = "Complete 5 Bounty Contracts";
                    bounty_any_5.RewardDescription = "50% XP to next level, 3 Darkbeat Keys, 3 Phials of Bloody Tears, 50 PK Trophies, 3 Boxes";
                    bounty_any_5.Rewards = new List<string>() { "XP%,50", "DBKEY,3", "PHIAL,3", "PKTROPHY,50", "BOX,3" };
                    bounty_any_5.TaskCount = 5;
                    _pkQuestList.Add(bounty_any_5);

                    // Complete 25 bounty contracts
                    var bounty_any_25 = new PKQuest();
                    bounty_any_25.QuestCode = "BOUNTY_ANY_25";
                    bounty_any_25.Description = "Complete 25 Bounty Contracts";
                    bounty_any_25.RewardDescription = "100% XP to next level, 7 Darkbeat Keys, 5 Phials of Bloody Tears, 100 PK Trophies, 5 Boxes";
                    bounty_any_25.Rewards = new List<string>() { "XP%,100", "DBKEY,7", "PHIAL,5", "PKTROPHY,100", "BOX,5" };
                    bounty_any_25.TaskCount = 25;
                    _pkQuestList.Add(bounty_any_25);

                    // Complete bounties on 10 different players
                    var bounty_unique_10 = new PKQuest();
                    bounty_unique_10.QuestCode = "BOUNTY_UNIQUE_10";
                    bounty_unique_10.Description = "Complete bounties on 10 different players";
                    bounty_unique_10.RewardDescription = "75% XP to next level, 5 Darkbeat Keys, 3 Phials of Bloody Tears, 50 PK Trophies, 3 Boxes";
                    bounty_unique_10.Rewards = new List<string>() { "XP%,75", "DBKEY,5", "PHIAL,3", "PKTROPHY,50", "BOX,3" };
                    bounty_unique_10.TaskCount = 10;
                    _pkQuestList.Add(bounty_unique_10);

                    // Complete bounties on 25 different players
                    var bounty_unique_25 = new PKQuest();
                    bounty_unique_25.QuestCode = "BOUNTY_UNIQUE_25";
                    bounty_unique_25.Description = "Complete bounties on 25 different players";
                    bounty_unique_25.RewardDescription = "150% XP to next level, 10 Darkbeat Keys, 5 Phials of Bloody Tears, 100 PK Trophies, 5 Boxes";
                    bounty_unique_25.Rewards = new List<string>() { "XP%,150", "DBKEY,10", "PHIAL,5", "PKTROPHY,100", "BOX,5" };
                    bounty_unique_25.TaskCount = 25;
                    _pkQuestList.Add(bounty_unique_25);

                    // Complete 1 High Priority Bounty Contract
                    var bounty_priority_1 = new PKQuest();
                    bounty_priority_1.QuestCode = "BOUNTY_PRIORITY_1";
                    bounty_priority_1.Description = "Complete 1 High Priority Bounty Contract";
                    bounty_priority_1.RewardDescription = "50% XP to next level, 3 Darkbeat Keys, 2 Phials of Bloody Tears, 25 PK Trophies, 2 Boxes";
                    bounty_priority_1.Rewards = new List<string>() { "XP%,50", "DBKEY,3", "PHIAL,2", "PKTROPHY,25", "BOX,2" };
                    bounty_priority_1.TaskCount = 1;
                    _pkQuestList.Add(bounty_priority_1);

                    // Complete 5 High Priority Bounty Contracts
                    var bounty_priority_5 = new PKQuest();
                    bounty_priority_5.QuestCode = "BOUNTY_PRIORITY_5";
                    bounty_priority_5.Description = "Complete 5 High Priority Bounty Contracts";
                    bounty_priority_5.RewardDescription = "100% XP to next level, 5 Darkbeat Keys, 5 Phials of Bloody Tears, 50 PK Trophies, 5 Boxes";
                    bounty_priority_5.Rewards = new List<string>() { "XP%,100", "DBKEY,5", "PHIAL,5", "PKTROPHY,50", "BOX,5" };
                    bounty_priority_5.TaskCount = 5;
                    _pkQuestList.Add(bounty_priority_5);

                    // Kill 1 player who is on a kill streak
                    var bounty_killstreak_1 = new PKQuest();
                    bounty_killstreak_1.QuestCode = "BOUNTY_KILLSTREAK_1";
                    bounty_killstreak_1.Description = "Kill 1 player who is on a kill streak";
                    bounty_killstreak_1.RewardDescription = "25% XP to next level, 2 Darkbeat Keys, 2 Phials of Bloody Tears, 15 PK Trophies, 1 Box";
                    bounty_killstreak_1.Rewards = new List<string>() { "XP%,25", "DBKEY,2", "PHIAL,2", "PKTROPHY,15", "BOX,1" };
                    bounty_killstreak_1.TaskCount = 1;
                    _pkQuestList.Add(bounty_killstreak_1);

                    // Kill 5 players who are on kill streaks
                    var bounty_killstreak_5 = new PKQuest();
                    bounty_killstreak_5.QuestCode = "BOUNTY_KILLSTREAK_5";
                    bounty_killstreak_5.Description = "Kill 5 players who are on kill streaks";
                    bounty_killstreak_5.RewardDescription = "75% XP to next level, 10 Darkbeat Keys, 5 Phials of Bloody Tears, 50 PK Trophies, 5 Boxes";
                    bounty_killstreak_5.Rewards = new List<string>() { "XP%,75", "DBKEY,10", "PHIAL,5", "PKTROPHY,50", "BOX,5" };
                    bounty_killstreak_5.TaskCount = 5;
                    _pkQuestList.Add(bounty_killstreak_5);

                    // Town Control kills
                    var pkkill_TC_1 = new PKQuest();
                    pkkill_TC_1.QuestCode = "PKKILL_TC_1";
                    pkkill_TC_1.Description = "Kill 1 member of an opposing whitelisted allegiance in a Town Control event.";
                    pkkill_TC_1.RewardDescription = "25% XP to next level, 2 Darkbeat Keys, 15 PK Trophies";
                    pkkill_TC_1.TaskCount = 1;
                    pkkill_TC_1.Rewards = new List<string>() { "XP%,25", "DBKEY,2", "PKTROPHY,15" };
                    _pkQuestList.Add(pkkill_TC_1);

                    var pkkill_TC_5 = new PKQuest();
                    pkkill_TC_5.QuestCode = "PKKILL_TC_5";
                    pkkill_TC_5.Description = "Kill 5 members of an opposing whitelisted allegiance in a Town Control event.";
                    pkkill_TC_5.RewardDescription = "75% XP to next level, 4 Darkbeat Keys, 2 Phials of Bloody Tears, 25 PK Trophies, 1 Box";
                    pkkill_TC_5.TaskCount = 5;
                    pkkill_TC_5.Rewards = new List<string>() { "XP%,75", "DBKEY,4", "PHIAL,2", "PKTROPHY,25", "BOX,1" };
                    _pkQuestList.Add(pkkill_TC_5);

                    var pkkill_TC_30 = new PKQuest();
                    pkkill_TC_30.QuestCode = "PKKILL_TC_30";
                    pkkill_TC_30.Description = "Kill 30 members of an opposing whitelisted allegiance in a Town Control event.";
                    pkkill_TC_30.RewardDescription = "150% XP to next level, 8 Darkbeat Keys, 3 Phials of Bloody Tears, 50 PK Trophies, 5 Boxes";
                    pkkill_TC_30.TaskCount = 30;
                    pkkill_TC_30.Rewards = new List<string>() { "XP%,150", "DBKEY,8", "PHIAL,3", "PKTROPHY,50", "BOX,5" };
                    _pkQuestList.Add(pkkill_TC_30);
                }

                return _pkQuestList;
            }
        }

        public static string[] PKQuests_ParticipateAnyArena = { "ARENA_ANY_5", "ARENA_ANY_15", "ARENA_ANY_30", "ARENA_ANY_50" };

        public static string[] PKQuests_WinAnyArena = { "ARENA_ANY_WIN_10", "ARENA_ANY_WIN_20", "ARENA_ANY_WIN_30" };

        public static string[] PKQuests_Participate1v1Arena = { "ARENA_1v1_10", "ARENA_1v1_20" };

        public static string[] PKQuests_Win1v1Arena = { "ARENA_1v1_WIN_5", "ARENA_1v1_WIN_15" };

        public static string[] PKQuests_Participate2v2Arena = { "ARENA_2v2_3", "ARENA_2v2_10" };

        public static string[] PKQuests_Win2v2Arena = { "ARENA_2v2_WIN_2" };

        public static string[] PKQuests_ParticipateTugakArena = { "ARENA_TUGAK_2", "ARENA_TUGAK_25" };

        public static string[] PKQuests_WinTugakArena = { "ARENA_TUGAK_WIN_1", "ARENA_TUGAK_WIN_20" };

        public static string[] PKQuests_ParticipateGroupArena = { "ARENA_GROUP_1", "ARENA_GROUP_3", "ARENA_GROUP_10" };

        public static string[] PKQuests_WinGroupArena = { "ARENA_GROUP_WIN_1", "ARENA_GROUP_WIN_5", "ARENA_GROUP_WIN_10" };

        public static string[] PKQuests_KillAnywhere = { "KILL_ANY_10", "KILL_ANY_30" };

        public static string[] PKQuests_KillT9 = { "PKKILL_T9_3", "PKKILL_T9_15", "PKKILL_T9_100" };

        public static string[] PKQuests_KillTC = { "PKKILL_TC_1", "PKKILL_TC_5", "PKKILL_TC_30" };

        // Bounty Hunter quest code groups
        public static string[] PKQuests_BountyAny        = { "BOUNTY_ANY_1", "BOUNTY_ANY_5", "BOUNTY_ANY_25" };
        public static string[] PKQuests_BountyUnique     = { "BOUNTY_UNIQUE_10", "BOUNTY_UNIQUE_25" };
        public static string[] PKQuests_BountyPriority   = { "BOUNTY_PRIORITY_1", "BOUNTY_PRIORITY_5" };
        public static string[] PKQuests_BountyKillStreak = { "BOUNTY_KILLSTREAK_1", "BOUNTY_KILLSTREAK_5" };

        public static PKQuest GetPkQuestByCode(string questCode)
        {
            return PKQuests.PkQuestList.FirstOrDefault(x => x.QuestCode.Equals(questCode));
        }
    }

    public class PKQuest
    {
        public string QuestCode { get; set; }

        public string Description { get; set; }

        public string RewardDescription { get; set; }

        public int TaskCount { get; set; }

        public List<string> Rewards { get; set; }
    }

    public class PlayerPKQuest
    {
        public string QuestCode { get; set; }

        public int TaskDoneCount { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime? CompletedTime { get; set; }

        public bool IsCompleted { get; set; }

        public bool RewardDelivered { get; set; }
    }
}
