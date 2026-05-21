using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

using log4net;

using ACE.Database.Models.Log;

namespace ACE.Database
{
    public class LogDatabase
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static bool IsConfigured => Common.ConfigManager.Config.MySql.Log != null;

        public bool Exists(bool retryUntilFound)
        {
            if (!IsConfigured)
                return false;

            var config = Common.ConfigManager.Config.MySql.Log;

            for (;;)
            {
                using (var context = new LogDbContext())
                {
                    if (((RelationalDatabaseCreator)context.Database.GetService<IDatabaseCreator>()).Exists())
                    {
                        log.Debug($"[DATABASE] Successfully connected to {config.Database} database on {config.Host}:{config.Port}.");
                        return true;
                    }
                }

                log.Error($"[DATABASE] Attempting to reconnect to {config.Database} database on {config.Host}:{config.Port} in 5 seconds...");

                if (retryUntilFound)
                    Thread.Sleep(5000);
                else
                    return false;
            }
        }

        #region Account Session Log

        public void LogAccountSessionStart(uint accountId, string accountName, string sessionIP)
        {
            if (!IsConfigured) return;
            try
            {
                using (var context = new LogDbContext())
                {
                    context.Database.ExecuteSql(
                        @$"INSERT INTO account_session_log (accountId, accountName, sessionIP, loginDateTime)
                            VALUES ({accountId}, {accountName}, {sessionIP}, {DateTime.Now});");
                }
            }
            catch (Exception ex)
            {
                log.Error($"Exception in LogAccountSessionStart saving session log data to DB. Ex: {ex}");
            }
        }

        public void LogAccountSessionEnd(uint accountId)
        {
            if (!IsConfigured) return;
            try
            {
                using (var context = new LogDbContext())
                {
                    context.Database.ExecuteSql(
                        @$"UPDATE account_session_log SET logoutDateTime = {DateTime.Now}
                            WHERE accountId = {accountId} AND logoutDateTime IS NULL;");
                }
            }
            catch (Exception ex)
            {
                log.Error($"Exception in LogAccountSessionEnd saving session log data to DB for AccountId = {accountId}. Ex: {ex}");
            }
        }

        #endregion

        #region Character Login Log

        public void LogCharacterLogin(uint accountId, string accountName, string sessionIP, uint characterId, string characterName)
        {
            if (!IsConfigured) return;
            try
            {
                using (var context = new LogDbContext())
                {
                    context.Database.ExecuteSql(
                        @$"INSERT INTO character_login_log (accountId, accountName, characterId, characterName, sessionIP, loginDateTime)
                            VALUES ({accountId}, {accountName}, {characterId}, {characterName}, {sessionIP}, {DateTime.Now});");
                }
            }
            catch (Exception ex)
            {
                log.Error($"Exception in LogCharacterLogin saving character login info to DB for AccountId = {accountId}, CharacterId = {characterId}. Ex: {ex}");
            }
        }

        public void LogCharacterLogout(uint characterId)
        {
            if (!IsConfigured) return;
            try
            {
                using (var context = new LogDbContext())
                {
                    context.Database.ExecuteSql(
                        @$"UPDATE character_login_log SET logoutDateTime = {DateTime.Now}
                            WHERE characterId = {characterId} AND logoutDateTime IS NULL;");
                }
            }
            catch (Exception ex)
            {
                log.Error($"Exception in LogCharacterLogout saving session log data to DB for CharacterId = {characterId}. Ex: {ex}");
            }
        }

        #endregion

        #region Tinkering Log

        public void LogTinkeringEvent(uint characterId, string characterName, uint itemBiotaId, float chance, float roll, bool isSuccess, uint itemNumPreviousTinks, uint itemWorkmanship, string salvageType, uint salvageWorkmanship)
        {
            if (!IsConfigured) return;
            try
            {
                using (var context = new LogDbContext())
                {
                    context.Database.ExecuteSql(
                        @$"INSERT INTO tinker_log (characterId, characterName, itemBiotaId, tinkDateTime, successChance, roll, isSuccess, itemNumPreviousTinks, itemWorkmanship, salvageType, salvageWorkmanship)
                            VALUES ({characterId}, {characterName}, {itemBiotaId}, {DateTime.Now}, {chance}, {roll}, {isSuccess}, {itemNumPreviousTinks}, {itemWorkmanship}, {salvageType}, {salvageWorkmanship});");
                }
            }
            catch (Exception ex)
            {
                log.Error($"Exception in LogTinkeringEvent saving data to DB for CharacterId = {characterId}, ItemBiotaId = {itemBiotaId}. Ex: {ex}");
            }
        }

        #endregion

        #region PK Kills Log

        public void LogPkKill(uint victimId, uint killerId, uint? victimMonarchId, uint? killerMonarchId, uint? victimArenaPlayerId = null, uint? killerArenaPlayerId = null)
        {
            if (!IsConfigured) return;
            try
            {
                using (var context = new LogDbContext())
                {
                    context.Database.ExecuteSql(
                        @$"INSERT INTO pk_kills_log (killer_id, victim_id, killer_monarch_id, victim_monarch_id, kill_datetime, killer_arena_player_id, victim_arena_player_id)
                            VALUES ({killerId}, {victimId}, {killerMonarchId}, {victimMonarchId}, {DateTime.Now}, {killerArenaPlayerId}, {victimArenaPlayerId});");
                }
            }
            catch (Exception ex)
            {
                log.Error($"Exception in LogPkKill saving kill data to DB for KillerId = {killerId}, VictimId = {victimId}. Ex: {ex}");
            }
        }

        #endregion

        #region Arenas

        public uint SaveArenaEvent(ArenaEvent arenaEvent)
        {
            if (!IsConfigured) return 0;
            try
            {
                using (var context = new LogDbContext())
                {
                    if (arenaEvent.Id <= 0)
                        context.ArenaEvents.Add(arenaEvent);
                    else
                        context.Entry(arenaEvent).State = EntityState.Modified;

                    context.SaveChanges();

                    foreach (var arenaPlayer in arenaEvent.Players)
                    {
                        arenaPlayer.EventId = arenaEvent.Id;

                        if (arenaPlayer.Id <= 0)
                            context.ArenaPlayers.Add(arenaPlayer);
                        else
                            context.Entry(arenaPlayer).State = EntityState.Modified;
                    }

                    context.SaveChanges();

                    return arenaEvent.Id;
                }
            }
            catch (Exception ex)
            {
                log.Error($"Exception in SaveArenaEvent. Ex: {ex}");
            }

            return 0;
        }

        public void AddToArenaStats(uint characterId, string characterName, string eventType, uint totalMatches, uint totalWins, uint totalDraws, uint totalLosses, uint totalDisqualified, uint totalDeaths, uint totalKills, uint totalDmgDealt, uint totalDmgReceived, uint? newRankPoints = null)
        {
            if (!IsConfigured) return;
            try
            {
                using (var context = new LogDbContext())
                {
                    var stats = context.ArenaCharacterStats.FirstOrDefault(x => x.CharacterId == characterId && x.EventType.Equals(eventType));
                    if (stats == null)
                    {
                        stats = new ArenaCharacterStats
                        {
                            CharacterId = characterId,
                            CharacterName = characterName,
                            EventType = eventType
                        };
                        context.ArenaCharacterStats.Add(stats);
                    }
                    else
                    {
                        context.Entry(stats).State = EntityState.Modified;
                    }

                    stats.TotalMatches      += totalMatches;
                    stats.TotalWins         += totalWins;
                    stats.TotalDraws        += totalDraws;
                    stats.TotalLosses       += totalLosses;
                    stats.TotalDisqualified += totalDisqualified;
                    stats.TotalDeaths       += totalDeaths;
                    stats.TotalKills        += totalKills;
                    stats.TotalDmgDealt     += totalDmgDealt;
                    stats.TotalDmgReceived  += totalDmgReceived;
                    stats.RankPoints = newRankPoints.HasValue ? newRankPoints.Value : stats.RankPoints;

                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                log.Error($"Exception saving ArenaCharacterStats. ex: {ex}");
            }
        }

        public ArenaCharacterStats GetCharacterArenaStatsByEvent(uint characterId, string eventType)
        {
            if (!IsConfigured) return null;
            try
            {
                using (var context = new LogDbContext())
                {
                    return context.ArenaCharacterStats.FirstOrDefault(x => x.CharacterId == characterId && x.EventType.Equals(eventType));
                }
            }
            catch (Exception ex)
            {
                log.Error($"Error in GetCharacterArenaStatsByEvent. ex:{ex}");
            }

            return null;
        }

        public string GetArenaStatsByCharacterId(uint characterId, string characterName)
        {
            if (!IsConfigured) return "Log database is not configured.";
            var returnMsg = new System.Text.StringBuilder();

            try
            {
                using (var context = new LogDbContext())
                {
                    var stats = context.ArenaCharacterStats.Where(x => x.CharacterId == characterId)?.ToList() ?? new List<ArenaCharacterStats>();

                    returnMsg.Append($"********* Arena Stats for {characterName} *********\n\n");

                    void AppendStats(ArenaCharacterStats s, string label, bool showRank)
                    {
                        returnMsg.Append($"{label}\n");
                        if (showRank)
                        {
                            returnMsg.Append($"  Rank: {GetArenaRank(s.EventType, s.RankPoints).ToString("n0")}\n");
                            returnMsg.Append($"  Rank Points: {s.RankPoints.ToString("n0")}\n");
                        }
                        returnMsg.Append($"  Matches: {s.TotalMatches.ToString("n0")}\n");
                        returnMsg.Append($"  Wins: {s.TotalWins.ToString("n0")}\n");
                        returnMsg.Append($"  Draws: {s.TotalDraws.ToString("n0")}\n");
                        returnMsg.Append($"  Losses: {s.TotalLosses.ToString("n0")}\n");
                        returnMsg.Append($"  Disqualified: {s.TotalDisqualified.ToString("n0")}\n");
                        returnMsg.Append($"  Kills: {s.TotalKills.ToString("n0")}\n");
                        returnMsg.Append($"  Deaths: {s.TotalDeaths.ToString("n0")}\n");
                        returnMsg.Append($"  Damage Dealt: {s.TotalDmgDealt.ToString("n0")}\n");
                        returnMsg.Append($"  Damage Received: {s.TotalDmgReceived.ToString("n0")}\n\n");
                    }

                    AppendStats(stats.FirstOrDefault(x => x.EventType.Equals("1v1"))   ?? new ArenaCharacterStats { EventType = "1v1"   }, "1v1",   true);
                    AppendStats(stats.FirstOrDefault(x => x.EventType.Equals("2v2"))   ?? new ArenaCharacterStats { EventType = "2v2"   }, "2v2",   true);
                    AppendStats(stats.FirstOrDefault(x => x.EventType.Equals("ffa"))   ?? new ArenaCharacterStats { EventType = "ffa"   }, "FFA",   true);
                    AppendStats(stats.FirstOrDefault(x => x.EventType.Equals("tugak")) ?? new ArenaCharacterStats { EventType = "tugak" }, "Tugak", true);
                    AppendStats(stats.FirstOrDefault(x => x.EventType.Equals("group")) ?? new ArenaCharacterStats { EventType = "group" }, "Group", false);

                    returnMsg.Append($"Totals:\n");
                    returnMsg.Append($"  Total Matches: {stats.Sum(x => x.TotalMatches).ToString("n0")}\n");
                    returnMsg.Append($"  Total Wins: {stats.Sum(x => x.TotalWins).ToString("n0")}\n");
                    returnMsg.Append($"  Total Draws: {stats.Sum(x => x.TotalDraws).ToString("n0")}\n");
                    returnMsg.Append($"  Total Losses: {stats.Sum(x => x.TotalLosses).ToString("n0")}\n");
                    returnMsg.Append($"  Total Disqualified: {stats.Sum(x => x.TotalDisqualified).ToString("n0")}\n");
                    returnMsg.Append($"  Total Kills: {stats.Sum(x => x.TotalKills).ToString("n0")}\n");
                    returnMsg.Append($"  Total Deaths: {stats.Sum(x => x.TotalDeaths).ToString("n0")}\n");
                    returnMsg.Append($"  Total Damage Dealt: {stats.Sum(x => x.TotalDmgDealt).ToString("n0")}\n");
                    returnMsg.Append($"  Total Damage Received: {stats.Sum(x => x.TotalDmgReceived).ToString("n0")}\n\n");
                    returnMsg.Append($"*****************************\n");
                }
            }
            catch (Exception ex)
            {
                log.Error($"Exception in GetArenaStatsByCharacterId for characterId = {characterId}. ex: {ex}");
            }

            return returnMsg.ToString();
        }

        public int GetArenaRank(string eventType, uint rankPoints)
        {
            if (!IsConfigured) return -1;
            try
            {
                using (var context = new LogDbContext())
                {
                    var higherPlayers = context.ArenaCharacterStats.Where(x => x.EventType.Equals(eventType) && x.RankPoints > rankPoints);
                    return (higherPlayers?.Count() ?? 0) + 1;
                }
            }
            catch (Exception ex)
            {
                log.Error($"Error in GetArenaRank. ex:{ex}");
            }

            return -1;
        }

        public List<ArenaCharacterStats> GetArenaTopRankedByEventType(string eventType)
        {
            if (!IsConfigured) return new List<ArenaCharacterStats>();
            try
            {
                using (var context = new LogDbContext())
                {
                    var topTen = context.ArenaCharacterStats
                        .Where(x => x.EventType.Equals(eventType))
                        ?.OrderByDescending(x => x.RankPoints)
                        ?.Take(10);

                    if (topTen != null)
                        return topTen.ToList();
                }
            }
            catch (Exception ex)
            {
                log.Error($"Error in GetArenaTopRankedByEventType. ex:{ex}");
            }

            return new List<ArenaCharacterStats>();
        }

        public uint CreateArenaPlayer(ArenaPlayer player)
        {
            if (!IsConfigured) return 0;
            try
            {
                using (var context = new LogDbContext())
                {
                    context.ArenaPlayers.Add(player);
                    context.SaveChanges();
                    return player.Id;
                }
            }
            catch (Exception ex)
            {
                log.Error($"Exception in CreateArenaPlayer. Ex: {ex}");
            }

            return 0;
        }

        public void UpdateArenaPlayer(ArenaPlayer player)
        {
            if (!IsConfigured) return;
            using (var context = new LogDbContext())
            {
                context.Entry(player).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        public List<ArenaEvent> GetAllArenaEvents()
        {
            if (!IsConfigured) return new List<ArenaEvent>();
            List<ArenaEvent> eventList = null;

            try
            {
                using (var context = new LogDbContext())
                {
                    eventList = context.ArenaEvents
                        .AsNoTracking()
                        .OrderByDescending(r => r.StartDateTime)
                        .Where(r => r.EndDateTime.HasValue)
                        ?.ToList() ?? new List<ArenaEvent>();
                }

                foreach (var arenaEvent in eventList)
                    arenaEvent.Players = GetAllArenaPlayersByEvent(arenaEvent.Id);
            }
            catch (Exception ex)
            {
                log.Error($"Exception in GetAllArenaEvents. Ex: {ex}");
            }

            return eventList ?? new List<ArenaEvent>();
        }

        public List<ArenaPlayer> GetAllArenaPlayersByEvent(uint eventId)
        {
            if (!IsConfigured) return new List<ArenaPlayer>();
            List<ArenaPlayer> playerList = null;

            try
            {
                using (var context = new LogDbContext())
                {
                    playerList = context.ArenaPlayers
                        .AsNoTracking()
                        .Where(x => x.EventId == (uint?)eventId)
                        ?.ToList();
                }
            }
            catch (Exception ex)
            {
                log.Error($"Exception in GetAllArenaPlayersByEvent. Ex: {ex}");
            }

            return playerList ?? new List<ArenaPlayer>();
        }

        #endregion

        #region Rare Log

        public void LogRare(RareLog rareLog)
        {
            if (!IsConfigured || rareLog == null) return;
            try
            {
                using (var context = new LogDbContext())
                {
                    context.Database.ExecuteSql(
                        @$"INSERT INTO rare_log (characterName, characterId, itemName, itemBiotaId, itemWeenieId, createDateTime)
                            VALUES ({rareLog.CharacterName}, {rareLog.CharacterId}, {rareLog.ItemName}, {rareLog.ItemBiotaId}, {rareLog.ItemWeenieId}, {DateTime.Now});");
                }
            }
            catch (Exception ex)
            {
                log.Error($"Exception in LogRare saving rare event to DB. ex: {ex}");
            }
        }

        #endregion

        #region Stuck Character Log

        public void LogStuckCharacter(StuckCharacterLog stuckLog)
        {
            if (!IsConfigured || stuckLog == null) return;
            try
            {
                using (var context = new LogDbContext())
                {
                    context.Database.ExecuteSql(
                        @$"INSERT INTO stuck_character_log
                        (playerGuid, playerName, accountName, accountId, sessionInfo, landblock, location,
                         isLoggingOut, isInDeathProcess, foundOnLandblock, forcedLogOffRequested,
                         pkLogoutState, materializedLogoutState, logoffPath, createdAtUtc)
                        VALUES
                        ({stuckLog.PlayerGuid},
                         {stuckLog.PlayerName ?? (object)DBNull.Value},
                         {stuckLog.AccountName ?? (object)DBNull.Value},
                         {stuckLog.AccountId},
                         {stuckLog.SessionInfo ?? (object)DBNull.Value},
                         {stuckLog.Landblock ?? (object)DBNull.Value},
                         {stuckLog.Location ?? (object)DBNull.Value},
                         {stuckLog.IsLoggingOut},
                         {stuckLog.IsInDeathProcess},
                         {stuckLog.FoundOnLandblock},
                         {stuckLog.ForcedLogOffRequested},
                         {stuckLog.PkLogoutState},
                         {stuckLog.MaterializedLogoutState},
                         {stuckLog.LogoffPath ?? (object)DBNull.Value},
                         {DateTime.Now});");
                }
            }
            catch (Exception ex)
            {
                log.Error($"Exception in LogStuckCharacter saving stuck character event to DB. ex: {ex}");
            }
        }

        #endregion
    }
}
